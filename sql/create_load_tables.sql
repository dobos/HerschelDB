IF (NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'load'))
BEGIN
	EXEC('CREATE SCHEMA [load]')
END

GO


IF OBJECT_ID (N'load.RawObservation', N'U') IS NOT NULL
DROP TABLE [load].[RawObservation]

CREATE TABLE [load].[RawObservation]
(
	[inst] tinyint NOT NULL,
	[obsID] bigint NOT NULL,
	[obsLevel] tinyint NOT NULL,
		/*
			PACS photo (13)
			PACS parallel (13)
			PACS line spec (16)
			PACS range spec (16)
			SPIRE photo (15)
			SPIRE parallel (8)
			SPIRE spectro (16)
			HIFI (10)

			-- LEVEL3_PROCESSED
			-- LEVEL2_PROCESSED
			-- LEVEL2_5_PROCESSED
			-- LEVEL1_PROCESSED
			-- LEVEL0_PROCESSED
			-- LEVEL0_5_PROCESSED
			-- CREATED
		*/
	[obsMode] tinyint NOT NULL,
		/*  
			PACS photo: (5)													20237
			-- Scan map						Mapping							18016
			-- Point-source photometry		(only calibration)				 2185
			-- Small-source photometry		(only calibration)				   23
			-- Chopped raster				(only calibration)				   13
			
			PACS parallel: (5) 
			-- Parallel Mode				Parallel						  856
			
			PACS spec line:	(7)												 2840
			-- Pointed						Pointed							 1925
			-- Pointed with dither			(only calibration)					2
			-- Mapping						Raster							  913
			
			PACS spec range: (7)											 3317
			-- Pointed						Pointed							 3142
			-- Mapping						Raster							  175
			
			SPIRE photo: (7)												 6593
			-- Point Source					Pointed							  286
			-- Small Map					Mapping							 4311
			-- Large Map					Mapping							 1908
			-- Mode: null					(only calibrations)				   88
			
			SPIRE parallel: (4)
			-- "Parallel Mode"				Parallel						  856
			
			SPIRE spec (7)													 2175
			-- "Mode: null"					(only calibration)				   29
			-- "Single Pointing"			Pointed							 2124
			-- "Raster"						Raster							   22

			HIFI (5)														 9998
			-- "Position Switch"											  920
			-- "DBS Cross fastChop"											   26
			-- "DBS Raster fastChop"										  435
			-- "DBS fastChop"												 6393
			-- "DBS slowChop"												  302
			-- "Frequency Switch Ref"										  147
			-- "Frequency Switch noRef"										  219
			-- "Load Chop													 1556
			-- "OTF - Frequency Switch Ref"									   15
			-- "OTF - Position Switch"										  539
			-- "OTF - Load Chop Ref"										  439
			-- "OTF - Load Chop noRef"										    6
				DBS: dual-beam switch
				OTF: on-the-fly
		*/
	[instMode] smallint NOT NULL,
		/*
			PACS photo: (2)													20237
			-- none															  261
			-- blue1 (70um)					PacsPhotoBlue					11114
			-- blue2 (100um)				PacsPhotoGreen					 8862

			PACS parallel: (2)												  856
			-- blue1 (70um)					PacsPhotoBlue					  597
			-- blue2 (100um)				PacsPhotoGreen					  259

			PACS line spec: (5)
			-- PacsLineSpec					PacsSpecLine					 2840

			PACS range spec: (5)
			-- PacsRangeSpec				PacsSpecRange					 3317
		*/
	[object] varchar(50) NOT NULL,
		/*
			PACS photo (14)
			PACS parallel (14)
			PACS line spec (17)
			PACS range spec (17)
			SPIRE photo (17)
			SPIRE parallel (9)
			SPIRE spec (18)
		*/
	[calibration] bit NOT NULL,
	[pointingMode] smallint NOT NULL,
		/*
			PACS photo:	(11)												20237
			-- Line_scan					LineScan						18016
			-- Nodding						(only 73 non-calib)				 2185
			-- Raster						(only calibration)				   13
			-- Nodding-raster				(only calibration)				   23

			PACS parallel: (11)
			-- Line_scan					LineScan						  856

			PACS line spec:	(10, 11)										 2840
			-- Nodding						Nodding							 1925
			-- Raster						Raster							  655
			-- Nodding-raster				NoddingRaster					  260
			(combine 10 and 11: nodding mode, chopper mode)

			PACS range spec:
			-- Nodding						Nodding							 2473
			-- Raster						Raster							  116
			-- Nodding-raster				NoddingRaster					   59
			-- Basic-fine					BasicFine						  669

			SPIRE photo: (10)												 6593
			-- Nodding						(only 4 non-calib)				  328
			   cat spire_photo_header.txt | grep Cal -v | sed -r -e 's/"([^"]*)"/xxx/g' | cut -d" " -f 1,10 | grep Nodding
			-- Cross_scan					CrossScan						 6020
			-- Line_scan					LineScan						  157
			-- Basic-fine					(only calibration)				   88

			SPIRE parallel: (7)												  856
			-- Line_scan					LineScan						  856

			SPIRE spectro: (10)												 2175
			-- No-pointing					(only calibrations)				   24
			-- Basic-fine					BasicFine						 2129
			-- Custom-map-pointing			Raster (only 16 non-calib)		   22


			http://research.uleth.ca/spire/documents/pdf/Sibthorpe_et_al2004.pdf
			SPIRE photo (5,6)												 6593
			-- POF10 = SpirePhotoSmallScan									 4269
			-- POF2  = SpirePhotoPointJiggle								  286
			-- POF3	 = SpirePhotoSmall		(only calibration)				   42
			-- POF5  = SpirePhotoLargeScan									 1908
			-- SpirePhotoSample				(only calibration)				   88

			??? Mi a parallel mód POF-ja ???
			SPIRE parallel (2)
			-- PARALLEL						PacsSpireParallel				  856

			SPIRE spectro (-)
	
			http://herschel.esac.esa.int/hcss-doc-13.0/load/pdd/html/SPIREproducts.html
			cat Spire_Spectro/spire_spectro_header.txt | grep Cal -v | sed -r -e 's/"([^"]*)"/xxx/g' | cut -d" " -f 5,6,10,12 | sort | uniq
			SOF1: Spectrometer single pointing/raster (sparse sampling) 
			SOF2: Spectrometer jiggle map/raster (intermediate or full sampling)
			SPIRE spectro: (5,6)
			-- SOF1 -- point source	(jiggle)								 1822
			-- SOF2	-- small map (scan + jiggle)							  324
			-- SpireSpectroPeakup			(only calibration)				    2
			-- SpireSpectroSample			(only calibration)				    3
			-- SpireSpectroScalOff			(only calibration)				    4
			-- SpireSpectroScalOn			(only calibration)				   20

			SPIRE spectro: (12)
			-- none
			-- sparse
			-- intermediate
			-- full

			HIFI: (3)
			-- HifiMappingModeDBSCross
			-- HifiMappingModeDBSRaster
			-- HifiMappingModeFSwitchOTF
			-- HifiMappingModeFastDBSCross
			-- HifiMappingModeFastDBSRaster
			-- HifiMappingModeLoadChopOTF
			-- HifiMappingModeLoadChopOTFNoRef
			-- HifiMappingModeOTF
			-- HifiPointModeDBS
			-- HifiPointModeFSwitch
			-- HifiPointModeFSwitchNoRef
			-- HifiPointModeFastDBS
			-- HifiPointModeLoadChop
			-- HifiPointModeLoadChopNoRef
			-- HifiPointModePositionSwitch
			-- HifiSScanModeDBS
			-- HifiSScanModeFSwitch
			-- HifiSScanModeFSwitchNoRef
			-- HifiSScanModeFastDBS
			-- HifiSScanModeLoadChop
			-- HifiSScanModeLoadChopNoRef

		*/
	[ra] float,
		/*
			PACS photo (-)
			PACS parallel (-)
			PACS line spec (-)
			SPIRE photo (2)
			SPIRE parallel (-)
			SPIRE spectro (2)
		*/
	[dec] float,
		/*
			PACS photo (-)
			PACS parallel (-)
			PACS line spec (-)
			SPIRE photo (3)
			SPIRE parallel (-)
			SPIRE spectro (3)
		*/
	[pa] float,
		/*
			PACS photo (-)
			PACS parallel (-)
			PACS line spec (-)
			SPIRE photo (4)
			SPIRE parallel (-)
			SPIRE spectro (4)
		*/
	[aperture] tinyint,
		/*
			PACS photo (-)
			PACS parallel (-)
			PACS line spec (-)

			SPIRE photo	(11)												 6593
			-- S14															 6531
			-- none															   62

			SPIRE parallel (-)
			-- ??? mi lehet ennek az S száma? S14 ???

			SPIRE spectro: (11)
			-- none
			-- S20
			-- S21
			-- S23
			-- S24
			-- S25
			-- S27
			-- S28
			-- S34
			-- S36
			-- S37
			-- S38
			-- S40
			-- S41
			-- S44
			-- S45
			-- S49
			-- S51
			-- S60

		*/
	[repetition] int,
		/*
			PACS photo (10)
			PACS parallel (10)
			PACS line spec (parse from string)
			SPIRE photo (18)
			SPIRE parallel (use from PACS)
			SPIRE spectro (19)
		*/
	[mapScanSpeed] float,
		/*
			PACS photo: (15)
			-- none
			-- low (10)
			-- medium (20)
			-- high (60)

			PACS parallel: (15)
			-- slow (20)
			-- fast (60)

			PACS line spec (-)
			PACS range spec (-)

			SPIRE photo (from pointing)
			SPIRE parallel (use from PACS)
			SPIRE spectro (-)
		*/
	[mapHeight] float,
		/*
			PACS photo (-)
			PACS parallel (-)
			PACS line spec (-)
			PACS range spec (-)
			SPIRE photo (14)
			SPIRE parallel (-)
			SPIRE spectro (14)
		*/
	[mapWidth] float,
		/*
			PACS photo (-)
			PACS parallel (-)
			PACS line spec (-)
			PACS range spec (-)
			SPIRE photo (13)
			SPIRE parallel (-)
			SPIRE spectro (13)
		*/
	[rasterNumPoint] int,
		/*
			PACS photo (-)
			PACS parallel (-)
			PACS line spec (-)
			PACS range spec (-)
			SPIRE photo (-)
			SPIRE parallel (-)
			SPIRE spectro (15)
		*/
	[rasterPointStep] float,
		/*
			PACS photo (-)
			PACS parallel (-)
			PACS line spec (12)
			PACS range spec (12)
			SPIRE photo (-)
			SPIRE parallel (-)
			SPIRE spectro (-)
		*/
	[rasterLine] int,
		/*
			PACS photo (-)
			PACS parallel (-)
			PACS line spec (from pointing)
			PACS range spec (from pointing)
			SPIRE photo (-)
			SPIRE parallel (-)
			SPIRE spectro (-)
		*/
	[rasterColumn] int,
		/*
			PACS photo (-)
			PACS parallel (-)
			PACS line spec (from pointing)
			PACS range spec (from pointing)
			SPIRE photo (-)
			SPIRE parallel (-)
			SPIRE spectro
		*/
	[specNumLine] int,
		/*
			PACS photo (-)
			PACS parallel (-)
			PACS line spec (18)
			PACS range spec
			SPIRE photo (-)
			SPIRE parallel (-)
			SPIRE spectro (-)
		*/
	[specRange] varchar(512),
		/*
			PACS photo (-)
			PACS parallel (-)
			PACS line spec (19)
			PACS range spec
			SPIRE photo (-)
			SPIRE parallel (-)
			SPIRE spectro (-)
		*/
	[specResolution] tinyint,
		/*
			PACS photo (-)
			PACS parallel (-)
			PACS line spec (-)
				 (parse from string?)

			SPIRE photo (19)
			-- none

			SPIRE parallel (-)

			SPIRE spectro: (20)												 2175
			-- none							(only 23 non-cal)				  190
			-- LR															  456
			-- MR															   74
			-- HR															 1094
			-- CR															  283
			-- H+LR															   78
		*/
	[AOR_Label] varchar(512) NOT NULL,	-- ObsCal, Calibration etc. mean calibration
		/*
			PACS photo (6)
			PACS parallel (6)
			PACS line spec (8)
			PACS range spec (8)
			SPIRE photo (8)
			SPIRE parallel (5)
			SPIRE spectro (8)
		*/
	[AOT] nvarchar(50) NOT NULL
		/*
			PACS photo (7)
			PACS parallel (7)
			PACS line spec (9)
			PACS range spec (9)
			SPIRE photo (9)
			SPIRE parallel (6)
			SPIRE spectro (9)
		*/
) ON [LOAD]

GO


IF OBJECT_ID (N'load.RawPointing', N'U') IS NOT NULL
DROP TABLE [load].[RawPointing]

CREATE TABLE [load].[RawPointing]
(
	[inst] [tinyint] NOT NULL,
	[obsID] [bigint] NOT NULL,
	[obsType] [tinyint] NOT NULL,
	[fineTime] [bigint] NOT NULL,
	[ra] [float] NOT NULL,
	[dec] [float] NOT NULL,
	[pa] [float] NOT NULL,
	[av] [float] NOT NULL
) ON [LOAD]

GO


IF OBJECT_ID (N'load.LegEnds', N'U') IS NOT NULL
DROP TABLE [load].[LegEnds]

CREATE TABLE [load].[LegEnds]
(
	[inst] tinyint,
	[obsID] bigint,
	[legID] smallint,
	[start] tinyint,
	[fineTime] bigint,
	[ra] float,
	[dec] float,
	[pa] float,

	CONSTRAINT PK_LegTemp PRIMARY KEY CLUSTERED 
	(
		[inst] ASC,
		[obsID] ASC,
		[legID] ASC,
		[start] ASC
	)
) ON [LOAD]

GO


IF OBJECT_ID (N'load.Leg', N'U') IS NOT NULL
DROP TABLE [load].[Leg]

CREATE TABLE [load].[Leg]
(
	[inst] tinyint NOT NULL,
	[obsID] bigint NOT NULL,
	[legID] smallint NOT NULL,
	[fineTimeStart] bigint NOT NULL,
	[fineTimeEnd] bigint NOT NULL,
	[raStart] float NOT NULL,
	[decStart] float NOT NULL,
	[paStart] float NOT NULL,
	[raEnd] float NOT NULL,
	[decEnd] float NOT NULL,
	[paEnd] float NOT NULL

	CONSTRAINT [PK_Leg] PRIMARY KEY CLUSTERED 
	(
		[inst] ASC,
		[obsID] ASC,
		[legID] ASC
	)
) ON [LOAD]

GO


IF OBJECT_ID (N'load.LegRegion', N'U') IS NOT NULL
DROP TABLE [load].[LegRegion]

CREATE TABLE [load].[LegRegion]
(
	[inst] tinyint NOT NULL,
	[obsID] bigint NOT NULL,
	[legID] smallint NOT NULL,
	[fineTimeStart] bigint NOT NULL,
	[fineTimeEnd] bigint NOT NULL,
	[region] varbinary(8000) NOT NULL,

	CONSTRAINT [PK_LegRegion] PRIMARY KEY CLUSTERED 
	(
		[inst] ASC,
		[obsID] ASC,
		[legID] ASC
	)
) ON [LOAD]

GO