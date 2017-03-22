SET NOCOUNT ON;

WITH q AS
(
	SELECT CAST(pointingMode AS binary(8)) AS pointingMode, 
		   CASE 
			WHEN region IS NOT NULL AND region.HasError(region) = 0 THEN 1
			ELSE 0
		   END AS footprint
	FROM dbo.Observation
	WHERE inst = 1 AND obsType = 2 AND instMode & 0x00040000 != 0 AND calibration = 0 AND failed = 0 AND sso = 0
)
SELECT pointingMode, footprint, COUNT(*) AS count
FROM q
GROUP BY pointingMode, footprint
ORDER BY 1,2
