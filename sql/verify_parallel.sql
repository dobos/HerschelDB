-- PACS/SPIRE parallel photometric observations

SELECT CAST(pointingMode AS binary(8)) AS [pointingMode], COUNT(*) AS [count]
FROM Observation
WHERE inst = 4 AND obsType = 1
GROUP BY CAST(pointingMode AS binary(8))