//------------------------------------------------------------------------------
// <copyright file="CSSqlFunction.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.SqlServer.Server;
using Herschel.Lib;
using Jhu.Spherical;

public partial class UserDefinedFunctions
{
    struct Corner
    {
        public byte ID;
        public double Ra;
        public double Dec;
        public double X;
        public double Y;
        public double Z;

        public Corner(byte id, double ra, double dec)
        {
            ID = id;
            Ra = ra;
            Dec = dec;
            X = Double.NaN;
            Y = Double.NaN;
            Z = Double.NaN;
        }

        public Corner(byte id, double x, double y, double z)
        {
            ID = id;
            Ra = Double.NaN;
            Dec = Double.NaN;
            X = x;
            Y = y;
            Z = z;
        }
    }

    [SqlFunction(Name = "GetDetectorCornersEq", TableDefinition = "id tinyint, ra float, dec float",
        IsPrecise = false, IsDeterministic = true, FillRowMethodName = "FillEq")]
    public static IEnumerable GetDetectorCornersEq(SqlDouble ra, SqlDouble dec, SqlDouble pa, SqlString detector)
    {
        var d = Detector.Create(detector.Value);

        var cc = d.GetCorners(new Cartesian(ra.Value, dec.Value), pa.Value);
        for (int i = 0; i < cc.Length; i++)
        {
            yield return new Corner((byte)i, cc[i].RA, cc[i].Dec);
        }
    }

    public static void FillEq(object obj, out SqlByte id, out SqlDouble ra, out SqlDouble dec)
    {
        var c = (Corner)obj;
        id = new SqlByte(c.ID);
        ra = new SqlDouble(c.Ra);
        dec = new SqlDouble(c.Dec);
    }

    [SqlFunction(Name = "GetDetectorCornersXyz", TableDefinition = "id tinyint, x float, y float, z float",
        IsPrecise = false, IsDeterministic = true, FillRowMethodName = "FillXyz")]
    public static IEnumerable GetDetectorCornersXyz(SqlDouble ra, SqlDouble dec, SqlDouble pa, SqlString detector)
    {
        var d = Detector.Create(detector.Value);

        var cc = d.GetCorners(new Cartesian(ra.Value, dec.Value), pa.Value);
        for (int i = 0; i < cc.Length; i++)
        {
            Corner c;
            c.ID = (byte)i;
            c.Ra = Double.NaN;
            c.Dec = Double.NaN;
            c.X = cc[i].X;
            c.Y = cc[i].Y;
            c.Z = cc[i].Z;
            yield return c;
        }
    }

    public static void FillXyz(object obj, out SqlByte id, out SqlDouble x, out SqlDouble y, out SqlDouble z)
    {
        var c = (Corner)obj;
        id = new SqlByte(c.ID);
        x = new SqlDouble(c.X);
        y = new SqlDouble(c.Y);
        z = new SqlDouble(c.Z);
    }

    [SqlFunction(Name = "GetDetectorRegion", IsPrecise = false, IsDeterministic = true)]
    public static SqlBytes GetDetectorRegion(SqlDouble ra, SqlDouble dec, SqlDouble pa, SqlDouble aperture, SqlString detector)
    {
        var d = Detector.Create(detector.Value);
        Region r;

        try
        {
            r = d.GetFootprint(new Cartesian(ra.Value, dec.Value), pa.Value, aperture.Value);
        }
        catch (Exception ex)
        {
            r = new Region();
            r.SetErrorMessage(ex);
        }

        return r.ToSqlBytes();
    }

    [SqlFunction(Name = "GetLegCornersEq", TableDefinition = "id tinyint, ra float, dec float",
        IsPrecise = false, IsDeterministic = true, FillRowMethodName = "FillEq")]
    public static IEnumerable GetLegCornersEq(SqlDouble raStart, SqlDouble decStart, SqlDouble paStart, SqlDouble raEnd, SqlDouble decEnd, SqlDouble paEnd, SqlString detector)
    {
        var points = GetLegCornersInternal(raStart.Value, decStart.Value, paStart.Value, raEnd.Value, decEnd.Value, paEnd.Value, detector.Value);
        var corners = new List<Corner>();
        for (int i = 0; i < points.Count; i ++)
        {
            corners.Add(new Corner((byte)i, points[i].RA, points[i].Dec));
        }
        return corners;
    }

    [SqlFunction(Name = "GetLegRegion", IsPrecise = false, IsDeterministic = true)]
    public static SqlBytes GetLegRegion(SqlDouble raStart, SqlDouble decStart, SqlDouble paStart, SqlDouble raEnd, SqlDouble decEnd, SqlDouble paEnd, SqlString detector)
    {
        var points = GetLegCornersInternal(raStart.Value, decStart.Value, paStart.Value, raEnd.Value, decEnd.Value, paEnd.Value, detector.Value);

        var hb = new Jhu.Spherical.ShapeBuilder();
        var convex = hb.CreateConvexHull(points);
        var region = new Region(convex, false);
        region.Simplify();

        return region.ToSqlBytes();
    }

    private static List<Cartesian> GetLegCornersInternal(double raStart, double decStart, double paStart, double raEnd, double decEnd, double paEnd, string detector)
    {
        var d = Detector.Create(detector);
        var cstart = d.GetCorners(new Cartesian(raStart, decStart), paStart);
        var cend = d.GetCorners(new Cartesian(raEnd, decEnd), paEnd);

        var points = new List<Cartesian>();

        points.AddRange(cstart);
        points.AddRange(cend);

        return points;
    }

    [SqlFunction(Name = "GetMapRegion", IsPrecise = false, IsDeterministic = true)]
    public static SqlBytes GetMapRegion(SqlDouble ra, SqlDouble dec, SqlDouble pa, SqlDouble width, SqlDouble height)
    {
        var d = new DetectorHifiMap()
        {
            Width = width.Value,
            Height = height.Value
        };

        Region r;

        try
        {
            r = d.GetFootprint(new Cartesian(ra.Value, dec.Value), pa.Value, 0.0);
        }
        catch (Exception ex)
        {
            r = new Region();
            r.SetErrorMessage(ex);
        }

        return r.ToSqlBytes();
    }
}
