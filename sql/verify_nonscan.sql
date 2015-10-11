SELECT inst, obsType, pointingMode, COUNT(*)
FROM Observation
GROUP BY inst, obsType, pointingMode
ORDER BY 1, 2, 3

/*
inst	obsType	pointingMode	(No column name)
1	1	2	13				-- raster mode
1	1	8	18872			-- scan map			OK
1	1	65	2185
1	1	66	23
1	2	1	5068
1	2	4	1088
2	1	1	88
2	1	8	1013
2	1	16	6020
2	1	65	328
2	2	0	24
2	2	1	2129
2	2	2	22
4	1	8	856
*/

SELECT * FROM Observation
WHERE inst = 1 AND obsType = 1 AND pointingMode = 2

SELECT *
FROM RasterMap