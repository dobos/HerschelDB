EXEC load.MergeObservations
GO

EXEC load.MergeObservationsParallel
GO

EXEC load.UpdateObservationsPointings
GO



EXEC load.MergeScanMaps
GO

EXEC load.MergeRasterMaps
GO

EXEC load.MergeSpectro
GO

EXEC [load].[MergeSso]
GO





--EXEC [load].[GenerateFootprint]
--GO

EXEC [load].[GeneratePacsSpireParallel]
GO

EXEC [load].[GenerateHtm]
GO


EXEC [load].[UpdateObservationParams]
GO