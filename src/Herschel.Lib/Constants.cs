using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Herschel.Lib
{
    class Constants
    {
        /// <summary>
        /// Constant for the theoretical limit of the relative error of double precision numbers.
        /// </summary>
        /// <remarks>
        /// With p=53 significand (IEEE-754), it is <c>Math.Pow(2,-53)</c>, which is roughly 1.1e-16.
        /// </remarks>
        public static readonly double DoublePrecision = Math.Pow(2, -53);

        /// <summary>
        /// Constant for the theoretical limit of the relative error of double precision numbers times two.
        /// </summary>
        /// <remarks>
        /// This is, for example, the maximum relative error in the difference of two numbers, <c>z = x-y</c>.
        /// </remarks>
        public static readonly double DoublePrecision2x = 2 * DoublePrecision;
        internal static readonly double DoublePrecision4x = 4 * DoublePrecision;

        /// <summary>
        /// Constant factor in radians for determining when two points are the same.
        /// </summary>
        public static readonly double Tolerance = 2e-8;
        /// <summary>
        /// Constant is the cosine of the tolerance.
        /// </summary>
        public static readonly double CosTolerance = Math.Cos(Tolerance);
        /// <summary>
        /// Constant is the sine of the tolerance.
        /// </summary>
        public static readonly double SinTolerance = Math.Sin(Tolerance);

        internal static readonly double TolHalf = Tolerance / 2;
        internal static readonly double CosHalf = Math.Cos(TolHalf);
        internal static readonly double SinHalf = Math.Sin(TolHalf);
        internal static readonly double TolArea = TolHalf * TolHalf * Math.PI;

        internal static readonly double SafeLimit = 1e-7;
        internal static readonly double CosSafe = Math.Cos(SafeLimit);
        internal static readonly double SinSafe = Math.Sin(SafeLimit);

        internal static readonly int HashDigit = 15;
        internal static readonly double HashMagic = 2; //29;

        /// <summary>
        /// Constant factor for converting degrees to radians.
        /// </summary>
        public static readonly double Degree2Radian = Math.PI / 180;

        /// <summary>
        /// Constant factor for converting radians to degrees.
        /// </summary>
        public static readonly double Radian2Degree = 1 / Degree2Radian;

        /// <summary>
        /// Constant factor for converting arc minutes to radians.
        /// </summary>
        public static readonly double Arcmin2Radian = Degree2Radian / 60;

        /// <summary>
        /// Constant factor for converting radians to arc minutes.
        /// </summary>
        public static readonly double Radian2Arcmin = 1 / Arcmin2Radian;

        /// <summary>
        /// Constant factor for converting square radians to square degrees.
        /// </summary>
        public static readonly double SquareRadian2SquareDegree = Radian2Degree * Radian2Degree;

        /// <summary>
        /// Constant factor for area of surface of the unit sphere in square degrees.
        /// </summary>
        public static readonly double WholeSphereInSquareDegree = 4 * Math.PI * SquareRadian2SquareDegree;
    }
}
