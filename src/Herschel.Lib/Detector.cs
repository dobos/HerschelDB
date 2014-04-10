using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spherical;

namespace Herschel.Lib
{
    public class Detector
    {
        public static Detector Blue
        {
            get
            {
                double a = 1.75 / 60.0;
                double b = 0.875 / 60.0;

                return new Detector()
                {
                    Name = "Blue",
                    Corners = new Cartesian[]
                                {
                                    new Cartesian(a, b),
                                    new Cartesian(-a, b),
                                    new Cartesian(-a, -b),
                                    new Cartesian(a, -b)
                                }
                };
            }
        }

        public static Detector Red
        {
            get
            {
                return null;
            }
        }

        public static Detector Create(string name)
        {
            switch (name)
            {
                case "Blue":
                    return Detector.Blue;
                case "Red":
                    return Detector.Red;
                default:
                    throw new NotImplementedException();
            }
        }

        public string Name { get; protected set; }
        public Cartesian[] Corners { get; protected set; }

        protected Detector()
        {
        }

        /// <summary>
        /// Calculates the footprint of the detector
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public Cartesian[] GetCorners(Cartesian pointing, double pa)
        {
            // Rotate around x (PA)
            double ang = pa * Constants.Degree2Radian;
            double[] r1 = new double[]
            {
                1, 0, 0,
                0, Math.Cos(ang), -Math.Sin(ang),
                0, Math.Sin(ang), Math.Cos(ang)
            };

            // Rotate around y (Dec)
            ang = -pointing.Dec * Spherical.Constant.Degree2Radian;
            double[] r2 = new double[]
            {
                Math.Cos(ang), 0, Math.Sin(ang),
                0, 1, 0,
                -Math.Sin(ang), 0, Math.Cos(ang)
            };

            // Rotate around z (RA)
            ang = pointing.RA * Spherical.Constant.Degree2Radian;
            double[] r3 = new double[]
            {
                Math.Cos(ang), -Math.Sin(ang), 0,
                Math.Sin(ang), Math.Cos(ang), 0,
                0, 0, 1
            };

            // Rotation matrix
            double[] r = Util.MatMul(Util.MatMul(r3, r2), r1);

            // Rotate corners
            var res = new Cartesian[Corners.Length];

            for (int i = 0; i < Corners.Length; i++)
            {
                res[i] = Util.Rotate(r, Corners[i]);
            }

            return res;
        }
    }
}
