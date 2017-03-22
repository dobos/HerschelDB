---------------------------------------------------------------

IF OBJECT_ID ('load.MergeObservations', N'P') IS NOT NULL
DROP PROC [load].[MergeObservations]

GO

CREATE PROC [load].[MergeObservations]
AS
	TRUNCATE TABLE dbo.Observation

	INSERT dbo.Observation WITH (TABLOCKX)
	SELECT o.inst, 
		o.obsID, 
		obsType, 
		obsLevel,
		instMode,
		pointingMode,
		band,
		object,
		calibration,
		failed,
		sso,
		ra, 
		dec, 
		pa,
		-9999 AS aperture,
		-9999 AS fineTimeStart,
		-9999 AS fineTimeEnd,		-- fine times will be updated from pointing
		repetition, 
		ISNULL(p.proposer, ''),
		AOR_Label, 
		AOT,
		NULL AS region								-- region will be computed later
	FROM load.Observation o
	LEFT OUTER JOIN load.ObsProposer p
		ON p.inst = o.inst AND p.obsID = o.obsID

	-- Verify valid observations with missing pointings
	-- Should return 0
	-- TODO: move it to verify script
	DECLARE @nopointcount int
	SELECT @nopointcount = ISNULL(COUNT(*), 0)
	FROM dbo.Observation
	WHERE calibration = 0 AND obsLevel < 250 AND fineTimeStart = -1

	-- TODO: raise error

GO

---------------------------------------------------------------

IF OBJECT_ID ('load.MergeObservationsParallel', N'P') IS NOT NULL
DROP PROC [load].[MergeObservationsParallel]

GO

CREATE PROC [load].[MergeObservationsParallel]
AS

	-- TODO: Update repetition value of parallel

	-- Add parallel observations
	WITH pacs AS
	(
		SELECT * FROM dbo.Observation
		WHERE inst = 1
	),
	spire AS
	(
		SELECT * FROM dbo.Observation
		WHERE inst = 2
	),
	parallel AS
	(
		SELECT 
			4 AS inst,
			pacs.obsID AS obsID,
			1 AS obsType,					-- Photometry
			CASE WHEN pacs.obsLevel < spire.obsLevel THEN pacs.obsLevel
			ELSE spire.obsLevel END AS obsLevel,
			pacs.instMode | 7 AS instMode,
			32 AS pointingMode,
			pacs.band,
			pacs.object AS object,
			pacs.calibration AS calibration,
			pacs.failed | spire.failed AS failed,
			pacs.sso | spire.sso AS sso,
			pacs.ra AS ra,
			pacs.dec AS dec,
			pacs.pa AS pa,
			pacs.aperture AS aperture,
			pacs.fineTimeStart AS fineTimeStart,
			pacs.fineTimeEnd AS fineTimeEnd,
			pacs.repetition AS repetition,
			pacs.proposer AS proposer,
			pacs.aor AS aor,
			pacs.aot AS aot,
			NULL AS region
		FROM pacs
		INNER JOIN spire
			ON pacs.obsID = spire.obsID
	)
	INSERT dbo.Observation WITH (TABLOCKX)
	SELECT * FROM parallel

GO

---------------------------------------------------------------

IF OBJECT_ID ('load.UpdateObservationsPointings', N'P') IS NOT NULL
DROP PROC [load].[UpdateObservationsPointings]

GO

CREATE PROC [load].[UpdateObservationsPointings]
AS
	-- PACS needs to be filtered for turn around

	/*UPDATE Observation
	SET fineTimeStart = s.fineTimeStart,
		fineTimeEnd = s.fineTimeEnd
	FROM Observation o
	INNER JOIN 
		(
			SELECT inst, obsID,
				MIN(fineTime) AS fineTimeStart, MAX(fineTime) AS fineTimeEnd
			FROM load.Pointing
			GROUP BY inst, obsID
		) s 
			ON s.inst = o.inst AND s.obsID = o.obsID*/

	-- Update obsType and fineTime of PACS/SPIRE parallel
	UPDATE dbo.Observation
	SET obsType = 1,
		fineTimeStart = s.fineTimeStart,
		fineTimeEnd = s.fineTimeEnd
	FROM dbo.Observation o
	INNER JOIN (
		SELECT obsID, MIN(fineTime) AS fineTimeStart, MAX(fineTime) AS fineTimeEnd
		FROM load.Pointing
		--WHERE -- TODO: add filter on BBID etc
		WHERE inst IN (1, 2)
		GROUP BY inst, obsID) s 
			ON s.obsID = o.obsID
	WHERE o.inst = 4;							-- only parallel



	-- HIFI
	WITH limits AS
	(
		SELECT inst, obsID, MIN(fineTime) AS fineTimeStart, MAX(fineTime) AS fineTimeEnd,
			MAX(ra) AS ra, MAX(dec) AS dec, MAX(pa) AS pa, MAX(aperture) AS aperture
		FROM load.Pointing
		GROUP BY inst, obsID
	)
	UPDATE dbo.Observation
	SET ra = limits.ra,
		dec = limits.dec,
		pa = limits.pa,
		aperture = limits.aperture,
		fineTimeStart = limits.fineTimeStart,
	    fineTimeEnd = limits.fineTimeEnd
	FROM dbo.Observation o
	INNER JOIN limits ON limits.inst = o.inst AND limits.ObsID = o.obsID


GO

---------------------------------------------------------------

IF OBJECT_ID ('load.UpdateObservationParams', N'P') IS NOT NULL
DROP PROC [load].[UpdateObservationParams]

GO

CREATE PROC [load].[UpdateObservationParams]
AS

	-- PACS scan map

	WITH p AS
	(
		SELECT inst, obsID, 
			AVG(pa) AS pa, MIN(fineTime) AS fineTimeStart, MAX(fineTime) AS fineTimeEnd
		FROM load.Pointing
		GROUP BY inst, obsID
	)
	UPDATE dbo.Observation
	SET ra = -999,
		dec = -999,
		pa = p.pa, 
		aperture = -999, 
		fineTimeStart = p.fineTimeStart, 
		fineTimeEnd = p.fineTimeEnd
	FROM dbo.Observation o
	INNER JOIN p ON p.inst = o.inst AND p.obsID = o.obsID
	WHERE o.inst = 1 AND o.obsType = 1 AND o.pointingMode = 8;

	-- PACS chop-nod
	-- TODO: no results here with v2 pointing files
	-- no pointing clusters found for chop-nods

	WITH p AS
	(
		SELECT inst, obsID, 
			point.AvgEq(ra, dec).RA ra, point.AvgEq(ra, dec).Dec dec, AVG(pa) pa,
			MIN(fineTimeStart) AS fineTimeStart, MAX(fineTimeEnd) AS fineTimeEnd
		FROM load.PointingCluster
		WHERE isRotated = 1
		GROUP BY inst, obsID
	)
	UPDATE dbo.Observation
	SET ra = p.ra,
		dec = p.dec,
		pa = p.pa, 
		aperture = -999, 
		fineTimeStart = p.fineTimeStart, 
		fineTimeEnd = p.fineTimeEnd
	FROM dbo.Observation o
	INNER JOIN p ON p.inst = o.inst AND p.obsID = o.obsID
	WHERE o.inst = 1 AND o.obsType = 1 AND o.pointingMode = 65;

	-- PACS spectro

	WITH p AS
	(
		SELECT inst, obsID, 
			MIN(fineTimeStart) AS fineTimeStart, MAX(fineTimeEnd) AS fineTimeEnd
		FROM load.PointingCluster
		WHERE isRotated = 1
		GROUP BY inst, obsID
	)
	UPDATE dbo.Observation
	SET fineTimeStart = p.fineTimeStart, 
		fineTimeEnd = p.fineTimeEnd
	FROM dbo.Observation o
	INNER JOIN p ON p.inst = o.inst AND p.obsID = o.obsID
	WHERE o.inst = 1 AND o.obsType = 2;

	-- SPIRE scan map (small and large)

	WITH p AS
	(
		SELECT inst, obsID, 
			AVG(pa) AS pa, MIN(fineTime) AS fineTimeStart, MAX(fineTime) AS fineTimeEnd
		FROM load.Pointing
		GROUP BY inst, obsID
	)
	UPDATE dbo.Observation
	SET aperture = -999, 
		fineTimeStart = p.fineTimeStart, 
		fineTimeEnd = p.fineTimeEnd
	FROM dbo.Observation o
	INNER JOIN p ON p.inst = o.inst AND p.obsID = o.obsID
	WHERE o.inst = 2 AND o.obsType = 1 AND o.pointingMode IN (8, 16);

	-- SPIRE parallel (copy from PACS)

	UPDATE o
	SET ra = -999,
		dec = -999,
		pa = pacs.pa,
		aperture = -999,
		fineTimeStart = pacs.fineTimeStart,
		fineTimeEnd = pacs.fineTimeEnd,
		repetition = -999
	FROM dbo.Observation o
	INNER JOIN dbo.Observation pacs ON pacs.inst = 1 AND pacs.obsID = o.obsID
	WHERE o.inst = 2 AND o.obsType = 1 AND o.pointingMode = 32;

	-- SPIRE spectro raster

	WITH p AS
	(
		SELECT inst, obsID, 
			AVG(pa) AS pa, MIN(fineTime) AS fineTimeStart, MAX(fineTime) AS fineTimeEnd
		FROM load.Pointing
		GROUP BY inst, obsID
	)
	UPDATE dbo.Observation
	SET aperture = 2.6, 
		fineTimeStart = p.fineTimeStart, 
		fineTimeEnd = p.fineTimeEnd
	FROM Observation o
	INNER JOIN p ON p.inst = o.inst AND p.obsID = o.obsID
	WHERE o.inst = 2 AND o.obsType = 2 AND o.pointingMode = 2;

	-- Parallel scan maps (copy from PACS)

	UPDATE o
	SET ra = -999,
		dec = -999,
		pa = pacs.pa,
		aperture = -999,
		fineTimeStart = pacs.fineTimeStart,
		fineTimeEnd = pacs.fineTimeEnd,
		repetition = -999
	FROM dbo.Observation o
	INNER JOIN dbo.Observation pacs ON pacs.inst = 1 AND pacs.obsID = o.obsID
	WHERE o.inst = 4 AND o.obsType = 1 AND o.pointingMode = 32;

---------------------------------------------------------------

IF OBJECT_ID ('load.UpdateObservationsFlags', N'P') IS NOT NULL
DROP PROC [load].[UpdateObservationsFlags]

GO

CREATE PROC [load].[UpdateObservationsFlags]
AS

	-- Update calibration flag based on proposer

	UPDATE Observation
	SET calibration = 1
	FROM Observation o
	INNER JOIN load.ObsProposer p
		ON o.inst = p.inst AND o.obsID = p.obsID
	WHERE LEFT(p.proposer, 5) = 'calib'

	-- Update failed flags

	UPDATE Observation
	SET failed = 1
	FROM Observation o
	INNER JOIN load.ObsQuality q
		ON q.inst = o.inst AND q.obsID = o.obsID
	WHERE q.failed = 1

	-- Update SSO flags

	UPDATE Observation
	SET sso = 1
	FROM Observation o
	INNER JOIN load.ObsSSO q
		ON q.inst = o.inst AND q.obsID = o.obsID
	WHERE q.sso = 1

	-- Update individual observations with wrong values

	-- Set funny obsID to calibration
	UPDATE Observation
	SET calibration = 0,
	    failed = 1,
		sso = 0
	WHERE inst = 1 AND obsID = 1342270750		-- this is a weird one with a parabola trajectory

	-- raster instead of single pointing
	UPDATE Observation
	SET pointingMode = 4
	WHERE inst = 1 AND obsID = 1342182010

	-- Missing SSO flags
	UPDATE Observation
	SET sso = 1
	WHERE inst = 1 AND obsID IN
	(
		1342199882,
		1342207192,
		1342228529,
		1342231008,
		1342231009,
		1342209013,
		1342231306,

		1342186802,
		1342199882,
		1342199883,
		1342199884,
		1342209012,
		1342209391,
		1342209392,
		1342209393,
		1342209394,
		1342210191,
		1342211198,
		1342212575,
		1342224008,
		1342227466,
		1342228529,
		1342228530,
		1342231304,
		1342231305,
		1342231309,
		1342231310,
		1342231956,
		1342231957,
		1342237588,
		1342248741
	)

	-- Add missing SPIRE calibration flags
	UPDATE Observation
	SET calibration = 1
	WHERE inst = 2 AND obsID IN
		(1342189688, 1342189699, 1342189701)

	UPDATE Observation
	SET sso = 1
	WHERE inst = 2 AND obsID IN
	(
		1342189688		-- 3 Juno
	)

	-- Missing HIFI flags
	UPDATE Observation
	SET failed = 1
	WHERE inst = 8 AND obsID IN
		(1342251551)

GO

---------------------------------------------------------------

IF OBJECT_ID ('load.MergeScanMaps', N'P') IS NOT NULL
DROP PROC [load].[MergeScanMaps]

GO

CREATE PROC [load].[MergeScanMaps]
AS
	TRUNCATE TABLE [ScanMap]

	-- Scan maps
	INSERT [ScanMap] WITH (TABLOCKX)
	SELECT inst, obsID, 
		ISNULL(mapScanSpeed, -1) AS av,				-- will be updated from footprint
		ISNULL(ra, -1) AS ra,
		ISNULL(dec, -1) AS dec,
		ISNULL(mapHeight, -1) AS height,			-- will be updated from footprint
		ISNULL(mapWidth, -1) AS width,				-- will be updated from footprint
		ISNULL(pa, -1) AS pa
	FROM load.Observation
	WHERE inst IN (1, 2, 4)	AND obsType = 1	AND pointingmode IN (8, 16, 32)	


	-- Update AV for spire scan maps
	DECLARE @binsize float = 1;

	WITH
	velhist AS
	(
		SELECT inst, obsID, ROUND(av / @binsize, 0) * @binsize vvv, COUNT(*) cnt
		FROM load.Pointing
		GROUP BY inst, obsID, ROUND(av / @binsize, 0) * @binsize
	),
	velhistmax AS
	(
		SELECT *, ROW_NUMBER() OVER(PARTITION BY obsID ORDER BY cnt DESC) rn
		FROM velhist
		--WHERE vvv > 2	-- velocity is low at turn around
	)
	UPDATE ScanMap
	SET	av = v.vvv
	FROM ScanMap s
	INNER JOIN Observation o
		ON o.inst = s.inst AND o.obsID = s.obsID
	INNER JOIN velhistmax v
		ON v.inst = s.inst AND v.obsID = s.obsID AND v.rn = 1
	WHERE o.inst = 2 AND o.pointingMode IN (8, 16);		-- SPIRE scan maps


	-- HIFI

	-- Use WCF header (wrong!)
	INSERT [ScanMap] WITH (TABLOCKX)
	SELECT inst, o.obsID, 
		ISNULL(mapScanSpeed, -1) AS av,
		ISNULL(p.ra, -1) AS ra,
		ISNULL(p.dec, -1) AS dec,
		CASE 
			WHEN o.mapWidth != 0 OR o.mapHeight != 0 THEN o.mapHeight	-- HIPE header
			ELSE ISNULL(ABS(p.height) * 60, -1)							-- WCF header
		END AS height,
		CASE 
			WHEN o.mapWidth != 0 OR o.mapHeight != 0 THEN o.mapWidth	-- HIPE header
			ELSE ISNULL(ABS(p.width) * 60, -1)							-- WCF header
		END AS width,
		ISNULL(a.flyAngle, -1) AS pa
	FROM load.Observation o
	INNER JOIN load.HifiPointing p ON p.obsID = o.obsID
	INNER JOIN load.HifiAngle a ON a.obsID = o.obsID
	WHERE inst IN (8) AND obsType = 2 AND pointingmode IN (4);

	-- Parallel scan maps from PACS and SPIRE
	WITH pacs AS
	(
		SELECT * FROM ScanMap
		WHERE inst = 1
	),
	spire AS
	(
		SELECT * FROM ScanMap
		WHERE inst = 2
	),
	parallel AS
	(
		SELECT 
			4 AS inst,
			pacs.obsID,
			pacs.av,
			pacs.ra,
			pacs.dec,
			pacs.height,
			pacs.width,
			pacs.pa
		FROM pacs
		INNER JOIN spire
			ON pacs.obsID = spire.obsID
	)
	INSERT [ScanMap] WITH (TABLOCKX)
	SELECT * FROM parallel
GO

---------------------------------------------------------------

IF OBJECT_ID ('load.MergeRasterMaps', N'P') IS NOT NULL
DROP PROC [load].[MergeRasterMaps]

GO

CREATE PROC [load].[MergeRasterMaps]
AS

	TRUNCATE TABLE [RasterMap]

	-- Rasters
	INSERT [RasterMap] WITH (TABLOCKX)
	SELECT inst, obsID,
		rasterPointStep AS [step],
		rasterLine AS [line],
		rasterColumn AS [column],
		rasterNumPoint AS [num],
		ra, dec, pa
	FROM load.Observation
	WHERE calibration = 0 AND obsLevel < 250
		AND obsType = 2								-- only spectroscopy
		AND inst IN (1, 2)							-- TODO: add HIFI maps
		AND pointingMode IN (4)

GO

---------------------------------------------------------------

IF OBJECT_ID ('load.MergeSpectro', N'P') IS NOT NULL
DROP PROC [load].[MergeSpectro]

GO

CREATE PROC [load].[MergeSpectro]
AS

	TRUNCATE TABLE [Spectro]

	-- Line and rande spectra (only PACS)
	INSERT [Spectro] WITH (TABLOCKX)
	SELECT inst, obsID,
		aperture,
		specNumLine AS num,
		specRangeFrom AS lambdaFrom,
		specRangeTo AS lambdaTo,
		specRange2From AS lambda2From,
		specRange2To AS lambda2To,
		specRangeID AS rangeID
	FROM load.Observation
	WHERE obsType = 2								-- only spectroscopy
		AND inst IN (1, 2, 8)

GO


---------------------------------------------------------------

IF OBJECT_ID ('load.MergeSso', N'P') IS NOT NULL
DROP PROC [load].[MergeSso]

GO

CREATE PROC [load].[MergeSso]
AS

	TRUNCATE TABLE dbo.Sso

	INSERT dbo.Sso WITH (TABLOCKX)
	SELECT
		1, obsID,
		ROW_NUMBER() OVER (PARTITION BY obsID ORDER BY name) AS ssoID,
		name,
		coverage, mag, hh, r0, delta, ra, dec, pm_ra, pm_dec, pm,
		alpha, flux, g_slope, eta, pv
	FROM load.Sso

GO

---------------------------------------------------------------

IF OBJECT_ID ('load.CleanUp', N'P') IS NOT NULL
DROP PROC [load].[CleanUp]

GO

CREATE PROC [load].[CleanUp]
AS
	TRUNCATE TABLE [load].[RawObservation];

	TRUNCATE TABLE [load].[RawPointing];

	TRUNCATE TABLE [load].[LegRegion];

	TRUNCATE TABLE [load].[LegEnds]

	TRUNCATE TABLE [load].[Leg]

GO