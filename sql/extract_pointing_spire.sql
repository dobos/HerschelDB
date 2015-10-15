SELECT
	o.obsID, p.fineTime, c.x, c.y, c.z
FROM Observation o
INNER JOIN load.Pointing p
	ON o.inst = p.inst AND o.obsID = p.obsID
CROSS APPLY dbo.GetDetectorCornersXyz(p.ra, p.dec, p.pa, 'SpirePhoto') c
WHERE o.inst = 2 AND o.pointingMode IN (8, 16)
ORDER BY obsID, fineTime, c.id