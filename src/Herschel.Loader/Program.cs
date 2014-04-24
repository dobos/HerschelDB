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

            switch (args[0])
            {
                case "prepare":
                    PreparePointings(args[1], args[2], int.Parse(args[3]));
                    break;
                case "load":
                    LoadPointings(args[1], int.Parse(args[2]));
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        static void PreparePointings(string path, string output, int fnum)
        {
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

                    ConvertPointingsFile(infile, String.Format(output, i), true);
                    Console.WriteLine("{0}: {1}", qq, infile);
                }
            });
        }

        static long GetID(string filename)
        {
            var fn = Path.GetFileNameWithoutExtension(filename).Substring(7);
            return long.Parse(fn, System.Globalization.CultureInfo.InvariantCulture);
        }

        static IEnumerable<Pointing> ReadPointingsFile(string filename)
        {
            // Open file
            using (var infile = new StreamReader(filename))
            {
                // Skip four lines
                for (int i = 0; i < 4; i++)
                {
                    infile.ReadLine();
                }

                string line;
                while ((line = infile.ReadLine()) != null)
                {
                    Pointing p = new Pointing();
                    p.Parse(line.Split(' '));
                    yield return p;
                }
            }
        }

        /// <summary>
        /// Reads Herschel pointing file and writes bulk-insert ready file
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="outputFile"></param>
        static void ConvertPointingsFile(string inputFile, string outputFile, bool append)
        {
            using (var outfile = new StreamWriter(outputFile, append))
            {
                foreach (var p in ReadPointingsFile(inputFile))
                {
                    p.Write(outfile);
                }
            }
        }

        static void LoadPointings(string filename, int fnum)
        {
            Parallel.For(0, fnum, i =>
            {
                var infile = String.Format(filename, i);

                Console.WriteLine("Loading from {0}...", infile);

                var sql = SqlScripts.LoadPointing;
                sql = sql.Replace("[$datafile]", infile);

                RunSqlScript(sql, 3600);
            });
        }

        static void RunSqlScript(string sql, int timeout)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                cn.Open();

                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.CommandTimeout = timeout;
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
