using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Herschel.Lib;

namespace Herschel.Loader
{
    class PointingsFilePacs : PointingsFile
    {

        protected override bool Parse(string[] parts, out Pointing pointing)
        {
            var pp = new PointingPacs();

            pp.Instrument = Instrument.Pacs;

            pp.ObsID = long.Parse(parts[1]);
            pp.FineTime = long.Parse(parts[6]);
            pp.BBID = long.Parse(parts[2]);
            pp.Ra = double.Parse(parts[27]);
            pp.RaError = double.Parse(parts[30]);
            pp.Dec = double.Parse(parts[28]);
            pp.DecError = double.Parse(parts[31]);
            pp.Pa = double.Parse(parts[29]);
            pp.PaError = double.Parse(parts[32]);
            pp.AVX = double.Parse(parts[50]);
            pp.AVXError = double.Parse(parts[53]);
            pp.AVY = double.Parse(parts[51]);
            pp.AVYError = double.Parse(parts[54]);
            pp.AVZ = double.Parse(parts[52]);
            pp.AVZError = double.Parse(parts[55]);
            pp.Utc = long.Parse(parts[56]);

            // Convert PACS pointing to unified format

            pointing = new Pointing()
            {
                Instrument = pp.Instrument,
                ObsID = pp.ObsID,
                FineTime = pp.FineTime,
                Ra = pp.Ra,
                Dec = pp.Dec,
                Pa = pp.Pa,
                AV = Math.Sqrt(pp.AVY * pp.AVY + pp.AVZ * pp.AVZ),
                // TODO: Utc
            };

            // TODO: change this to accept all valid BBIDs
            return pp.BBID == 215131301;
        }
    }
}
