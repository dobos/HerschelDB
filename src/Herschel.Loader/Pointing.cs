using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Herschel.Lib;

namespace Herschel.Loader
{
    public struct RawPointing
    {
        public Instrument Instrument;
        public long ObsID;
        public long BBID;
        public PointingObservationType ObsType;
        public long FineTime;
        public double Ra;
        public double Dec;
        public double Pa;
        public double AV;
        public double Aperture;
        public double Width;
        public double Height;
        public bool IsAPosition;
        public bool IsBPosition;
        public bool IsOffPosition;
        public bool IsOnTarget;
        public int RasterLineNum;
        public int RasterColumnNum;
        public double RasterAngle;
    }
}
