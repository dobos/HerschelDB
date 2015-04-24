-- Find and delete duplicates

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

GO

ALTER TABLE [load].Duplicate ADD CONSTRAINT
	PK_Duplicate PRIMARY KEY CLUSTERED 
	(
	inst,
	obsID,
	obsType,
	fineTime
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

WITH dup AS
(
	SELECT p.inst, p.obsID, p.obsType, p.fineTime, p.BBID, ROW_NUMBER() OVER (PARTITION BY p.inst, p.obsID, p.obsType, p.fineTime ORDER BY p.BBID) rn
	FROM load.RawPointing p
	INNER JOIN load.Duplicate d
		ON d.inst = p.inst AND d.obsID = p.obsID AND d.obsType = p.obsType AND d.fineTime = p.fineTime
)
DELETE load.RawPointing
FROM load.RawPointing p
INNER JOIN dup d
	ON d.inst = p.inst AND d.obsID = p.obsID AND d.obsType = p.obsType AND d.fineTime = p.fineTime AND d.BBID = p.BBID
WHERE d.rn > 1

GO

-- Verify that duplicates have gone

SELECT inst,  obsID, obsType, fineTime, COUNT(*) cnt
FROM load.RawPointing
GROUP BY inst,  obsID, obsType, fineTime
HAVING COUNT(*) > 1

-- But still one entry by fine time remains

SELECT COUNT(*) FROM load.Duplicate
-- 8244

SELECT COUNT(*)
FROM load.Duplicate d
INNER JOIN load.RawPointing p
	ON d.inst = p.inst AND d.obsID = p.obsID AND d.obsType = p.obsType AND d.fineTime = p.fineTime
-- 8244

GO

DROP TABLE load.Duplicate
