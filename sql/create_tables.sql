IF OBJECT_ID (N'Pointing', N'U') IS NOT NULL
DROP TABLE [Pointing]

CREATE TABLE [Pointing]
(
--/ <summary>Contains raw pointings data</summary>
--/ <remarks></remarks>
	[inst] [tinyint] NOT NULL,			--/ <column>Instrument</column>
	[obsID] [bigint] NOT NULL,			--/ <column>Unique ID of the observation</column>
	[obsType] [tinyint] NOT NULL,		--/ <column>Observations type: photometry or spectroscopy</column>
	[fineTime] [bigint] NOT NULL,		--/ <column unit="1e-6 s">Time of pointing (TAI)</column>
	[BBID] [bigint] NOT NULL,			--/ <column>BBID</column>
	[ra] [float] NOT NULL,				--/ <column unit="deg">Right ascension of PACS instrument center</column>
	[dec] [float] NOT NULL,				--/ <column unit="deg">Declination of instrument center</column>
	[pa] [float] NOT NULL,				--/ <column unit="deg">Position angle of instrument</column>
	[av] [float] NOT NULL,				--/ <column unit="arcsec s-1">Telescope's angular velocity</column>

	CONSTRAINT [PK_Pointing] PRIMARY KEY CLUSTERED 
	(
		[inst] ASC,
		[obsID] ASC,
		[obsType] ASC,
		[fineTime] ASC
	)
) ON [PRIMARY]

GRANT SELECT ON [Pointing] TO [User]

GO


IF OBJECT_ID (N'Observation', N'U') IS NOT NULL
DROP TABLE [Observation]

CREATE TABLE [Observation]
(
--/ <summary>Contains one entry for each observation</summary>
--/ <remarks></remarks>
	
	[inst] tinyint NOT NULL,			--/ <column>Instrument</column>
	[obsID] bigint NOT NULL,			--/ <column>Unique ID of the observation</column>
	[obsType] tinyint NOT NULL,			--/ <column>Observation type, bit values depend on instrument</column>
	[obsLevel] tinyint NOT NULL,		--/ <column>Maximum level of processing</column>
	[instMode] int NOT NULL,			--/ <column>Instrument mode, bit values depend on instrument</column>
	[pointingMode] tinyint NOT NULL,	--/ <column>Pointing mode</column>
	[object] varchar(50) NOT NULL,		--/ <column>Name of the observed object</column>
	[calibration] bit NOT NULL,
	[ra] float NOT NULL,
	[dec] float NOT NULL,
	[pa] float NOT NULL,
	[aperture] float NOT NULL,
	[fineTimeStart] bigint NOT NULL,	--/ <column unit="10e-6 s">Start time of observation (turnarounds removed)</column>
	[fineTimeEnd] bigint NOT NULL,		--/ <column unit="10e-6 s">End time of observation (turnarounds removed)</column>
	[repetition] int NOT NULL,			--/ <column>Number of repetitions of the observation</column>
	[aor] varchar(512) NOT NULL,		--/ <column>Astronomical observation request</column>
	[aot] varchar(50) NOT NULL,			--/ <column>Astronomical observation template</column>
	[region] varbinary(max) NULL,		--/ <column>Footprint of the observation in binary format</column>

	CONSTRAINT [PK_Observation] PRIMARY KEY CLUSTERED 
	(
		[inst] ASC,
		[obsID] ASC
	)
) ON [PRIMARY]

GRANT SELECT ON [Observation] TO [User]

GO

---------------------------------------------------------------

IF OBJECT_ID (N'ScanMap', N'U') IS NOT NULL
DROP TABLE [ScanMap]

GO

CREATE TABLE [ScanMap]
(
	[inst] tinyint NOT NULL,			--/ <column>Instrument</column>
	[obsID] bigint NOT NULL,			--/ <column>Unique ID of the observation</column>
	[av] float NOT NULL,				--/ <column unit="arcsec s-1">Target angular velocity of telescope during leg scans</column>
	[height] float NOT NULL,
	[width] float NOT NULL

	CONSTRAINT [PK_ScanMap] PRIMARY KEY CLUSTERED 
	(
		[inst] ASC,
		[obsID] ASC
	)
)

GRANT SELECT ON [ScanMap] TO [User]

GO

---------------------------------------------------------------

IF OBJECT_ID (N'RasterMap', N'U') IS NOT NULL
DROP TABLE [RasterMap]

GO

CREATE TABLE [RasterMap]
(
	[inst] tinyint NOT NULL,			--/ <column>Instrument</column>
	[obsID] bigint NOT NULL,			--/ <column>Unique ID of the observation</column>
	[step] float NOT NULL,
	[line] int NOT NULL,
	[column] int NOT NULL,
	[num] int NOT NULL,
	[ra] float NOT NULL,
	[dec] float NOT NULL,
	[pa] float NOT NULL,

	CONSTRAINT [PK_RasterMap] PRIMARY KEY CLUSTERED 
	(
		[inst] ASC,
		[obsID] ASC
	)
)

GRANT SELECT ON [RasterMap] TO [User]

GO

---------------------------------------------------------------

IF OBJECT_ID (N'Spectro', N'U') IS NOT NULL
DROP TABLE [Spectro]

GO

CREATE TABLE [Spectro]
(
	[inst] tinyint NOT NULL,			--/ <column>Instrument</column>
	[obsID] bigint NOT NULL,			--/ <column>Unique ID of the observation</column>
	[aperture] float NOT NULL,
	[num] int NOT NULL,
	[lambdaFrom] float NOT NULL,
	[lambdaTo] float NOT NULL,
	[lambda2From] float NOT NULL,
	[lambda2To] float NOT NULL,
	[rangeID] varchar(512) NULL,

	CONSTRAINT [PK_Spectro] PRIMARY KEY CLUSTERED 
	(
		[inst] ASC,
		[obsID] ASC
	)
)


GRANT SELECT ON [Spectro] TO [User]

---------------------------------------------------------------


IF OBJECT_ID (N'ObservationHtm', N'U') IS NOT NULL
DROP TABLE [ObservationHtm]

CREATE TABLE [ObservationHtm]
(
--/ <summary>HTM index for observation footprints</summary>
--/ <remarks></remarks>

	[inst] tinyint NOT NULL,			--/ <column>Instrument</column>
	[obsID] bigint NOT NULL,			--/ <column>Unique ID of the observation</column>
	[htmIDStart] bigint NOT NULL,		--/ <column>HTM ID range start</column>
	[htmIDEnd] bigint NOT NULL,			--/ <column>HTM ID range end</column>
	[fineTimeStart] bigint NOT NULL,	--/ <column unit="10e-6 s">Start time of observation (turnarounds removed)</column>
	[fineTimeEnd] bigint NOT NULL,		--/ <column unit="10e-6 s">End time of observation (turnarounds removed)</column>
	[partial] bit NOT NULL				--/ <column>Flag equals zero is HTM trixel is on boundary</column>
)

CREATE CLUSTERED INDEX [CI_ObservationHtm] ON [ObservationHtm]
(
	[htmIDStart] ASC,
	[htmIDEnd] ASC
)

CREATE NONCLUSTERED INDEX [IX_ObservationHtm_Reverse] ON [dbo].[ObservationHtm]
(
	[htmIDEnd] ASC,
	[htmIDStart] ASC
)
INCLUDE 
( 	[inst],
	[obsID],
	[fineTimeStart],
	[fineTimeEnd],
	[partial]
) WITH (SORT_IN_TEMPDB = ON)

GO

GRANT SELECT ON [ObservationHtm] TO [User]

GO