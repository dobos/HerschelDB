IF OBJECT_ID ('load.MergePointing', N'P') IS NOT NULL
DROP PROC [load].[MergePointing]

GO

CREATE PROC [load].[MergePointing]
AS
	CREATE CLUSTERED INDEX [IC_RawPointing] ON [load].[RawPointing]
	(
		[obsID] ASC,
		[fineTime] ASC
	)
	WITH (SORT_IN_TEMPDB = ON)
	ON [LOAD]

	-- Check duplicates

	IF (EXISTS
	(
		SELECT obsID, fineTime, COUNT(*)
		FROM [load].[RawPointing]
		GROUP BY obsID, fineTime
		HAVING COUNT(*) > 1
	))
	THROW 51000, 'Duplicate key.', 1;

	TRUNCATE TABLE [Pointing]

	INSERT [Pointing] WITH (TABLOCKX)
	SELECT * FROM [load].[RawPointing]

	TRUNCATE TABLE [load].[RawPointing];

GO


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
		SELECT obsID, ROUND(SQRT(avy*avy + avz*avz) / @binsize, 0) * @binsize vvv, COUNT(*) cnt
		FROM Pointing
		GROUP BY obsID, ROUND(SQRT(avy*avy + avz*avz) / @binsize, 0) * @binsize
	),
	velhistmax AS
	(
		SELECT *, ROW_NUMBER() OVER(PARTITION BY obsID ORDER BY cnt DESC) rn
		FROM velhist
		WHERE vvv > 2	-- velocity is low at turn around
	),
	minmax AS
	(
		SELECT obsID, MIN(fineTime) fineTimeStart, MAX(fineTime) fineTimeEnd
		FROM Pointing
		GROUP BY obsID
	)
	INSERT Observation
		(obsID, fineTimeOrigStart, fineTimeOrigEnd, av, fineTimeStart, fineTimeEnd, region)
	SELECT
		o.obsID, o.fineTimeStart, o.fineTimeEnd, v.vvv, 0, 0, NULL
	FROM minmax o
	INNER JOIN velhistmax v ON v.obsID = o.obsID AND v.rn = 1

GO


IF OBJECT_ID(N'load.FilterPointingTurnaround') IS NOT NULL
DROP FUNCTION [load].[FilterPointingTurnaround]

GO

CREATE FUNCTION [load].[FilterPointingTurnaround]
(
	@avDiffMax float,
	@avVarMax float
)
RETURNS TABLE
AS
RETURN
(
	WITH
	a AS	-- average velocities over +/-5 bins
	(
		SELECT
			*,
			SQRT(avy*avy + avz*avz) av,
			AVG(SQRT(avy*avy + avz*avz))
				OVER (PARTITION BY ObsID
					  ORDER BY fineTime
					  ROWS BETWEEN 5 PRECEDING
							   AND 5 FOLLOWING) av_avg,
			VAR(SQRT(avy*avy + avz*avz))
				OVER (PARTITION BY ObsID
					  ORDER BY fineTime
					  ROWS BETWEEN 5 PRECEDING
							   AND 5 FOLLOWING) av_var
		FROM Pointing
	),
	b AS
	(
		SELECT a.*,
			a.fineTime - LAG(a.fineTime, 1, NULL) OVER(PARTITION BY a.ObsID ORDER BY a.obsID, a.fineTime) deltaLag,
			a.fineTime - LEAD(a.fineTime, 1, NULL) OVER(PARTITION BY a.ObsID ORDER BY a.obsID, a.fineTime) deltaLead
		FROM a
		INNER JOIN Observation o ON o.obsID = a.obsID
		WHERE av_avg BETWEEN o.av - o.av * @avDiffMax / 100.0 AND o.av + o.av * @avDiffMax / 100.0
			AND (@avVarMax IS NULL OR av_var < @avVarMax)
	)
	SELECT * FROM b
)

GO


IF OBJECT_ID(N'load.FindLegEnds') IS NOT NULL
DROP FUNCTION [load].[FindLegEnds]

GO

CREATE FUNCTION [load].[FindLegEnds]
(
	@avDiffMax float,
	@avVarMax float,
	@legMinGap float
)
RETURNS TABLE
AS
RETURN
(
	WITH
	b AS
	(
		SELECT * FROM [load].FilterPointingTurnaround(@avDiffMax, @avVarMax)
	),
	leg AS
	(
		SELECT b.*,
			(ROW_NUMBER() OVER(PARTITION BY ObsID ORDER BY fineTime) + 1) / 2 leg,
			CASE
				WHEN (deltaLead < -@legMinGap OR deltaLead IS NULL) AND (deltaLag > @legMinGap OR deltaLag IS NULL) THEN -1
				WHEN deltaLead < -@legMinGap OR deltaLead IS NULL THEN 0
				WHEN deltaLag > @legMinGap OR deltaLag IS NULL THEN 1
				ELSE NULL
			END start
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
	@avDiffMax float = 25.0,			-- max deviation from observation velocity (turnaround removal)
	@avVarMax float = NULL,				-- max variance in velocity in moving window
	@legMinGap bigint = 5e6				-- minimum gap to start new leg
	
AS
/*
Detect scan legs from raw pointings

Raw points are filtered for scan legs. Legs are defined by almost constant
scan velocity along straight lines. Rolling average and variance of velocity
along scan curve is computed to find leg ends.
*/

	TRUNCATE TABLE [load].[LegEnds];

	INSERT [load].[LegEnds] WITH(TABLOCKX)
	SELECT obsID, leg, start, fineTime, ra, dec, pa
	FROM [load].[FindLegEnds](@avDiffMax, @avVarMax, @legMinGap)
	WHERE start IN (0, 1)
	ORDER BY obsID, fineTime;

	TRUNCATE TABLE [load].[Leg];

	INSERT [load].[Leg] WITH(TABLOCKX)
	SELECT a.obsID, a.legID, a.fineTime, b.fineTime, a.ra, a.dec, a.pa, b.ra, b.dec, b.pa
	FROM [load].[LegEnds] a
	INNER JOIN [load].[LegEnds] b ON a.obsID = b.obsID AND a.legID = b.legID
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
	SELECT leg.obsID, leg.legID, leg.fineTimeStart, leg.fineTimeEnd,
		   dbo.GetLegRegion(leg.raStart, leg.decStart, leg.paStart, leg.raEnd, leg.decEnd, leg.paEnd, 'Blue')
	FROM [load].Leg leg

	UPDATE [Observation]
	SET fineTimeStart = leg.fineTimeStart,
		fineTimeEnd = leg.fineTimeEnd,
		region = leg.region
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