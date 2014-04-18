IF OBJECT_ID (N'PointingTemp', N'U') IS NOT NULL
DROP TABLE [PointingTemp]

CREATE TABLE [PointingTemp]
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
	[utc] [bigint] NOT NULL
)