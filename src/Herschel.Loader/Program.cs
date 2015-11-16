using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Configuration;
using Jhu.Spherical;
using Herschel.Lib;

namespace Herschel.Loader
{
    class Program
    {
        static string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["Herschel"].ConnectionString; }
        }

        static void Main(string[] args)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            var verb = args[0].ToLowerInvariant();
            var obj = args[1].ToLowerInvariant();

            switch (obj)
            {
                case "pointing":
                    switch (verb)
                    {
                        case "prepare":
                            PreparePointings(args);
                            break;
                        case "load":
                            LoadPointings(args);
                            break;
                        case "merge":
                            MergePointings(args);
                            break;
                        case "cluster":
                            ClusterPointings(args);
                            break;
                        case "cleanup":
                            CleanupPointings(args);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    break;
                case "obs":
                    switch (verb)
                    {
                        case "prepare":
                            PrepareObservations(args);
                            break;
                        case "load":
                            LoadObservations(args);
                            break;
                        case "merge":
                            MergeObservations(args);
                            break;
                        /*case "cleanup":
                            CleanupObservations(args);
                            break;*/
                        default:
                            throw new NotImplementedException();
                    }
                    break;
                case "scanmap":
                    switch (verb)
                    {
                        case "generate":
                            GenerateScanMapFootprints(args);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    break;
                case "sso":
                    switch (verb)
                    {
                        case "prepare":
                            PrepareSsoCrossings(args);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private static ObservationsFile GetObservationsFile(string inst)
        {
            ObservationsFile file = null;

            switch (inst.ToLowerInvariant())
            {
                case "pacs":
                    file = new ObservationsFilePacs();
                    break;
                case "spire":
                    file = new ObservationsFileSpire();
                    break;
                case "hifi":
                    file = new ObservationsFileHifi();
                    break;
                default:
                    throw new NotImplementedException();
            }

            return file;
        }

        private static void PrepareObservations(string[] args)
        {
            var inst = args[2].ToLowerInvariant();
            var path = args[3];
            var output = args[4];

            Console.WriteLine("Preparing observation file for bulk load...");

            var file = GetObservationsFile(inst);
            file.ConvertObservationsFile(path, output, false);
        }

        private static void LoadObservations(string[] args)
        {
            ExecuteBulkInsert(args, SqlScripts.LoadObservation);
        }

        private static void MergeObservations(string[] args)
        {
            ExecuteScript(SqlScripts.MergeObservation);
        }

        private static void PreparePointings(string[] args)
        {
            var inst = args[2].ToLowerInvariant();
            var type = (PointingObservationType)byte.Parse(args[3]);
            var path = args[4];
            var output = args[5];
            int fnum = int.Parse(args[6]);

            // Run processing on multiple threads

            var dir = Path.GetDirectoryName(path);
            var pattern = Path.GetFileName(path);

            var files = Directory.GetFiles(dir, pattern);
            var queue = new Queue<string>(files);

            Console.WriteLine("Preparing pointing files for bulk load...", files.Length);
            Console.WriteLine("Found {0} files.", files.Length);

            int q = 0;

            Parallel.For(0, fnum, i =>
            {
                while (true)
                {
                    int qq;
                    string infile = null;

                    lock (queue)
                    {
                        if (queue.Count > 0)
                        {
                            infile = queue.Dequeue();
                            qq = q++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    var file = GetPointingsFile(inst, type);

                    try
                    {
                        file.ConvertPointingsFile(infile, String.Format(output, i), true);
                        Console.WriteLine("{0}: {1}", qq, infile);
                    }
                    catch (Exception ex)
                    {

                        Console.Error.WriteLine("Unhandled Exception processing '{0}'", infile);
                        Console.Error.WriteLine("Output file '{0}' may be corrupt.", String.Format(output, i));
                        Console.Error.WriteLine("Unhandled Exception: {0}: {1}", ex.GetType().FullName, ex.Message);
                        Console.Error.WriteLine(ex.StackTrace);
                        Console.WriteLine();
                    }
                }
            });
        }

        private static void LoadPointings(string[] args)
        {
            ExecuteBulkInsert(args, SqlScripts.LoadPointing);
        }

        private static void MergePointings(string[] args)
        {
            ExecuteScript(SqlScripts.MergePointing);
        }

        private static IEnumerable<Observation> LoadObservations(string sql)
        {
            var observations = new List<Observation>();

            using (var cmd = new SqlCommand(sql))
            {
                observations.AddRange(
                    DbHelper.ExecuteCommandReader<Observation>(cmd));
            }

            return observations;
        }

        private static void ClusterPointings(string[] args)
        {
            // PACS spectro chop-nod single pointings etc.

            var sql = @"
SELECT *
FROM Observation o
WHERE (inst = 1 AND obsType = 2 AND pointingMode IN (1, 2, 4))
   OR (inst = 1 AND obsType = 1 AND pointingMode = 0x0000000000000041)";



            if (args.Length > 2)
            {
                sql += "  AND o.inst = " + args[2];
            }

            if (args.Length > 3)
            {
                sql += "  AND o.obsID = " + args[3];
            }

            Parallel.ForEach(LoadObservations(sql), ClusterPointings);
        }

        private static void ClusterPointings(Observation obs)
        {
            Console.WriteLine("Processing pointings for {0}", obs.ObsID);

            // Find pointings
            var sql = @"
SELECT fineTime, ra, dec, pa
FROM Herschel_3.load.Pointing
WHERE inst = @inst AND obsID = @obsid AND isOnTarget = 1
ORDER BY fineTime";

            var pointings = new List<Pointing>();

            using (var cn = DbHelper.OpenConnection())
            {
                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.Add("@inst", SqlDbType.TinyInt).Value = (byte)obs.Instrument;
                    cmd.Parameters.Add("@obsID", SqlDbType.BigInt).Value = obs.ObsID;

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var p = new Pointing()
                            {
                                FineTime = dr.GetInt64(0),
                                Point = new Cartesian(dr.GetDouble(1), dr.GetDouble(2)),
                                PA = dr.GetDouble(3)
                            };

                            pointings.Add(p);
                        }
                    }
                }
            }

            try
            {
                var clusters = FindClusters(obs, pointings, 0.01);
                List<PointingGroup> groups;

                switch (obs.Type)
                {
                    case ObservationType.Photometry:
                        // PACS pointed chop measurements
                        groups = GroupClusters(clusters, 0.8);
                        SaveClusters_Photo(obs, groups);
                        break;
                    case ObservationType.Spectroscopy:
                        if ((obs.InstrumentMode & InstrumentMode.Chopping) != 0)
                        {
                            switch (obs.PointingMode)
                            {
                                case PointingMode.Pointed:
                                    groups = GroupClusters(clusters, 0.8);
                                    SaveClusters_ChopNod(obs, groups);
                                    break;
                                case PointingMode.Raster:
                                case PointingMode.Mapping:
                                    groups = GroupClusters(clusters, 0.8);
                                    FilterGroups_ChopNodRaster(obs, groups);
                                    SaveClusters_ChopNod(obs, groups);
                                    break;
                                default:
                                    throw new NotImplementedException();
                            }
                        }
                        else if ((obs.InstrumentMode & InstrumentMode.Chopping) == 0)
                        {
                            switch (obs.PointingMode)
                            {
                                case PointingMode.Pointed:
                                    groups = GroupClusters(clusters, 0.8);
                                    FilterGroups_NoChopSingle(obs, groups);
                                    SaveClusters_NoChop(obs, groups);
                                    break;
                                case PointingMode.Raster:
                                case PointingMode.Mapping:
                                    try
                                    {
                                        groups = GroupClusters(clusters, 0.8);
                                        FilterGroups_NoChopRaster(obs, groups);
                                    }
                                    catch (Exception)
                                    {
                                        // There's a few rasters where lines are very distant from each other
                                        // e.g. 1342267851,  1342267852
                                        groups = GroupClusters(clusters, 2.5);
                                        FilterGroups_NoChopRaster(obs, groups);
                                    }
                                    SaveClusters_NoChop(obs, groups);
                                    break;
                                default:
                                    throw new NotImplementedException();
                            }
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error determining pointing cluster for observation {0}", obs.ObsID);
                Console.Error.WriteLine(ex.Message);
            }
        }

        private static List<PointingCluster> FindClusters(Observation obs, List<Pointing> pointings, double dist)
        {
            var clusters = new List<PointingCluster>();

            foreach (var pointing in pointings)
            {
                bool found = false;
                PointingCluster cc = null;

                // Find next matching cluster
                for (int ci = 0; ci < clusters.Count; ci++)
                {
                    var c = clusters[ci];

                    for (int pi = 0; pi < c.Pointings.Count; pi++)
                    {
                        var p = c.Pointings[pi];
                        var d = pointing.Point.AngleInArcmin(p.Point);

                        // Shortcut when very far
                        if (d > 10 * dist)
                        {
                            break;
                        }
                        else if (d < dist)
                        {
                            if (cc != null)
                            {
                                // merge c into cc
                                cc.Pointings.AddRange(c.Pointings);
                                clusters.RemoveAt(ci);
                            }
                            else
                            {
                                // add to cluster
                                c.Pointings.Add(pointing);
                                cc = c;
                            }

                            found = true;
                            break;
                        }
                    }
                }

                if (!found)
                {
                    // new cluster
                    cc = new PointingCluster();
                    cc.Pointings.Add(pointing);
                    clusters.Add(cc);
                }
            }

            foreach (var cluster in clusters)
            {
                cluster.Observation = obs;
                cluster.CalculateAverage();
            }

            return clusters;
        }

        private static List<PointingGroup> GroupClusters(List<PointingCluster> clusters, double dist)
        {
            var groups = new List<PointingGroup>();

            foreach (var cluster in clusters)
            {
                bool found = false;
                PointingGroup gg = null;

                // Find next matching group
                for (int gi = 0; gi < groups.Count; gi++)
                {
                    var g = groups[gi];

                    for (int ci = 0; ci < g.Clusters.Count; ci++)
                    {
                        var c = g.Clusters[ci];
                        var d = cluster.Center.AngleInArcmin(c.Center);

                        if (d < dist)
                        {
                            if (gg != null)
                            {
                                // merge c into cc
                                gg.Clusters.AddRange(g.Clusters);
                                groups.RemoveAt(ci);
                            }
                            else
                            {
                                // add to group
                                g.Clusters.Add(cluster);
                                gg = g;
                            }

                            found = true;
                            break;
                        }
                    }
                }

                if (!found)
                {
                    // new cluster
                    gg = new PointingGroup();
                    gg.Clusters.Add(cluster);
                    groups.Add(gg);
                }
            }

            // Average out groups and determine mean distance
            for (int gi = 0; gi < groups.Count; gi++)
            {
                var group = groups[gi];

                double cxavg = 0;
                double cyavg = 0;
                double czavg = 0;
                double paavg = 0;
                int cnt = 0;

                foreach (var cluster in group.Clusters)
                {
                    cxavg += cluster.Center.X;
                    cyavg += cluster.Center.Y;
                    czavg += cluster.Center.Z;
                    paavg += cluster.PA;
                    cnt++;

                    cluster.GroupID = gi;
                }

                cxavg /= (double)cnt;
                cyavg /= (double)cnt;
                czavg /= (double)cnt;
                paavg /= (double)cnt;

                if (double.IsNaN(cxavg))
                {
                    Console.WriteLine("Nan in center coordinate");
                }

                group.Center = new Cartesian(cxavg, cyavg, czavg, true);
                group.PA = paavg;
            }

            return groups;
        }

        private static void FilterGroups_ChopNodRaster(Observation obs, List<PointingGroup> groups)
        {
            // Certain chop-nod observations have a false "reference point",
            // a group with just one cluster
            // e.g. 1342245400

            var gg = groups.ToArray();
            for (int i = 0; i < gg.Length; i++)
            {
                if (gg[i].Clusters.Count == 1)
                {
                    groups.Remove(gg[i]);
                }
            }

            if (groups.Count > 2)
            {
                throw new Exception("More than two pointing groups for chop-nod raster");
            }
        }

        private static void FilterGroups_NoChopSingle(Observation obs, List<PointingGroup> groups)
        {
            if (groups.Count > 1)
            {
                throw new Exception("More than one pointing group for non-chop single");
            }

            if (groups[0].Clusters.Count > 1)
            {
                // Find cluster with most number of points
                PointingCluster cc = null;

                for (int i = 0; i < groups[0].Clusters.Count; i++)
                {
                    if (cc == null || groups[0].Clusters[i].Pointings.Count > cc.Pointings.Count)
                    {
                        cc = groups[0].Clusters[i];
                    }
                }

                groups[0].Clusters.Clear();
                groups[0].Clusters.Add(cc);
            }
        }

        private static void FilterGroups_NoChopRaster(Observation obs, List<PointingGroup> groups)
        {
            if (groups.Count > 2)
            {
                throw new Exception("More than two pointing groups for non-chop");
            }
        }

        private static void SaveClusters_Photo(Observation obs, List<PointingGroup> groups)
        {
            // Rotate to the centerpoint 
            var axis = groups[0].Center.Cross(groups[1].Center, true);
            var ang = groups[0].Center.AngleInDegree(groups[1].Center);
            var rot = new Rotation(axis, ang / 2.0);

            for (var gi = 0; gi < groups.Count; gi++)
            {
                int c = 0;
                var group = groups[gi];

                foreach (var cluster in group.Clusters)
                {
                    cluster.ClusterID = c++;

                    // Save only real cluster (at least 1 sec integration)
                    if (cluster.Pointings.Count > 10)
                    {
                        cluster.Save();

                        if (cluster.GroupID == 0)
                        {
                            cluster.Center.Rotate(rot);
                        }
                        else
                        {
                            cluster.Center.RotateBack(rot);
                        }

                        cluster.IsRotated = true;
                        cluster.Save();
                    }
                }
            }
        }

        private static void SaveClusters_ChopNod(Observation obs, List<PointingGroup> groups)
        {
            // Rotate to the centerpoint 
            var axis = groups[0].Center.Cross(groups[1].Center, true);
            var ang = groups[0].Center.AngleInDegree(groups[1].Center);
            var rot = new Rotation(axis, ang / 2.0);

            for (var gi = 0; gi < groups.Count; gi++)
            {
                int c = 0;
                var group = groups[gi];

                foreach (var cluster in group.Clusters)
                {
                    cluster.ClusterID = c++;

                    // Save only real cluster (at least 1 sec integration)
                    if (cluster.Pointings.Count > 10)
                    {
                        cluster.Save();

                        if (cluster.GroupID == 0)
                        {
                            cluster.Center.Rotate(rot);
                        }
                        else
                        {
                            cluster.Center.RotateBack(rot);
                        }

                        cluster.IsRotated = true;
                        cluster.Save();
                    }
                }
            }
        }

        private static void SaveClusters_NoChop(Observation obs, List<PointingGroup> groups)
        {
            // Find group with least fineTime, that's the calibration point
            int ming = -1;

            if (groups.Count == 1)
            {
                // no calibration point
            }
            else
            {

                long minft = long.MaxValue;

                for (int i = 0; i < groups.Count; i++)
                {
                    if (ming == -1)
                    {
                        ming = i;
                    }

                    int minc = -1;

                    for (int j = 0; j < groups[i].Clusters.Count; j++)
                    {
                        if (minc == -1)
                        {
                            minc = j;
                        }

                        if (groups[i].Clusters[j].FineTimeStart < minft)
                        {
                            ming = i;
                            minc = j;
                            minft = groups[i].Clusters[j].FineTimeStart;
                        }
                    }
                }
            }

            for (int i = 0; i < groups.Count; i++)
            {
                var group = groups[i];
                int c = 0;

                if (i != ming)
                {
                    foreach (var cluster in group.Clusters)
                    {
                        cluster.ClusterID = c++;

                        // Save only real cluster (at least 1 sec integration)
                        if (cluster.Pointings.Count > 10)
                        {
                            cluster.Save();
                        }
                    }
                }
            }
        }

        private static void GenerateScanMapFootprints(string[] args)
        {
            // Generate scan map footprints

            // Find observations
            var sql = @"
SELECT *
FROM Observation o
LEFT OUTER JOIN ScanMap s ON s.inst = o.inst AND s.obsID = o.obsID
LEFT OUTER JOIN RasterMap r ON r.inst = o.inst AND r.obsID = o.obsID
LEFT OUTER JOIN Spectro p ON p.inst = o.inst AND p.obsID = o.obsID
WHERE
    (o.inst = 1 AND o.pointingMode IN (8, 16) OR
    o.inst = 2 AND o.pointingMode IN (8, 16, 32))
    -- AND o.calibration = 0      -- not a calibration
    -- AND o.failed = 0           -- only processed
    -- AND o.region IS NULL
";

            if (args.Length > 2)
            {
                sql += "  AND o.inst = " + args[2];
            }

            if (args.Length > 3)
            {
                sql += "  AND o.obsID = " + args[3];
            }

            var observations = new List<Observation>();

            using (var cmd = new SqlCommand(sql))
            {
                observations.AddRange(
                    DbHelper.ExecuteCommandReader<Observation>(cmd));
            }

            Parallel.ForEach(observations, GenerateScanMapFootprint);
        }

        private static void GenerateScanMapFootprint(Observation obs)
        {
            // Find legs belonging to observation

            var sql = @"
SELECT *
FROM load.LegRegion
WHERE inst = @inst AND obsID = @obsID
ORDER BY legID
";

            var legs = new List<Region>();

            using (var cn = DbHelper.OpenConnection())
            {
                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.Add("@inst", SqlDbType.TinyInt).Value = (byte)obs.Instrument;
                    cmd.Parameters.Add("@obsID", SqlDbType.BigInt).Value = obs.ObsID;

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var bytes = dr.GetSqlBytes(5);
                            var r = Region.FromSqlBytes(bytes);

                            legs.Add(r);
                        }
                    }
                }
            }

            if (legs.Count == 0)
            {
                SaveScanMapFootprint(obs, null);

                Console.WriteLine("Pointing missing for {0}", obs.ObsID);
            }
            else
            {
                // Calculate union of legs of a single observation
                Region union = null;
                int rep = Math.Max(obs.Repetition, 1);
                int scans = legs.Count / rep;
                int start = 0;

                if (obs.Instrument == Instrument.Spire &&
                    rep > 1)
                {
                    scans++;
                }

                // Retry for another re-scan if building footprint from
                // the first one fails
                while (start < rep)
                {
                    union = legs[start * scans];

                    for (int i = 1; i < scans; i++)
                    {
                        try
                        {
                            union.SmartUnion(legs[start * scans + i], 256);
                        }
                        catch (Exception ex)
                        {
                            union = new Region();
                            union.SetErrorMessage(ex);

                            Console.WriteLine("Error generating footprint for {0}", obs.ObsID);

                            break;
                        }
                    }

                    // If unioning succeeded
                    if (!union.HasError)
                    {
                        break;
                    }

                    start++;

                    Console.WriteLine("Retrying for the {0} time", start);
                }

                SaveScanMapFootprint(obs, union);

                //Console.WriteLine("Generated footprint for {0}", obs.ObsID);
            }
        }

        private static void SaveScanMapFootprint(Observation obs, Region r)
        {
            var sql = @"
UPDATE Observation
SET region = @region
WHERE inst = @inst AND obsID = @obsID";

            using (var cn = DbHelper.OpenConnection())
            {
                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.Add("@inst", SqlDbType.TinyInt).Value = (byte)obs.Instrument;
                    cmd.Parameters.Add("@obsID", SqlDbType.BigInt).Value = obs.ObsID;
                    cmd.Parameters.Add("@region", SqlDbType.VarBinary).Value = r == null ? (object)DBNull.Value : r.ToSqlBytes();

                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static void CleanupPointings(string[] args)
        {
            ExecuteScript(SqlScripts.CleanupPointing);
        }

        private static void ExecuteScript(string script)
        {
            // Split query
            var sql = DbHelper.SplitQuery(script);

            for (int i = 0; i < sql.Length; i++)
            {
                Console.WriteLine("Executing query:");
                Console.WriteLine(sql[i]);

                using (var cmd = new SqlCommand(sql[i]))
                {
                    DbHelper.ExecuteCommandNonQuery(cmd);
                }
            }
        }

        private static PointingsFile GetPointingsFile(string inst, PointingObservationType type)
        {
            PointingsFile file = null;

            switch (inst.ToLowerInvariant())
            {
                case "pacs":
                    file = new PointingsFilePacs();
                    break;
                case "spire":
                    file = new PointingsFileSpire();
                    break;
                case "hifi":
                    file = new PointingsFileHifi();
                    break;
                default:
                    throw new NotImplementedException();
            }

            file.ObservationType = type;

            return file;
        }

        static long GetID(string filename)
        {
            var fn = Path.GetFileNameWithoutExtension(filename).Substring(7);
            return long.Parse(fn, System.Globalization.CultureInfo.InvariantCulture);
        }

        private static void ExecuteBulkInsert(string[] args, string sql)
        {
            var path = args[2];
            int fnum = int.Parse(args[3]);

            var dir = Path.GetDirectoryName(path);
            var pattern = Path.GetFileName(path);

            var files = Directory.GetFiles(dir, pattern);
            var queue = new Queue<string>(files);

            Console.WriteLine("Bulk loading files...");
            Console.WriteLine("Found {0} files.", files.Length);

            var options = new ParallelOptions()
            {
                MaxDegreeOfParallelism = fnum
            };

            Parallel.ForEach(files, options, infile =>
            {
                Console.WriteLine("Loading from {0}...", infile);

                using (var cmd = new SqlCommand(sql.Replace("[$datafile]", Path.GetFullPath(infile))))
                {
                    cmd.CommandTimeout = 3600;  // 1h should be enough for bulk inserts

                    try
                    {
                        DbHelper.ExecuteCommandNonQuery(cmd);
                    }
                    catch (Exception ex)
                    {

                        Console.Error.WriteLine("Unhandled Exception processing '{0}'", infile);
                        Console.Error.WriteLine("Unhandled Exception: {0}: {1}", ex.GetType().FullName, ex.Message);
                        Console.Error.WriteLine(ex.StackTrace);
                        Console.WriteLine();
                    }
                }
            });
        }

        private static void PrepareSsoCrossings(string[] args)
        {
            var inst = args[2].ToLowerInvariant();
            var path = args[3];
            var output = args[4];

            var sep = new char[] { ' ' };

            Console.WriteLine("Preparing sso crossings file for bulk load...");

            using (var infile = new StreamReader(path))
            {
                using (var outfile = new StreamWriter(output))
                {
                    string line;

                    while ((line = infile.ReadLine()) != null)
                    {
                        if (line[0] == '#')
                        {
                            continue;
                        }

                        // "%d %12s %6.4f %9.1f %5.2f %5.2f %5.2f %5.2f %9.5f %9.5f %8.5f %8.5f %8.5f %4.1f %7.1f %3.1f %3.1f %3.1f\n"

                        var parts = line.Split(sep, StringSplitOptions.RemoveEmptyEntries);

                        int o = 0;

                        long obsId = long.Parse(parts[o++]);
                        var name = parts[o++].Replace("_", " ");
                        var coverage = float.Parse(parts[o++]);
                        var coverage2 = float.Parse(parts[o++]);
                        var mag = float.Parse(parts[o++]);
                        var hh = float.Parse(parts[o++]);
                        var r0 = float.Parse(parts[o++]);
                        var delta = float.Parse(parts[o++]);
                        var ra = double.Parse(parts[o++]);
                        var dec = double.Parse(parts[o++]);
                        var pm_ra = float.Parse(parts[o++]);
                        var pm_dec = float.Parse(parts[o++]);
                        var pm = float.Parse(parts[o++]);
                        var alpha = float.Parse(parts[o++]);
                        var flux = float.Parse(parts[o++]);
                        var g_slope = float.Parse(parts[o++]);
                        var eta = float.Parse(parts[o++]);
                        var pv = float.Parse(parts[o++]);

                        /*
                            obsid,o->name,
                                coverage,coverage*jspan*2.0*86400.0+0.1,
                                mag,o->hh,r0,delta,
                                ra,dec,vra*3600.0/1440.0,vdec*3600.0/1440.0,vra*3600.0/1440.0*cos(dec*M_PI/180.0),
                                alpha,flux,g_slope,eta,pv
                        */

                        outfile.Write("{0}|", obsId);
                        outfile.Write("{0}|", name);
                        outfile.Write("{0}|", coverage);
                        outfile.Write("{0}|", mag);
                        outfile.Write("{0}|", hh);
                        outfile.Write("{0}|", r0);
                        outfile.Write("{0}|", delta);
                        outfile.Write("{0}|", ra);
                        outfile.Write("{0}|", dec);
                        outfile.Write("{0}|", pm_ra);
                        outfile.Write("{0}|", pm_dec);
                        outfile.Write("{0}|", pm);
                        outfile.Write("{0}|", alpha);
                        outfile.Write("{0}|", flux);
                        outfile.Write("{0}|", g_slope);
                        outfile.Write("{0}|", eta);
                        outfile.Write("{0}", pv);
                        outfile.WriteLine();
                    }
                }
            }
        }
    }
}
