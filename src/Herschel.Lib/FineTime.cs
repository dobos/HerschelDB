using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Herschel.Lib
{
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
            return Value.ToString();
        }
    }
}
