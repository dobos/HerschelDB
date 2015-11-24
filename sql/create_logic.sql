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
		SELECT DISTINCT inst, obsID
		FROM ObservationHtm htm
		WHERE htmid.FromEq(@ra, @dec) BETWEEN htmIDStart AND htmIDEnd AND [partial] = 0

		UNION

		SELECT DISTINCT htm.inst, htm.obsID
		FROM ObservationHtm htm
		INNER JOIN Observation r ON r.obsID = htm.obsID
		WHERE htmid.FromEq(@ra, @dec) BETWEEN htmIDStart AND htmIDEnd AND [partial] = 1
		-- TODO: add containment filter
	)
	SELECT inst, obsID
	FROM q
)

GO

GRANT SELECT ON [dbo].[FindObservationEq] TO [User]

GO

---------------------------------------------------------------

IF OBJECT_ID(N'dbo.FindRegionIntersectHtm') IS NOT NULL
DROP FUNCTION dbo.FindRegionIntersectHtm

GO

CREATE FUNCTION [dbo].[FindRegionIntersectHtm]
(	
	@region varbinary(max)
)
RETURNS TABLE 
AS
RETURN 
(
	WITH cover AS
	(
		SELECT * FROM htm.CoverAdvanced(@region, 0, 0.9, 2)
	),
	q AS
	(
		SELECT htm.*
		FROM cover
		INNER LOOP JOIN ObservationHtm htm ON
			htm.htmIDStart BETWEEN cover.htmIDStart AND cover.htmIDEnd

		UNION

		SELECT htm.*
		FROM cover
		INNER LOOP JOIN ObservationHtm htm ON
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
	SELECT q.*
	FROM q
)

GO

GRANT SELECT ON [dbo].[FindRegionIntersectHtm] TO [User]

GO

---------------------------------------------------------------

IF OBJECT_ID(N'dbo.FindObsIDIntersect') IS NOT NULL
DROP FUNCTION dbo.FindObsIDIntersect

GO

CREATE FUNCTION [dbo].[FindObsIDIntersect]
(	
	@region varbinary(max)
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT DISTINCT inst, obsID
	FROM dbo.FindRegionIntersectHtm(@region) htm
)

GO

GRANT SELECT ON [dbo].[FindObsIDIntersect] TO [User]

GO

---------------------------------------------------------------


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
	SELECT o.* --, region.[IntersectAdvanced](o.Region, @region, 1, 256) region
	FROM dbo.FindObsIDIntersect(@region) q
	INNER JOIN Observation o
		ON o.inst = q.inst AND o.obsID = q.obsID
)

GO

GRANT SELECT ON [dbo].[FindObservationRegionIntersect] TO [User]

GO

---------------------------------------------------------------

IF OBJECT_ID(N'dbo.FindObsIDContain') IS NOT NULL
DROP FUNCTION dbo.FindObsIDContain

GO

CREATE FUNCTION [dbo].[FindObsIDContain]
(	
	@region varbinary(max)
)
RETURNS @ret TABLE 
(
	inst tinyint NOT NULL,
	obsID bigint NOT NULL
)
AS
BEGIN 

	DECLARE @htmTemp TABLE
	(
		[inst] [tinyint] NOT NULL,
		[obsID] [bigint] NOT NULL,
		[htmIDStart] [bigint] NOT NULL,
		[htmIDEnd] [bigint] NOT NULL,
		[fineTimeStart] [bigint] NOT NULL,
		[fineTimeEnd] [bigint] NOT NULL,
		[partial] [bit] NOT NULL
	);

	INSERT @htmTemp
	SELECT * FROM [dbo].[FindRegionIntersectHtm](@region);

	WITH __intersecting AS
	(
		SELECT DISTINCT inst, obsID
		FROM @htmTemp
	),
	__contained AS
	(
		SELECT htm.*
		FROM __intersecting
		INNER LOOP JOIN ObservationHtm htm
			ON __intersecting.inst = htm.inst AND __intersecting.obsID = htm.obsID

		EXCEPT

		SELECT *
		FROM @htmTemp
	)
	INSERT @ret
		(inst, obsID)
	SELECT DISTINCT inst, obsID
		FROM __contained

	RETURN;

END;

GO

GRANT SELECT ON [dbo].[FindObsIDContain] TO [User]

GO

---------------------------------------------------------------

IF OBJECT_ID(N'dbo.FindObservationRegionContain') IS NOT NULL
DROP FUNCTION dbo.FindObservationRegionContain

GO

CREATE FUNCTION [dbo].[FindObservationRegionContain]
(	
	@region varbinary(max)
)
RETURNS TABLE
AS
RETURN 
(
	SELECT o.*
	FROM dbo.FindObsIDContain(@region) q
	INNER JOIN Observation o
		ON o.inst = q.inst AND o.obsID = q.obsID
)

GO

GRANT SELECT ON [dbo].[FindObservationRegionContain] TO [User]

GO