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

        public void Parse(string[] parts)
        {
            var c = System.Globalization.CultureInfo.InvariantCulture;

            ObsID = long.Parse(parts[1], c);
            FineTime = long.Parse(parts[6], c);
            BBID = long.Parse(parts[2], c);
            Ra = double.Parse(parts[27], c);
            RaError = double.Parse(parts[30], c);
            Dec = double.Parse(parts[28], c);
            DecError = double.Parse(parts[31], c);
            Pa = double.Parse(parts[29], c);
            PaError = double.Parse(parts[32], c);
            AVX = double.Parse(parts[50], c);
            AVXError = double.Parse(parts[53], c);
            AVY = double.Parse(parts[51], c);
            AVYError = double.Parse(parts[54], c);
            AVZ = double.Parse(parts[52], c);
            AVZError = double.Parse(parts[55], c);
            Utc = long.Parse(parts[56], c);
        }

        public void Write(TextWriter writer)
        {
            writer.Write("{0} ", ObsID);
            writer.Write("{0} ", FineTime);
            writer.Write("{0} ", BBID);
            writer.Write("{0} ", Ra);
            writer.Write("{0} ", RaError);
            writer.Write("{0} ", Dec);
            writer.Write("{0} ", DecError);
            writer.Write("{0} ", Pa);
            writer.Write("{0} ", PaError);
            writer.Write("{0} ", AVX);
            writer.Write("{0} ", AVXError);
            writer.Write("{0} ", AVY);
            writer.Write("{0} ", AVYError);
            writer.Write("{0} ", AVZ);
            writer.Write("{0} ", AVZError);
            writer.WriteLine("{0}", Utc);
        }
    }
}
