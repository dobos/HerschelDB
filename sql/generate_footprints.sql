-- Generate leg regions

TRUNCATE TABLE LegRegion

INSERT LegRegion WITH (TABLOCKX)
SELECT obsID, legID, fineTimeStart, fineTimeEnd,
       dbo.fGetLegRegion(raStart, decStart, paStart, raEnd, decEnd, paEnd, 'Blue')
FROM Leg

GO


-- Generate leg HTM

TRUNCATE TABLE LegHtm

INSERT LegHtm WITH (TABLOCKX)
SELECT obsID, legID, htm.htmidStart, htm.htmidEnd, fineTimeStart, fineTimeEnd, htm.partial
FROM LegRegion
CROSS APPLY dbo.fGetHtmCover(region) htm

GO


-- Generate unions of legs

TRUNCATE TABLE ObservationRegion

INSERT ObservationRegion WITH (TABLOCKX)
SELECT obsID, MIN(fineTimeStart), Max(fineTimeEnd), dbo.fRegionUnion(region)
FROM LegRegion
GROUP BY obsID

GO


-- Generate observation HTM

TRUNCATE TABLE ObservationHtm

INSERT ObservationHtm WITH (TABLOCKX)
SELECT obsID, htm.htmidStart, htm.htmidEnd, fineTimeStart, fineTimeEnd, htm.partial
FROM ObservationRegion
CROSS APPLY dbo.fGetHtmCover(region) htm