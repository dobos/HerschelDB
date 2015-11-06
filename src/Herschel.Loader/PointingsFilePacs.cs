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

        protected override bool Parse(string[] parts, out RawPointing pointing)
        {
            bool keep = true;
            var pp = new RawPointingPacs();

            pp.Instrument = Instrument.Pacs;

            // Parse columns

            switch (ObservationType)
            {
                case PointingObservationType.PacsPhoto:
                    if (parts.Length == 56 || parts.Length == 59)
                    {
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

                        pp.IsAPosition = bool.Parse(parts[45]);
                        pp.IsBPosition = bool.Parse(parts[46]);
                        pp.IsOffPosition = bool.Parse(parts[40]);
                        pp.IsOnTarget = bool.Parse(parts[37]);
                        pp.RasterLineNum = int.Parse(parts[34]);
                        pp.RasterColumnNum = int.Parse(parts[35]);

                        // NOTE: this could be used to filter turn-around but
                        // it's better to do it from SQL
                        //keep &= pp.BBID == 215131301;
                    }
                    else if (parts.Length == 57)
                    {
                        pp.ObsID = long.Parse(parts[1]);
                        pp.FineTime = long.Parse(parts[6]);
                        pp.BBID = long.Parse(parts[2]);
                        pp.Ra = double.Parse(parts[27]);
                        pp.RaError = double.Parse(parts[30]);
                        pp.Dec = double.Parse(parts[28]);
                        pp.DecError = double.Parse(parts[31]);
                        pp.Pa = double.Parse(parts[29]);
                        pp.PaError = double.Parse(parts[32]);
                        pp.AVX = double.Parse(parts[51]);
                        pp.AVXError = double.Parse(parts[54]);
                        pp.AVY = double.Parse(parts[52]);
                        pp.AVYError = double.Parse(parts[55]);
                        pp.AVZ = double.Parse(parts[53]);
                        pp.AVZError = double.Parse(parts[56]);

                        pp.IsAPosition = bool.Parse(parts[45]);
                        pp.IsBPosition = bool.Parse(parts[46]);
                        pp.IsOffPosition = bool.Parse(parts[40]);
                        pp.IsOnTarget = bool.Parse(parts[37]);
                        pp.RasterLineNum = int.Parse(parts[34]);
                        pp.RasterColumnNum = int.Parse(parts[35]);
                    }
                    else if (parts.Length == 60)
                    {
                        pp.ObsID = long.Parse(parts[1]);
                        pp.FineTime = long.Parse(parts[6]);
                        pp.BBID = long.Parse(parts[2]);
                        pp.Ra = double.Parse(parts[27]);
                        pp.RaError = double.Parse(parts[30]);
                        pp.Dec = double.Parse(parts[28]);
                        pp.DecError = double.Parse(parts[31]);
                        pp.Pa = double.Parse(parts[29]);
                        pp.PaError = double.Parse(parts[32]);
                        pp.AVX = double.Parse(parts[51]);
                        pp.AVXError = double.Parse(parts[54]);
                        pp.AVY = double.Parse(parts[52]);
                        pp.AVYError = double.Parse(parts[55]);
                        pp.AVZ = double.Parse(parts[54]);
                        pp.AVZError = double.Parse(parts[56]);

                        pp.IsAPosition = bool.Parse(parts[45]);
                        pp.IsBPosition = bool.Parse(parts[46]);
                        pp.IsOffPosition = bool.Parse(parts[40]);
                        pp.IsOnTarget = bool.Parse(parts[37]);
                        pp.RasterLineNum = int.Parse(parts[34]);
                        pp.RasterColumnNum = int.Parse(parts[35]);
                    }
                    else
                    {
                        throw new Exception("Unknown file format");
                    }

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

                    pp.IsAPosition = bool.Parse(parts[48]);
                    pp.IsBPosition = bool.Parse(parts[49]);
                    pp.IsOffPosition = bool.Parse(parts[43]);
                    pp.IsOnTarget = bool.Parse(parts[40]);
                    pp.RasterLineNum = int.Parse(parts[37]);
                    pp.RasterColumnNum = int.Parse(parts[38]);

                    break;
                case PointingObservationType.PacsSpectroLine:
                    // There are two different format here
                    var first = long.Parse(parts[0]);

                    if (first < 1000000000)
                    {
                        // RESETINDEX in the 0th column

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

                        pp.IsAPosition = bool.Parse(parts[48]);
                        pp.IsBPosition = bool.Parse(parts[49]);
                        pp.IsOffPosition = bool.Parse(parts[43]);
                        pp.IsOnTarget = bool.Parse(parts[40]);
                        pp.RasterLineNum = int.Parse(parts[37]);
                        pp.RasterColumnNum = int.Parse(parts[38]);
                    }
                    else
                    {
                        // small files with much less columns

                        pp.ObsID = long.Parse(parts[0]);
                        pp.BBID = long.Parse(parts[1]);
                        pp.FineTime = long.Parse(parts[2]);
                        pp.Ra = double.Parse(parts[3]);
                        pp.Dec = double.Parse(parts[4]);
                        pp.Pa = double.Parse(parts[5]);
                        pp.AVX = double.Parse(parts[6]);
                        pp.AVY = double.Parse(parts[7]);
                        pp.AVZ = double.Parse(parts[8]);

                        pp.IsAPosition = true;
                        pp.IsBPosition = true;
                        pp.IsOffPosition = true;
                        pp.IsOnTarget = true;
                        pp.RasterLineNum = -1;
                        pp.RasterColumnNum = -1;
                    }
                    break;
                default:
                    throw new NotImplementedException();

            }

            // Convert PACS pointing to unified format

            pointing = new RawPointing()
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

                IsAPosition = pp.IsAPosition,
                IsBPosition = pp.IsBPosition,
                IsOffPosition = pp.IsOffPosition,
                IsOnTarget = pp.IsOnTarget,
                RasterLineNum = pp.RasterLineNum,
                RasterColumnNum = pp.RasterColumnNum,
            };

            // Accept only valid BBIDs
            return keep;
        }
    }
}
