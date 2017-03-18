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

            // TODO: we don't need the RawPointing... structure here anymore
            var pp = new RawPointingPacs();

            pp.Instrument = Instrument.Pacs;

            // Parse columns
            pp.ObsID = long.Parse(parts[Columns["OBSID"]]);
            pp.FineTime = long.Parse(parts[Columns["FINETIME"]]);
            pp.BBID = long.Parse(parts[Columns["BBID"]]);
            pp.Ra = double.Parse(parts[Columns["RaArray"]]);
            pp.RaError = double.Parse(parts[Columns["RaArrayErr"]]);
            pp.Dec = double.Parse(parts[Columns["DecArray"]]);
            pp.DecError = double.Parse(parts[Columns["DecArrayErr"]]);
            pp.Pa = double.Parse(parts[Columns["PaArray"]]);
            pp.PaError = double.Parse(parts[Columns["PaArrayErr"]]);
            pp.AVX = double.Parse(parts[Columns["AngularVelocityX"]]);
            pp.AVXError = double.Parse(parts[Columns["AngularVelocityXError"]]);
            pp.AVY = double.Parse(parts[Columns["AngularVelocityY"]]);
            pp.AVYError = double.Parse(parts[Columns["AngularVelocityYError"]]);
            pp.AVZ = double.Parse(parts[Columns["AngularVelocityZ"]]);
            pp.AVZError = double.Parse(parts[Columns["AngularVelocityZError"]]);

            pp.IsAPosition = bool.Parse(parts[Columns["IsAPosition"]]);
            pp.IsBPosition = bool.Parse(parts[Columns["IsBPosition"]]);
            pp.IsOffPosition = bool.Parse(parts[Columns["IsOffPos"]]);
            pp.IsOnTarget = bool.Parse(parts[Columns["OnTarget"]]);
            pp.RasterLineNum = int.Parse(parts[Columns["RasterLineNum"]]);
            pp.RasterColumnNum = int.Parse(parts[Columns["RasterColumnNum"]]);

            // NOTE: this could be used to filter turn-around but
            // it's better to do it from SQL
            //keep &= pp.BBID == 215131301;

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
