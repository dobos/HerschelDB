

SELECT inst, obsType, pointingMode, COUNT(*)
FROM Observation
WHERE calibration = 0 AND failed = 0
GROUP BY inst, obsType, pointingMode
ORDER BY 1, 2, 3

/*
V1:

inst	obsType	pointingMode	(No column name)
1	1	8	17303		missing: ra, dec, pa, aperture, fineTimeStart, fineTimeEnd, repetition(parallel)
1	1	65	37			missing: ra, dec, pa, aperture, fineTimeStart, fineTimeEnd
1	2	1	4033		missing: fineTimeStart, fineTimeEnd
1	2	4	1012		missing: fineTimeStart, fineTimeEnd
2	1	8	62			missing: aperture, fineTimeStart, fineTimeEnd
2	1	16	5029		missing: aperture, fineTimeStart, fineTimeEnd
2	1	32	760			missing: ra, dec, pa, aperture, fineTimeStart, fineTimeEnd, repetition
2	2	1	1082		OK
2	2	2	15			missing: ra, dec, pa, aperture
4	1	32	757			missing: ra, dec, pa, aperture, fineTimeStart, fineTimeEnd, repetition
8	2	1	7733		OK
8	2	4	782			OK

V2:

inst	obsType	pointingMode	(No column name)
1	1	8	17578
1	1	65	67
1	2	1	4154
1	2	4	1026
2	1	8	849
2	1	16	5074
2	1	65	1
2	2	1	1239
2	2	2	16
4	1	32	784
*/


SELECT TOP 10 *
FROM Observation
WHERE inst = 2 AND obsType = 2 AND pointingMode = 2
  AND calibration = 0 AND failed = 0




SELECT TOP 100 pa FROM load.Pointing
WHERE inst = 1 AND obsID = 1342185546



SELECT Avg(pa), STDEV(pa) FROM load.Pointing
WHERE inst = 1 AND obsID = 1342185546



SELECT * FROM load.PointingCluster
WHERE inst = 1 AND obsID = 1342183651