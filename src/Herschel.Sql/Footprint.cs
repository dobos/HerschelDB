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

        public Corner(byte id, double ra, double dec)
        {
            ID = id;
            Ra = ra;
            Dec = dec;
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
            Corner c;
            c.ID = (byte)i;
            c.Ra = cc[i].RA;
            c.Dec = cc[i].Dec;
            yield return c;
        }
    }

    public static void FillEq(object obj, out SqlByte id, out SqlDouble ra, out SqlDouble dec)
    {
        var c = (Corner)obj;
        id = new SqlByte(c.ID);
        ra = new SqlDouble(c.Ra);
        dec = new SqlDouble(c.Dec);
    }

    [SqlFunction(Name = "GetDetectorRegion", IsPrecise = false, IsDeterministic = true)]
    public static SqlBytes GetDetectorRegion(SqlDouble ra, SqlDouble dec, SqlDouble pa, SqlString detector)
    {
        var d = Detector.Create(detector.Value);

        var r = d.GetFootprint(new Cartesian(ra.Value, dec.Value), pa.Value);

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
}
