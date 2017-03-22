SET NOCOUNT ON;

WITH q AS
(
	SELECT CAST(pointingMode AS binary(8)) AS pointingMode, 
		   CASE 
			WHEN region IS NOT NULL AND region.HasError(region) = 0 THEN 1
			ELSE 0
		   END AS footprint
	FROM dbo.Observation
	WHERE inst = 4 AND obsType = 1
)
SELECT pointingMode, footprint, COUNT(*) AS count
FROM q
GROUP BY pointingMode, footprint
ORDER BY 1,2