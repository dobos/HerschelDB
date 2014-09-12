using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Spherical;

namespace Herschel.Lib
{
    public class DetectorPacsPhoto : Detector
    {
        public override Cartesian[] Corners
        {
            get
            {
                double a = 1.75 / 60.0;
                double b = 0.875 / 60.0;

                return new Cartesian[]
                {
                    new Cartesian(a, b),
                    new Cartesian(-a, b),
                    new Cartesian(-a, -b),
                    new Cartesian(a, -b)
                };
            }
        }

        /// <summary>
        /// Returns the footprint of the detector
        /// </summary>
        /// <returns></returns>
        public override Region GetFootprint(Cartesian pointing, double pa)
        {
            return GetFootprintRectangle(pointing, pa);
        }
    }
}
