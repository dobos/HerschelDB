-- This file generates simple footprints only
-- Scan maps are generated directly from C# by hload

---------------------------------------------------------------

IF OBJECT_ID ('load.GenerateFootprint_PacsSpectro', N'P') IS NOT NULL
DROP PROC [load].[GenerateFootprint_PacsPhoto]

GO

CREATE PROC [load].[GenerateFootprint_PacsPhoto]
AS

-- TODO: implement PACS single pointing photometry

GO

---------------------------------------------------------------

IF OBJECT_ID ('load.GenerateFootprint_PacsSpectro', N'P') IS NOT NULL
DROP PROC [load].[GenerateFootprint_PacsSpectro]

GO

CREATE PROC [load].[GenerateFootprint_PacsSpectro]
AS

	-- unchopped single point spectroscopy (5 x 5 spaxels)
	-- inst = 1, obsType = 2, pointingMode = 1
	UPDATE Observation
	SET aperture = 50.0 / 3600,
		region = dbo.GetDetectorRegion(p.ra, p.dec, p.pa, 0, 'PacsSpectro')
	FROM Observation o
	INNER JOIN load.PointingCluster p
		ON p.inst = o.inst AND p.obsID = o.obsID
	WHERE o.inst = 1 AND obsType = 2 AND pointingMode = 1 
		  AND (instMode = 0x0000000000040021 OR instMode = 0x0000000000080021)

	-- unchopped raster spectroscopy
	-- inst = 1, obsType = 2, pointingMode = 4
	WITH r AS
	(
		SELECT inst, obsID, region.UnionEvery(dbo.GetDetectorRegion(ra, dec, pa, 0, 'PacsSpectro')) AS region
		FROM load.PointingCluster
		GROUP BY inst, obsID
	)
	UPDATE Observation
	SET aperture = 50.0 / 3600,
		region = r.region
	FROM Observation o
	INNER JOIN r ON r.inst = o.inst AND r.obsID = o.obsID
	WHERE o.inst = 1 AND o.obsType = 2 AND o.pointingMode IN (2, 4)
	    AND (instMode = 0x0000000000040021 OR instMode = 0x0000000000080021)

	-- chopped single point spectroscopy
	UPDATE Observation
	SET aperture = 50.0 / 3600,
		region = dbo.GetDetectorRegion(p.ra, p.dec, p.pa, 0, 'PacsSpectro')
	FROM Observation o
	INNER JOIN load.PointingCluster p
		ON p.inst = o.inst AND p.obsID = o.obsID
	WHERE o.inst = 1 AND obsType = 2 AND pointingMode = 1 
		  AND (instMode = 0x0000000000140021 OR instMode = 0x0000000000180021)
		  AND p.clusterID = 0 AND p.isRotated = 1

	-- 3964

	-- chopped raster spectroscopy
	-- inst = 1, obsType = 2, pointingMode = 4
	WITH r AS
	(
		SELECT inst, obsID, region.UnionEvery(dbo.GetDetectorRegion(ra, dec, pa, 0, 'PacsSpectro')) AS region
		FROM load.PointingCluster
		WHERE groupID = 0 AND isRotated = 1
		GROUP BY inst, obsID
	)
	UPDATE Observation
	SET aperture = 50.0 / 3600,
		region = r.region
	FROM Observation o
	INNER JOIN r ON r.inst = o.inst AND r.obsID = o.obsID
	WHERE o.inst = 1 AND o.obsType = 2 AND o.pointingMode IN (2, 4)
	    AND (instMode = 0x0000000000140021 OR instMode = 0x0000000000180021)

	-- 313

GO

IF OBJECT_ID ('load.GenerateFootprint', N'P') IS NOT NULL
DROP PROC [load].[GenerateFootprint]

GO

CREATE PROC [load].[GenerateFootprint]
AS

	

	-- SPIRE single point or jiggle spectroscopy

	WITH p AS
	(
		SELECT ROW_NUMBER() OVER (PARTITION BY inst, obsID ORDER BY fineTime) rn, *
		FROM load.Pointing
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
		region = dbo.GetDetectorRegion(start.ra, start.dec, 0, 0, 0, 0, 0, 'SpireSpectro')
	FROM Observation o
	INNER JOIN start ON start.inst = o.inst AND start.obsID = o.ObsID
	INNER JOIN stop ON stop.inst = o.inst AND stop.obsID = o.ObsID	
	WHERE o.inst = 2 AND o.pointingMode = 1;		-- SPIRE single pointing or jiggle

	-- SPIRE raster spectroscopy

	WITH p AS
	(
		SELECT ROW_NUMBER() OVER (PARTITION BY p.inst, p.obsID ORDER BY p.fineTime) rn, p.*
		FROM load.Pointing p
		INNER JOIN Observation o
			ON o.inst = p.inst AND o.obsID = p.obsID
		WHERE o.inst = 2 AND o.pointingMode = 2 AND p.obsType = 32
	),
	limits AS
	(
		SELECT p.inst, p.obsID,
			MIN(fineTime) AS fineTimeStart,
			MAX(fineTime) AS fineTimeEnd,
			COUNT(*) AS cnt
		FROM p
		GROUP BY p.inst, p.obsID
	),
	r AS
	(
		SELECT p.inst, p.obsID,
			region.UnionEvery(dbo.GetDetectorRegion(p.ra, p.dec, 0, 'SpireSpectro')) AS region
		FROM p
		INNER JOIN Observation o
			ON o.inst = p.inst AND o.obsID = p.obsID
		INNER JOIN limits
			ON limits.inst = o.inst AND limits.obsID = o.ObsID
		WHERE p.rn % 2 = 1		-- take only start position
		GROUP BY p.inst, p.obsID
	)
	UPDATE Observation
	SET ra = -1,
		dec = -1,
		pa = -1,
		aperture = -1,
		fineTimeStart = limits.fineTimeStart,
		fineTimeEnd = limits.fineTimeEnd,
		region = r.region
	FROM Observation o
	INNER JOIN limits ON limits.inst = o.inst AND limits.obsID = o.ObsID
	INNER JOIN r ON r.inst = o.inst AND r.obsID = o.obsID
	WHERE o.inst = 2 AND o.pointingMode = 2		-- SPIRE raster spectro
		AND o.repetition != 0;

	-- HIFI pointed spectroscopy

	WITH p AS
	(
		SELECT ROW_NUMBER() OVER (PARTITION BY p.inst, p.obsID ORDER BY p.fineTime) rn, p.*
		FROM load.Pointing p
		INNER JOIN Observation o
			ON o.inst = p.inst AND o.obsID = p.obsID
		WHERE o.inst = 8 AND o.pointingMode = 1
	),
	limits AS
	(
		SELECT * FROM p WHERE rn = 1
	)
	UPDATE Observation
	SET region = dbo.GetDetectorRegion(limits.ra, limits.dec, limits.pa, 0.3 / 60.0, 'SpireSpectro')
	FROM Observation o
	INNER JOIN limits ON limits.inst = o.inst AND limits.obsID = o.obsID
	WHERE o.inst = 8 AND o.pointingMode = 1
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