-- This file generates simple footprints only
-- Scan maps are generated directly from C# by hload

---------------------------------------------------------------

IF OBJECT_ID ('load.GenerateFootprint', N'P') IS NOT NULL
DROP PROC [load].[GenerateFootprint]

GO

CREATE PROC [load].[GenerateFootprint]
AS

	-- SPIRE single point or jiggle spectroscopy

	WITH p AS
	(
		SELECT ROW_NUMBER() OVER (PARTITION BY inst, obsID ORDER BY fineTime) rn, *
		FROM Pointing
	),
	start AS
	(
		SELECT * FROM p WHERE rn = 1
	),
	stop AS
	(
		SELECT * FROM p WHERE rn = 2
	)
	UPDATE Observation
	SET ra = start.ra,
		dec = start.dec,
		pa = start.pa,
		aperture = 2,		-- 2 arc min in diameter
		fineTimeStart = start.fineTime,
		fineTimeEnd = stop.fineTime,
		region = dbo.GetDetectorRegion(start.ra, start.dec, 0, 'SpireSpectro')
	FROM Observation o
	INNER JOIN start ON start.inst = o.inst AND start.obsID = o.ObsID
	INNER JOIN stop ON stop.inst = o.inst AND stop.obsID = o.ObsID	
	WHERE o.inst = 2 AND o.pointingMode = 1		-- SPIRE single pointing or jiggle

	-- 5:14

GO

---------------------------------------------------------------

IF OBJECT_ID ('load.GenerateHtm', N'P') IS NOT NULL
DROP PROC [load].[GenerateHtm]

GO

CREATE PROC [load].[GenerateHtm]
AS
/*
	Generate HTM from observation regions
*/

	DROP INDEX [IX_ObservationHtm_Reverse] ON ObservationHtm;

	TRUNCATE TABLE ObservationHtm;

	DBCC SETCPUWEIGHT(1000); 

	INSERT ObservationHtm WITH (TABLOCKX)
	SELECT inst, obsID, htm.htmIDstart, htm.htmIDEnd, fineTimeStart, fineTimeEnd, htm.partial
	FROM Observation
	CROSS APPLY htm.Cover(region) htm
	WHERE region IS NOT NULL		-- for debugging only

	DBCC SETCPUWEIGHT(1); 

	CREATE NONCLUSTERED INDEX [IX_ObservationHtm_Reverse] ON [dbo].[ObservationHtm]
	(
		[htmIDEnd] ASC,
		[htmIDStart] ASC
	)
	INCLUDE ( 	
		[inst],
		[obsID],
		[fineTimeStart],
		[fineTimeEnd],
		[partial]
	)
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO