IF OBJECT_ID (N'Pointing', N'U') IS NOT NULL
DROP TABLE [Pointing]

CREATE TABLE [Pointing]
(
--/ <summary>Contains raw pointings data</summary>
--/ <remarks></remarks>

	[obsID] [bigint] NOT NULL,			--/ <column>Unique ID of the observation</column>
	[fineTime] [bigint] NOT NULL,		--/ <column unit="1e-6 s">Time of pointing (TAI)</column>
	[bbID] [bigint] NOT NULL,			--/ <column></column>
	[ra] [float] NOT NULL,				--/ <column unit="deg">Right ascension of PACS instrument center</column>
	[raError] [float] NOT NULL,			--/ <column unit="deg">Error of right ascension</column>
	[dec] [float] NOT NULL,				--/ <column unit="deg">Declination of PACS instrument center</column>
	[decError] [float] NOT NULL,		--/ <column unit="deg">Error of declination</column>
	[pa] [float] NOT NULL,				--/ <column unit="deg">Position angle of PACS instrument</column>
	[paError] [float] NOT NULL,			--/ <column unit="deg">Error of position angle</column>
	[avX] [float] NOT NULL,				--/ <column unit="arcsec s-1">Telescope angular velocity X</column>
	[avXError] [float] NOT NULL,		--/ <column unit="arcsec s-1">Error of telescope velocity X</column>
	[avY] [float] NOT NULL,				--/ <column unit="arcsec s-1">Telescope angular velocity Y</column>
	[avYError] [float] NOT NULL,		--/ <column unit="arcsec s-1">Error of telescope velocity Y</column>
	[avZ] [float] NOT NULL,				--/ <column unit="arcsec s-1">Telescope angular velocity Z</column>
	[avZError] [float] NOT NULL,		--/ <column unit="arcsec s-1">Error of telescope velocity Z</column>
	[utc] [bigint] NOT NULL,			--/ <column unit="1e-6 s">Time of pointing (UTC)</column>

	CONSTRAINT [PK_Pointing] PRIMARY KEY CLUSTERED 
	(
		[obsID] ASC,
		[fineTime] ASC
	)
) ON [PRIMARY]

GO


IF OBJECT_ID (N'Observation', N'U') IS NOT NULL
DROP TABLE [Observation]

CREATE TABLE [Observation]
(
--/ <summary>Contains one entry for each PACS observation</summary>
--/ <remarks></remarks>
	
	[obsID] bigint NOT NULL,			--/ <column>Unique ID of the observation</column>
	[fineTimeOrigStart] bigint NOT NULL,--/ <column unit="10e-6 s">Start time of observation (turnarounds not removed)</column>
	[fineTimeOrigEnd] bigint NOT NULL,	--/ <column unit="10e-6 s">End time of observation (turnarounds not removed)</column>
	[av] float NOT NULL,				--/ <column unit="arcsec s-1">Target angular velocity of telescope during leg scans</column>
	[fineTimeStart] bigint NOT NULL,	--/ <column unit="10e-6 s">Start time of observation (turnarounds removed)</column>
	[fineTimeEnd] bigint NOT NULL,		--/ <column unit="10e-6 s">End time of observation (turnarounds removed)</column>
	[region] varbinary(max) NULL,		--/ <column>Footprint of the observation in binary format</column>

	CONSTRAINT [PK_Observation] PRIMARY KEY CLUSTERED 
	(
		[obsID] ASC
	)
) ON [PRIMARY]

GO


IF OBJECT_ID (N'ObservationHtm', N'U') IS NOT NULL
DROP TABLE [ObservationHtm]

CREATE TABLE [ObservationHtm]
(
--/ <summary>HTM index for observation footprints</summary>
--/ <remarks></remarks>

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

GO