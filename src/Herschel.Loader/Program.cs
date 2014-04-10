using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Configuration;
using Herschel.Lib;

namespace Herschel.Loader
{
    class Program
    {
        static long lastObsId = 0;
        static long lastFineTime = 0;

        static string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["Herschel"].ConnectionString; }
        }

        static void Main(string[] args)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            switch (args[0])
            {
                case "prepare":
                    PreparePointings(args[1], args[2], args[3] == "-a");
                    break;
                case "load":
                    LoadPointings(args[1]);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        static void PreparePointings(string path, string output, bool append)
        {
            var dir = Path.GetDirectoryName(path);
            var pattern = Path.GetFileName(path);

            var files = Directory.GetFiles(dir, pattern);
            Array.Sort(files);

            Console.WriteLine("Preparing pointing files for bulk load...", files.Length);
            Console.WriteLine("Found {0} files.", files.Length);

            int q = 0;
            foreach (var infile in files)
            {
                ConvertPointingsFile(infile, output, append);

                Console.WriteLine("{0}: {1}", ++q, infile);
            }
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
                int line = 0;
                foreach (var p in ReadPointingsFile(inputFile))
                {
                    line++;
                    if (p.ObsID < lastObsId || p.FineTime <= lastFineTime)
                    {
                        //
                        Console.WriteLine("Error in {0} at line {1}, skipping {2} {3}", inputFile, line, p.ObsID, p.FineTime);
                    }
                    else
                    {
                        p.Write(outfile);
                        lastObsId = p.ObsID;
                        lastFineTime = p.FineTime;
                    }
                }
            }
        }

        static void LoadPointings(string filename)
        {
            var sql = @"
";
        }
    }
}
