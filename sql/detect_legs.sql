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

DECLARE @legMinGap bigint = 25 * 1e6;			-- minimum gap to start new leg
DECLARE @avVarMax float = 0.15;
DECLARE @avDiffMax float = 0.5;

WITH
a AS	-- average velocities over +/-5 bins
(
	SELECT
		*,
		AVG(SQRT(avy*avy + avz*avz))
			OVER (PARTITION BY ObsID
				  ORDER BY fineTime
				  ROWS BETWEEN 5 PRECEDING
						   AND 5 FOLLOWING) av_avg,
		VAR(SQRT(avy*avy + avz*avz))
			OVER (PARTITION BY ObsID
				  ORDER BY fineTime
				  ROWS BETWEEN 5 PRECEDING
						   AND 5 FOLLOWING) av_var
	FROM Pointing
),
b AS
(
	SELECT a.*,
		a.fineTime - LAG(a.fineTime, 1, NULL) OVER(PARTITION BY a.ObsID ORDER BY a.obsID, a.fineTime) deltaLag,
		a.fineTime - LEAD(a.fineTime, 1, NULL) OVER(PARTITION BY a.ObsID ORDER BY a.obsID, a.fineTime) deltaLead
	FROM a
	INNER JOIN Observation o ON o.obsID = a.obsID
	WHERE av_avg BETWEEN o.av - @avDiffMax AND o.av + @avDiffMax AND
          av_var < @avVarMax
),
leg AS
(
	SELECT b.*,
		(ROW_NUMBER() OVER(PARTITION BY ObsID ORDER BY fineTime) + 1) / 2 leg,
		CASE
			WHEN (deltaLead < -@legMinGap OR deltaLead IS NULL) AND (deltaLag > @legMinGap OR deltaLag IS NULL) THEN -1
			WHEN deltaLead < -@legMinGap OR deltaLead IS NULL THEN 0
			WHEN deltaLag > @legMinGap OR deltaLag IS NULL THEN 1
			ELSE NULL
		END start
	FROM b
	WHERE (deltaLead < -@legMinGap OR deltaLead IS NULL) OR 
	      (deltaLag > @legMinGap OR deltaLag IS NULL) AND
		  NOT ((deltaLead < -@legMinGap OR deltaLead IS NULL) AND (deltaLag > @legMinGap OR deltaLag IS NULL))
)
INSERT #LegTemp WITH(TABLOCKX)
SELECT obsID, leg, start, fineTime, ra, dec, pa
FROM leg
WHERE start IN (0, 1)
ORDER BY obsID, fineTime

TRUNCATE TABLE Leg

INSERT Leg WITH(TABLOCKX)
SELECT a.obsID, a.legID, a.fineTime, b.fineTime, a.ra, a.dec, a.pa, b.ra, b.dec, b.pa
FROM #LegTemp a
INNER JOIN #LegTemp b ON a.obsID = b.obsID AND a.legID = b.legID
WHERE a.start = 1 AND b.start = 0

DROP TABLE #LegTemp

GO


-- Generate leg regions

TRUNCATE TABLE LegRegion

INSERT LegRegion WITH (TABLOCKX)
SELECT obsID, legID, dbo.fGetLegRegion(raStart, decStart, paStart, raEnd, decEnd, paEnd, 'Blue')
FROM Leg

GO


-- Generate leg HTM

TRUNCATE TABLE LegHtm

INSERT LegHtm WITH (TABLOCKX)
SELECT obsID, legID, htm.htmidStart, htm.htmidEnd, htm.partial
FROM LegRegion
CROSS APPLY dbo.fGetHtmCover(region) htm

GO


-- Generate unions of legs

TRUNCATE TABLE ObservationRegion

INSERT ObservationRegion WITH (TABLOCKX)
SELECT obsID, dbo.fRegionUnion(region)
FROM LegRegion
GROUP BY obsID

GO


-- Generate observation HTM

TRUNCATE TABLE ObservationHtm

INSERT ObservationHtm WITH (TABLOCKX)
SELECT obsID, htm.htmidStart, htm.htmidEnd, htm.partial
FROM ObservationRegion
CROSS APPLY dbo.fGetHtmCover(region) htm