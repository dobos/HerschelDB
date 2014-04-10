using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Herschel.Lib
{
    public struct Pointing
    {
        public long ObsID;
        public long FineTime;
        public long BBID;
        public double Ra;
        public double RaError;
        public double Dec;
        public double DecError;
        public double Pa;
        public double PaError;
        public double AVX;
        public double AVXError;
        public double AVY;
        public double AVYError;
        public double AVZ;
        public double AVZError;
        public long Utc;
    }
}
