-- Find and delete duplicates

IF OBJECT_ID (N'load.RawPointing_ID', N'U') IS NOT NULL
DROP TABLE [load].[RawPointing_ID]

CREATE TABLE [load].[RawPointing_ID]
(
	[ID] [bigint] NOT NULL PRIMARY KEY IDENTITY(1, 1),
	[inst] [tinyint] NOT NULL,
	[obsID] [bigint] NOT NULL,
	[BBID] [bigint] NOT NULL,
	[obsType] [tinyint] NOT NULL,
	[fineTime] [bigint] NOT NULL,
	[ra] [float] NOT NULL,
	[dec] [float] NOT NULL,
	[pa] [float] NOT NULL,
	[av] [float] NOT NULL,
	[aperture] [float] NOT NULL,
	[width] [float] NOT NULL,
	[height] [float] NOT NULL,
	[isAPosition] [bit] NOT NULL,
	[isBPosition] [bit] NOT NULL,
	[isOffPosition] [bit] NOT NULL,
	[isOnTarget] [bit] NOT NULL,
	[rasterLineNum] [tinyint] NOT NULL,
	[rasterColumnNum] [tinyint] NOT NULL,
	[rasterAngle] [float] NOT NULL
) ON [LOAD]

GO

INSERT [load].[RawPointing_ID] WITH (TABLOCKX)
	([inst], [obsID], [BBID], [obsType], [fineTime], [ra], [dec], [pa], [av],
	 [aperture], [width], [height],
	 [isAPosition], [isBPosition], [isOffPosition], [isOnTarget], [rasterLineNum], [rasterColumnNum], [rasterAngle])
SELECT * FROM [load].[RawPointing]
-- 16:03

GO


---

DROP TABLE [load].[RawPointing]

GO

EXEC sp_rename 'load.RawPointing_ID', 'Pointing'

---

CREATE INDEX [ID_Pointing]
ON [load].[Pointing]
(
		inst,
		obsID,
		obsType,
		fineTime
) WITH (SORT_IN_TEMPDB = ON)
ON [load]
-- 16:04

GO

---

IF OBJECT_ID('load.Duplicate') IS NOT NULL
DROP TABLE load.Duplicate

GO

CREATE TABLE [load].[Duplicate](
	[inst] [tinyint] NOT NULL,
	[obsID] [bigint] NOT NULL,
	[obsType] [tinyint] NOT NULL,
	[fineTime] [bigint] NOT NULL,
	[cnt] [int] NULL
) ON [LOAD]

GO


INSERT load.Duplicate WITH (TABLOCKX)
SELECT inst,  obsID, obsType, fineTime, COUNT(*) cnt
FROM load.Pointing
GROUP BY inst,  obsID, obsType, fineTime
HAVING COUNT(*) > 1
-- 9804
-- 4:49

GO

ALTER TABLE [load].Duplicate ADD CONSTRAINT
	PK_Duplicate PRIMARY KEY CLUSTERED 
	(
	inst,
	obsID,
	obsType,
	fineTime
	)  ON [LOAD]

GO

WITH dup AS
(
	SELECT p.inst, p.obsID, p.obsType, p.fineTime, p.ra, p.ID, ROW_NUMBER() OVER (PARTITION BY p.inst, p.obsID, p.obsType, p.fineTime ORDER BY p.ID) rn
	FROM load.Pointing p
	INNER JOIN load.Duplicate d
		ON d.inst = p.inst AND d.obsID = p.obsID AND d.obsType = p.obsType AND d.fineTime = p.fineTime
	-- 17290
)
DELETE load.Pointing
FROM load.Pointing p
INNER JOIN dup d
	ON d.inst = p.inst AND d.obsID = p.obsID AND d.obsType = p.obsType AND d.fineTime = p.fineTime AND d.ID = p.ID
WHERE d.rn > 1

GO

-- Verify that duplicates have gone

SELECT inst,  obsID, obsType, fineTime, COUNT(*) cnt
FROM load.Pointing
GROUP BY inst,  obsID, obsType, fineTime
HAVING COUNT(*) > 1

-- But still one entry by fine time remains

SELECT COUNT(*) FROM load.Duplicate
-- 9804

SELECT COUNT(*)
FROM load.Duplicate d
INNER JOIN load.Pointing p
	ON d.inst = p.inst AND d.obsID = p.obsID AND d.obsType = p.obsType AND d.fineTime = p.fineTime
-- 9804

GO

DROP TABLE load.Duplicate
