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
        public override Cartesian[] Corners
        {
            get { throw new NotImplementedException(); }
        }

        public override Region GetFootprint(Cartesian pointing, double pa)
        {
            return GetFootprintRectangle(pointing, pa);
        }
    }
}
