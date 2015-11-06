using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Spherical;

namespace Herschel.Lib
{
    public class DetectorPacsSpectro : Detector
    {
        public override Jhu.Spherical.Cartesian[] GetDefaultCorners()
        {
            double a = 23.5 / 3600.0;
            double b = 23.5 / 3600.0;

            return new Cartesian[]
            {
                new Cartesian(a, b),
                new Cartesian(-a, b),
                new Cartesian(-a, -b),
                new Cartesian(a, -b)
            };
        }

        public static Jhu.Spherical.Cartesian[] GetRasterCorners(int rasterCols, int rasterRows, double rasterStep)
        {
            double a = rasterCols * rasterStep / 2.0 / 3600.0;
            double b = rasterRows * rasterStep / 2.0 / 3600.0;

            return new Cartesian[]
            {
                new Cartesian(a, b),
                new Cartesian(-a, b),
                new Cartesian(-a, -b),
                new Cartesian(a, -b)
            };
        }

        public override Jhu.Spherical.Region GetFootprint(Jhu.Spherical.Cartesian pointing, double pa, double aperture)
        {
            return GetFootprintRectangle(GetDefaultCorners(), pointing, pa);
        }
    }
}
