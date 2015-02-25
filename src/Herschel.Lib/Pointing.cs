using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Herschel.Lib
{
    public struct Pointing
    {
        public Instrument Instrument;
        public long ObsID;
        public long BBID;
        public byte ObsType;
        public long FineTime;
        public double Ra;
        public double Dec;
        public double Pa;
        public double AV;
    }
}
