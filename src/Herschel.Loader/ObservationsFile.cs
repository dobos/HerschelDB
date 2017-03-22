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
    abstract class ObservationsFile : InputFile
    {
        protected abstract bool Parse(string[] parts, out Observation observation);

        protected void Write(Observation o, TextWriter writer)
        {
            writer.Write("{0}|", (byte)o.Instrument);
            writer.Write("{0}|", o.ObsID);
            writer.Write("{0}|", (sbyte)o.Type);
            writer.Write("{0}|", (int)o.InstrumentMode);
            writer.Write("{0}|", (short)o.PointingMode);
            writer.Write("{0}|", o.Band);
            writer.Write("{0}|", o.Object);
            writer.Write("{0}|", o.Calibration ? 1 : 0);
            writer.Write("{0}|", o.Failed ? 1 : 0);
            writer.Write("{0}|", o.Sso ? 1 : 0);
            writer.Write("{0}|", (sbyte)o.Level);
            writer.Write("{0}|", o.RA);
            writer.Write("{0}|", o.Dec);
            writer.Write("{0}|", o.PA);
            writer.Write("{0}|", double.IsNaN(o.Aperture) ? "" : o.Aperture.ToString());
            writer.Write("{0}|", o.Repetition);
            writer.Write("{0}|", double.IsNaN(o.ScanMap.AV) ? "" : o.ScanMap.AV.ToString());
            writer.Write("{0}|", double.IsNaN(o.ScanMap.Height) ? "" : o.ScanMap.Height.ToString());
            writer.Write("{0}|", double.IsNaN(o.ScanMap.Width) ? "" : o.ScanMap.Width.ToString());
            writer.Write("{0}|", o.RasterMap.Num < 0 ? "" : o.RasterMap.Num.ToString());
            writer.Write("{0}|", double.IsNaN(o.RasterMap.Step) ? "" : o.RasterMap.Step.ToString());
            writer.Write("{0}|", o.RasterMap.Line < 0 ? "" : o.RasterMap.Line.ToString());
            writer.Write("{0}|", o.RasterMap.Column < 0 ? "" : o.RasterMap.Column.ToString());

            writer.Write("{0}|", o.Spectro.Num);
            writer.Write("{0}|", o.Spectro.LambdaFrom);
            writer.Write("{0}|", o.Spectro.LambdaTo);
            writer.Write("{0}|", o.Spectro.Lambda2From);
            writer.Write("{0}|", o.Spectro.Lambda2To);
            writer.Write("{0}|", o.Spectro.RangeID);

            writer.Write("{0}|", o.AOR);   // TODO
            writer.WriteLine("{0}", o.AOT);     // TODO
        }

        protected IEnumerable<Observation> ReadObservationsFile(string filename)
        {
            // Open file
            using (var reader = new StreamReader(filename))
            {
                string line;

                // Load column headers
                line = reader.ReadLine();
                ParseColumns(line);

                // Skip additional three lines
                for (int i = 0; i < 3; i++)
                {
                    reader.ReadLine();
                }

                while ((line = reader.ReadLine()) != null)
                {
                    var parts = SplitLine(line);

                    Observation o;
                    if (Parse(parts, out o))
                    {
                        yield return o;
                    }
                }
            }
        }

        /// <summary>
        /// Reads Herschel pointing file and writes bulk-insert ready file
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="outputFile"></param>
        public virtual void ConvertObservationsFile(string inputFile, string outputFile, bool append)
        {
            append &= File.Exists(outputFile);

            using (var outfile = new StreamWriter(outputFile, append))
            {
                foreach (var o in ReadObservationsFile(inputFile))
                {
                    Write(o, outfile);
                }
            }
        }


        protected ObservationLevel ParseObservationLevel(string value)
        {
            switch (value)
            {
                case "LEVEL3_PROCESSED":
                    return ObservationLevel.Level3;
                case "LEVEL2_PROCESSED":
                    return ObservationLevel.Level2;
                case "LEVEL2_5_PROCESSED":
                    return ObservationLevel.Level2_5;
                case "LEVEL1_PROCESSED":
                    return ObservationLevel.Level1;
                case "LEVEL0_PROCESSED":
                    return ObservationLevel.Level0;
                case "LEVEL0_5_PROCESSED":
                    return ObservationLevel.Level0_5;
                case "CREATED":
                    return ObservationLevel.Created;
                default:
                    throw new NotImplementedException();
            }
        }

        protected abstract InstrumentMode ParseInstrumentMode(string value);
        protected abstract PointingMode ParsePointingMode(string value);
    }
}
