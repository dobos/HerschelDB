-- This file generates simple footprints only
-- Scan maps are generated directly from C# by hload

---------------------------------------------------------------

IF OBJECT_ID ('load.GenerateFootprint_PacsSpectro', N'P') IS NOT NULL
DROP PROC [load].[GenerateFootprint_PacsPhoto]

GO

CREATE PROC [load].[GenerateFootprint_PacsPhoto]
AS
	-- PACS chop/nod single pointing

	WITH p AS
	(
		SELECT inst, obsID, point.AvgEq(ra, dec).RA ra, point.AvgEq(ra, dec).Dec dec, AVG(pa) pa
		FROM load.PointingCluster
		WHERE isRotated = 1
		GROUP BY inst, obsID
	)
	UPDATE Observation
	SET aperture = 50.0 / 3600,
		region = dbo.GetDetectorRegion(p.ra, p.dec, p.pa, 0, 'PacsPhotoChopNod')
	FROM Observation o
	INNER JOIN p ON p.inst = o.inst AND p.obsID = o.obsID
	WHERE o.inst = 1 AND o.obsType = 1 AND o.pointingMode = 0x0000000000000041

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
		  AND (instMode = 0x0000000000040021 OR instMode = 0x0000000000080021);
	-- 671

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
	    AND (instMode = 0x0000000000040021 OR instMode = 0x0000000000080021);
	-- 753

	-- chopped single point spectroscopy
	UPDATE Observation
	SET aperture = 50.0 / 3600,
		region = dbo.GetDetectorRegion(p.ra, p.dec, p.pa, 0, 'PacsSpectro')
	FROM Observation o
	INNER JOIN load.PointingCluster p
		ON p.inst = o.inst AND p.obsID = o.obsID
	WHERE o.inst = 1 AND obsType = 2 AND pointingMode = 1 
		  AND (instMode = 0x0000000000140021 OR instMode = 0x0000000000180021)
		  AND p.clusterID = 0 AND p.isRotated = 1;

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
	    AND (instMode = 0x0000000000140021 OR instMode = 0x0000000000180021);

	-- 313

GO

---------------------------------------------------------------

IF OBJECT_ID ('load.GenerateFootprint_SpireSpectro', N'P') IS NOT NULL
DROP PROC [load].[GenerateFootprint_SpireSpectro]

GO

CREATE PROC [load].[GenerateFootprint_SpireSpectro]
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
		aperture = 2.6,		-- 2 arc min in diameter
		fineTimeStart = start.fineTime,
		fineTimeEnd = stop.fineTime,
		region = dbo.GetDetectorRegion(start.ra, start.dec, 0, 2.6, 'SpireSpectro')
	FROM Observation o
	INNER JOIN start ON start.inst = o.inst AND start.obsID = o.ObsID
	INNER JOIN stop ON stop.inst = o.inst AND stop.obsID = o.ObsID	
	WHERE o.inst = 2 AND o.pointingMode = 1;

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
			region.UnionEvery(dbo.GetDetectorRegion(p.ra, p.dec, 0, 2.6, 'SpireSpectro')) AS region
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

GO

---------------------------------------------------------------
---------------------------------------------------------------

IF OBJECT_ID ('load.GetHifiAperture') IS NOT NULL
DROP FUNCTION [load].[GetHifiAperture]

GO

CREATE FUNCTION [load].[GetHifiAperture]
(
	@band varchar(2),
	@pol varchar(2)
)
RETURNS [float]
AS 
BEGIN
	
	RETURN 	
		CASE 
			WHEN @band = '1' AND @pol = 'H' THEN 43.1
			WHEN @band = '2' AND @pol = 'H' THEN 32.9
			WHEN @band = '3' AND @pol = 'H' THEN 26.3
			WHEN @band = '4' AND @pol = 'H' THEN 21.9
			WHEN @band = '5' AND @pol = 'H' THEN 19.6
			WHEN @band = '6' AND @pol = 'H' THEN 14.9
			WHEN @band = '7' AND @pol = 'H' THEN 11.1

			WHEN @band = '1' AND @pol = 'V' THEN 43.5
			WHEN @band = '2' AND @pol = 'V' THEN 32.8
			WHEN @band = '3' AND @pol = 'V' THEN 25.8
			WHEN @band = '4' AND @pol = 'V' THEN 21.7
			WHEN @band = '5' AND @pol = 'V' THEN 19.4
			WHEN @band = '6' AND @pol = 'V' THEN 14.7
			WHEN @band = '7' AND @pol = 'V' THEN 11.1

			WHEN @band = '1' AND @pol = '' THEN 43.5
			WHEN @band = '2' AND @pol = '' THEN 32.9
			WHEN @band = '3' AND @pol = '' THEN 23.6
			WHEN @band = '4' AND @pol = '' THEN 21.9
			WHEN @band = '5' AND @pol = '' THEN 19.6
			WHEN @band = '6' AND @pol = '' THEN 14.9
			WHEN @band = '7' AND @pol = '' THEN 11.1
			ELSE NULL
		END
	
END
GO



---------------------------------------------------------------


IF OBJECT_ID ('load.GenerateFootprint_HifiPointed', N'P') IS NOT NULL
DROP PROC [load].[GenerateFootprint_HifiPointed]

GO

CREATE PROC [load].[GenerateFootprint_HifiPointed]
AS

	-- HIFI pointed spectroscopy with slight offset in V and H polarizations
	WITH H AS
	(
		SELECT *,
			CASE 
				WHEN p.band = '1' AND p.pol = 'H' THEN 43.1
				WHEN p.band = '2' AND p.pol = 'H' THEN 32.9
				WHEN p.band = '3' AND p.pol = 'H' THEN 26.3
				WHEN p.band = '4' AND p.pol = 'H' THEN 21.9
				WHEN p.band = '5' AND p.pol = 'H' THEN 19.6
				WHEN p.band = '6' AND p.pol = 'H' THEN 14.9
				WHEN p.band = '7' AND p.pol = 'H' THEN 11.1
				WHEN p.pol = '-' THEN -1
			END aperture
		FROM load.HifiPointing p
		WHERE p.pol = 'H'
	),
	V AS
	(
			SELECT *,
			CASE 
				WHEN p.band = '1' AND p.pol = 'V' THEN 43.5
				WHEN p.band = '2' AND p.pol = 'V' THEN 32.8
				WHEN p.band = '3' AND p.pol = 'V' THEN 25.8
				WHEN p.band = '4' AND p.pol = 'V' THEN 21.7
				WHEN p.band = '5' AND p.pol = 'V' THEN 19.4
				WHEN p.band = '6' AND p.pol = 'V' THEN 14.7
				WHEN p.band = '7' AND p.pol = 'V' THEN 11.1
				WHEN p.pol = '-' THEN -1
			END aperture
		FROM load.HifiPointing p
		WHERE p.pol = 'V'
	),
	r AS
	(
		SELECT
			H.obsID,
			(H.aperture + V.aperture) / 2.0 AS aperture,
			
			-- ideal solution
			region.[Union](
					dbo.GetDetectorRegion(H.ra, H.dec, -1, H.aperture / 60.0 / 2.0, 'Hifi'),
					dbo.GetDetectorRegion(V.ra, V.dec, -1, V.aperture / 60.0 / 2.0, 'Hifi')) AS region

			-- a possible fix
			/*CASE WHEN point.GetAngleEq(H.ra, H.dec, V.ra, V.dec) > 0.01 THEN
				region.[Union](
					dbo.GetDetectorRegion(H.ra, H.dec, -1, H.aperture / 60.0, 'Hifi'),
					dbo.GetDetectorRegion(V.ra, V.dec, -1, V.aperture / 60.0, 'Hifi'))
				 ELSE
				    dbo.GetDetectorRegion(H.ra, H.dec, -1, H.aperture / 60.0, 'Hifi')
			END AS region*/

			-- quick solution
			--dbo.GetDetectorRegion((H.ra + V.ra) / 2.0, (H.dec + V.dec) / 2.0, -1, (H.aperture + V.aperture) / 2 / 60.0, 'Hifi') AS region

		FROM H INNER JOIN V ON H.obsID = v.obsID
	)
	UPDATE Observation
	SET aperture = r.aperture / 60.0,
		region = r.region
	FROM Observation o
	INNER JOIN r ON r.obsID = o.obsID
	WHERE o.inst = 8 AND o.pointingMode = 1

GO

---------------------------------------------------------------

IF OBJECT_ID ('load.GenerateFootprint_HifiMap', N'P') IS NOT NULL
DROP PROC [load].[GenerateFootprint_HifiMap]

GO

CREATE PROC [load].[GenerateFootprint_HifiMap]
AS

-- HIFI maps
	
	-- Rectangular corners
	UPDATE Observation
	SET region = dbo.GetMapRegion(
		m.ra, 
		m.dec, 
		90 + m.pa, 
		m.width + load.GetHifiAperture(LEFT(o.band, 1), '') / 60.0, 
		m.height + load.GetHifiAperture(LEFT(o.band, 1), '') / 60.0)
	FROM Observation o
	INNER JOIN ScanMap m ON m.inst = o.inst AND m.obsID = o.obsID
	WHERE o.inst = 8 AND o.pointingMode = 4
	AND o.band != 'none'

	-- Rounded corners - BUGGY!
	/*
	UPDATE Observation
	SET region = dbo.GetRoundedMapRegion(m.ra, m.dec, m.pa, m.width, m.height,
	    CASE 
			WHEN o.band = '1a' OR o.band = '1b' THEN 43.5
			WHEN o.band = '2a' OR o.band = '2b' THEN 32.9
			WHEN o.band = '3a' OR o.band = '3b' THEN 26.3
			WHEN o.band = '4a' OR o.band = '4b' THEN 21.9
			WHEN o.band = '5a' OR o.band = '5b' THEN 19.6
			WHEN o.band = '6a' OR o.band = '6b' THEN 14.9
			WHEN o.band = '7a' OR o.band = '7b' THEN 11.1
		END)
	FROM Observation o
	INNER JOIN ScanMap m ON m.inst = o.inst AND m.obsID = o.obsID
	WHERE o.inst = 8 AND o.pointingMode = 4*/

GO

---------------------------------------------------------------
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

	DROP INDEX [IX_ObservationHtm_ObsID] ON ObservationHtm;

	TRUNCATE TABLE ObservationHtm;

	DBCC SETCPUWEIGHT(1000); 

	INSERT ObservationHtm WITH (TABLOCKX)
	SELECT inst, obsID, htm.htmIDstart, htm.htmIDEnd, fineTimeStart, fineTimeEnd, htm.partial
	FROM Observation o
	CROSS APPLY htm.CoverAdvanced(region, 0, 0.9, 2) htm
	WHERE o.region IS NOT NULL		-- for debugging only

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
	ON [PRIMARY];

	CREATE NONCLUSTERED INDEX [IX_ObservationHtm_ObsID] ON [dbo].[ObservationHtm]
	(
		[inst] ASC,
		[obsID] ASC,
		[htmIDStart] ASC
	)
	INCLUDE ( 	
		[htmIDEnd],
		[fineTimeStart],
		[fineTimeEnd],
		[partial]
	)
	ON [PRIMARY];

GO