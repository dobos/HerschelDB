using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Herschel.Lib
{
    [Serializable]
    public struct FineTime
    {
        private long value;

        public long Value 
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public FineTime(long value)
        {
            this.value = value;
        }

        public static implicit operator FineTime(long value)
        {
            return new FineTime(value);
        }

        public override string ToString()
        {
            return value.ToString();
        }

        public static FineTime Parse(string value)
        {
            return new FineTime(long.Parse(value));
        }

        public static FineTime Undefined
        {
            get { return new FineTime(0); }
        }

        public static bool IsUndefined(FineTime fineTime)
        {
            return fineTime.value == 0;
        }
    }
}
