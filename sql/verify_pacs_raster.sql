-- single point / no-chop
SELECT TOP 100 *
FROM Observation
WHERE inst = 1 AND calibration = 0 AND failed = 0
  AND pointingMode = 1 AND obsType = 2 AND sso = 0
  AND (instMode & 0x0000000000100000) = 0
  AND obsID IN (SELECT obsID FROM load.PointingCluster)

SELECT TOP 100 *
FROM Observation
WHERE inst = 1 AND calibration = 0 AND failed = 0
  AND pointingMode = 1 AND obsType = 2 AND (instMode & 0x0000000000100000) = 0
  AND obsID IN (
	SELECT obsID FROM load.PointingCluster
	GROUP BY obsID
	HAVING MAX(groupID) > 1)

-- raster / no-chop
SELECT TOP 100 *
FROM Observation
WHERE inst = 1 AND calibration = 0 AND failed = 0
  AND pointingMode IN (2, 4) AND obsType = 2 AND (instMode & 0x0000000000100000) = 0
  AND obsID IN (SELECT obsID FROM load.PointingCluster)

-- single point w/ chop-nod
SELECT TOP 100 *
FROM Observation
WHERE inst = 1 AND calibration = 0 AND failed = 0
  AND pointingMode = 1 AND obsType = 2 AND (instMode & 0x0000000000100000) != 0
  AND obsID IN (SELECT obsID FROM load.PointingCluster)

-- raster w/ chop-nod
SELECT TOP 100 *
FROM Observation
WHERE inst = 1 AND calibration = 0 AND failed = 0
  AND pointingMode IN (2, 4) AND obsType = 2 AND (instMode & 0x0000000000100000) != 0
  AND obsID IN (SELECT obsID FROM load.PointingCluster)

