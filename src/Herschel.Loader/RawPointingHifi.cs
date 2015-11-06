using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Herschel.Lib;

namespace Herschel.Loader
{
    public struct RawPointingHifi
    {
        public Instrument Instrument;
        public long ObsID;
        public PointingObservationType ObsType;
        public double Ra;
        public double Dec;
        public double Pa;
        public double AV;
        public double Aperture;
        public double Width;
        public double Height;
        public long FineTime;
        public double PatternAngle;
    }
}
