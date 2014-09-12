using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using Herschel.Lib;

namespace Herschel.Loader
{
    abstract class PointingsFile : DbObjectBase
    {
        protected abstract Pointing Parse(string[] parts);

        protected void Write(Pointing p, TextWriter writer)
        {
            writer.Write("{0} ", p.ObsID);
            writer.Write("{0} ", p.FineTime);
            writer.Write("{0} ", (byte)p.Instrument);
            writer.Write("{0} ", p.BuldingBlockType);
            writer.Write("{0} ", p.Ra);
            writer.Write("{0} ", p.RaError);
            writer.Write("{0} ", p.Dec);
            writer.Write("{0} ", p.DecError);
            writer.Write("{0} ", p.Pa);
            writer.Write("{0} ", p.PaError);
            writer.Write("{0} ", p.AVX);
            writer.Write("{0} ", p.AVXError);
            writer.Write("{0} ", p.AVY);
            writer.Write("{0} ", p.AVYError);
            writer.Write("{0} ", p.AVZ);
            writer.Write("{0} ", p.AVZError);
            writer.WriteLine("{0}", p.Utc);
        }

        public void PreparePointings(string path, string output, int fnum)
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

        protected IEnumerable<Pointing> ReadPointingsFile(string filename)
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
                    yield return Parse(line.Split(' '));
                }
            }
        }

        /// <summary>
        /// Reads Herschel pointing file and writes bulk-insert ready file
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="outputFile"></param>
        void ConvertPointingsFile(string inputFile, string outputFile, bool append)
        {
            using (var outfile = new StreamWriter(outputFile, append))
            {
                foreach (var p in ReadPointingsFile(inputFile))
                {
                    Write(p, outfile);
                }
            }
        }

        public void LoadPointings(string filename, int fnum)
        {
            Parallel.For(0, fnum, i =>
            {
                var infile = String.Format(filename, i);

                Console.WriteLine("Loading from {0}...", infile);

                var sql = SqlScripts.LoadPointing;
                sql = sql.Replace("[$datafile]", infile);

                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandTimeout = 3600;  // 1h should be enough for bulk inserts
                    ExecuteCommandNonQuery(cmd);
                }
            });
        }
    }
}
