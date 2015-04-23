using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Configuration;
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
                case "obs":
                    switch (verb)
                    {
                        case "prepare":
                            PrepareObservations(args);
                            break;
                        /*case "load":
                            LoadObservations(args);
                            break;
                        case "merge":
                            MergeObservations(args);
                            break;
                        case "cleanup":
                            CleanupObservations(args);
                            break;*/
                        default:
                            throw new NotImplementedException();
                    }
                    break;
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
                        case "scanmap": // TODO: merge with merge ;-)
                            GenerateScanMapFootprints(args);
                            break;
                        case "cleanup":
                            CleanupPointings(args);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
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

        private static void PreparePointings(string[] args)
        {
            var inst = args[2].ToLowerInvariant();
            var type = (ObservationType)byte.Parse(args[3]);
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
                    file.ConvertPointingsFile(infile, String.Format(output, i), true);
                    Console.WriteLine("{0}: {1}", qq, infile);
                }
            });
        }

        private static void LoadPointings(string[] args)
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

                var sql = SqlScripts.LoadPointing;
                sql = sql.Replace("[$datafile]", Path.GetFullPath(infile));

                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandTimeout = 3600;  // 1h should be enough for bulk inserts
                    DbHelper.ExecuteCommandNonQuery(cmd);
                }
            });
        }

        private static void MergePointings(string[] args)
        {
            ExecuteScript(SqlScripts.MergePointing);
        }

        private static void GenerateScanMapFootprints(string[] args)
        {
            // Generate scan map footprints

            // Find observations
            var sql = @"
SELECT *
FROM Observation
WHERE inst IN (1, 2)            -- PACS or SPIRE
  AND pointingMode IN (8, 16)   -- Scan map
  AND calibration = 0           -- not a calibration
  AND obsLevel < 250            -- only processed";

            var observations = new List<Observation>();

            using (var cmd = new SqlCommand(sql))
            {
                observations.AddRange(
                    DbHelper.ExecuteCommandReader<Observation>(cmd));
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

        private static PointingsFile GetPointingsFile(string inst, ObservationType type)
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

    }
}
