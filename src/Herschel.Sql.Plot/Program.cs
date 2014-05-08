using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Configuration;
using Jhu.Spherical;

namespace Herschel.Sql.Plot
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            var cstr = ConfigurationManager.ConnectionStrings["Herschel"].ConnectionString;
            Region region;

            string verb = args[0];
            string sql = args[1];

            using (var cn = new SqlConnection(cstr))
            {
                cn.Open();

                using (var cmd = new SqlCommand(sql, cn))
                {
                    using (var dr = cmd.ExecuteReader())
                    {
                        int index = 0;
                        while (dr.Read())
                        {
                            index++;

                            var bytes = dr.GetSqlBytes(0);
                            region = Herschel.Lib.Util.GetRegion(bytes);

                            switch (verb)
                            {
                                case "arcs":
                                    PrintArcs(index, region);
                                    break;
                                case "outline":
                                    PrintOutline(index, region);
                                    break;
                                case "htm_in":
                                    PrintHtm(index, region, Jhu.Spherical.Htm.Markup.Inner);
                                    break;
                                case "htm_part":
                                    PrintHtm(index, region, Jhu.Spherical.Htm.Markup.Partial);
                                    break;
                                case "htm_out":
                                    PrintHtm(index, region, Jhu.Spherical.Htm.Markup.Outer);
                                    break;
                                case "ds9":
                                    PrintDS9(region);
                                    break;
                                default:
                                    throw new NotImplementedException();
                            }
                        }
                    }
                }
            }
        }

        static void PrintArcs(int index, Region region)
        {
            for (int ic = 0; ic < region.ConvexList.Count; ic++)
            {
                var c = region.ConvexList[ic];
                for (int i = 0; i < c.PatchList.Count; i++)
                {
                    for (int j = 0; j < c.PatchList[i].ArcList.Length; j++)
                    {
                        Console.WriteLine("{0}\t{1}\t{2}", c.PatchList[i].ArcList[j].Point1.RA, c.PatchList[i].ArcList[j].Point1.Dec, index);
                        Console.WriteLine("{0}\t{1}\t{2}", c.PatchList[i].ArcList[j].Point2.RA, c.PatchList[i].ArcList[j].Point2.Dec, index);
                    }

                    Console.WriteLine();
                }
            }
        }

        static void PrintOutline(int index, Region region)
        {
            var ol = region.GetOutline();

            foreach (var s in ol.PartList)
            {
                foreach (var a in s.ArcList)
                {
                    Console.WriteLine("{0}\t{1}\t{2}", a.Point1.RA, a.Point1.Dec, index);
                    Console.WriteLine("{0}\t{1}\t{2}", a.Point2.RA, a.Point2.Dec, index);
                    Console.WriteLine();
                }
            }
        }

        static void PrintHtm(int index, Region region, Jhu.Spherical.Htm.Markup markup)
        {
            var cb = new Jhu.Spherical.Htm.CoverBuilder(region);
            var cover = cb.Run();

            var hids = cover.GetTrixels(markup);
            foreach (var hid in hids)
            {
                Cartesian a, b, c;
                hid.GetTriangle(out a, out b, out c);

                Console.WriteLine("{0}\t{1}\t{2}", a.RA, a.Dec, index);
                Console.WriteLine("{0}\t{1}\t{2}", b.RA, b.Dec, index);
                Console.WriteLine("{0}\t{1}\t{2}", c.RA, c.Dec, index);
                Console.WriteLine("{0}\t{1}\t{2}", a.RA, a.Dec, index);
                Console.WriteLine();
            }
        }

        static void PrintDS9(Region region)
        {
            var ol = region.GetOutline();

            Console.WriteLine("ICRS");

            foreach (var s in ol.PartList)
            {
                foreach (var a in s.ArcList)
                {
                    Console.WriteLine("line {0:0.000000}d {1:0.000000}d {2:0.000000}d {3:0.000000}d # color = blue", a.Point1.RA, a.Point1.Dec, a.Point2.RA, a.Point2.Dec);
                }
            }
        }
    }
}
