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

            ps.Instrument = Instrument.SpirePhoto;

            ps.ObsID = ObservationID;
            ps.Ra = double.Parse(parts[0]);
            ps.Dec = double.Parse(parts[1]);
            ps.AV = double.Parse(parts[2]);
            ps.Pa = double.Parse(parts[3]);
            ps.SampleTime = double.Parse(parts[4]);
            ps.CorrTime = double.Parse(parts[5]);

            pointing = new Pointing()
            {
                Instrument = ps.Instrument,
                ObsID = ps.ObsID,
                FineTime = (long)Math.Floor(ps.SampleTime * 1e6),
                Ra = ps.Ra,
                Dec = ps.Dec,
                Pa = ps.Pa,
                AV = ps.AV * 3600,  // convert from deg s-1 to arcsec s-1
            };
            
            return true;
        }

        protected override void ConvertPointingsFile(string inputFile, string outputFile, bool append)
        {
            var file = Path.GetFileName(inputFile);
            ObservationID = long.Parse(file.Substring(8, 10));

            base.ConvertPointingsFile(inputFile, outputFile, append);
        }
    }
}
