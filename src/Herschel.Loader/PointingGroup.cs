using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Spherical;

namespace Herschel.Loader
{
    class PointingGroup
    {
        public List<PointingCluster> Clusters;
        public Cartesian Center;
        public double PA;

        public PointingGroup()
        {
            Clusters = new List<PointingCluster>();
            Center = Cartesian.NaN;
            PA = double.NaN;
        }
    }
}
