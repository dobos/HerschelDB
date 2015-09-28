EXEC load.MergeObservations
GO

EXEC load.MergeScanMaps
GO

EXEC load.MergeRasterMaps
GO

EXEC load.MergeSpectro
GO

--EXEC [load].[GenerateFootprint]
--GO

EXEC [load].[GeneratePacsSpireParallel]
GO

EXEC [load].[GenerateHtm]
GO
