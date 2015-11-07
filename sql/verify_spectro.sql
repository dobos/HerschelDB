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

-------------------------------------

-- PACS pointed spectro
-- ra, dec and pa should be correct in the obs header

SELECT CAST(instMode AS binary(8)), COUNT(*)
FROM Observation
WHERE inst = 1 AND obsType = 2 AND pointingMode = 1
GROUP BY CAST(instMode AS binary(8))

/*
(No column name)	(No column name)
0x0000000000140021	2456			-- range + chopper
0x0000000000040021	685				-- range
0x0000000000180021	1550			-- line + chopper
0x0000000000080021	376				-- line
*/

-- uncopped single pointing
SELECT *
FROM Observation
WHERE inst = 1 AND obsType = 2 AND pointingMode = 1
	--AND calibration = 0 AND failed = 0
	AND (instMode = 0x0000000000040021 OR instMode = 0x0000000000080021)

-- 1061 (702)

SELECT *
FROM Observation
WHERE inst = 1 AND obsType = 2 AND pointingMode = 1
	AND calibration = 0 AND failed = 0
	AND (instMode = 0x0000000000040021 OR instMode = 0x0000000000080021)
    AND obsID NOT IN (SELECT obsID FROM load.PointingCluster)

-- 0 (77)

-- More than one pointing:

SELECT o.obsID
FROM Observation o
INNER JOIN load.PointingCluster p
	ON p.inst = o.inst AND p.obsID = o.obsID
WHERE o.inst = 1 AND obsType = 2 AND pointingMode = 1
	AND calibration = 0 AND failed = 0
	AND (instMode = 0x0000000000040021 OR instMode = 0x0000000000080021)
GROUP BY o.ObsID
HAVING COUNT(*) > 1

---------------------------------------------------------------

-- uncopped raster pointing
SELECT *
FROM Observation
WHERE inst = 1 AND obsType = 2 AND pointingMode IN (2, 4)
	AND calibration = 0 AND failed = 0
	AND (instMode = 0x0000000000040021 OR instMode = 0x0000000000080021)

-- 742 (772)

SELECT *
FROM Observation
WHERE inst = 1 AND obsType = 2 AND pointingMode IN (2, 4)
	AND calibration = 0 AND failed = 0
	AND (instMode = 0x0000000000040021 OR instMode = 0x0000000000080021)
	AND obsID NOT IN (SELECT obsID FROM load.PointingCluster)

-- 6 (21)

/*
obsID
1342245811
1342269922
1342270642
1342271194
1342271195
1342271196
*/

---------------------------------------------------------------

-- chopped single pointing

SELECT *
FROM Observation
WHERE inst = 1 AND obsType = 2 AND pointingMode = 1
	AND calibration = 0 AND failed = 0
	AND (instMode = 0x0000000000140021 OR instMode = 0x0000000000180021)

-- 3331 (4006)

-- with more than one point

SELECT o.obsID
FROM Observation o
INNER JOIN load.PointingCluster p
	ON p.inst = o.inst AND p.obsID = o.obsID
WHERE o.inst = 1 AND obsType = 2 AND pointingMode = 1
	AND calibration = 0 AND failed = 0 AND sso = 0
	AND (instMode = 0x0000000000140021 OR instMode = 0x0000000000180021)
	AND p.isRotated = 1
GROUP BY o.ObsID
HAVING COUNT(*) > 2

-- with not enough points

SELECT o.obsID
FROM Observation o
WHERE o.inst = 1 AND obsType = 2 AND pointingMode = 1
	AND calibration = 0 AND failed = 0 AND sso = 0
	AND (instMode = 0x0000000000140021 OR instMode = 0x0000000000180021)
	AND obsID NOT IN (SELECT obsID FROM load.PointingCluster WHERE clusterID = 0 AND isRotated = 1)

-- 0


---------------------------------------------------------------

-- PACS chopped raster

SELECT *
FROM Observation
WHERE inst = 1 AND obsType = 2 AND pointingMode IN (2, 4)
	AND calibration = 0 AND failed = 0
	AND (instMode = 0x0000000000140021 OR instMode = 0x0000000000180021)

-- 270 (317)

SELECT *
FROM Observation o
INNER JOIN RasterMap r ON r.inst = o.inst AND r.obsID = o.obsID
WHERE o.inst = 1 AND obsType = 2 AND pointingMode IN (2, 4)
	AND calibration = 0 AND failed = 0
	AND (instMode = 0x0000000000140021 OR instMode = 0x0000000000180021)

-- 270 (270)

-- missing pointing

SELECT *
FROM Observation o
WHERE o.inst = 1 AND obsType = 2 AND pointingMode IN (2, 4)
	AND calibration = 0 AND failed = 0
	AND (instMode = 0x0000000000140021 OR instMode = 0x0000000000180021)
	AND obsID NOT IN (SELECT obsID FROM load.PointingCluster)

SELECT *
FROM Observation o
WHERE o.inst = 1 AND obsType = 2 AND pointingMode IN (2, 4)
	AND calibration = 0 AND failed = 0
	AND (instMode = 0x0000000000140021 OR instMode = 0x0000000000180021)
	AND obsID NOT IN (SELECT obsID FROM load.PointingCluster WHERE groupID = 0 AND isRotated = 1)

-- unreal number of pointings

WITH pp AS
(
	SELECT inst, obsID, COUNT(*) cc
	FROM load.PointingCluster
	WHERE groupID = 0 AND isRotated = 1
	GROUP BY inst, obsID
)
SELECT o.obsID, pp.cc, r.num
FROM Observation o
INNER JOIN RasterMap r ON r.inst = o.inst AND r.obsID = o.obsID
INNER JOIN pp ON pp.inst = o.inst AND pp.obsID = o.obsID
WHERE o.inst = 1 AND obsType = 2 AND pointingMode IN (2, 4)
	AND calibration = 0 AND failed = 0
	AND (instMode = 0x0000000000140021 OR instMode = 0x0000000000180021)
	AND pp.cc != r.num

---------------------------------------------------------------

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