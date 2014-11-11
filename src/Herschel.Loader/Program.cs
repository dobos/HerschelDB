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
                case "cleanup":
                    CleanupPointings(args);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private static void PreparePointings(string[] args)
        {
            var inst = args[1].ToLowerInvariant();
            var type = byte.Parse(args[2]);
            var path = args[3];
            var output = args[4];
            int fnum = int.Parse(args[5]);

            var file = GetPointingsFile(inst, type);
            file.PreparePointings(path, output, fnum);
        }

        private static void LoadPointings(string[] args)
        {
            var path = args[1];
            int fnum = int.Parse(args[2]);

            LoadPointings(path, fnum);
        }

        private static void LoadPointings(string filename, int fnum)
        {
            Parallel.For(0, fnum, i =>
            {
                var infile = Path.GetFullPath(String.Format(filename, i));

                Console.WriteLine("Loading from {0}...", infile);

                var sql = SqlScripts.LoadPointing;
                sql = sql.Replace("[$datafile]", infile);

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

        private static PointingsFile GetPointingsFile(string inst, byte type)
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
