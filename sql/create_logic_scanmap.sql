IF OBJECT_ID(N'load.FilterPointingTurnaround') IS NOT NULL
DROP FUNCTION [load].[FilterPointingTurnaround]

GO
CREATE FUNCTION [load].[FilterPointingTurnaround]
(
	@avDiffMax float,
	@avVarMax float
)
RETURNS TABLE
AS
RETURN
(
	WITH 
	a AS	-- average velocities over +/-5 bins 
	( 
		SELECT 
			*, 
			AVG(av) 
				OVER (PARTITION BY inst, ObsID, obsType
					  ORDER BY fineTime 
					  ROWS BETWEEN 5 PRECEDING 
							   AND 5 FOLLOWING) av_avg, 
			VAR(av) 
				OVER (PARTITION BY inst, ObsID, obsType
					  ORDER BY fineTime 
					  ROWS BETWEEN 5 PRECEDING 
							   AND 5 FOLLOWING) av_var 
		FROM Pointing 
	), 
	b AS 
	( 
			SELECT a.*, 
				a.fineTime - LAG(a.fineTime, 1, NULL) OVER(PARTITION BY a.inst, a.ObsID ORDER BY a.inst, a.obsID, a.fineTime) deltaLag, 
				a.fineTime - LEAD(a.fineTime, 1, NULL) OVER(PARTITION BY a.inst, a.ObsID ORDER BY a.inst, a.obsID, a.fineTime) deltaLead 
			FROM a 
			INNER JOIN Observation o ON o.inst = a.inst AND o.obsID = a.obsID 
			INNER JOIN ScanMap s ON s.inst = a.inst AND s.obsID = a.obsID 
			WHERE 
				((o.inst = 1 AND o.pointingMode = 8 AND BBID = 215131301)		-- PACS scan maps
				OR (o.inst = 1 AND o.obsID = 1342178069 AND BBID != 1073741824) -- something special
				OR o.inst = 2 AND o.pointingMode IN (8, 16))					-- SPIRE scan map, turn-around is filtered by default
				--AND av_avg BETWEEN s.av * (1 - @avDiffMax) AND s.av * (1 + @avDiffMax)
				--AND (@avVarMax IS NULL OR av_var < @avVarMax)
 	) 
	SELECT *
	FROM b
)

GO

----------------------------------------------------------------


IF OBJECT_ID(N'load.FindLegEnds') IS NOT NULL
DROP FUNCTION [load].[FindLegEnds]

GO

CREATE FUNCTION [load].[FindLegEnds]
(
	@legMinGap float
)
RETURNS TABLE
AS
RETURN
(
/* leg ends are determined by gaps */

	SELECT inst, obsID,
		(ROW_NUMBER() OVER(PARTITION BY inst, obsID ORDER BY fineTime) + 1) / 2 leg,
		CASE
			WHEN (deltaLead < -@legMinGap OR deltaLead IS NULL) AND (deltaLag > @legMinGap OR deltaLag IS NULL) THEN -1
			WHEN deltaLead < -@legMinGap OR deltaLead IS NULL THEN 0
			WHEN deltaLag > @legMinGap OR deltaLag IS NULL THEN 1
			ELSE NULL
		END start,
		fineTime, ra, dec, pa
	FROM [load].FilterPointingTurnaround(0.1, 1)
	WHERE (deltaLead < -@legMinGap OR deltaLead IS NULL)
		  OR (deltaLag > @legMinGap OR deltaLag IS NULL) 
		  AND NOT ((deltaLead < -@legMinGap OR deltaLead IS NULL) AND (deltaLag > @legMinGap OR deltaLag IS NULL))
		  
)

GO

----------------------------------------------------------------

IF OBJECT_ID ('load.DetectLegs', N'P') IS NOT NULL
DROP PROC [load].[DetectLegs]

GO

CREATE PROC [load].[DetectLegs]
	@legMinGap bigint = 5e6				-- minimum gap to start new leg, in units of fine time
AS
/*
Detect scan legs from raw pointings

Raw points are filtered for scan legs. Legs are detected from gaps in pointings.
*/

	TRUNCATE TABLE [load].[LegEnds];

	INSERT [load].[LegEnds] WITH(TABLOCKX)
	SELECT inst, obsID, leg, start, fineTime, ra, dec, pa
	FROM [load].[FindLegEnds](@legMinGap)
	WHERE start IN (0, 1)
	ORDER BY obsID, fineTime;

	TRUNCATE TABLE [load].[Leg];

	INSERT [load].[Leg] WITH(TABLOCKX)
	SELECT a.inst, a.obsID, 2 * a.legID, a.fineTime, b.fineTime, a.ra, a.dec, a.pa, b.ra, b.dec, b.pa
	FROM [load].[LegEnds] a
	INNER JOIN [load].[LegEnds] b ON a.inst = b.inst AND a.obsID = b.obsID AND a.legID = b.legID
	WHERE a.start = 1 AND b.start = 0

	-- SPIRE cross-scan maps also connect ends of legs
	INSERT [load].[Leg] WITH(TABLOCKX)
	SELECT a.inst, a.obsID, 2 * a.legID + 1, a.fineTime, b.fineTime, a.ra, a.dec, a.pa, b.ra, b.dec, b.pa
	FROM [load].[LegEnds] a
	INNER JOIN [load].[LegEnds] b ON a.inst = b.inst AND a.obsID = b.obsID AND a.legID = b.legID - 1
	INNER JOIN Observation o ON o.inst = a.inst AND o.obsID = a.obsID
	WHERE a.start = 0 AND b.start = 1
		AND a.inst = 2 AND o.pointingMode IN (8, 16)

	--TRUNCATE TABLE [load].[LegEnds];

GO

----------------------------------------------------------------

IF OBJECT_ID ('load.VerifyLegs', N'P') IS NOT NULL
DROP PROC [load].[VerifyLegs]

GO

CREATE PROC [load].[VerifyLegs]
AS
/*
Verify that legs are generated for every PACS and SPIRE scan maps
*/

	SELECT o.inst, o.obsType, o.obsID, o.pointingMode, o.calibration, o.obsLevel, l.cnt
	FROM Observation o
	LEFT OUTER JOIN 
		( SELECT inst, obsID, COUNT(*) cnt
		  FROM load.Leg l 
		  GROUP BY inst, obsID
		) AS l
		  ON l.inst = o.inst AND l.obsID = o.obsID
	WHERE o.inst IN (1, 2) AND o.pointingMode IN (8, 16)
		AND cnt IS NULL
	ORDER BY 1,2
	
GO

----------------------------------------------------------------

IF OBJECT_ID ('load.GenerateLegFootprints', N'P') IS NOT NULL
DROP PROC [load].[GenerateLegFootprints]

GO

CREATE PROC [load].[GenerateLegFootprints]
AS
/*
	Generate regions for legs
*/

	TRUNCATE TABLE [load].LegRegion;

	DBCC SETCPUWEIGHT(1000); 

	INSERT [load].[LegRegion] WITH (TABLOCKX)
	SELECT leg.inst, leg.obsID, leg.legID, leg.fineTimeStart, leg.fineTimeEnd,
		   dbo.GetLegRegion(leg.raStart, leg.decStart, leg.paStart, leg.raEnd, leg.decEnd, leg.paEnd,
		    CASE inst
			WHEN 1 THEN 'PacsPhoto'
			WHEN 2 THEN 'SpirePhoto'
			END)
	FROM [load].Leg leg

GO

----------------------------------------------------------------


IF OBJECT_ID ('load.GenerateFootprint', N'P') IS NOT NULL
DROP PROC [load].[GenerateFootprint]

GO

CREATE PROC [load].[GenerateFootprint]
AS
/*
	Generate regions for legs then union them into observations

	NOT USED... moved to hload instead
*/

	-- Generate region using standard 'UNION' method

	DBCC SETCPUWEIGHT(1000); 

	UPDATE [Observation]
	SET region = leg.region
	FROM [Observation] obs
	INNER JOIN
		(SELECT inst, obsID, MIN(fineTimeStart) fineTimeStart, Max(fineTimeEnd) fineTimeEnd, region.UnionEvery(region) region
		 FROM [load].LegRegion
		 GROUP BY inst, obsID) leg
		ON leg.inst = obs.inst AND leg.obsID = obs.obsID
	WHERE    (obs.inst = 1 AND obs.pointingMode = 8)			-- PACS scan maps
		  OR (obs.inst = 2 AND obs.pointingMode IN (8, 16))		-- SPIRE scan maps
	OPTION (MAXDOP 4);

	WITH 
	legs AS
	(
		SELECT *, ROW_NUMBER() OVER(PARTITION BY inst, obsID ORDER BY fineTimeStart) rn
		FROM [load].LegRegion
	),
	legcount AS
	(
		SELECT inst, obsid, COUNT(*) cnt
		FROM legs
		GROUP BY inst, obsid
	),
	legunion AS
	(
		SELECT legs.inst, legs.obsID, MIN(legs.fineTimeStart) fineTimeStart, Max(legs.fineTimeEnd) fineTimeEnd, region.UnionEvery(legs.region) region
		FROM Observation o
		INNER JOIN legs ON legs.inst = o.inst AND legs.obsID = o.obsID
		INNER JOIN legcount c ON c.inst = o.inst AND c.obsID = o.obsID
		WHERE rn < c.cnt / o.repetition
		GROUP BY legs.inst, legs.obsID
	)
	UPDATE [Observation]
	SET region = leg.region
	FROM [Observation] obs
	INNER JOIN legunion leg
		ON leg.inst = obs.inst AND leg.obsID = obs.obsID
	WHERE  
		--obs.region IS NOT NULL AND region.HasError(obs.Region) = 1	AND		-- only fixes  
		(    (obs.inst = 1 AND obs.pointingMode = 8)			-- PACS scan maps
		  OR (obs.inst = 2 AND obs.pointingMode IN (8, 16)))		-- SPIRE scan maps
	OPTION (MAXDOP 4);

	-- Fill in problematic ones with 'CHULL' method
	
	WITH points AS
	(
		SELECT leg.inst, leg.obsid, arcs.x1 x, arcs.y1 y, arcs.z1 z
		FROM Observation obs
		INNER JOIN [load].LegRegion leg
			ON leg.inst = obs.inst AND leg.obsID = obs.obsID
		CROSS APPLY region.GetArcs(leg.region) arcs

		UNION

		SELECT leg.inst, leg.obsid, arcs.x2 x, arcs.y2 y, arcs.z2 z
		FROM Observation obs
		INNER JOIN [load].LegRegion leg
			ON leg.inst = obs.inst AND leg.obsID = obs.obsID
		CROSS APPLY region.GetArcs(leg.region) arcs
	), chull AS
	(
		SELECT inst, obsid, region.ConvexHullXyz(x,y,z) region
		FROM points
		GROUP BY inst, obsid
	)
	UPDATE [Observation]
	SET region = chull.region
	FROM [Observation] obs
	INNER JOIN chull ON chull.inst = obs.inst AND chull.obsID = obs.obsID
	WHERE obs.region IS NULL OR region.HasError(obs.region) = 1

	DBCC SETCPUWEIGHT(1); 

GO

----------------------------------------------------------------

IF OBJECT_ID ('load.UpdateParallelFootprints', N'P') IS NOT NULL
DROP PROC [load].[UpdateParallelFootprints]

GO

CREATE PROC [load].[UpdateParallelFootprints]
AS
/*
	Update region for PACS-SPIRE parallel observations
*/

	DBCC SETCPUWEIGHT(1000); 

	-- Attempt to compute intersection directly

	WITH parallel AS
	(
		SELECT obs.inst, obs.obsID, region.IntersectAdvanced(a.region, b.region, 1, 1000) region
		FROM [Observation] obs WITH (FORCESCAN)
		INNER JOIN Observation a ON a.inst = 1 AND a.obsID = obs.obsID
		INNER JOIN Observation b ON b.inst = 2 AND b.obsID = obs.obsID
		WHERE a.region IS NOT NULL AND b.region IS NOT NULL
			AND obs.inst = 4
	)
	UPDATE obs WITH (TABLOCKX)
	SET region = parallel.region
	FROM [Observation] obs
	INNER JOIN parallel ON parallel.inst = obs.inst AND parallel.obsID = obs.obsID
	WHERE obs.region IS NULL

	/*
	-- Fill in problematic ones with 'CHULL' method
	
	WITH points AS
	(
		SELECT obs.inst, obs.obsid, arcs.x1 x, arcs.y1 y, arcs.z1 z
		FROM Observation obs
		CROSS APPLY region.GetArcs(obs.region) arcs

		UNION

		SELECT obs.inst, obs.obsid, arcs.x2 x, arcs.y2 y, arcs.z2 z
		FROM Observation obs
		CROSS APPLY region.GetArcs(obs.region) arcs
	), chull AS
	(
		SELECT inst, obsid, region.ConvexHullXyz(x,y,z) region
		FROM points
		GROUP BY inst, obsid
	), parallel AS
	(
		SELECT a.obsID, a.region r1, b.region r2
		FROM chull a
		INNER JOIN chull b ON a.obsID = b.obsID
		WHERE a.inst = 1		-- PACS
			  AND b.inst = 2	-- SPIRE
	)
	UPDATE [Observation] WITH (TABLOCKX)
	SET region = region.IntersectAdvanced(r1, r2, 1, 1000)
	FROM [Observation] obs
	INNER JOIN parallel ON obs.inst = 4 AND parallel.obsID = obs.obsID
	WHERE obs.inst = 4 AND region.HasError(obs.region) = 1		-- Parallel
	*/

	DBCC SETCPUWEIGHT(1); 

GO

----------------------------------------------------------------

IF OBJECT_ID ('load.VerifyFootprints', N'P') IS NOT NULL
DROP PROC [load].[VerifyFootprints]

GO

CREATE PROC [load].[VerifyFootprints]
AS

	SELECT inst, obsid, repetition, region.GetErrorMessage(region)
	FROM Observation
	WHERE inst IN (1,2,4) AND pointingMode IN (8, 16)
		AND calibration = 0 AND obsLevel < 250
		AND (region IS NULL OR region.HasError(region) = 1)
	ORDER BY inst, obsid