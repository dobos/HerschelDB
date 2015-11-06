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
                case DetectorFootprint.Hifi:
                    return new DetectorHifi();
                default:
                    throw new NotImplementedException();
            }
        }

        public string Name { get; protected set; }

        public virtual Cartesian[] GetDefaultCorners()
        {
            return null;
        }

        protected Detector()
        {
        }

        public abstract Region GetFootprint(Cartesian pointing, double pa, double aperture);

        protected Region GetFootprintRectangle(Cartesian[] corners, Cartesian pointing, double pa)
        {
            corners = GetCorners(corners, pointing, pa);

            var r = new Region();
            r.Add(new Convex(new List<Cartesian>(corners), PointOrder.CCW));
            r.Simplify();

            return r;
        }

        protected Region GetFootprintCircle(Cartesian pointing, double radius)
        {
            // radius is in arc min
            var cos0 = Math.Cos(radius * Constant.Arcmin2Radian);

            var r = new Region();
            r.Add(new Convex(new Halfspace(pointing, cos0)));
            r.Simplify();

            return r;
        }

        public Cartesian[] GetCorners(Cartesian pointing, double pa)
        {
            return GetCorners(GetDefaultCorners(), pointing, pa);
        }

        /// <summary>
        /// Calculates the footprint of the detector
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public Cartesian[] GetCorners(Cartesian[] corners, Cartesian pointing, double pa)
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
            var res = new Cartesian[corners.Length];

            for (int i = 0; i < corners.Length; i++)
            {
                res[i] = Util.Rotate(r, corners[i]);
            }

            return res;
        }
    }
}
