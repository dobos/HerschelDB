DECLARE @binsize float = 1;

WITH
velhist AS
(
	SELECT obsID, ROUND(SQRT(avy*avy + avz*avz) / @binsize, 0) * @binsize vvv, COUNT(*) cnt
	FROM Pointing
	--WHERE obsID = 1342224854
	GROUP BY obsID, ROUND(SQRT(avy*avy + avz*avz) / @binsize, 0) * @binsize
),
velhistmax AS
(
	SELECT *, ROW_NUMBER() OVER(PARTITION BY obsID ORDER BY cnt DESC) rn
	FROM velhist
	WHERE vvv > 2	-- velocity is low at turn around
),
minmax AS
(
	SELECT obsID, MIN(fineTime) fineTimeStart, MAX(fineTime) fineTimeEnd
	FROM Pointing
	GROUP BY obsID
)
INSERT Observation
SELECT
	o.obsID, o.fineTimeStart, o.fineTimeEnd, v.vvv
FROM minmax o
INNER JOIN velhistmax v ON v.obsID = o.obsID AND v.rn = 1

--(17414 row(s) affected)
--1:18


-- Verify counts

SELECT obsID, COUNT(*)
FROM Pointing
GROUP BY obsID

--17414
--0:21

-- Verify av value histogram

SELECT av, COUNT(*)
FROM Observation
GROUP BY av
ORDER BY 1

SELECT * FROM Observation WHERE av = 431