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
        public long BuldingBlockType;
        public long FineTime;
        public Instrument Instrument;
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

        public void Write(TextWriter writer)
        {
            writer.Write("{0} ", ObsID);
            writer.Write("{0} ", FineTime);
            writer.Write("{0} ", (byte)Instrument);
            writer.Write("{0} ", BuldingBlockType);
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
