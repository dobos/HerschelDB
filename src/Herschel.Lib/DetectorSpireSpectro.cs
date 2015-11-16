using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Herschel.Lib
{
    public class DetectorSpireSpectro : Detector
    {
        public override Jhu.Spherical.Region GetFootprint(Jhu.Spherical.Cartesian pointing, double pa, double aperture)
        {
            return GetFootprintCircle(pointing, aperture / 2.0);
        }
    }
}
