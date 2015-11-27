using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Herschel.Lib
{
    public class LeapSeconds
    {

        private static readonly string FILE_NAME = "leapSeconds";

        private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// TAI-UTC seconds at 1 Jan 1972.
        /// </summary>
        private static readonly int LEAP_1972 = 10;

        /// <summary>
        /// Java Date representation of TAI epoch 1 Jan 1958.
        /// </summary>
        private static readonly long D58 = -((1970 - 1958) * 365 + 3) * 86400L * 1000;

        /// <summary>
        /// Java Date representation of 1 Jan 1972 UTC.
        /// </summary>
        private static readonly long D72 = (1972 - 1970) * 365 * 86400L * 1000;

        /// <summary>
        /// Microseconds (epoch 1958) of 1 Jan 1972 UTC.
        /// </summary>
        private static readonly long UTC_72 = (D72 - D58 + LEAP_1972 * 1000) * 1000;

        /// <summary>
        /// Number of seconds from 1 Jan 1958 to 1 Jan 1972.
        /// </summary>
        private static readonly long D72_58 = (D72 - D58) * 1000;


        /// <summary>
        /// Table of leap seconds in microseconds since the epoch 1958.
        /// An entry is the end of the leap second (e.g. 00:00:00 and not 23:59:60).
        /// </summary>
        private static long[] _leap;


        static LeapSeconds()
        {
            LoadLeapSecondData();
        }

        /// <summary>
        /// Private constructor to prevent instantiation of this class.
        /// </summary>
        private LeapSeconds()
        {
        }

        /// <summary>
        /// Reloads the table of leap-second data.
        /// </summary>
        public static void LoadLeapSecondData()
        {

            string[] dates = {
                "1972-07",
                "1973-01",
                "1974-01",
                "1975-01",
                "1976-01",
                "1977-01",
                "1978-01",
                "1979-01",
                "1980-01",
                "1981-07",
                "1982-07",
                "1983-07",
                "1985-07",
                "1988-01",
                "1990-01",
                "1991-01",
                "1992-07",
                "1993-07",
                "1994-07",
                "1996-01",
                "1997-07",
                "1999-01",
                "2006-01",
                "2009-01",
                "2012-07",
            };

            _leap = new long[dates.Length];

            

            for (int i = 0; i < dates.Length; i++)
            {
                var year = int.Parse(dates[i].Substring(0, 4));
                var month = int.Parse(dates[i].Substring(5, 2));

                var utc = UtcToLong(new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc));

                _leap[i] = (utc + (i + 1 + LEAP_1972) * 1000 - D58) * 1000;
            }
        }

        private static long UtcToLong(DateTime dt)
        {
            var utc = (long)(dt - epoch).TotalMilliseconds;
            return utc;
        }

        private static DateTime LongToUtc(long utc)
        {
            return epoch.AddMilliseconds(utc);
        }

        /**
         * Check that time is in valid range for leap-second calculation.
         *
         * @param tai Microseconds since TAI epoch
         * @throws IllegalArgumentException if time is before 1 Jan 1972 UTC.
         */
        private static void CheckTai(long tai)
        {
            if (tai < UTC_72)
            {
                throw new ArgumentException("Cannot handle time before 1972 UTC");
            }
        }

        /**
         * Return true if the specified time is within a leap second.<p>
         *
         * The result is true if the UTC second value is in the range [60, 60.999999].
         *
         * @param tai Microseconds since TAI epoch
         * @return true if the time is within a leap-second
         * @throws IllegalArgumentException if time is before 1 Jan 1972 UTC.
         */
        public static bool IsLeapSecond(long tai)
        {
            CheckTai(tai);
            long taiSeconds = tai / 1000000 + 1; // End of the second

            // Scan table backwards as times near the present are more likely.
            for (int i = _leap.Length - 1; i >= 0; i--)
            {
                if (taiSeconds == _leap[i] / 1000000)
                {
                    return true;
                }
            }

            return false;
        }

        /**
         * Return the number of leap seconds (TAI - UTC) at a specified time.
         * This includes the difference TAI-UTC = 10 that existed when leap-seconds
         * were introduced in 1972.<p>
         *
         * The result is the number of complete leap-seconds and consequently
         * increments at the end of the leap second (i.e. midnight UTC).
         *
         * @param tai Microseconds since TAI epoch
         * @return TAI - UTC seconds
         * @throws IllegalArgumentException if time is before 1 Jan 1972 UTC.
         */
        public static int GetLeapSeconds(long tai)
        {
            CheckTai(tai);

            // Scan table backwards as times near the present are more likely.
            int i = _leap.Length;

            while ((i >= 1) && (tai < _leap[i - 1]))
            {
                i--;
            }

            return LEAP_1972 + i;
        }

        /**
         * Subtract the leap-seconds from a TAI time and convert epoch to 1970.<p>
         *
         * All times within a leap-second are mapped onto the start of the next
         * second. i.e. 23:59:60.123 -> 00:00:00. Hence compressLeapSeconds is
         * a many-to-one function.<p>
         *
         * Does not support times before 1 Jan 1972 UTC.
         *
         * @param tai Microseconds since TAI epoch
         * @return Microseconds since 1970, ignoring leap-seconds
         * @throws IllegalArgumentException if time is before 1 Jan 1972 UTC.
         */
        static long CompressLeapSeconds(long tai)
        {
            CheckTai(tai);

            long taiSeconds = tai / 1000000; // Truncate to start of second
            bool isLeap = false;

            int i = _leap.Length;

            // Scan table backwards as times near the present are more likely.
            while ((i >= 1) && (tai < _leap[i - 1]))
            {
                if (taiSeconds + 1 == _leap[i - 1] / 1000000)
                {
                    isLeap = true;
                }
                i--;
            }

            // We subtract one more leap-second when we reach the end of
            // the leap second, whereas during the leap-second we use the
            // START of the leap second as the result.
            if (isLeap)
            {
                tai = taiSeconds * 1000000;
            }

            return tai + D58 * 1000 - (LEAP_1972 + i) * 1000000L;
        }

        /**
         * Insert leap seconds and change epoch to 1958 TAI.<p>
         *
         * Leap seconds are not represented in the utc representation
         * and hence the function is injective.<p>
         *
         * Does not support times before 1 Jan 1972 UTC.
         *
         * @param utc  Microseconds since 1970 ignoring leap seconds
         * @return TAI microseconds since 1958
         * @throws IllegalArgumentException if time is before 1 Jan 1972 UTC.
         */
        static long InsertLeapSeconds(long utc)
        {
            if (utc < D72 * 1000)
            {
                throw new ArgumentException("Cannot handle time before 1972 UTC");
            }

            // Scan table backwards as times near the present are more likely.
            long tai = (utc - D58 * 1000) + 1000000 * (LEAP_1972 + _leap.Length);

            for (int i = _leap.Length; i > 0; i--)
            {
                if (tai < _leap[i - 1])
                {
                    tai -= 1000000;
                }
            }

            return tai;
        }

        /// <summary>
        /// Convert a UTC time to TAI, both with epoch 1 Jan 1958 TAI.<p>
        /// </summary>
        /// <remarks>
        /// <B>WARNING:</B> This function should only be used for compatibility
        /// with legacy applications that represent UTC as an elapsed time omitting
        /// leap-seconds.<p>
        /// 
        /// Does not support times before 1 Jan 1972 UTC.
        /// </remarks>
        /// <returns>
        /// TAI microseconds since 1958
        /// </returns>
        public static long UtcToTai(long utc)
        {
            if (utc < D72_58)
            {
                throw new ArgumentException("Cannot handle time before 1972 UTC");
            }

            // Scan table backwards as times near the present are more likely.
            long tai = utc + 1000000 * (LEAP_1972 + _leap.Length);

            for (int i = _leap.Length; i > 0; i--)
            {
                if (tai < _leap[i - 1])
                {
                    tai -= 1000000;
                }
            }

            return tai;
        }

        public static long DateTimeToTai(DateTime dt)
        {
            return UtcToTai(UtcToLong(dt));
        }

        /// <summary>
        /// Convert a TAI time to UTC, both with epoch 1 Jan 1958 TAI.<p>
        /// </summary>
        /// <remarks>
        /// <B>WARNING:</B> This function should only be used for compatibility
        /// with legacy applications that represent UTC as an elapsed time omitting
        /// leap-seconds.<p>
        /// 
        /// Does not support times before 1 Jan 1972 UTC.
        /// </remarks>
        /// <returns>
        /// UTC microseconds since 1958 ignoring leap seconds
        /// </returns>
        public static long TaiToUtc(long tai)
        {
            return CompressLeapSeconds(tai) - D58 * 1000;
        }

        public static DateTime TaiToDateTime(long tai)
        {
            return LongToUtc(TaiToUtc(tai));
        }
    }


}
