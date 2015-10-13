SELECT inst, obsType, pointingMode, COUNT(*)
FROM Observation
GROUP BY inst, obsType, pointingMode
ORDER BY 1, 2, 3

/*
inst	obsType	pointingMode	(No column name)
1	1	2	13				-- raster mode		TODO
1	1	8	18872			-- scan map			OK
1	1	65	2185			-- chop-nod			TODO
1	1	66	23				-- chop-nod raster	calib only
1	2	1	5068			-- spec
1	2	4	1088			-- spec
2	1	1	88				-- pointed			calib only
2	1	8	1013			-- scan map			OK
2	1	16	6020			-- scan map			OK
2	1	65	328				-- chop-nod			mostly cal, single point
2	2	0	24				-- spec, no pointing		calib only
2	2	1	2129
2	2	2	22
4	1	8	856				--					OK
*/

-- PACS raster maps

SELECT * FROM Observation
WHERE inst = 1 AND obsType = 1 AND pointingMode = 2

SELECT * FROM load.RawObservation WHERE inst = 1
	AND obsID IN 
	(
	SELECT obsid FROM Observation
	WHERE inst = 1 AND obsType = 1 AND pointingMode = 2
	)

SELECT * FROM load.RawPointing WHERE inst = 1 AND obsID = 1342184600

SELECT BBID, COUNT(*)
FROM load.RawPointing WHERE inst = 1 AND obsID = 1342184600
GROUP BY BBID

----------*****************--------------

SELECT * FROM Observation
WHERE inst = 1 AND obsType = 1 AND pointingMode = 66


SELECT * FROM load.RawObservation WHERE inst = 1
	AND obsID IN 
	(
	SELECT obsid FROM Observation
	WHERE inst = 1 AND obsType = 1 AND pointingMode = 66
	)

SELECT BBID, COUNT(*)
FROM load.RawPointing WHERE inst = 1 AND obsID = 1342183560
GROUP BY BBID

---------------************---------------

-- SPIRE, pointing mode 0???

SELECT * FROM Observation
WHERE inst = 2 AND obsType = 2 AND pointingMode = 0