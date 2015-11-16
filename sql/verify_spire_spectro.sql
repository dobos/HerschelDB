-- SPIRE spectroscopic observations by pointing mode

SELECT CAST(pointingMode AS binary(8)) AS [pointingMode], COUNT(*) AS [count]
FROM Observation
WHERE inst = 2 AND obsType = 2
GROUP BY CAST(pointingMode AS binary(8))

/*
0x0000000000000001	2129		-- pointed
0x0000000000000000	24			-- calibration?
0x0000000000000002	22			-- raster
*/

----------------------------------------------------------------------------

-- only valid observations

SELECT CAST(pointingMode AS binary(8)) AS [pointingMode], COUNT(*) AS [count]
FROM Observation
WHERE inst = 2 AND obsType = 2
  AND calibration = 0 AND failed = 0
GROUP BY CAST(pointingMode AS binary(8))

/*
pointingMode	count
0x0000000000000001	1206		-- pointed
0x0000000000000002	15			-- raster
*/

-- any observation with footprint

SELECT CAST(pointingMode AS binary(8)) AS [pointingMode], COUNT(*) AS [count]
FROM Observation
WHERE inst = 2 AND obsType = 2
  AND region IS NOT NULL AND region.HasError(region) = 0
GROUP BY CAST(pointingMode AS binary(8))

/*
pointingMode	count
0x0000000000000001	2129
0x0000000000000002	21
*/

-- valid observations with footprint

SELECT CAST(pointingMode AS binary(8)) AS [pointingMode], COUNT(*) AS [count]
FROM Observation
WHERE inst = 2 AND obsType = 2
      AND region IS NOT NULL AND region.HasError(region) = 0
      AND calibration = 0 AND failed = 0
GROUP BY CAST(pointingMode AS binary(8))

/*
pointingMode	count
0x0000000000000001	1206
0x0000000000000002	15
*/

-- missing valid observations

SELECT inst, obsID, CAST(pointingMode AS binary(8)) AS [pointingMode], calibration, failed, obsLevel, sso, region.Error(region)
FROM Observation
WHERE inst = 2 AND obsType = 2 
      AND (region IS NULL OR region.HasError(region) = 1)
      AND calibration = 0 AND failed = 0


-- no results