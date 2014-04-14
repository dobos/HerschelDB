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

GO



BULK INSERT [PointingTemp]
FROM 'c:\Data\Temp\vo\herschel\all2.dat' -- '[$datafile]'
WITH 
( 
   DATAFILETYPE = 'char',
   FIELDTERMINATOR = ' ',
   ROWTERMINATOR = '\n',
   TABLOCK
)

--(179654004 row(s) affected)
--57:04

--(196697043 row(s) affected)
--53:37



SELECT COUNT(*) FROM PointingTemp



CREATE CLUSTERED INDEX [IC_PointingTemp] ON [dbo].[PointingTemp]
(
	[obsID] ASC,
	[fineTime] ASC
)


-- Check duplicates

SELECT obsID, fineTime, COUNT(*)
FROM PointingTemp
GROUP BY obsID, fineTime
HAVING COUNT(*) > 1

-- Copy to target table

INSERT Pointing WITH (TABLOCKX)
SELECT * FROM PointingTemp

--(196697043 row(s) affected)
--6:42