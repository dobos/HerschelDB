-- HIFI spectroscopic observations by pointing mode

SELECT CAST(pointingMode AS binary(8)) AS [pointingMode], COUNT(*) AS [count]
FROM Observation
WHERE inst = 8 AND obsType = 2
GROUP BY CAST(pointingMode AS binary(8))
ORDER BY 1

/*
pointingMode	count
0x0000000000000001	8537		-- pointed
0x0000000000000004	1460		-- raster (mapping)
*/

------------------------------------------------------

-- only valid observations

SELECT CAST(pointingMode AS binary(8)) AS [pointingMode], COUNT(*) AS [count]
FROM Observation
WHERE inst = 8 AND obsType = 2
  AND calibration = 0 AND failed = 0
GROUP BY CAST(pointingMode AS binary(8))
ORDER BY 1

/*
0x0000000000000001	7898		-- pointed
0x0000000000000004	1257		-- mapping (raster)
*/

-- any observation with footprint

SELECT CAST(pointingMode AS binary(8)) AS [pointingMode], COUNT(*) AS [count]
FROM Observation
WHERE inst = 8 AND obsType = 2
  AND region IS NOT NULL AND region.HasError(region) = 0
GROUP BY CAST(pointingMode AS binary(8))
ORDER BY 1

/*
0x0000000000000001	8537		-- pointed
*/