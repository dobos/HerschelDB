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
            bool keep = true;
            var pp = new PointingPacs();

            pp.Instrument = Instrument.Pacs;

            // Parse columns

            switch (ObservationType)
            {
                case Lib.PointingObservationType.PacsPhoto:
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

                    //keep &= pp.BBID == 215131301;

                    break;
                case PointingObservationType.PacsSpectroRange:
                    pp.ObsID = long.Parse(parts[1]);
                    pp.FineTime = long.Parse(parts[6]);
                    pp.BBID = long.Parse(parts[2]);
                    pp.Ra = double.Parse(parts[30]);
                    pp.RaError = double.Parse(parts[33]);
                    pp.Dec = double.Parse(parts[31]);
                    pp.DecError = double.Parse(parts[34]);
                    pp.Pa = double.Parse(parts[32]);
                    pp.PaError = double.Parse(parts[35]);
                    pp.AVX = double.Parse(parts[55]);
                    pp.AVXError = double.Parse(parts[56]);
                    pp.AVY = double.Parse(parts[54]);
                    pp.AVYError = double.Parse(parts[57]);
                    pp.AVZ = double.Parse(parts[55]);
                    pp.AVZError = double.Parse(parts[58]);

                    keep &= bool.Parse(parts[40]);  // onTarget
                    keep &= !bool.Parse(parts[43]); // isOffPosition
                    keep &= !bool.Parse(parts[50]); // isOutOfField
                    break;
                case PointingObservationType.PacsSpectroLine:
                    // There are two different format here
                    var first = long.Parse(parts[0]);

                    if (first < 1000000000)
                    {
                        pp.ObsID = long.Parse(parts[1]);
                        pp.FineTime = long.Parse(parts[6]);
                        pp.BBID = long.Parse(parts[2]);
                        pp.Ra = double.Parse(parts[30]);
                        pp.RaError = double.Parse(parts[33]);
                        pp.Dec = double.Parse(parts[31]);
                        pp.DecError = double.Parse(parts[34]);
                        pp.Pa = double.Parse(parts[32]);
                        pp.PaError = double.Parse(parts[35]);
                        pp.AVX = double.Parse(parts[55]);
                        pp.AVXError = double.Parse(parts[56]);
                        pp.AVY = double.Parse(parts[54]);
                        pp.AVYError = double.Parse(parts[57]);
                        pp.AVZ = double.Parse(parts[55]);
                        pp.AVZError = double.Parse(parts[58]);

                        keep &= bool.Parse(parts[40]);  // onTarget
                        keep &= !bool.Parse(parts[43]); // isOffPosition
                        keep &= !bool.Parse(parts[50]); // isOutOfField
                    }
                    else
                    {
                        pp.ObsID = long.Parse(parts[0]);
                        pp.BBID = long.Parse(parts[1]);
                        pp.FineTime = long.Parse(parts[2]);
                        pp.Ra = double.Parse(parts[3]);
                        pp.Dec = double.Parse(parts[4]);
                        pp.Pa = double.Parse(parts[5]);
                        pp.AVX = double.Parse(parts[6]);
                        pp.AVY = double.Parse(parts[7]);
                        pp.AVZ = double.Parse(parts[8]);
                    }
                    break;
                default:
                    throw new NotImplementedException();

            }

            // Convert PACS pointing to unified format

            pointing = new Pointing()
            {
                Instrument = pp.Instrument,
                ObsID = pp.ObsID,
                BBID = pp.BBID,
                ObsType = ObservationType,
                FineTime = pp.FineTime,
                Ra = pp.Ra,
                Dec = pp.Dec,
                Pa = pp.Pa,
                AV = Math.Sqrt(pp.AVY * pp.AVY + pp.AVZ * pp.AVZ),
            };

            // Accept only valid BBIDs
            return keep;
        }
    }
}
