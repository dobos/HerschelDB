/*
Detect scan legs from raw pointings

Raw points are filtered for scan legs. Legs are defined by almost constant
scan velocity along straight lines. Rolling average and variance of velocity
along scan curve is computed to find leg ends.

*/


-- Create temp table for legs

IF OBJECT_ID (N'#LegTemp', N'U') IS NOT NULL
DROP TABLE #LegTemp

CREATE TABLE #LegTemp
(
	obsID bigint,
	legID smallint,
	start tinyint,
	fineTime bigint,
	ra float,
	dec float,
	pa float,
	CONSTRAINT PK_LegTemp PRIMARY KEY CLUSTERED 
	(
		obsID ASC,
		legID ASC,
		start ASC
	)
)

DECLARE @legMinGap bigint = 5 * 1e6;			-- minimum gap to start new leg
DECLARE @avVarMax float = NULL;					-- max variance in velocity in moving window
DECLARE @avDiffMax float = 25.0;				-- max deviation from observation velocity (turnaround removal)

INSERT #LegTemp WITH(TABLOCKX)
SELECT obsID, leg, start, fineTime, ra, dec, pa
FROM dbo.FindLegEnds(@avDiffMax, @avVarMax, @legMinGap)
WHERE start IN (0, 1) AND obsID = 1342204285
ORDER BY obsID, fineTime

TRUNCATE TABLE Leg

INSERT Leg WITH(TABLOCKX)
SELECT a.obsID, a.legID, a.fineTime, b.fineTime, a.ra, a.dec, a.pa, b.ra, b.dec, b.pa
FROM #LegTemp a
INNER JOIN #LegTemp b ON a.obsID = b.obsID AND a.legID = b.legID
WHERE a.start = 1 AND b.start = 0

DROP TABLE #LegTemp

GO