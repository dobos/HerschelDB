IF OBJECT_ID (N'Pointing', N'U') IS NOT NULL
DROP TABLE [Pointing]

CREATE TABLE [Pointing]
(
	[obsID] [bigint] NOT NULL,
	[fineTime] [bigint] NOT NULL,
	[bbID] [bigint] NOT NULL,
	[ra] [float] NOT NULL,
	[raError] [float] NOT NULL,
	[dec] [float] NOT NULL,
	[decError] [float] NOT NULL,
	[pa] [float] NOT NULL,
	[paError] [float] NOT NULL,
	[avX] [float] NOT NULL,
	[avXError] [float] NOT NULL,
	[avY] [float] NOT NULL,
	[avYError] [float] NOT NULL,
	[avZ] [float] NOT NULL,
	[avZError] [float] NOT NULL,
	[utc] [bigint] NOT NULL,
	CONSTRAINT [PK_Pointing] PRIMARY KEY CLUSTERED 
	(
		[obsID] ASC,
		[fineTime] ASC
	)
)

GO


IF OBJECT_ID (N'Observation', N'U') IS NOT NULL
DROP TABLE [Observation]

CREATE TABLE [Observation]
(
--/ <summary>Contains one entry for each observation</summary>
--/ <remarks></remarks>
	
	[obsID] bigint NOT NULL,			--/ <column>Unique ID of the observation</column>
	[fineTimeStart] bigint NOT NULL,
	[fineTimeEnd] bigint NOT NULL,
	--[vel] float NOT NULL,
	CONSTRAINT [PK_Observation] PRIMARY KEY CLUSTERED 
	(
		[obsID] ASC
	)
)

GO


IF OBJECT_ID (N'ObservationRegion', N'U') IS NOT NULL
DROP TABLE [ObservationRegion]

CREATE TABLE [ObservationRegion]
(
	[obsID] bigint NOT NULL,
	[region] varbinary(max) NOT NULL,
	CONSTRAINT [PK_ObservationRegion] PRIMARY KEY CLUSTERED 
	(
		[obsID] ASC
	)
)

GO


IF OBJECT_ID (N'ObservationHtm', N'U') IS NOT NULL
DROP TABLE [ObservationHtm]

CREATE TABLE [ObservationHtm]
(
	[obsID] bigint NOT NULL,
	[legID] smallint NOT NULL,
	[htmIDStart] bigint NOT NULL,
	[htmIDEnd] bigint NOT NULL,
	[partial] bit NOT NULL
)

CREATE CLUSTERED INDEX [CI_ObservationHtm] ON [ObservationHtm]
(
	[htmIDStart] ASC,
	[htmIDEnd] ASC
)

GO


IF OBJECT_ID (N'Leg', N'U') IS NOT NULL
DROP TABLE [Leg]

CREATE TABLE [Leg]
(
	[obsID] bigint NOT NULL,
	[legID] smallint NOT NULL,
	[fineTimeStart] bigint NOT NULL,
	[fineTimeEnd] bigint NOT NULL,
	[raStart] float NOT NULL,
	[decStart] float NOT NULL,
	[paStart] float NOT NULL,
	[raEnd] float NOT NULL,
	[decEnd] float NOT NULL,
	[paEnd] float NOT NULL,
	--[vel] float,
	CONSTRAINT [PK_Leg] PRIMARY KEY CLUSTERED 
	(
		[obsID] ASC,
		[legID] ASC
	)
)

GO


IF OBJECT_ID (N'LegRegion', N'U') IS NOT NULL
DROP TABLE [LegRegion]

CREATE TABLE [LegRegion]
(
	[obsID] bigint NOT NULL,
	[legID] smallint NOT NULL,
	[region] varbinary(8000) NOT NULL,
	CONSTRAINT [PK_LegRegion] PRIMARY KEY CLUSTERED 
	(
		[obsID] ASC,
		[legID] ASC
	)
)

GO


IF OBJECT_ID (N'LegHtm', N'U') IS NOT NULL
DROP TABLE [LegHtm]

CREATE TABLE [LegHtm]
(
	[obsID] bigint NOT NULL,
	[legID] smallint NOT NULL,
	[htmIDStart] bigint NOT NULL,
	[htmIDEnd] bigint NOT NULL,
	[partial] bit NOT NULL
)

CREATE CLUSTERED INDEX [CI_LegHtm] ON [LegHtm]
(
	[htmIDStart] ASC,
	[htmIDEnd] ASC
)

GO
