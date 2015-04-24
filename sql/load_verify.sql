USE Herschel

GO


SELECT inst, obsType, pointingMode, COUNT(*) cnt
FROM load.RawObservation
WHERE calibration = 0 AND obsLevel < 250
GROUP BY inst, obsType, pointingMode
ORDER BY 1,2,3

/*

inst	obsType	pointingMode	cnt

1	0	8	16551		-- scan map
1	0	65	65			-- point-nod photometry
1	1	1	4071		-- pointed spectroscopy
1	1	4	1015		-- mapping spectroscopy

2	0	8	62			-- scan map (line)
2	0	16	5035		-- scan map (cross)
2	0	65	4			-- point-nod photometry
2	1	1	1219		-- pointed spectroscopy
2	1	2	15			-- raster spectroscopy

4	0	8	781			-- scan map

8	1	1	7933		-- pointed spectroscopy
8	1	4	1262		-- mapping spectroscopy

*/

---------------------------------------------------------------

-- PACS

-- -- scan map

		SELECT *
		FROM load.RawObservation
		WHERE obsID NOT IN
				(
					SELECT obsID
					FROM load.RawPointing
					WHERE inst = 1 AND BBID = 215131301
				)
			AND inst = 1 AND obsType = 0 AND pointingMode IN (8, 16) AND calibration = 0 AND obsLevel < 250

		-- missing: 
		-- 1342270750	-- this is a weird one with a parabola trajectory

		SELECT * FROM load.RawPointing WHERE inst = 1 AND obsID = 1342270750

-- -- point-nod photometry

		SELECT *
		FROM load.RawObservation
		WHERE obsID NOT IN
				(
					SELECT obsID
					FROM load.RawPointing
					WHERE inst = 1
				)
			AND inst = 1 AND obsType = 0 AND pointingMode = 65 AND calibration = 0 AND obsLevel < 250

		-- missing: none

		SELECT * FROM load.RawObservation 
		WHERE inst = 1 AND obsType = 0 AND pointingMode = 65 AND calibration = 0 AND obsLevel < 250

-- -- pointed spectroscopy

		SELECT *
		FROM load.RawObservation
		WHERE obsID NOT IN
				(
					SELECT obsID
					FROM load.RawPointing
					WHERE inst = 1
				)
			AND inst = 1 AND obsType = 1 AND pointingMode = 1 AND calibration = 0 AND obsLevel < 250

		-- missing:
		-- 1342188041

		SELECT * FROM load.RawPointing WHERE obsID = 1342188041

		-- take fineTime from pointings

-- -- mapping spectroscopy

		SELECT *
		FROM load.RawObservation
		WHERE obsID NOT IN
				(
					SELECT obsID
					FROM load.RawPointing
					WHERE inst = 1
				)
			AND inst = 1 AND obsType = 1 AND pointingMode = 4 AND calibration = 0 AND obsLevel < 250

		-- missing:
		-- 1342245811

		SELECT * FROM load.RawObservation
		WHERE inst = 1 AND obsID = 1342245811

		SELECT * FROM load.RawObservation
		WHERE inst = 1 AND obsType = 1 AND pointingMode = 4 AND calibration = 0 AND obsLevel < 250

		-- take fineTime from pointings

---------------------------------------------------------------

-- SPIRE

-- -- scan map

		SELECT *
		FROM load.RawObservation
		WHERE obsID NOT IN
				(
					SELECT obsID
					FROM load.RawPointing
					WHERE inst = 2
				)
			AND inst = 2 AND obsType = 0 AND pointingMode = 8 AND calibration = 0 AND obsLevel < 250

		-- missing: none

		SELECT * FROM load.RawObservation
		WHERE inst = 2 AND obsType = 0 AND pointingMode = 8 AND calibration = 0 AND obsLevel < 250

-- -- point nod photometry

		SELECT *
		FROM load.RawObservation
		WHERE obsID NOT IN
				(
					SELECT obsID
					FROM load.RawPointing
					WHERE inst = 2
				)
			AND inst = 2 AND obsType = 0 AND pointingMode = 65 AND calibration = 0 AND obsLevel < 250

		-- missing: none

		SELECT * FROM load.RawObservation
		WHERE inst = 2 AND obsType = 0 AND pointingMode = 65 AND calibration = 0 AND obsLevel < 250

		-- single pointing entry for each one
		-- take fineTime from pointing

-- -- pointed spectroscopy

		SELECT *
		FROM load.RawObservation
		WHERE obsID NOT IN
				(
					SELECT obsID
					FROM load.RawPointing
					WHERE inst = 2
				)
			AND inst = 2 AND obsType = 1 AND pointingMode = 1 AND calibration = 0 AND obsLevel < 250

-- -- raster spectroscopy

		-- no pointing is necessary to build footprint
		-- where to get fineTimeStart and fineTimeEnd from?

---------------------------------------------------------------

-- PARALLEL

		SELECT *
		FROM load.RawObservation
		WHERE obsID NOT IN
				(
					SELECT obsID
					FROM load.RawPointing
					WHERE inst = 1		-- use pointing from PACS
				)
			AND inst = 4 AND obsType = 0 AND pointingMode = 8 AND calibration = 0 AND obsLevel < 250

		-- missing:
		-- 1342211368
		-- 1342211369
		-- 1342211370

		SELECT *
		FROM load.RawObservation
		WHERE obsID NOT IN
				(
					SELECT obsID
					FROM load.RawPointing
					WHERE inst = 2		-- use pointing from SPIRE
				)
			AND inst = 4 AND obsType = 0 AND pointingMode = 8 AND calibration = 0 AND obsLevel < 250

---------------------------------------------------------------

-- HIFI

-- -- pointed spectroscopy

		SELECT *
		FROM load.RawObservation
		WHERE obsID NOT IN
				(
					SELECT obsID
					FROM load.RawPointing
					WHERE inst = 8
				)
			AND inst = 8 AND obsType = 1 AND pointingMode = 1 AND calibration = 0 AND obsLevel < 250

-- -- mapping spectroscopy

		SELECT *
		FROM load.RawObservation
		WHERE obsID NOT IN
				(
					SELECT obsID
					FROM load.RawPointing
					WHERE inst = 8
				)
			AND inst = 8 AND obsType = 1 AND pointingMode = 4 AND calibration = 0 AND obsLevel < 250

		-- theres pointing for only 4 special observations