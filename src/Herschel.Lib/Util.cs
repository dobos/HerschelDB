using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SqlTypes;
using Jhu.Spherical;
using Jhu.Spherical.Htm;

namespace Herschel.Lib
{
    public static class Util
    {
        public static double[] MatMul(double[] a, double[] b)
        {
            double[] r = new double[9];

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    r[3 * i + j] = 0;

                    for (int k = 0; k < 3; k++)
                    {
                        r[3 * i + j] += a[3 * i + k] * b[3 * k + j];
                    }
                }
            }

            return r;
        }

        public static Cartesian Rotate(double[] r, Cartesian b)
        {
            Cartesian res = new Cartesian();

            res.X = r[3 * 0 + 0] * b.X + r[3 * 0 + 1] * b.Y + r[3 * 0 + 2] * b.Z;
            res.Y = r[3 * 1 + 0] * b.X + r[3 * 1 + 1] * b.Y + r[3 * 1 + 2] * b.Z;
            res.Z = r[3 * 2 + 0] * b.X + r[3 * 2 + 1] * b.Y + r[3 * 2 + 2] * b.Z;

            return res;
        }
    }
}
