IF OBJECT_ID(N'dbo.FindObservationEq') IS NOT NULL
DROP FUNCTION dbo.FindObservationEq

GO

CREATE FUNCTION [dbo].[FindObservationEq]
(	
	@ra float,
	@dec float
)
RETURNS TABLE 
AS
RETURN 
(
	WITH q AS
	(
		SELECT DISTINCT obsID
		FROM ObservationHtm htm
		WHERE htm.FromEq(@ra, @dec) BETWEEN htmIDStart AND htmIDEnd AND [partial] = 0

		UNION

		SELECT DISTINCT htm.obsID
		FROM ObservationHtm htm
		INNER JOIN Observation r ON r.obsID = htm.obsID
		WHERE htm.FromEq(@ra, @dec) BETWEEN htmIDStart AND htmIDEnd AND [partial] = 1
		-- TODO: add containment filter
	)
	SELECT q.obsID
	FROM q
)

GO

GRANT SELECT ON [dbo].[FindObservationEq] TO [User]

GO


IF OBJECT_ID(N'dbo.FindObservationRegionIntersect') IS NOT NULL
DROP FUNCTION dbo.FindObservationRegionIntersect

GO

CREATE FUNCTION [dbo].[FindObservationRegionIntersect]
(	
	@region varbinary(max)
)
RETURNS TABLE 
AS
RETURN 
(
	WITH cover AS
	(
		SELECT * FROM htm.Cover(@region)
	),
	q AS
	(
		SELECT DISTINCT obsID
		FROM ObservationHtm htm WITH(FORCESEEK)
		INNER JOIN cover ON
			htm.htmIDStart BETWEEN cover.htmIDStart AND cover.htmIDEnd

		UNION

		SELECT DISTINCT obsID
		FROM ObservationHtm htm WITH(FORCESEEK)
		INNER JOIN cover ON
			(htm.htmIDStart = cover.htmIDStart OR
			htm.htmIDStart = cover.htmIDStart & 0xFFFFFFFFFFFFFFFC OR
			htm.htmIDStart = cover.htmIDStart & 0xFFFFFFFFFFFFFFF0 OR
			htm.htmIDStart = cover.htmIDStart & 0xFFFFFFFFFFFFFFC0 OR
			htm.htmIDStart = cover.htmIDStart & 0xFFFFFFFFFFFFFF00 OR
			htm.htmIDStart = cover.htmIDStart & 0xFFFFFFFFFFFFFC00 OR
			htm.htmIDStart = cover.htmIDStart & 0xFFFFFFFFFFFFF000 OR
			htm.htmIDStart = cover.htmIDStart & 0xFFFFFFFFFFFFC000 OR
			htm.htmIDStart = cover.htmIDStart & 0xFFFFFFFFFFFF0000 OR
			htm.htmIDStart = cover.htmIDStart & 0xFFFFFFFFFFFC0000 OR
			htm.htmIDStart = cover.htmIDStart & 0xFFFFFFFFFFF00000 OR
			htm.htmIDStart = cover.htmIDStart & 0xFFFFFFFFFFC00000 OR
			htm.htmIDStart = cover.htmIDStart & 0xFFFFFFFFFF000000 OR
			htm.htmIDStart = cover.htmIDStart & 0xFFFFFFFFFC000000 OR
			htm.htmIDStart = cover.htmIDStart & 0xFFFFFFFFF0000000 OR
			htm.htmIDStart = cover.htmIDStart & 0xFFFFFFFFC0000000 OR
			htm.htmIDStart = cover.htmIDStart & 0xFFFFFFFF00000000 OR
			htm.htmIDStart = cover.htmIDStart & 0xFFFFFFFC00000000 OR
			htm.htmIDStart = cover.htmIDStart & 0xFFFFFFF000000000 OR
			htm.htmIDStart = cover.htmIDStart & 0xFFFFFFC000000000)
			AND htm.htmIDEnd >= cover.htmIDStart
	)
	SELECT q.obsID, region.[IntersectAdvanced](o.Region, @region, 1) region
	FROM q
	INNER JOIN Observation o WITH (FORCESEEK)
		ON o.obsID = q.obsID
)

GO

GRANT SELECT ON [dbo].[FindObservationRegionIntersect] TO [User]

GO