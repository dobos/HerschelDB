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

        protected override Pointing Parse(string[] parts)
        {
            var p = new Pointing();

            p.Instrument = Instrument.Pacs;

            p.ObsID = long.Parse(parts[1]);
            p.FineTime = long.Parse(parts[6]);
            p.BuldingBlockType = long.Parse(parts[2]);
            p.Ra = double.Parse(parts[27]);
            p.RaError = double.Parse(parts[30]);
            p.Dec = double.Parse(parts[28]);
            p.DecError = double.Parse(parts[31]);
            p.Pa = double.Parse(parts[29]);
            p.PaError = double.Parse(parts[32]);
            p.AVX = double.Parse(parts[50]);
            p.AVXError = double.Parse(parts[53]);
            p.AVY = double.Parse(parts[51]);
            p.AVYError = double.Parse(parts[54]);
            p.AVZ = double.Parse(parts[52]);
            p.AVZError = double.Parse(parts[55]);
            p.Utc = long.Parse(parts[56]);

            return p;
        }
    }
}
