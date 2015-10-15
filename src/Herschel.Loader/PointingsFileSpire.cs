using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Herschel.Lib;

namespace Herschel.Loader
{
    class PointingsFileSpire : PointingsFile
    {
        public long ObservationID;

        protected override bool Parse(string[] parts, out Pointing pointing)
        {
            var ps = new PointingSpire();

            ps.Instrument = Instrument.Spire;

            // Parse columns

            switch (ObservationType)
            {
                case PointingObservationType.SpirePhoto:
                case PointingObservationType.SpirePhotoSmallMap:
                case PointingObservationType.SpirePhotoLargeMap:
                    if (parts[5] != "FAILED")
                    {
                        ps.ObsID = ObservationID;
                        ps.Ra = double.Parse(parts[0]);
                        ps.Dec = double.Parse(parts[1]);
                        ps.AV = String.IsNullOrWhiteSpace(parts[2]) ? 0.0 : double.Parse(parts[2]);
                        ps.Pa = String.IsNullOrWhiteSpace(parts[3]) ? 0.0 : double.Parse(parts[3]);
                        ps.SampleTime = Math.Floor(double.Parse(parts[4]) * 1e6);
                        ps.CorrTime = double.Parse(parts[5]);
                    }
                    else
                    {
                        // TODO: delete this

                        ps.ObsID = ObservationID;
                        ps.Ra = double.Parse(parts[0]);
                        ps.Dec = double.Parse(parts[1]);
                        ps.AV = -1;
                        ps.Pa = String.IsNullOrWhiteSpace(parts[2]) ? 0.0 : double.Parse(parts[2]);
                        ps.SampleTime = Math.Floor(double.Parse(parts[3]) * 1e6);
                        ps.CorrTime = -1;
                    }
                    break;
                case PointingObservationType.SpireSpectro1:
                case PointingObservationType.SpireSpectro7:
                    ps.ObsID = ObservationID;
                    ps.Ra = double.Parse(parts[0]);
                    ps.Dec = double.Parse(parts[1]);
                    ps.Pa = double.Parse(parts[2]);
                    ps.SampleTime = long.Parse(parts[3]);
                    break;
                case PointingObservationType.SpireSpectro64:
                    if (parts.Length == 5)
                    {
                        ps.ObsID = ObservationID;
                        ps.Ra = double.Parse(parts[0]);
                        ps.Dec = double.Parse(parts[1]);
                        ps.AV = String.IsNullOrWhiteSpace(parts[2]) ? 0.0 : double.Parse(parts[2]);
                        ps.Pa = String.IsNullOrWhiteSpace(parts[3]) ? 0.0 : double.Parse(parts[3]);
                        ps.SampleTime = (long)double.Parse(parts[4]);
                    }
                    else if (parts.Length == 4)
                    {
                        ps.ObsID = ObservationID;
                        ps.Ra = double.Parse(parts[0]);
                        ps.Dec = double.Parse(parts[1]);
                        ps.Pa = String.IsNullOrWhiteSpace(parts[2]) ? 0.0 : double.Parse(parts[2]);
                        ps.SampleTime = (long)double.Parse(parts[3]);
                    }
                    break;
                case PointingObservationType.SpireSpectroRaster:
                    ps.ObsID = ObservationID;
                    ps.Ra = double.Parse(parts[0]);
                    ps.Dec = double.Parse(parts[1]);
                    ps.Pa = String.IsNullOrWhiteSpace(parts[2]) ? 0.0 : double.Parse(parts[2]);
                    ps.AV = -1;
                    ps.SampleTime = Math.Floor(double.Parse(parts[4]) * 1e6);
                    ps.CorrTime = double.Parse(parts[5]);
                    break;
                default:
                    throw new NotImplementedException();
            }

            // Convert to common format

            pointing = new Pointing()
            {
                Instrument = ps.Instrument,
                ObsID = ps.ObsID,
                ObsType = ObservationType,
                FineTime = (long)ps.SampleTime,
                Ra = ps.Ra,
                Dec = ps.Dec,
                Pa = ps.Pa,
                AV = ps.AV * 3600,  // convert from deg s-1 to arcsec s-1
            };

            return true;
        }

        public override void ConvertPointingsFile(string inputFile, string outputFile, bool append)
        {
            var file = Path.GetFileName(inputFile);
            ObservationID = long.Parse(file.Substring(8, 10));

            base.ConvertPointingsFile(inputFile, outputFile, append);
        }
    }
}
