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

        protected override bool Parse(string[] parts, out RawPointing pointing)
        {
            var ps = new RawPointingSpire();

            ps.Instrument = Instrument.Spire;

            // Parse columns

            switch (ObservationType)
            {
                case PointingObservationType.SpirePhotoSmallMap:
                case PointingObservationType.SpirePhotoLargeMap:
                case PointingObservationType.SpirePhoto:
                    ps.ObsID = ObservationID;
                    ps.Ra = double.Parse(parts[Columns["RA"]]);
                    ps.Dec = double.Parse(parts[Columns["DEC"]]);
                    ps.AV = String.IsNullOrWhiteSpace(parts[Columns["angVel"]]) ? 0.0 : double.Parse(parts[Columns["angVel"]]);
                    ps.Pa = String.IsNullOrWhiteSpace(parts[Columns["PA"]]) ? 0.0 : double.Parse(parts[Columns["PA"]]);
                    ps.SampleTime = Math.Floor(double.Parse(parts[Columns["sampleTime"]]) * 1e6);
                    ps.CorrTime = double.Parse(parts[Columns["corrTime"]]);
                    break;
                case PointingObservationType.SpireSpectro:
                case PointingObservationType.SpireSpectro1:
                case PointingObservationType.SpireSpectro7:
                case PointingObservationType.SpireSpectro64:
                case PointingObservationType.SpireSpectroRaster:
                    ps.ObsID = ObservationID;
                    ps.Ra = double.Parse(parts[Columns["RA"]]);
                    ps.Dec = double.Parse(parts[Columns["DEC"]]);
                    ps.AV = -1;
                    ps.Pa = String.IsNullOrWhiteSpace(parts[Columns["PA"]]) ? 0.0 : double.Parse(parts[Columns["PA"]]);
                    ps.SampleTime = long.Parse(parts[Columns["Time"]]);
                    break;
                default:
                    throw new NotImplementedException();
            }

            // Fix fine time scale problem with certain input files
            if (ps.SampleTime < 1000000000000000)
            {
                ps.SampleTime *= 1e6;
            }

            // Convert to common format

            pointing = new RawPointing()
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
