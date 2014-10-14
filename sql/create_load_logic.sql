IF OBJECT_ID ('load.MergePointing', N'P') IS NOT NULL
DROP PROC [load].[MergePointing]

GO

CREATE PROC [load].[MergePointing]
AS
	/*CREATE CLUSTERED INDEX [IC_RawPointing] ON [load].[RawPointing]
	(
		[inst] ASC,
		[obsID] ASC,
		[fineTime] ASC
	)
	WITH (SORT_IN_TEMPDB = ON)
	ON [LOAD]*/

	-- Check duplicates

	IF (EXISTS
	(
		SELECT obsID, fineTime, COUNT(*)
		FROM [load].[RawPointing]
		WHERE inst = 1
		GROUP BY obsID, fineTime
		HAVING COUNT(*) > 1
	))
	THROW 51000, 'Duplicate key.', 1;

	--TRUNCATE TABLE [Pointing]

	INSERT [Pointing] WITH (TABLOCKX)
		(ObsID, fineTime, inst, ra, dec, pa, av, utc)
	SELECT
		ObsID, fineTime, inst, ra, dec, pa, av, 0
	FROM [load].[RawPointing]

	--TRUNCATE TABLE [load].[RawPointing];

GO

---------------------------------------------------------------


IF OBJECT_ID ('load.DetectObservations', N'P') IS NOT NULL
DROP PROC [load].[DetectObservations]

GO

CREATE PROC [load].[DetectObservations]
AS
/*
Detect observations from raw pointings

Pointings contain obsID but velocity needs to be figured out from
the maximum of the histogram of velocities.

TODO: add minimum enclosing circle center, coverage, area, pointing count etc.

*/
	TRUNCATE TABLE Observation

	DECLARE @binsize float = 1;		-- velocity bins for histogram

	WITH
	velhist AS
	(
		SELECT obsID, inst, ROUND(av / @binsize, 0) * @binsize vvv, COUNT(*) cnt
		FROM Pointing
		GROUP BY obsID, inst, ROUND(av / @binsize, 0) * @binsize
	),
	velhistmax AS
	(
		SELECT *, ROW_NUMBER() OVER(PARTITION BY obsID ORDER BY cnt DESC) rn
		FROM velhist
		--WHERE vvv > 2	-- velocity is low at turn around
	),
	minmax AS
	(
		SELECT obsID, inst, MIN(fineTime) fineTimeStart, MAX(fineTime) fineTimeEnd
		FROM Pointing
		GROUP BY obsID, inst
	)
	INSERT Observation
		(obsID, inst, fineTimeStart, fineTimeEnd, av, region)
	SELECT
		o.obsID, o.inst, o.fineTimeStart, o.fineTimeEnd, v.vvv, NULL
	FROM minmax o
	INNER JOIN velhistmax v ON v.obsID = o.obsID AND v.rn = 1

GO


IF OBJECT_ID(N'load.FilterPointingTurnaround') IS NOT NULL
DROP FUNCTION [load].[FilterPointingTurnaround]

GO

CREATE FUNCTION [load].[FilterPointingTurnaround]
(
)
RETURNS TABLE
AS
RETURN
(
	WITH
	b AS
	(
		SELECT a.*,
			a.fineTime - LAG(a.fineTime, 1, NULL) OVER(PARTITION BY a.inst, a.ObsID ORDER BY a.fineTime) deltaLag,
			a.fineTime - LEAD(a.fineTime, 1, NULL) OVER(PARTITION BY a.inst, a.ObsID ORDER BY a.fineTime) deltaLead
		FROM Pointing a
		INNER JOIN Observation o ON o.inst = a.inst AND o.obsID = a.obsID
	)
	SELECT * FROM b
)

GO

IF OBJECT_ID(N'load.FindLegEnds') IS NOT NULL
DROP FUNCTION [load].[FindLegEnds]

GO

CREATE FUNCTION [load].[FindLegEnds]
(
	@legMinGap float
)
RETURNS TABLE
AS
RETURN
(
	WITH
	b AS
	(
		SELECT * FROM [load].FilterPointingTurnaround()
	),
	leg AS
	(
		SELECT inst, obsID,
			(ROW_NUMBER() OVER(PARTITION BY inst, obsID ORDER BY fineTime) + 1) / 2 leg,
			CASE
				WHEN (deltaLead < -@legMinGap OR deltaLead IS NULL) AND (deltaLag > @legMinGap OR deltaLag IS NULL) THEN -1
				WHEN deltaLead < -@legMinGap OR deltaLead IS NULL THEN 0
				WHEN deltaLag > @legMinGap OR deltaLag IS NULL THEN 1
				ELSE NULL
			END start,
			fineTime, ra, dec, pa
		FROM b
		WHERE (deltaLead < -@legMinGap OR deltaLead IS NULL) OR 
			  (deltaLag > @legMinGap OR deltaLag IS NULL) AND
			  NOT ((deltaLead < -@legMinGap OR deltaLead IS NULL) AND (deltaLag > @legMinGap OR deltaLag IS NULL))
	)
	SELECT * FROM leg
)

GO

IF OBJECT_ID ('load.DetectLegs', N'P') IS NOT NULL
DROP PROC [load].[DetectLegs]

GO

CREATE PROC [load].[DetectLegs]
	@legMinGap bigint = 5e6				-- minimum gap to start new leg
AS
/*
Detect scan legs from raw pointings

Raw points are filtered for scan legs. Legs are detected from gaps in pointings.
*/

	TRUNCATE TABLE [load].[LegEnds];

	INSERT [load].[LegEnds] WITH(TABLOCKX)
	SELECT inst, obsID, leg, start, fineTime, ra, dec, pa
	FROM [load].[FindLegEnds](@legMinGap)
	WHERE start IN (0, 1)
	ORDER BY obsID, fineTime;

	--TRUNCATE TABLE [load].[Leg];

	INSERT [load].[Leg] WITH(TABLOCKX)
	SELECT a.inst, a.obsID, a.legID, a.fineTime, b.fineTime, a.ra, a.dec, a.pa, b.ra, b.dec, b.pa
	FROM [load].[LegEnds] a
	INNER JOIN [load].[LegEnds] b ON a.inst = b.inst AND a.obsID = b.obsID AND a.legID = b.legID
	WHERE a.start = 1 AND b.start = 0;

	TRUNCATE TABLE [load].[LegEnds];

GO


IF OBJECT_ID ('load.GenerateFootprint', N'P') IS NOT NULL
DROP PROC [load].[GenerateFootprint]

GO

CREATE PROC [load].[GenerateFootprint]
AS
/*
	Generate regions for legs then union them into observations
*/

	TRUNCATE TABLE [load].LegRegion;

	DBCC SETCPUWEIGHT(1000); 

	INSERT [load].[LegRegion] WITH (TABLOCKX)
	SELECT leg.inst, leg.obsID, leg.legID, leg.fineTimeStart, leg.fineTimeEnd,
		   dbo.GetLegRegion(leg.raStart, leg.decStart, leg.paStart, leg.raEnd, leg.decEnd, leg.paEnd,
		    CASE inst
			WHEN 1 THEN 'PacsPhoto'
			WHEN 2 THEN 'SpirePhoto'
			END)
	FROM [load].Leg leg

	UPDATE [Observation]
	SET region = leg.region
	FROM [Observation] obs
	INNER JOIN
		(SELECT obsID, MIN(fineTimeStart) fineTimeStart, Max(fineTimeEnd) fineTimeEnd, region.UnionEvery(region) region
		 FROM [load].LegRegion
		 GROUP BY obsID) leg
		ON leg.obsID = obs.obsID;

	DBCC SETCPUWEIGHT(1); 

GO


IF OBJECT_ID ('load.GenerateHtm', N'P') IS NOT NULL
DROP PROC [load].[GenerateHtm]

GO

CREATE PROC [load].[GenerateHtm]
AS
/*
	Generate HTM from observation regions
*/

	TRUNCATE TABLE ObservationHtm;

	DBCC SETCPUWEIGHT(1000); 

	INSERT ObservationHtm WITH (TABLOCKX)
	SELECT obsID, htm.htmIDstart, htm.htmIDEnd, fineTimeStart, fineTimeEnd, htm.partial
	FROM Observation
	CROSS APPLY htm.Cover(region) htm;

	DBCC SETCPUWEIGHT(1); 

GO


IF OBJECT_ID ('load.CleanUp', N'P') IS NOT NULL
DROP PROC [load].[CleanUp]

GO

CREATE PROC [load].[CleanUp]
AS

	TRUNCATE TABLE [load].[LegRegion];

	TRUNCATE TABLE [load].[Leg]

GO