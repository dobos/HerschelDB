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
        private static readonly DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);

        private long value;

        public long Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public bool IsUndefined
        {
            get { return value == 0; }
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

        public static bool TryParse(string value, out FineTime fineTime)
        {
            DateTime d;
            long ft;
            
            if (DateTime.TryParse(value, out d))
            {
                fineTime = FineTime.FromDateTime(d);
                return true;
            }
            else if (long.TryParse(value, out ft))
            {
                fineTime = new FineTime(ft);
                return true;
            }
            else
            {
                throw new FormatException();
            }
        }

        public static FineTime Parse(string value)
        {
            FineTime ft;
            TryParse(value, out ft);

            return ft;
        }

        public static FineTime Undefined
        {
            get { return new FineTime(0); }
        }

        public static FineTime FromDateTime(DateTime dateTime)
        {
            var ft = new FineTime((long)Math.Round((dateTime.ToUniversalTime() - unixEpoch).TotalSeconds * 1e6));
            return ft;
        }

        public DateTime ToDateTime()
        {
            var dt = unixEpoch.AddSeconds(this.value * 1e-6);
            return dt;
        }
    }
}
