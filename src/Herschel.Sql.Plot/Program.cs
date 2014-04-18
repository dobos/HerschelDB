using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Configuration;
using Spherical;

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
                        while (dr.Read())
                        {
                            var bytes = dr.GetSqlBytes(0);
                            region = Herschel.Lib.Util.GetRegion(bytes);

                            switch (verb)
                            {
                                case "arcs":
                                    PrintArcs(region);
                                    break;
                                case "outline":
                                    PrintOutline(region);
                                    break;
                                case "htm_in":
                                    PrintHtm(region, Spherical.Htm.Markup.Inner);
                                    break;
                                case "htm_part":
                                    PrintHtm(region, Spherical.Htm.Markup.Partial);
                                    break;
                                case "htm_out":
                                    PrintHtm(region, Spherical.Htm.Markup.Outer);
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

        static void PrintArcs(Region region)
        {
            for (int ic = 0; ic < region.ConvexList.Count; ic++)
            {
                var c = region.ConvexList[ic];
                for (int i = 0; i < c.PatchList.Count; i++)
                {
                    for (int j = 0; j < c.PatchList[i].ArcList.Length; j++)
                    {
                        Console.WriteLine("{0}\t{1}", c.PatchList[i].ArcList[j].Point1.RA, c.PatchList[i].ArcList[j].Point1.Dec);
                        Console.WriteLine("{0}\t{1}", c.PatchList[i].ArcList[j].Point2.RA, c.PatchList[i].ArcList[j].Point2.Dec);
                    }

                    Console.WriteLine();
                }
            }
        }

        static void PrintOutline(Region region)
        {
            var ol = new Outline(region.EnumPatches());

            foreach (Spherical.PatchPart s in ol.PartList)
            {
                foreach (Spherical.Arc a in s.ArcList)
                {
                    Console.WriteLine("{0}\t{1}", a.Point1.RA, a.Point1.Dec);
                    Console.WriteLine("{0}\t{1}", a.Point2.RA, a.Point2.Dec);
                    Console.WriteLine();
                }
            }
        }

        static void PrintHtm(Region region, Spherical.Htm.Markup markup)
        {
            var cover = Herschel.Lib.Htm.GetCover(region);

            var hids = cover.GetTrixels(markup);
            foreach (var hid in hids)
            {
                Cartesian a, b, c;
                Spherical.Htm.Trixel.ToTriangle(hid, out a, out b, out c);

                Console.WriteLine("{0}\t{1}", a.RA, a.Dec);
                Console.WriteLine("{0}\t{1}", b.RA, b.Dec);
                Console.WriteLine("{0}\t{1}", c.RA, c.Dec);
                Console.WriteLine("{0}\t{1}", a.RA, a.Dec);
                Console.WriteLine();
            }
        }

        static void PrintDS9(Region region)
        {
            var ol = new Outline(region.EnumPatches());

            Console.WriteLine("ICRS");

            foreach (Spherical.PatchPart s in ol.PartList)
            {
                foreach (Spherical.Arc a in s.ArcList)
                {
                    Console.WriteLine("line {0:0.000000}d {1:0.000000}d {2:0.000000}d {3:0.000000}d # color = blue", a.Point1.RA, a.Point1.Dec, a.Point2.RA, a.Point2.Dec);
                }
            }
        }
    }
}
