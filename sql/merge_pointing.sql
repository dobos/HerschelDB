EXEC [load].[MergePointing]
GO

EXEC [load].[DetectObservations]
GO

EXEC [load].[DetectLegs]
GO

EXEC [load].[GenerateFootprint]
GO

EXEC [load].[GeneratePacsSpireParallel]
GO

EXEC [load].[GenerateHtm]
GO
