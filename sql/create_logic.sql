IF OBJECT_ID(N'dbo.FilterPointingTurnaround') IS NOT NULL
DROP FUNCTION dbo.FilterPointingTurnaround

GO

CREATE FUNCTION dbo.FilterPointingTurnaround
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
			SQRT(avy*avy + avz*avz) av,
			AVG(SQRT(avy*avy + avz*avz))
				OVER (PARTITION BY ObsID
					  ORDER BY fineTime
					  ROWS BETWEEN 5 PRECEDING
							   AND 5 FOLLOWING) av_avg,
			VAR(SQRT(avy*avy + avz*avz))
				OVER (PARTITION BY ObsID
					  ORDER BY fineTime
					  ROWS BETWEEN 5 PRECEDING
							   AND 5 FOLLOWING) av_var
		FROM Pointing
	),
	b AS
	(
		SELECT a.*,
			a.fineTime - LAG(a.fineTime, 1, NULL) OVER(PARTITION BY a.ObsID ORDER BY a.obsID, a.fineTime) deltaLag,
			a.fineTime - LEAD(a.fineTime, 1, NULL) OVER(PARTITION BY a.ObsID ORDER BY a.obsID, a.fineTime) deltaLead
		FROM a
		INNER JOIN Observation o ON o.obsID = a.obsID
		WHERE av_avg BETWEEN o.av - o.av * @avDiffMax / 100.0 AND o.av + o.av * @avDiffMax / 100.0
			--AND av_var < @avVarMax
	)
	SELECT * FROM b
)

GO



IF OBJECT_ID(N'dbo.FindLegEnds') IS NOT NULL
DROP FUNCTION dbo.FindLegEnds

GO

CREATE FUNCTION dbo.FindLegEnds
(
	@avDiffMax float,
	@avVarMax float,
	@legMinGap float
)
RETURNS TABLE
AS
RETURN
(
	WITH
	b AS
	(
		SELECT * FROM dbo.FilterPointingTurnaround(@avDiffMax, @avVarMax)
	),
	leg AS
	(
		SELECT b.*,
			(ROW_NUMBER() OVER(PARTITION BY ObsID ORDER BY fineTime) + 1) / 2 leg,
			CASE
				WHEN (deltaLead < -@legMinGap OR deltaLead IS NULL) AND (deltaLag > @legMinGap OR deltaLag IS NULL) THEN -1
				WHEN deltaLead < -@legMinGap OR deltaLead IS NULL THEN 0
				WHEN deltaLag > @legMinGap OR deltaLag IS NULL THEN 1
				ELSE NULL
			END start
		FROM b
		WHERE (deltaLead < -@legMinGap OR deltaLead IS NULL) OR 
			  (deltaLag > @legMinGap OR deltaLag IS NULL) AND
			  NOT ((deltaLead < -@legMinGap OR deltaLead IS NULL) AND (deltaLag > @legMinGap OR deltaLag IS NULL))
	)
	SELECT * FROM leg
)

GO


IF OBJECT_ID(N'dbo.FindLegEq') IS NOT NULL
DROP FUNCTION dbo.FindLegEq

GO

CREATE FUNCTION dbo.FindLegEq
(	
	@ra float,
	@dec float,
	@fineTime float = NULL
)
RETURNS TABLE 
AS
RETURN 
(
	WITH q AS
	(
		SELECT DISTINCT obsID, legID
		FROM LegHtm htm
		WHERE dbo.fHtmEq(@ra, @dec) BETWEEN htmIDStart AND htmIDEnd
			  AND partial = 0
			  AND (@fineTime IS NULL OR @fineTime BETWEEN fineTimeStart AND fineTimeEnd)

		UNION

		SELECT DISTINCT htm.obsID, htm.legID
		FROM LegHtm htm
		INNER JOIN LegRegion r ON r.obsID = htm.obsID AND r.legID = htm.legID
		WHERE dbo.fHtmEq(@ra, @dec) BETWEEN htmIDStart AND htmIDEnd
			  AND partial = 1 AND dbo.fContainsEq(r.region, @ra, @dec) = 1
			  AND (@fineTime IS NULL OR @fineTime BETWEEN htm.fineTimeStart AND htm.fineTimeEnd)
	)
	SELECT q.obsID, q.legID
	FROM q
)

GO


IF OBJECT_ID(N'dbo.FindObservationEq') IS NOT NULL
DROP FUNCTION dbo.FindLegEq

GO

CREATE FUNCTION dbo.FindObservationEq
(	
	@ra float,
	@dec float,
	@fineTime float = NULL
)
RETURNS TABLE 
AS
RETURN 
(
	WITH q AS
	(
		SELECT DISTINCT obsID
		FROM ObservationHtm htm
		WHERE dbo.fHtmEq(@ra, @dec) BETWEEN htmIDStart AND htmIDEnd
			  AND partial = 0
			  AND (@fineTime IS NULL OR @fineTime BETWEEN fineTimeStart AND fineTimeEnd)

		UNION

		SELECT DISTINCT htm.obsID
		FROM ObservationHtm htm
		INNER JOIN ObservationRegion r ON r.obsID = htm.obsID
		WHERE dbo.fHtmEq(@ra, @dec) BETWEEN htmIDStart AND htmIDEnd
			  AND partial = 1 AND dbo.fContainsEq(r.region, @ra, @dec) = 1
			  AND (@fineTime IS NULL OR @fineTime BETWEEN htm.fineTimeStart AND htm.fineTimeEnd)
	)
	SELECT q.obsID
	FROM q
)

GO