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
SELECT TOP 100 *
FROM Observation
WHERE inst = 1 AND obsType = 2 AND pointingMode = 1
	AND calibration = 0 AND failed = 0
	AND (instMode = 0x0000000000040021 OR instMode = 0x0000000000080021)

-- uncopped raster pointing
SELECT TOP 100 *
FROM Observation
WHERE inst = 1 AND obsType = 2 AND pointingMode = 4
	AND calibration = 0 AND failed = 0
	AND (instMode = 0x0000000000040021 OR instMode = 0x0000000000080021)

-- raster points + a calibration point is visible
-- filter out calibration based on BBID
SELECT DISTINCT CAST(BBID as binary(8))
FROM Observation o
INNER JOIN load.Pointing p ON p.inst = o.inst AND p.obsID = o.obsID
WHERE o.inst = 1 AND o.obsType = 2 AND pointingMode = 4
	AND calibration = 0 AND failed = 0
	AND (instMode = 0x0000000000040021 OR instMode = 0x0000000000080021)

-- PACS pointed spectro with no pointing
SELECT *
FROM Observation
WHERE inst = 1 AND obsType = 2 AND pointingMode = 1 AND
	obsID NOT IN (SELECT obsID FROM load.Pointing WHERE inst IN (1))

-- PACS pointed spectro with no valid pointing
SELECT *
FROM Observation
WHERE inst = 1 AND obsType = 2 AND pointingMode = 1 
	AND	obsID NOT IN 
	(SELECT obsID 
	 FROM load.Pointing 
	 WHERE inst IN (1) 
	 AND BBID IN (0x00000000401C0001, 0x0000000040230001))

-- PACS pointed spectro where raster col/line num is set
-- these are not simple pointings but rasters

SELECT DISTINCT p.inst, p.obsID
FROM load.Pointing p
INNER JOIN Observation o
	ON o.inst = p.inst AND o.obsID = p.obsID
WHERE o.inst = 1 AND o.pointingMode = 1 AND p.isOnTarget = 1
AND p.rasterColumnNum > 0 AND p.rasterColumnNum != 255
AND p.rasterLineNum > 0 AND p.rasterLineNum != 255

/*
inst	obsID
1	1342182010
*/

SELECT *
FROM Observation
WHERE inst = 1 AND obsID = 1342182010

-- Apparently, 1342182010 is a raster instead of single pointing

SELECT DISTINCT p.inst, p.obsID
FROM load.Pointing p
INNER JOIN Observation o
	ON o.inst = p.inst AND o.obsID = p.obsID
WHERE o.inst = 1 AND o.pointingMode = 1 AND p.isOnTarget = 1
AND (p.rasterColumnNum = 255 OR p.rasterLineNum = 255)

/*
inst	obsID
1	1342182005
1	1342182011
1	1342188041
1	1342182003
1	1342182002
1	1342182004
*/

-- These seem to be wrong/calibration observations with incorrect raster col/num

SELECT *
FROM Observation
WHERE inst = 1 AND obsID IN
(
1342182005,
1342182011,
1342188041,
1342182003,
1342182002,
1342182004
)

SELECT *
FROM load.Pointing
WHERE inst = 1 AND obsID = 1342182005


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

-- PACS mapping (raster) spectro

SELECT CAST(instMode AS binary(8)), COUNT(*)
FROM Observation
WHERE inst = 1 AND obsType = 2 AND pointingMode = 4
GROUP BY CAST(instMode AS binary(8))

/*
(No column name)	(No column name)
0x0000000000040021	116		-- range
0x0000000000080021	656		-- line
0x0000000000140021	59		-- range + chopper
0x0000000000180021	258		-- line + chopper
*/

SELECT TOP 100 *
FROM Observation
WHERE inst = 1 AND obsType = 2 AND pointingMode = 4
      AND instMode = 0x0000000000180021

SELECT TOP 100 *
FROM Observation
WHERE inst = 1 AND obsType = 2 AND pointingMode = 4
      AND region IS NOT NULL

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