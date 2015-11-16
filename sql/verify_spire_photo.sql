-- SPIRE photometric observations by pointing mode

SELECT CAST(pointingMode AS binary(8)) AS [pointingMode], COUNT(*) AS [count]
FROM Observation
WHERE inst = 2 AND obsType = 1
GROUP BY CAST(pointingMode AS binary(8))
ORDER BY 1

/*
pointingMode	count
0x0000000000000001	88			-- pointed
0x0000000000000008	157			-- scan line
0x0000000000000010	6020		-- scan cross
0x0000000000000020	856			-- parallel
0x0000000000000041	328			-- point + nod
*/

-- only valid observations

SELECT CAST(pointingMode AS binary(8)) AS [pointingMode], COUNT(*) AS [count]
FROM Observation
WHERE inst = 2 AND obsType = 1
  AND calibration = 0 AND failed = 0
GROUP BY CAST(pointingMode AS binary(8))
ORDER BY 1

/*
0x0000000000000008	62			-- scan line
0x0000000000000010	5035		-- scan cross
0x0000000000000020	764			-- parallel
*/

-- any observation with footprint

SELECT CAST(pointingMode AS binary(8)) AS [pointingMode], COUNT(*) AS [count]
FROM Observation
WHERE inst = 2 AND obsType = 1 
  AND region IS NOT NULL AND region.HasError(region) = 0
GROUP BY CAST(pointingMode AS binary(8))
ORDER BY 1

/*
pointingMode	count
0x0000000000000008	151			-- scan line
0x0000000000000010	5968		-- scan cross
0x0000000000000020	837			-- parallel
*/

-- valid observations with footprint

SELECT CAST(pointingMode AS binary(8)) AS [pointingMode], COUNT(*) AS [count]
FROM Observation
WHERE inst = 2 AND obsType = 1 
  AND region IS NOT NULL AND region.HasError(region) = 0
  AND calibration = 0 AND failed = 0
GROUP BY CAST(pointingMode AS binary(8))
ORDER BY 1

/*
pointingMode	count
0x0000000000000008	62			-- scan line
0x0000000000000010	5035		-- scan cross
0x0000000000000020	764			-- parallel
*/


SELECT inst, obsID, CAST(pointingMode AS binary(8)) AS [pointingMode], calibration, failed, obsLevel, sso
FROM Observation
WHERE inst = 2 AND obsType = 1
  AND (region IS NULL OR region.HasError(region) = 1)
  AND calibration = 0 AND failed = 0

-- no results