
IF (OBJECT_ID('htm.FromXyz') IS NOT NULL)
BEGIN
    DROP FUNCTION [htm].[FromXyz]
END

GO


IF (OBJECT_ID('htm.FromEq') IS NOT NULL)
BEGIN
    DROP FUNCTION [htm].[FromEq]
END

GO


IF (OBJECT_ID('htm.GetCenter') IS NOT NULL)
BEGIN
    DROP FUNCTION [htm].[GetCenter]
END

GO


IF (OBJECT_ID('htm.GetCorners') IS NOT NULL)
BEGIN
    DROP FUNCTION [htm].[GetCorners]
END

GO


IF (OBJECT_ID('htm.Parse') IS NOT NULL)
BEGIN
    DROP FUNCTION [htm].[Parse]
END

GO


IF (OBJECT_ID('htm.ToString') IS NOT NULL)
BEGIN
    DROP FUNCTION [htm].[ToString]
END

GO


IF (OBJECT_ID('htm.Cover') IS NOT NULL)
BEGIN
    DROP FUNCTION [htm].[Cover]
END

GO


IF (OBJECT_ID('htm.CoverAdvanced') IS NOT NULL)
BEGIN
    DROP FUNCTION [htm].[CoverAdvanced]
END

GO


IF (OBJECT_ID('htm.CoverRegion') IS NOT NULL)
BEGIN
    DROP FUNCTION [htm].[CoverRegion]
END

GO


IF (OBJECT_ID('htm.CoverCircleXyz') IS NOT NULL)
BEGIN
    DROP FUNCTION [htm].[CoverCircleXyz]
END

GO


IF (OBJECT_ID('htm.CoverCircleEq') IS NOT NULL)
BEGIN
    DROP FUNCTION [htm].[CoverCircleEq]
END

GO


IF (OBJECT_ID('point.ConvertEqToXyz') IS NOT NULL)
BEGIN
    DROP FUNCTION [point].[ConvertEqToXyz]
END

GO


IF (OBJECT_ID('point.ConvertXyzToEq') IS NOT NULL)
BEGIN
    DROP FUNCTION [point].[ConvertXyzToEq]
END

GO


IF (OBJECT_ID('point.GetAngleXyz') IS NOT NULL)
BEGIN
    DROP FUNCTION [point].[GetAngleXyz]
END

GO


IF (OBJECT_ID('point.GetAngleEq') IS NOT NULL)
BEGIN
    DROP FUNCTION [point].[GetAngleEq]
END

GO


IF (OBJECT_ID('region.GetHalfspaces') IS NOT NULL)
BEGIN
    DROP FUNCTION [region].[GetHalfspaces]
END

GO


IF (OBJECT_ID('region.ToString') IS NOT NULL)
BEGIN
    DROP FUNCTION [region].[ToString]
END

GO


IF (OBJECT_ID('region.Parse') IS NOT NULL)
BEGIN
    DROP FUNCTION [region].[Parse]
END

GO


IF (OBJECT_ID('region.ParseAdvanced') IS NOT NULL)
BEGIN
    DROP FUNCTION [region].[ParseAdvanced]
END

GO


IF (OBJECT_ID('region.GetArea') IS NOT NULL)
BEGIN
    DROP FUNCTION [region].[GetArea]
END

GO


IF (OBJECT_ID('region.GetPatches') IS NOT NULL)
BEGIN
    DROP FUNCTION [region].[GetPatches]
END

GO


IF (OBJECT_ID('region.Simplify') IS NOT NULL)
BEGIN
    DROP FUNCTION [region].[Simplify]
END

GO


IF (OBJECT_ID('region.SimplifyAdvanced') IS NOT NULL)
BEGIN
    DROP FUNCTION [region].[SimplifyAdvanced]
END

GO


IF (OBJECT_ID('region.GetArcs') IS NOT NULL)
BEGIN
    DROP FUNCTION [region].[GetArcs]
END

GO


IF (OBJECT_ID('region.GetOutlineArcs') IS NOT NULL)
BEGIN
    DROP FUNCTION [region].[GetOutlineArcs]
END

GO


IF (OBJECT_ID('region.GetConvexes') IS NOT NULL)
BEGIN
    DROP FUNCTION [region].[GetConvexes]
END

GO


IF (OBJECT_ID('region.Grow') IS NOT NULL)
BEGIN
    DROP FUNCTION [region].[Grow]
END

GO


IF (OBJECT_ID('region.GrowAdvanced') IS NOT NULL)
BEGIN
    DROP FUNCTION [region].[GrowAdvanced]
END

GO


IF (OBJECT_ID('region.AddHalfspace') IS NOT NULL)
BEGIN
    DROP FUNCTION [region].[AddHalfspace]
END

GO


IF (OBJECT_ID('region.ContainsXyz') IS NOT NULL)
BEGIN
    DROP FUNCTION [region].[ContainsXyz]
END

GO


IF (OBJECT_ID('region.ContainsEq') IS NOT NULL)
BEGIN
    DROP FUNCTION [region].[ContainsEq]
END

GO


IF (OBJECT_ID('region.Difference') IS NOT NULL)
BEGIN
    DROP FUNCTION [region].[Difference]
END

GO


IF (OBJECT_ID('region.DifferenceAdvanced') IS NOT NULL)
BEGIN
    DROP FUNCTION [region].[DifferenceAdvanced]
END

GO


IF (OBJECT_ID('region.Intersect') IS NOT NULL)
BEGIN
    DROP FUNCTION [region].[Intersect]
END

GO


IF (OBJECT_ID('region.IntersectAdvanced') IS NOT NULL)
BEGIN
    DROP FUNCTION [region].[IntersectAdvanced]
END

GO


IF (OBJECT_ID('region.Union') IS NOT NULL)
BEGIN
    DROP FUNCTION [region].[Union]
END

GO


IF (OBJECT_ID('region.UnionAdvanced') IS NOT NULL)
BEGIN
    DROP FUNCTION [region].[UnionAdvanced]
END

GO


IF (OBJECT_ID('region.GetErrorMessage') IS NOT NULL)
BEGIN
    DROP FUNCTION [region].[GetErrorMessage]
END

GO


IF (OBJECT_ID('region.HasError') IS NOT NULL)
BEGIN
    DROP FUNCTION [region].[HasError]
END

GO


IF (OBJECT_ID('region.GetConvexHull') IS NOT NULL)
BEGIN
    DROP FUNCTION [region].[GetConvexHull]
END

GO


IF (OBJECT_ID('region.Reduce') IS NOT NULL)
BEGIN
    DROP FUNCTION [region].[Reduce]
END

GO


IF (OBJECT_ID('region.UnionEvery') IS NOT NULL)
BEGIN
    DROP AGGREGATE [region].[UnionEvery]
END

GO


IF (OBJECT_ID('region.IntersectEvery') IS NOT NULL)
BEGIN
    DROP AGGREGATE [region].[IntersectEvery]
END

GO


IF (OBJECT_ID('region.ConvexHullXyz') IS NOT NULL)
BEGIN
    DROP AGGREGATE [region].[ConvexHullXyz]
END

GO


IF (OBJECT_ID('region.ConvexHullEq') IS NOT NULL)
BEGIN
    DROP AGGREGATE [region].[ConvexHullEq]
END

GO


DROP ASSEMBLY [Jhu.Spherical.Sql]

GO


DROP ASSEMBLY [Jhu.Spherical.Htm]

GO


DROP ASSEMBLY [Jhu.Spherical]

GO


DROP SCHEMA [htm]

GO


DROP SCHEMA [point]

GO


DROP SCHEMA [region]

GO

