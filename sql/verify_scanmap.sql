







-- Mik azok a kalibrációs megfigyelések, amihez nincsen footprint

SELECT inst, obsType, calibration, obsLevel, COUNT(*)
FROM Observation
WHERE region IS NULL
GROUP BY inst, obsType, calibration, obsLevel
ORDER BY 1, 2, 3, 4

-- Ezekbõl túl sok van, mik ezek?
/*
inst	obsType	calibration	obsLevel	(No column name)
1	1	1	10	14
1	1	1	20	2109
*/

SELECT *
FROM Observation
WHERE inst = 1 AND obsType = 1 AND obsLevel < 250 AND region IS NULL

-- Itt a pointing mode nagy része 65-ös, ami rosszul sikerült chop-nod mérés

-- OK, akkor van-e még olyan scan map, amire nincsen footprint?
SELECT inst, obsType, calibration, obsLevel, COUNT(*)
FROM Observation
WHERE obsLevel < 250 AND pointingMode IN (8, 16) AND region IS NULL
GROUP BY inst, obsType, calibration, obsLevel
ORDER BY 1, 2, 3, 4

/*
  Most már alig maradt nagyon problémás:

inst	obsType	calibration	obsLevel	(No column name)
1	1	0	10	2
1	1	0	20	1
1	1	1	10	1
1	1	1	20	1
2	1	0	10	2
2	1	0	20	11
2	1	0	25	1
2	1	0	30	5
4	1	0	10	2
4	1	0	20	11
4	1	0	25	1
4	1	0	30	5
*/

-- Mik ezek?

SELECT inst, obsID, object, calibration, fineTimeStart
FROM Observation
WHERE obsLevel < 250 AND pointingMode IN (8, 16) AND region IS NULL
      AND obsType = 1

-- 43


-- Ehhez pl nincsen footprint

SELECT *
FROM load.RawPointing
WHERE inst IN (1, 2, 4) AND obsID = 1342211368

-- Van-e egyáltalán valamelyikhez pointing?

SELECT inst, obsID, object, calibration, fineTimeStart
FROM Observation
WHERE obsLevel < 250 AND pointingMode IN (8, 16) AND region IS NULL
      AND obsType = 1
	  AND (SELECT COUNT(*)
		   FROM load.RawPointing
		   WHERE inst = Observation.inst AND obsID = Observation.obsID) = 0

-- 41, amihez garantáltan nincsen pointing

-- És mi az, amihez van, de valamiért nem lett belõle footprint?

SELECT inst, obsID, object, calibration, obsLevel, pointingMode, fineTimeStart
FROM Observation
WHERE obsLevel < 250 AND pointingMode IN (8, 16) AND region IS NULL
      AND obsType = 1
	  AND (SELECT COUNT(*)
		   FROM load.RawPointing
		   WHERE inst = Observation.inst AND obsID = Observation.obsID) > 0

/*
inst	obsID	object	calibration	obsLevel	fineTimeStart
1	1342178069	m51	1	20	1623314561451217
1	1342270750	2000 Herschel	1	10	1745547770307411
*/

-- Vajon miért nincsen ezekhez footprint?

SELECT BBID, COUNT(*)
FROM Pointing
WHERE inst = 1 AND obsID = 1342270750
GROUP BY BBID



SELECT *
FROM load.[FilterPointingTurnaround](0, 0)
WHERE inst = 1 AND obsID = 1342178069


------------------------------------------------

/*
ezeknél hibás volt a pointingfájl-beolvasó, de most már elvileg OK

1342211368
1342211369
1342211370
*/

/*
inst	obsID
2	1342210963
2	1342210964
2	1342210982
2	1342210983
2	1342219620
2	1342219621
2	1342219642
2	1342219643
2	1342192680
2	1342192681
2	1342192682
2	1342192683
2	1342192697
2	1342192698
2	1342192699
2	1342250232
4	1342192680
4	1342192681
4	1342192682
4	1342192683
4	1342192697
4	1342192698
4	1342192699
4	1342210963
4	1342210964
4	1342210982
4	1342210983
4	1342219620
4	1342219621
4	1342219642
4	1342219643
4	1342250232
*/

SELECT *
FROM load.RawPointing WHERE inst = 1 AND obsID = 1342192697


SELECT *
FROM load.[FilterPointingTurnaround](0, 0)
WHERE inst = 1 AND obsID = 1342178069



----

-- Olyan SPIRE parallel mérések, amikhez nincsen használható pointing

SELECT inst, obsID, object, calibration, pointingMode, obsLevel, fineTimeStart
FROM Observation
WHERE obsLevel < 250 AND pointingMode IN (8, 16) AND region IS NULL
      AND obsType = 1 AND fineTimeStart > 0
	  AND (SELECT COUNT(*)
		   FROM load.RawPointing
		   WHERE inst = Observation.inst AND obsID = Observation.obsID) = 0