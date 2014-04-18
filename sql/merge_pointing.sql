CREATE CLUSTERED INDEX [IC_PointingTemp] ON [dbo].[PointingTemp]
(
	[obsID] ASC,
	[fineTime] ASC
)


-- Check duplicates

/*SELECT obsID, fineTime, COUNT(*)
FROM PointingTemp
GROUP BY obsID, fineTime
HAVING COUNT(*) > 1*/

INSERT Pointing WITH (TABLOCKX)
SELECT * FROM PointingTemp
