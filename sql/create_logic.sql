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