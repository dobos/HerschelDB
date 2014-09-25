IF (NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'load'))
BEGIN
	EXEC('CREATE SCHEMA [load]')
END

GO


IF OBJECT_ID (N'load.RawPointing', N'U') IS NOT NULL
DROP TABLE [load].[RawPointing]

CREATE TABLE [load].[RawPointing]
(
	[obsID] [bigint] NOT NULL,
	[fineTime] [bigint] NOT NULL,
	[Inst] [tinyint] NOT NULL,
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
	[av] [float] NOT NULL,
	[utc] [bigint] NOT NULL,
	[sampleTime] [float] NOT NULL,
	[corrTime] [float] NOT NULL
) ON [LOAD]

GO


IF OBJECT_ID (N'load.LegEnds', N'U') IS NOT NULL
DROP TABLE [load].[LegEnds]

CREATE TABLE [load].[LegEnds]
(
	[obsID] bigint,
	[legID] smallint,
	[start] tinyint,
	[fineTime] bigint,
	[ra] float,
	[dec] float,
	[pa] float,

	CONSTRAINT PK_LegTemp PRIMARY KEY CLUSTERED 
	(
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
		[obsID] ASC,
		[legID] ASC
	)
) ON [LOAD]

GO


IF OBJECT_ID (N'load.LegRegion', N'U') IS NOT NULL
DROP TABLE [load].[LegRegion]

CREATE TABLE [load].[LegRegion]
(
	[obsID] bigint NOT NULL,
	[legID] smallint NOT NULL,
	[fineTimeStart] bigint NOT NULL,
	[fineTimeEnd] bigint NOT NULL,
	[region] varbinary(8000) NOT NULL,

	CONSTRAINT [PK_LegRegion] PRIMARY KEY CLUSTERED 
	(
		[obsID] ASC,
		[legID] ASC
	)
) ON [LOAD]

GO