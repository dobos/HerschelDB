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
    abstract class PointingsFile
    {
        private byte observationType;

        public byte ObservationType
        {
            get { return observationType; }
            set { observationType = value; }
        }

        protected abstract bool Parse(string[] parts, out Pointing pointing);

        protected void Write(Pointing p, TextWriter writer)
        {
            writer.Write("{0} ", (byte)p.Instrument);
            writer.Write("{0} ", p.ObsID);
            writer.Write("{0} ", p.ObsType);
            writer.Write("{0} ", p.FineTime);
            writer.Write("{0} ", p.Ra);
            writer.Write("{0} ", p.Dec);
            writer.Write("{0} ", p.Pa);
            writer.WriteLine("{0} ", p.AV);
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

                    // TODO: observationid from infile


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
                    Pointing p;
                    if (Parse(line.Split(' '), out p))
                    {
                        yield return p;
                    }
                }
            }
        }

        /// <summary>
        /// Reads Herschel pointing file and writes bulk-insert ready file
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="outputFile"></param>
        protected virtual void ConvertPointingsFile(string inputFile, string outputFile, bool append)
        {
            append &= File.Exists(outputFile);

            using (var outfile = new StreamWriter(outputFile, append))
            {
                foreach (var p in ReadPointingsFile(inputFile))
                {
                    Write(p, outfile);
                }
            }
        }

    }
}
