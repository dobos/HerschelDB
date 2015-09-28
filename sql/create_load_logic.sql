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
	WHERE (inst = 1 AND obsType = 1)			-- PACS photo
	   OR (inst = 2 AND obsType IN (1, 2, 3))	-- SPIRE photo
		-- TODO: add other instruments and obsTypes


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
		-9999 AS ra, 
		-9999 AS dec, 
		-9999 AS pa,
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

	-- Add parallel observations for both PACS and SPIRE
	INSERT Observation WITH (TABLOCKX)
	SELECT 1 AS inst, obsID,			-- PACS
		1 AS obsType,					-- Photometry
		obsLevel,
		0x15 AS instMode,				-- Pacs Parallel Photometry PacsPhotoBlue
		pointingMode, object,	
		calibration,
		ra, dec, pa,
		aperture,
		fineTimeStart, fineTimeEnd,
		repetition, aor, aot,
		NULL AS region 
	FROM Observation
	WHERE inst = 4
		
	INSERT Observation WITH (TABLOCKX)
	SELECT 2 AS inst, obsID,			-- SPIRE
		1 AS obsType,					-- Photometry
		obsLevel,
		0x16 AS instMode,				-- Pacs Parallel Photometry PacsPhotoBlue
		pointingMode, object,	
		calibration,
		ra, dec, pa,
		aperture,
		fineTimeStart, fineTimeEnd,
		repetition, aor, aot,
		NULL AS region
	FROM Observation
	WHERE inst = 4

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
	WHERE calibration = 0 AND obsLevel < 250
		AND inst IN (1, 2, 4)
		AND obsType = 1								-- only photometry
		AND pointingmode IN (8, 16)					-- line-scan and cross-scan

	-- Parallel scan maps, added for both PACS and SPIRE as individual
	-- observations
	INSERT [ScanMap] WITH (TABLOCKX)
	SELECT 1, obsID,							-- PACS
		av, height, width
	FROM [ScanMap]
	WHERE inst = 4

	INSERT [ScanMap] WITH (TABLOCKX)
	SELECT 2, obsID,							-- Spire
		av, height, width
	FROM [ScanMap]
	WHERE inst = 4

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
		AND obsType = 1								-- only spectroscopy
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
		AND obsType = 1								-- only spectroscopy
		AND (
			   inst = 1 AND (instMode & 0x00080000) > 0
			OR inst = 1 AND (instMode & 0x00040000) > 0)

	-- TODO: add SPIRE and HIFI spec

GO

---------------------------------------------------------------

-- generate footprints here

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