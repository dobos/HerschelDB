/*
TODO: delete
IF OBJECT_ID ('load.MergePointing', N'P') IS NOT NULL
DROP PROC [load].[MergePointing]

GO

CREATE PROC [load].[MergePointing]
AS

	TRUNCATE TABLE [Pointing]

	INSERT [Pointing] WITH (TABLOCKX)
		(inst, ObsID, obsType, fineTime, BBID, ra, dec, pa, av)
	SELECT inst, ObsID, obsType, fineTime, BBID, ra, dec, pa, av
	FROM [load].[RawPointing]
	WHERE (inst = 1 AND obsType IN (1))			-- PACS photo
	   OR (inst = 2 AND obsType IN (1, 2, 3))	-- SPIRE photo (small, large, both)


	INSERT [Pointing] WITH (TABLOCKX)
		(inst, ObsID, obsType, fineTime, BBID, ra, dec, pa, av)
	SELECT inst, ObsID, obsType, fineTime, BBID, ra, dec, pa, av
	FROM [load].[RawPointing]
	WHERE inst = 2 AND obsType IN (4)		-- SPIRE spectro (point)
		
	-- Avoid repetitions here
	INSERT [Pointing] WITH (TABLOCKX)
		(inst, ObsID, obsType, fineTime, BBID, ra, dec, pa, av)
	SELECT inst, ObsID, obsType, fineTime, BBID, ra, dec, pa, av
	FROM [load].[RawPointing]
	WHERE inst = 2 AND obsType IN (8)		-- SPIRE spectro (jiggle7)
		  AND obsID NOT IN (SELECT obsID FROM Pointing WHERE inst = 2)
*/
/*
inst	obsType	(No column name)
1	1	288249679
1	2	93443502
1	4	72493427
2	1	50760930
2	2	16675956
2	3	96230561
2	4	4336
2	8	654
*/

GO

---------------------------------------------------------------

IF OBJECT_ID ('load.MergeObservations', N'P') IS NOT NULL
DROP PROC [load].[MergeObservations]

GO

CREATE PROC [load].[MergeObservations]
AS
	TRUNCATE TABLE Observation

	INSERT Observation WITH (TABLOCKX)
	SELECT inst, obsID, 
		obsType, 
		obsLevel,
		instMode,
		pointingMode,
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
		repetition, AOR_Label, AOT,
		NULL AS region								-- region will be computed later
	FROM load.RawObservation

	-- Set funny obsID to calibration
	UPDATE Observation
	SET calibration = 1
	WHERE inst = 1 AND obsID = 1342270750		-- this is a weird one with a parabola trajectory

	-- Update failed and SSO flags

	UPDATE Observation
	SET failed = 1
	FROM Observation o
	INNER JOIN load.ObsQuality q
		ON q.inst = o.inst AND q.obsID = o.obsID
	WHERE q.failed = 1

	UPDATE Observation
	SET sso = 1
	FROM Observation o
	INNER JOIN load.ObsSSO q
		ON q.inst = o.inst AND q.obsID = o.obsID
	WHERE q.sso = 1

	-- TODO: Update repetition value of parallel

	-- Update obsType and fineTime from pointings (all observations)
	-- TODO: fineTime of scan maps will need to be updated once scan legs
	--       are identified and turn-around filtering is done
	UPDATE Observation
	SET fineTimeStart = s.fineTimeStart,
		fineTimeEnd = s.fineTimeEnd
	FROM Observation o
	INNER JOIN 
		(
			SELECT inst, obsID,
				MIN(fineTime) AS fineTimeStart, MAX(fineTime) AS fineTimeEnd
			FROM Pointing
			GROUP BY inst, obsID
		) s 
			ON s.inst = o.inst AND s.obsID = o.obsID

	-- Verify valid observations with missing pointings
	-- Should return 0
	-- TODO: move it to verify script
	DECLARE @nopointcount int
	SELECT @nopointcount = ISNULL(COUNT(*), 0)
	FROM Observation
	WHERE calibration = 0 AND obsLevel < 250 AND fineTimeStart = -1

	-- TODO: raise error

	-- Update obsType and fineTime of PACS/SPIRE parallel
	UPDATE Observation
	SET obsType = 1,
		fineTimeStart = s.fineTimeStart,
		fineTimeEnd = s.fineTimeEnd
	FROM Observation o
	INNER JOIN (
		SELECT obsID, MIN(fineTime) AS fineTimeStart, MAX(fineTime) AS fineTimeEnd
		FROM Pointing
		--WHERE -- TODO: add filter on BBID etc
		WHERE inst IN (1, 2)
		GROUP BY inst, obsID) s 
			ON s.obsID = o.obsID
	WHERE o.inst = 4							-- only parallel

	-- Add parallel observations
	WITH pacs AS
	(
		SELECT * FROM Observation
		WHERE inst = 1
	),
	spire AS
	(
		SELECT * FROM Observation
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
			pacs.aor AS aor,
			pacs.aot AS aot,
			NULL AS region
		FROM pacs
		INNER JOIN spire
			ON pacs.obsID = spire.obsID
	)
	INSERT Observation WITH (TABLOCKX)
	SELECT * FROM parallel

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
		ISNULL(mapHeight, -1) AS height,			-- will be updated from footprint
		ISNULL(mapWidth, -1) AS width				-- will be updated from footprint
	FROM load.RawObservation
	WHERE inst IN (1, 2, 4)
		AND obsType = 1								-- only photometry
		AND pointingmode IN (8, 16, 32)				-- line-scan, cross-scan and parallel

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
			pacs.height,
			pacs.width
		FROM pacs
		INNER JOIN spire
			ON pacs.obsID = spire.obsID
	)
	INSERT [ScanMap] WITH (TABLOCKX)
	SELECT * FROM parallel

	-- Update AV for spire scan maps
	DECLARE @binsize float = 1;

	WITH
	velhist AS
	(
		SELECT inst, obsID, ROUND(av / @binsize, 0) * @binsize vvv, COUNT(*) cnt
		FROM Pointing
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
	WHERE o.inst = 2 AND o.pointingMode IN (8, 16)		-- SPIRE scan maps

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
	FROM load.RawObservation
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
	FROM load.RawObservation
	WHERE calibration = 0 AND obsLevel < 250
		AND obsType = 2								-- only spectroscopy
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