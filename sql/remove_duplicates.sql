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
	[av] [float] NOT NULL
) ON [LOAD]

GO

INSERT [load].[RawPointing_ID] WITH (TABLOCKX)
	([inst], [obsID], [BBID], [obsType], [fineTime], [ra], [dec], [pa], [av])
SELECT * FROM [load].[RawPointing]
-- 12:02

GO

---

DROP TABLE [load].[RawPointing]

GO

EXEC sp_rename 'load.RawPointing_ID', 'RawPointing'

---

CREATE INDEX [ID_RawPointing]
ON [load].[RawPointing]
(
		inst,
		obsID,
		obsType,
		fineTime
) WITH (SORT_IN_TEMPDB = ON)
ON [load]
-- 41:53

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
FROM load.RawPointing
GROUP BY inst,  obsID, obsType, fineTime
HAVING COUNT(*) > 1
-- 8645
-- 2:00

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
	FROM load.RawPointing p
	INNER JOIN load.Duplicate d
		ON d.inst = p.inst AND d.obsID = p.obsID AND d.obsType = p.obsType AND d.fineTime = p.fineTime
	-- 17290
)
DELETE load.RawPointing
FROM load.RawPointing p
INNER JOIN dup d
	ON d.inst = p.inst AND d.obsID = p.obsID AND d.obsType = p.obsType AND d.fineTime = p.fineTime AND d.ID = p.ID
WHERE d.rn > 1

GO

-- Verify that duplicates have gone

SELECT inst,  obsID, obsType, fineTime, COUNT(*) cnt
FROM load.RawPointing
GROUP BY inst,  obsID, obsType, fineTime
HAVING COUNT(*) > 1

-- But still one entry by fine time remains

SELECT COUNT(*) FROM load.Duplicate
-- 8645

SELECT COUNT(*)
FROM load.Duplicate d
INNER JOIN load.RawPointing p
	ON d.inst = p.inst AND d.obsID = p.obsID AND d.obsType = p.obsType AND d.fineTime = p.fineTime
-- 8645

GO

DROP TABLE load.Duplicate
