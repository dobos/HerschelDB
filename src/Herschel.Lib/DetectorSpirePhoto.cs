using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Spherical;

namespace Herschel.Lib
{
    public class DetectorSpirePhoto : Detector
    {
        public override Cartesian[] GetDefaultCorners()
        {
            double a = 4.0 / 60.0;
            double b = 2.0 / 60.0;
            return new Cartesian[]
                            {
                                new Cartesian(a, b),
                                new Cartesian(-a, b),
                                new Cartesian(-a, -b),
                                new Cartesian(a, -b)
                            };
        }

        public override Region GetFootprint(Cartesian pointing, double pa, double aperture)
        {
            return GetFootprintRectangle(GetDefaultCorners(), pointing, pa);
        }
    }
}
