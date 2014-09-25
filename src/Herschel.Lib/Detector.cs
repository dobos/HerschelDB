using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Spherical;

namespace Herschel.Lib
{
    public abstract class Detector
    {
        public static Detector Create(string name)
        {
            DetectorFootprint det;
            Enum.TryParse(name, out det);

            switch (det)
            {
                case DetectorFootprint.PacsPhoto:
                    return new DetectorPacsPhoto();
                case DetectorFootprint.PacsSpectro:
                    return new DetectorPacsSpectro();
                case DetectorFootprint.SpirePhoto:
                    return new DetectorSpirePhoto();
                case DetectorFootprint.SpireSpectro:
                    return new DetectorSpireSpectro();
                default:
                    throw new NotImplementedException();
            }
        }

        public string Name { get; protected set; }

        public abstract Cartesian[] Corners { get; }

        protected Detector()
        {
        }

        public abstract Region GetFootprint(Cartesian pointing, double pa);

        protected Region GetFootprintRectangle(Cartesian pointing, double pa)
        {
            var corners = GetCorners(pointing, pa);

            var r = new Region();
            r.Add(new Convex(new List<Cartesian>(corners), PointOrder.CW));
            r.Simplify();

            return r;
        }

        protected Region GetFootprintCircle(Cartesian pointing, double radius)
        {
            // TODO:
            throw new NotImplementedException();
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
