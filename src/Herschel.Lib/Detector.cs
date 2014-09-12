using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Spherical;

namespace Herschel.Lib
{
    public class Detector
    {
        public static Detector PacsPhoto
        {
            get
            {
                double a = 1.75 / 60.0;
                double b = 0.875 / 60.0;

                return new Detector()
                {
                    Name = DetectorFootprint.PacsPhoto.ToString(),
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

        public static Detector PacsSpectro
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public static Detector SpirePhoto
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public static Detector SpireSpectro
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public static Detector Create(string name)
        {
            DetectorFootprint det;
            Enum.TryParse(name, out det);

            switch (det)
            {
                case DetectorFootprint.PacsPhoto:
                    return Detector.PacsPhoto;
                case DetectorFootprint.PacsSpectro:
                    return Detector.PacsSpectro;
                case DetectorFootprint.SpirePhoto:
                    return Detector.SpirePhoto;
                case DetectorFootprint.SpireSpectro:
                    return Detector.SpireSpectro;
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
            double ang = (180 - pa) * Constants.Degree2Radian;
            double[] r1 = new double[]
            {
                1, 0, 0,
                0, Math.Cos(ang), -Math.Sin(ang),
                0, Math.Sin(ang), Math.Cos(ang)
            };

            // Rotate around y (Dec)
            ang = -pointing.Dec * Jhu.Spherical.Constant.Degree2Radian;
            double[] r2 = new double[]
            {
                Math.Cos(ang), 0, Math.Sin(ang),
                0, 1, 0,
                -Math.Sin(ang), 0, Math.Cos(ang)
            };

            // Rotate around z (RA)
            ang = pointing.RA * Jhu.Spherical.Constant.Degree2Radian;
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
