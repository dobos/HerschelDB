using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Spherical;

namespace Herschel.Loader
{
    public struct Pointing
    {
        public long FineTime;
        public Cartesian Point;
        public double PA;
    }
}
