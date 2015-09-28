using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Herschel.Lib
{
    public struct PointingSpire
    {
        public Instrument Instrument;
        public long ObsID;
        public PointingObservationType ObsType;
        public double Ra;
        public double Dec;
        public double Pa;
        public double AV;
        public double SampleTime;
        public double CorrTime;
    }
}
