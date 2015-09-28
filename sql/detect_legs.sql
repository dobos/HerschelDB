EXEC [load].[DetectLegs]
GO

EXEC [load].[VerifyLegs]
GO

EXEC [load].[GenerateLegFootprints]

-- generate footprint with command-line tool

EXEC [load].[UpdateParallelFootprints]

EXEC [load].[VerifyFootprints]

EXEC [load].[GenerateHtm]