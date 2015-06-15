using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Configuration;
using System.IO;
using Jhu.Spherical;
using Jhu.Spherical.Visualizer;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Herschel.Plot
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            //PlotRegions("SELECT region.Parse('CIRCLE J2000 10 10 0.91') AS region", true, "htmcover.pdf");

            //PlotRegions("SELECT region FROM Observation WHERE inst = 1 AND obsID = 1342225536", false, "pacs_full.pdf");
            //PlotRegions("SELECT region.GetConvexHull(region) AS region FROM Observation WHERE inst = 1 AND obsID = 1342225536", false, false, false, "pacs_chull.pdf");
            //PlotRegions("SELECT region FROM Observation WHERE inst = 1 AND obsID = 1342225536", false, false, true, "pacs_reduce.pdf");

            //PlotRegions("SELECT region FROM load.LegRegion WHERE inst = 1 AND obsID = 1342225536", false, "pacs_legs.pdf");
            //PlotPoints("SELECT ra AS point_ra, dec AS point_dec FROM Pointing WHERE inst = 1 AND obsID = 1342225536 --AND finetime % 4 = 0", "pacs_pointing.pdf");

            PlotRegions("SELECT region FROM Observation WHERE inst = 2 AND obsID = 1342186861", false, false, false, "spire_full.pdf", 4.5f, 3.5f, true);
            PlotRegions("SELECT region.GetConvexHull(region) AS region FROM Observation WHERE inst = 2 AND obsID = 1342186861", false, false, false, "spire_chull.pdf", 4.5f, 3.5f, true);
            PlotRegions("SELECT region FROM Observation WHERE inst = 2 AND obsID = 1342186861", false, false, true, "spire_reduce.pdf", 4.5f, 3.5f, true);

            PlotRegions("SELECT region FROM Observation WHERE inst = 2 AND obsID = 1342186861", false, false, false, "spire_full_small.pdf", 1.5f, 2.0f, false);
            PlotRegions("SELECT region.GetConvexHull(region) AS region FROM Observation WHERE inst = 2 AND obsID = 1342186861", false, false, false, "spire_chull_small.pdf", 1.5f, 2.0f, false);
            PlotRegions("SELECT region FROM Observation WHERE inst = 2 AND obsID = 1342186861", false, false, true, "spire_reduce_small.pdf", 1.5f, 2.0f, false);

            //PlotRegions("SELECT region FROM load.LegRegion WHERE inst = 2 AND obsID = 1342186861", false, false, false, "spire_legs.pdf");
            //PlotPoints("SELECT ra AS point_ra, dec AS point_dec FROM Pointing WHERE inst = 2 AND obsID = 1342186861 --AND finetime % 4 = 0", "spire_pointing.pdf");
        }

        static void PlotRegions(string sql, bool htmcover, bool chull, bool reduce, string filename, float w, float h, bool borders)
        {
            var csb = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["Herschel"].ConnectionString);

            using (var cn = new SqlConnection(csb.ConnectionString))
            {
                cn.Open();

                using (var cmd = new SqlCommand(sql, cn))
                {
                    PlotRegions(cmd, htmcover, chull, reduce, filename, w, h, borders);
                }
            }
        }

        static void PlotPoints(string sql, string filename)
        {
            var csb = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["Herschel"].ConnectionString);

            using (var cn = new SqlConnection(csb.ConnectionString))
            {
                cn.Open();

                using (var cmd = new SqlCommand(sql, cn))
                {
                    PlotPoints(cmd, filename);
                }
            }
        }


        static void PlotRegions(SqlCommand cmd, bool htmcover, bool chull, bool reduce, string filename, float w, float h, bool borders)
        {
            var ds = new SqlQueryDataSource(cmd);

            var regions = new RegionsLayer()
            {
                DataSource = ds,
                RegionDataField = "region",
            };
            regions.Outline.Visible = false;
            regions.Fill.Brushes = new Brush[] { Brushes.LightYellow };
            regions.Reduce = reduce;

            var outlines = new RegionsLayer()
            {
                DataSource = ds,
                RegionDataField = "region",
            };
            outlines.Outline.Pens = new Pen[] { Pens.Red };
            outlines.Reduce = reduce;


            outlines.ReduceEpsilon = regions.ReduceEpsilon = 50.0 / 3600 * Constant.Degree2Radian;

            var htminnerpen = new Pen(Brushes.Black, 1.0f)
            {
                StartCap = LineCap.Flat,
                EndCap = LineCap.Flat,
                LineJoin = System.Drawing.Drawing2D.LineJoin.Bevel
            };

            var htminner = new HtmCoversLayer()
            {
                DataSource = ds,
                RegionDataField = "region",
                MinDepth = 3,
                MaxDepth = 15,
            };
            htminner.Markup = Jhu.Spherical.Htm.Markup.Inner;
            htminner.Outline.Pens = new Pen[] { htminnerpen };
            htminner.Fill.Visible = false;

            var htmpartialpen = new Pen(Brushes.Blue, 1.0f)
            {
                StartCap = LineCap.Flat,
                EndCap = LineCap.Flat,
                LineJoin = System.Drawing.Drawing2D.LineJoin.Bevel
            };

            var htmpartial = new HtmCoversLayer()
            {
                DataSource = ds,
                RegionDataField = "region",
                MinDepth = 3,
                MaxDepth = 15,
            };
            htmpartial.Markup = Jhu.Spherical.Htm.Markup.Partial;
            htmpartial.Outline.Pens = new Pen[] { htmpartialpen };
            htmpartial.Fill.Visible = false;


            outlines.Fill.Visible = false;

            w = (float)(w * 96);
            h = (float)(h * 96);

            var plot = new Jhu.Spherical.Visualizer.Plot()
            {
                AutoRotate = true,
                AutoZoom = true,
                AutoScale = true,
                Width = w,
                Height = h,
                ImageSize = new System.Drawing.SizeF(w, h),
                Projection = new StereographicProjection(),
            };

            if (borders)
            {
                plot.Margins.Left = 48f;
                plot.Margins.Bottom = 48f;
            }
            else
            {
                plot.Margins.Left = 1f;
                plot.Margins.Right = 1f;
                plot.Margins.Top = 1f;
                plot.Margins.Bottom = 1f;
            }

            var font = new Font("Consolas", 7.5f);

            var axes = new AxesLayer();
            axes.X1Axis.Title.Text = "right ascension (deg)";
            axes.X1Axis.Title.Font = font;
            axes.X1Axis.Labels.Font = font;
            axes.X2Axis.Scale.DegreeFormat.DegreeStyle = DegreeStyle.Decimal;
            axes.X2Axis.Labels.Visible = false;
            axes.Y1Axis.Title.Text = "declination (deg)";
            axes.Y1Axis.Title.Font = font;
            axes.Y1Axis.Labels.Font = font;
            axes.Y2Axis.Scale.DegreeFormat.DegreeStyle = DegreeStyle.Decimal;
            axes.Y2Axis.Labels.Visible = false;

            axes.X1Axis.Labels.Visible = borders;
            axes.Y1Axis.Labels.Visible = borders;
            axes.X1Axis.Title.Visible = borders;
            axes.Y1Axis.Title.Visible = borders;

            var grid = new GridLayer();
            grid.Line.Pen = Pens.LightGray;

            plot.Layers.Add(new BorderLayer());
            plot.Layers.Add(regions);

            plot.Layers.Add(grid);

            plot.Layers.Add(outlines);

            if (htmcover)
            {
                plot.Layers.Add(htminner);
                plot.Layers.Add(htmpartial);
            }

            plot.Layers.Add(axes);

            plot.Projection.InvertX = true;

            plot.RenderToPdf(filename);
        }



        static void PlotPoints(SqlCommand cmd, string filename)
        {
            var ds = new SqlQueryDataSource(cmd);

            var points = new PointsLayer()
            {
                DataSource = ds,
                PointDataField = "point",
                Figure = FigureType.Dot,
                Size = new SizeF(0.3f, 0.3f)
            };
            points.Outline.Visible = false;
            points.Fill.Visible = true;
            points.Fill.Brushes = new Brush[] { Brushes.Red };

            float w = (float)(4.5 * 96);
            float h = (float)(3.5 * 96);

            var plot = new Jhu.Spherical.Visualizer.Plot()
            {
                AutoRotate = true,
                AutoZoom = true,
                AutoScale = true,
                Width = w,
                Height = h,
                ImageSize = new System.Drawing.SizeF(w, h),
                Projection = new StereographicProjection(),
            };

            plot.Margins.Left = 48f;
            plot.Margins.Bottom = 48f;

            var font = new Font("Consolas", 7.5f);

            var axes = new AxesLayer();
            axes.X1Axis.Title.Text = "right ascension (deg)";
            axes.X1Axis.Title.Font = font;
            axes.X1Axis.Labels.Font = font;
            axes.X2Axis.Scale.DegreeFormat.DegreeStyle = DegreeStyle.Decimal;
            axes.X2Axis.Labels.Visible = false;
            axes.Y1Axis.Title.Text = "declination (deg)";
            axes.Y1Axis.Title.Font = font;
            axes.Y1Axis.Labels.Font = font;
            axes.Y2Axis.Scale.DegreeFormat.DegreeStyle = DegreeStyle.Decimal;
            axes.Y2Axis.Labels.Visible = false;

            var grid = new GridLayer();
            grid.Line.Pen = Pens.LightGray;

            plot.Layers.Add(new BorderLayer());
            plot.Layers.Add(grid);
            plot.Layers.Add(points);

            plot.Layers.Add(axes);

            plot.Projection.InvertX = true;

            plot.RenderToPdf(filename);
        }

        /*static void Main(string[] args)
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
                            region = Region.FromSqlBytes(bytes);

                            switch (verb)
                            {
                                case "binary":
                                    WriteBinary(index, region);
                                    break;
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

        static void WriteBinary(int index, Region region)
        {
            using (var rw = new Jhu.Spherical.IO.RegionWriter(Console.OpenStandardOutput()))
            {
                rw.Write(region);
            }
        }

        static void PrintArcs(int index, Region region)
        {
            for (int ic = 0; ic < region.ConvexList.Count; ic++)
            {
                var c = region.ConvexList[ic];
                for (int i = 0; i < c.PatchList.Count; i++)
                {
                    for (int j = 0; j < c.PatchList[i].ArcList.Count; j++)
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
            var ol = region.Outline;

            foreach (var loop in ol.LoopList)
            {
                foreach (var arc in loop.ArcList)
                {
                    Console.WriteLine("{0}\t{1}\t{2}", arc.Point1.RA, arc.Point1.Dec, index);
                    Console.WriteLine("{0}\t{1}\t{2}", arc.Point2.RA, arc.Point2.Dec, index);
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
            var ol = region.Outline;

            Console.WriteLine("ICRS");

            foreach (var loop in ol.LoopList)
            {
                foreach (var arc in loop.ArcList)
                {
                    Console.WriteLine("line {0:0.000000}d {1:0.000000}d {2:0.000000}d {3:0.000000}d # color = blue", arc.Point1.RA, arc.Point1.Dec, arc.Point2.RA, arc.Point2.Dec);
                }
            }
        }*/
    }
}
