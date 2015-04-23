using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Herschel.Lib;

namespace Herschel.Loader
{
    class PointingsFileHifi : PointingsFile
    {
        public long ObservationID;

        protected override bool Parse(string[] parts, out Pointing pointing)
        {
            var ps = new PointingHifi();

            ps.Instrument = Instrument.Hifi;

            // Parse columns

            switch (ObservationType)
            {
                case ObservationType.HifiPoint:
                    ps.ObsID = ObservationID;
                    ps.Ra = double.Parse(parts[0]);
                    ps.Dec = double.Parse(parts[1]);
                    ps.Pa = double.Parse(parts[2]);
                    ps.FineTime = long.Parse(parts[3]);
                    break;
                case ObservationType.HifiSpectralScan:
                    ps.ObsID = ObservationID;
                    ps.Ra = double.Parse(parts[0]);
                    ps.Dec = double.Parse(parts[1]);
                    ps.Pa = double.Parse(parts[2]);
                    ps.FineTime = long.Parse(parts[4]);
                    break;
                case ObservationType.HifiMapping:
                    ps.ObsID = ObservationID;
                    ps.Ra = double.Parse(parts[0]);
                    ps.Dec = double.Parse(parts[1]);
                    ps.Pa = double.Parse(parts[2]);
                    ps.FineTime = long.Parse(parts[5]);
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
                FineTime = ps.FineTime,
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
