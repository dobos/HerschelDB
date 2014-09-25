using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Herschel.Lib
{
    public class DetectorPacsSpectro : Detector
    {
        public override Jhu.Spherical.Cartesian[] Corners
        {
            get { throw new NotImplementedException(); }
        }

        public override Jhu.Spherical.Region GetFootprint(Jhu.Spherical.Cartesian pointing, double pa)
        {
            throw new NotImplementedException();
        }
    }
}
