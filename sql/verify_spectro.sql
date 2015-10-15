-- Count all spectro observations grouped by pointing mode

SELECT inst, obsType, pointingMode, COUNT(*)
FROM Observation
WHERE obsType = 2
GROUP BY inst, obsType, pointingMode
ORDER BY 1, 2, 3

/*
inst	obsType	pointingMode	(No column name)
1	2	1	5068		-- pointed
1	2	4	1088		-- mapping?
2	2	0	24			-- calibration?
2	2	1	2129		-- pointed						OK
2	2	2	22			-- raster						OK
*/

-- Count those only which have pointing info associated

SELECT inst, obsType, pointingMode, COUNT(*)
FROM Observation
WHERE obsType = 2 AND obsID IN (SELECT obsID FROM load.RawPointing WHERE inst IN (1, 2))
GROUP BY inst, obsType, pointingMode
ORDER BY 1, 2, 3

/*
inst	obsType	pointingMode	(No column name)
1	2	1	4982
1	2	4	1074
2	2	0	24
2	2	1	2129
2	2	2	15
*/

-------------------------------------

-- PACS pointed spectro

SELECT TOP 100 *
FROM Observation
WHERE inst = 1 AND obsType = 2 AND pointingMode = 1

-- PACS pointed spectro with no pointing
SELECT *
FROM Observation
WHERE inst = 1 AND obsType = 2 AND pointingMode = 1 AND
	obsID NOT IN (SELECT obsID FROM load.Pointing WHERE inst IN (1))

-- 86 with no pointing at all

SELECT *
FROM load.Pointing
WHERE inst = 1 AND obsID = 1342186796

SELECT AVG(ra), AVG(dec), AVG(pa)
FROM load.Pointing
WHERE inst = 1 AND obsID = 1342186307 AND isOnTarget = 1

SELECT * FROM Observation
WHERE inst = 1 AND obsID = 1342186307

-------------------------------------

-- PACS mapping spectro

SELECT TOP 100 *
FROM Observation
WHERE inst = 1 AND obsType = 2 AND pointingMode = 4

SELECT *
FROM load.Pointing
WHERE inst = 1 AND obsID = 1342186971

-------------------------------------

-- SPIRE no pointing

SELECT TOP 100 *
FROM Observation
WHERE inst = 2 AND obsType = 2 AND pointingMode = 0

-- all calibration

-------------------------------------

-- SPIRE pointed spectro

SELECT *
FROM Observation
WHERE inst = 2 AND obsType = 2 AND pointingMode = 1


SELECT *
FROM load.RAwObservation
WHERE inst = 2 AND obsType = 2 AND pointingMode = 1

-- Number of pointings per obs

SELECT p.obsID, COUNT(*)
FROM Observation o
INNER JOIN load.RawPointing p ON p.inst = o.inst AND p.obsID = o.obsID
WHERE o.inst = 2 AND o.obsType = 2 AND o.pointingMode = 1
GROUP BY p.obsID

-- Observations with more than two pointings

SELECT * FROM Observation
WHERE inst = 2 AND obsID IN
(
	SELECT p.obsID
	FROM Observation o
	INNER JOIN load.RawPointing p ON p.inst = o.inst AND p.obsID = o.obsID
	WHERE o.inst = 2 AND o.obsType = 2 AND o.pointingMode = 1
	GROUP BY p.obsID
	HAVING COUNT(*) != 2
)
ORDER BY obsID

--

SELECT * FROM Observation
WHERE inst = 2 AND obsID = 1342245117

SELECT * FROM load.RawPointing
WHERE inst = 2 AND obsID = 1342245117

SELECT *
FROM load.RawPointing
WHERE inst = 2 AND obsID = 1342188189

-------------------------------------

-- SPIRE raster spectro

SELECT *
FROM Observation
WHERE inst = 2 AND obsType = 2 AND pointingMode = 2

SELECT *
FROM load.RAwObservation
WHERE inst = 2 AND obsType = 2 AND pointingMode = 2

SELECT *
FROM load.rawPointing 
WHERE inst = 2 AND obsID = 1342252289


SELECT *
FROM load.RAwObservation
WHERE inst = 2 AND obsType = 2 AND pointingMode = 2 AND obsID = 1342227519



SELECT *
FROM load.RAwObservation
WHERE inst = 2 AND obsType = 2 AND pointingMode = 2 AND obsID = 1342227454


----------------------------