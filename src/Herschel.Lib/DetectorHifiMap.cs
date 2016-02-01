using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Spherical;

namespace Herschel.Lib
{
    public class DetectorHifiMap : Detector
    {
        public double Width { get; set; }
        public double Height { get; set; }

        public override Cartesian[] GetDefaultCorners()
        {
            double a = Width / 2.0 / 60.0;
            double b = Height / 2.0 / 60.0;

            return new Cartesian[]
                {
                    new Cartesian(a, b),
                    new Cartesian(-a, b),
                    new Cartesian(-a, -b),
                    new Cartesian(a, -b)
                };
        }

        /// <summary>
        /// Returns the footprint of the map
        /// </summary>
        /// <returns></returns>
        public override Region GetFootprint(Cartesian pointing, double pa, double aperture)
        {
            if (aperture == 0.0)
            {
                return GetFootprintRectangle(GetDefaultCorners(), pointing, pa);
            }
            else
            {
                double a = Width / 2.0 / 60.0;
                double b = Height / 2.0 / 60.0;
                double r = aperture / 2.0 / 3600.0;

                var corners = new Cartesian[]
                {
                    new Cartesian(a + r, b + r),
                    new Cartesian(-a - r, b + r),
                    new Cartesian(-a - r, -b - r),
                    new Cartesian(a + r, -b - r)
                };

                return GetFootprintRectangle(corners, pointing, pa);

#if false
                double a = Width / 2.0 / 60.0;
                double b = Height / 2.0 / 60.0;
                double r = aperture / 2.0 / 3600.0;

                var cornersC = new Cartesian[]
                {
                    new Cartesian(a, b),
                    new Cartesian(-a, b),
                    new Cartesian(-a, -b),
                    new Cartesian(a, -b)
                };

                var cornersW = new Cartesian[]
                {
                    new Cartesian(a + r, b),
                    new Cartesian(-a - r, b),
                    new Cartesian(-a - r, -b),
                    new Cartesian(a + r, -b)
                };

                var cornersH = new Cartesian[]
                {
                    new Cartesian(a, b + r),
                    new Cartesian(-a, b + r),
                    new Cartesian(-a, -b - r),
                    new Cartesian(a, -b - r)
                };

                var sb = new ShapeBuilder();

                var region = new Region();

                var cc = GetCorners(cornersC, pointing, pa);

                for (int i = 0; i < cc.Length; i++)
                {
                    var ci = sb.CreateCircle(cc[i], r * 60);
                    var rr = new Region(ci, false);
                    rr.Simplify();
                    region.SmartUnion(rr);
                }

                //region.SmartUnion(GetFootprintRectangle(cornersW, pointing, pa));
                //region.SmartUnion(GetFootprintRectangle(cornersH, pointing, pa));

                return region;
#endif
            }
        }
    }
}
