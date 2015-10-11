SELECT inst, obsType, pointingMode, COUNT(*)
FROM Observation
WHERE obsType = 2
GROUP BY inst, obsType, pointingMode
ORDER BY 1, 2, 3

-------------------------------------

-- PACS pointed spectro

SELECT TOP 100 *
FROM Observation
WHERE inst = 1 AND obsType = 2 AND pointingMode = 1

SELECT *
FROM load.RawPointing
WHERE inst = 1 AND obsID = 1342186307

-------------------------------------

-- PACS mapping spectro

SELECT TOP 100 *
FROM Observation
WHERE inst = 1 AND obsType = 2 AND pointingMode = 4

SELECT *
FROM load.RawPointing
WHERE inst = 1 AND obsID = 1342189410

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
