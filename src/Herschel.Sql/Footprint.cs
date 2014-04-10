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
using Spherical;

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

    [SqlFunction(Name = "fGetDetectorCornersEq", TableDefinition = "id tinyint, ra float, dec float",
        IsPrecise=false, IsDeterministic=true, FillRowMethodName="FillGetDetectorCornersEq")]
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

    public static void FillGetDetectorCornersEq(object obj, out SqlByte id, out SqlDouble ra, out SqlDouble dec)
    {
        var c = (Corner)obj;
        id = new SqlByte(c.ID);
        ra = new SqlDouble(c.Ra);
        dec = new SqlDouble(c.Dec);
    }

    [SqlFunction(Name = "fGetDetectorRegion", IsPrecise = false, IsDeterministic = true)]
    public static SqlBytes GetDetectorRegion(SqlDouble ra, SqlDouble dec, SqlDouble pa, SqlString detector)
    {
        var d = Detector.Create(detector.Value);

        var cc = d.GetCorners(new Cartesian(ra.Value, dec.Value), pa.Value);

        var r = new Region();
        r.Add(new Convex(new List<Cartesian>(cc), PointOrder.CW));
        r.Simplify();

        return Util.SetRegion(r);
    }

    [SqlFunction(Name = "fGetLegRegion", IsPrecise = false, IsDeterministic = true)]
    public static SqlBytes GetLegRegion(SqlDouble raStart, SqlDouble decStart, SqlDouble paStart, SqlDouble raEnd, SqlDouble decEnd, SqlDouble paEnd, SqlString detector)
    {
        var d = Detector.Create(detector.Value);
        var cstart = d.GetCorners(new Cartesian(raStart.Value, decStart.Value), paStart.Value);
        var cend = d.GetCorners(new Cartesian(raEnd.Value, decEnd.Value), paEnd.Value);

        Spherical.Shape.Chull.Cherror error;
        var points = new List<Cartesian>();

        points.AddRange(cstart);
        points.AddRange(cend);

        var convex = Spherical.Shape.Chull.Make(points, out error);
        var region = new Spherical.Region(convex, false);
        region.Simplify();

        return Util.SetRegion(region);
    }
}
