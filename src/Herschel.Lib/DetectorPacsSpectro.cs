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
        public override Jhu.Spherical.Cartesian[] Corners
        {
            get {
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
        }

        public override Jhu.Spherical.Region GetFootprint(Jhu.Spherical.Cartesian pointing, double pa)
        {
            return GetFootprintRectangle(pointing, pa);
        }
    }
}
