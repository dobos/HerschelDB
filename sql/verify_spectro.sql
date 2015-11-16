-- Count all spectro observations grouped by pointing mode

SELECT inst, obsType, pointingMode, COUNT(*)
FROM Observation
WHERE obsType = 2
GROUP BY inst, obsType, pointingMode
ORDER BY 1, 2, 3

/*
inst	obsType	pointingMode	(No column name)
1	2	1	5067		-- pointed
1	2	4	1089		-- raster (mapping)
2	2	0	24			-- calibration?
2	2	1	2129		-- pointed
2	2	2	22			-- raster
8	2	1	8537		-- pointed
8	2	4	1460		-- raster (mapping)
*/

-- Count those only which have pointing info associated

SELECT inst, obsType, pointingMode, COUNT(*)
FROM Observation
WHERE obsType = 2 AND obsID IN (SELECT obsID FROM load.Pointing WHERE inst IN (1, 2))
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

--=============================================================================

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