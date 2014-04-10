-- Create temp table for legs

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

DECLARE @fineTimeDelta bigint = 1100000;
DECLARE @avVarMax float = 0.05;

WITH a AS
(
	SELECT --TOP 10000
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
		fineTime - LAG(fineTime, 1, NULL) OVER(PARTITION BY ObsID ORDER BY obsID, fineTime) deltaLag,
		fineTime - LEAD(fineTime, 1, NULL) OVER(PARTITION BY ObsID ORDER BY obsID, fineTime) deltaLead
	FROM a
	WHERE av_avg BETWEEN 19 AND 21 AND
          av_var < @avVarMax
),
leg AS
(
	SELECT b.*,
		(ROW_NUMBER() OVER(PARTITION BY ObsID ORDER BY fineTime) + 1) / 2 leg,
		CASE
			WHEN deltaLead < -@fineTimeDelta OR deltaLead IS NULL THEN 0
			WHEN deltaLag > @fineTimeDelta OR deltaLag IS NULL THEN 1
			ELSE NULL
		END start
	FROM b
	WHERE deltaLag > @fineTimeDelta OR deltaLag IS NULL OR deltaLead < -@fineTimeDelta OR deltaLead IS NULL
)
INSERT #LegTemp
SELECT obsID, leg, start, fineTime, ra, dec, pa
FROM leg
ORDER BY obsID, fineTime

INSERT Leg
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