using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Herschel.Lib;

namespace Herschel.Loader
{
    class PointingsFileSpire : PointingsFile
    {
        public long ObservationID;

        protected override Pointing Parse(string[] parts)
        {
            var p = new Pointing();

            p.Instrument = Instrument.Spire;

            p.ObsID = ObservationID;
            p.Ra = double.Parse(parts[0]);
            p.Dec = double.Parse(parts[1]);
            p.AV = double.Parse(parts[2]);
            p.Pa = double.Parse(parts[3]);
            p.SampleTime = double.Parse(parts[4]);
            p.CorrTime = double.Parse(parts[5]);
            return p;
        }

        protected override void ConvertPointingsFile(string inputFile, string outputFile, bool append)
        {
            ObservationID = long.Parse( inputFile.Substring(8, 17));

            base.ConvertPointingsFile(inputFile, outputFile, append);
        }
    }
}
