IF OBJECT_ID (N'Pointing', N'U') IS NOT NULL
DROP TABLE [Pointing]

CREATE TABLE [Pointing]
(
--/ <summary>Contains raw pointings data</summary>
--/ <remarks></remarks>
	[inst] [tinyint] NOT NULL,			--/ <column>Instrument</column>
	[obsID] [bigint] NOT NULL,			--/ <column>Unique ID of the observation</column>
	[fineTime] [bigint] NOT NULL,		--/ <column unit="1e-6 s">Time of pointing (TAI)</column>
	[ra] [float] NOT NULL,				--/ <column unit="deg">Right ascension of PACS instrument center</column>
	[dec] [float] NOT NULL,				--/ <column unit="deg">Declination of instrument center</column>
	[pa] [float] NOT NULL,				--/ <column unit="deg">Position angle of instrument</column>
	[av] [float] NOT NULL,				--/ <column unit="arcsec s-1">Telescope's angular velocity</column>

	CONSTRAINT [PK_Pointing] PRIMARY KEY CLUSTERED 
	(
		[inst] ASC,
		[obsID] ASC,
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
	[fineTimeStart] bigint NOT NULL,	--/ <column unit="10e-6 s">Start time of observation (turnarounds removed)</column>
	[fineTimeEnd] bigint NOT NULL,		--/ <column unit="10e-6 s">End time of observation (turnarounds removed)</column>
	[av] float NOT NULL,				--/ <column unit="arcsec s-1">Target angular velocity of telescope during leg scans</column>
	[region] varbinary(max) NULL,		--/ <column>Footprint of the observation in binary format</column>

	CONSTRAINT [PK_Observation] PRIMARY KEY CLUSTERED 
	(
		[inst] ASC,
		[obsID] ASC
	)
) ON [PRIMARY]

GRANT SELECT ON [Observation] TO [User]

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

GRANT SELECT ON [ObservationHtm] TO [User]

GO