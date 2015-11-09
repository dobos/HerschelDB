-- PACS photometric observations by pointing mode

SELECT CAST(pointingMode AS binary(8)), COUNT(*)
FROM Observation
WHERE inst = 1 AND obsType = 1
GROUP BY CAST(pointingMode AS binary(8))

/*
0x0000000000000041	2185				-- pointed with nodding
0x0000000000000042	23					-- raster with nodding
0x0000000000000008	18872				-- line scan
0x0000000000000002	13					-- raster
*/

-- only valid observations

SELECT CAST(pointingMode AS binary(8)), COUNT(*)
FROM Observation
WHERE inst = 1 AND obsType = 1
  AND calibration = 0 AND failed = 0
GROUP BY CAST(pointingMode AS binary(8))

/*
0x0000000000000041	67					-- pointed with nodding
0x0000000000000008	17311				-- scans
*/

-- point-nod photometry

-- some are sso

SELECT sso, COUNT(*)
FROM Observation
WHERE inst = 1 AND obsType = 1 AND calibration = 0 AND failed = 0
  AND pointingMode = 0x0000000000000041
GROUP BY sso

/*
0	55
1	12
*/

SELECT obsID, sso
FROM Observation
WHERE inst = 1 AND obsType = 1 AND calibration = 0 AND failed = 0
  AND pointingMode = 0x0000000000000041