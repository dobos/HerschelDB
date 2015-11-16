-- PACS photometric observations by pointing mode

SELECT CAST(pointingMode AS binary(8)) AS [pointingMode], COUNT(*) AS [count]
FROM Observation
WHERE inst = 1 AND obsType = 1
GROUP BY CAST(pointingMode AS binary(8))

/*
0x0000000000000041	2185				-- pointed with nodding
0x0000000000000042	23					-- raster with nodding		** not implemented
0x0000000000000008	18872				-- scan map
0x0000000000000002	13					-- raster					** not implemented
*/

-- only valid observations

SELECT CAST(pointingMode AS binary(8)) AS [pointingMode], COUNT(*) AS [count]
FROM Observation
WHERE inst = 1 AND obsType = 1
  AND calibration = 0 AND failed = 0
GROUP BY CAST(pointingMode AS binary(8))

/*
0x0000000000000041	67					-- pointed with nodding
0x0000000000000008	17311				-- scans
*/

-- any observation with footprint

SELECT CAST(pointingMode AS binary(8)) AS [pointingMode], COUNT(*) AS [count]
FROM Observation
WHERE inst = 1 AND obsType = 1 AND region IS NOT NULL AND region.HasError(region) = 0
GROUP BY CAST(pointingMode AS binary(8))

/*
0x0000000000000008	18601				-- scan map
0x0000000000000041	2154				-- pointed with nodding
*/

-- valid observations with footprint

SELECT CAST(pointingMode AS binary(8)) AS [pointingMode], COUNT(*) AS [count]
FROM Observation
WHERE inst = 1 AND obsType = 1 AND region IS NOT NULL AND region.HasError(region) = 0
      AND calibration = 0 AND failed = 0
GROUP BY CAST(pointingMode AS binary(8))

/*
0x0000000000000008	17311
0x0000000000000041	65
*/

-- missing valid observations

SELECT inst, obsID, CAST(pointingMode AS binary(8)) AS [pointingMode], calibration, failed, obsLevel, sso
FROM Observation
WHERE inst = 1 AND obsType = 1 AND (region IS NULL OR region.HasError(region) = 1)
      AND calibration = 0 AND failed = 0

/*
1	1342185632	0x0000000000000041	0	0	255
1	1342185633	0x0000000000000041	0	0	255
*/

-- two non-source measurements... calibration?


--======================================================================================================






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