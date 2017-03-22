-- PACS photometric observations by pointing mode

SELECT CAST(pointingMode AS binary(8)) AS [pointingMode], COUNT(*) AS [count]
FROM Observation
WHERE inst = 1 AND obsType = 1
GROUP BY CAST(pointingMode AS binary(8))
ORDER BY 1

/*
V1:

0x0000000000000041	2185				-- pointed with nodding
0x0000000000000042	23					-- raster with nodding	** not implemented
0x0000000000000008	18872				-- scan map
0x0000000000000002	13					-- raster				** not implemented

V2:

pointingMode	count
0x0000000000000041	2185
0x0000000000000042	23
0x0000000000000008	18872
0x0000000000000002	13
*/

-- only valid observations

SELECT CAST(pointingMode AS binary(8)) AS [pointingMode], COUNT(*) AS [count]
FROM Observation
WHERE inst = 1 AND obsType = 1
  AND calibration = 0 AND failed = 0
GROUP BY CAST(pointingMode AS binary(8))

/*
V1:

0x0000000000000041	67					-- pointed with nodding
0x0000000000000008	17311				-- scans

V2:

pointingMode	count
0x0000000000000041	67
0x0000000000000008	17578
*/

-- any observation with footprint

SELECT CAST(pointingMode AS binary(8)) AS [pointingMode], 
	   CASE 
	    WHEN region IS NOT NULL AND region.HasError(region) = 0 THEN 0
		ELSE 1 
	   END AS [footprint],
	   COUNT(*) AS [count]
FROM Observation
WHERE inst = 1 AND obsType = 1
GROUP BY
	CAST(pointingMode AS binary(8)),
	CASE 
	    WHEN region IS NOT NULL AND region.HasError(region) = 0 THEN 0
		ELSE 1 
	   END
ORDER BY 1, 2

/*
V1:

0x0000000000000008	18601				-- scan map
0x0000000000000041	2154				-- pointed with nodding

V2:

0x0000000000000008	17915
		-- pointed with nodding missing
*/

-- valid observations with footprint

SELECT CAST(pointingMode AS binary(8)) AS [pointingMode], COUNT(*) AS [count]
FROM Observation
WHERE inst = 1 AND obsType = 1 AND region IS NOT NULL AND region.HasError(region) = 0
      AND calibration = 0 AND failed = 0
GROUP BY CAST(pointingMode AS binary(8))

/*
V1:

0x0000000000000008	17311
0x0000000000000041	65

V2:

0x0000000000000008	16719
*/

-- missing valid observations

SELECT inst, obsID, CAST(pointingMode AS binary(8)) AS [pointingMode], calibration, failed, obsLevel, sso
FROM Observation
WHERE inst = 1 AND obsType = 1 AND (region IS NULL OR region.HasError(region) = 1)
      AND calibration = 0 AND failed = 0

/*
V1:

1	1342185632	0x0000000000000041	0	0	255
1	1342185633	0x0000000000000041	0	0	255

V2:

long list of 926 observations
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
V1:

0	55
1	12

V2:

0	67
*/

SELECT obsID, sso
FROM Observation
WHERE inst = 1 AND obsType = 1 AND calibration = 0 AND failed = 0
  AND pointingMode = 0x0000000000000041