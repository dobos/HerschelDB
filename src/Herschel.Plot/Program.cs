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

            //PlotRegions("SELECT region.Parse('CIRCLE J2000 10 10 0.91') AS region", true, false, false, "htmcover.emf", 6f, 5f, true);

            //PlotRegions("SELECT region FROM Observation WHERE inst = 1 AND obsID = 1342225536", false, false, false, "pacs_full.emf", 6f, 5f, true);
            //PlotRegions("SELECT region.GetConvexHull(region) AS region FROM Observation WHERE inst = 1 AND obsID = 1342225536", false, false, false, "pacs_chull.pdf", 3f, 2.5f, true);
            //PlotRegions("SELECT region FROM Observation WHERE inst = 1 AND obsID = 1342225536", false, false, true, "pacs_reduce.pdf", 3f, 2.5f, true);

            //PlotRegions("SELECT region FROM load.LegRegion WHERE inst = 1 AND obsID = 1342225536", false, false, false, "pacs_legs.emf", 6f, 5f, true);
            //PlotPoints("SELECT ra AS point_ra, dec AS point_dec FROM load.Pointing WHERE inst = 1 AND obsID = 1342225536 --AND finetime % 4 = 0", "pacs_pointing.emf", 6f, 5f);

            //PlotRegions("SELECT region FROM Observation WHERE inst = 2 AND obsID = 1342186861", false, false, false, "spire_full.emf", 6f, 5f, true);
            //PlotRegions("SELECT region.ConvexHull(region) AS region FROM Observation WHERE inst = 2 AND obsID = 1342186861", false, false, false, "spire_chull.emf", 6f, 5f, true);
            //PlotRegions("SELECT region FROM Observation WHERE inst = 2 AND obsID = 1342186861", false, false, true, "spire_reduce.emf", 6f, 5f, true);

            //PlotRegions("SELECT region FROM Observation WHERE inst = 2 AND obsID = 1342186861", false, false, false, "spire_full_small.pdf", 1.5f, 2.0f, false);
            //PlotRegions("SELECT region.GetConvexHull(region) AS region FROM Observation WHERE inst = 2 AND obsID = 1342186861", false, false, false, "spire_chull_small.pdf", 1.5f, 2.0f, false);
            //PlotRegions("SELECT region FROM Observation WHERE inst = 2 AND obsID = 1342186861", false, false, true, "spire_reduce_small.pdf", 1.5f, 2.0f, false);

            //PlotRegions("SELECT region FROM load.LegRegion WHERE inst = 2 AND obsID = 1342186861", false, false, false, "spire_legs.pdf");
            //PlotPoints("SELECT ra AS point_ra, dec AS point_dec FROM Pointing WHERE inst = 2 AND obsID = 1342186861 --AND finetime % 4 = 0", "spire_pointing.pdf");

            // SPIRE connect leg ends example


            //PlotPoints("SELECT ra AS point_ra, dec AS point_dec FROM load.Pointing WHERE inst = 2 AND obsID = 1342183681 --AND finetime % 4 = 0", "spire_pointing_2.emf", 6f, 5f);
            //PlotRegions("SELECT region FROM load.LegRegion WHERE inst = 2 AND obsID = 1342183681 AND legID % 2 = 1", false, false, false, "spire_legs_2.emf", 6f, 5f, true);
            //PlotRegions("SELECT region FROM load.LegRegion WHERE inst = 2 AND obsID = 1342183681", false, false, false, "spire_leg_ends_2.emf", 6f, 5f, true);
            //PlotRegions("SELECT region FROM Observation WHERE inst = 2 AND obsID = 1342183681", false, false, false, "spire_full_2.emf", 6f, 5f, true);



            /*PlotPacsRaster("pacs_raster_1.pdf", 1342212598, 3f, 2.5f);
            PlotPacsRaster("pacs_raster_2.pdf", 1342240160, 3f, 2.5f);*/

            // PACS spectro raster
            PlotRegions("SELECT region FROM Observation WHERE inst = 1 AND obsID = 1342212598", false, false, false, "pacs_spectro_raster.emf", 6f, 5f, true);
            //PlotRegions("SELECT region FROM Observation WHERE inst = 1 AND obsID = 1342191352", false, false, false, "pacs_spectro_map.emf", 6f, 5f, true);
            //PlotRegions("SELECT region FROM Observation WHERE inst = 2 AND obsID = 1342250523", false, false, false, "spire_spectro_map.emf", 6f, 5f, true);

            //PlotRegions("SELECT region FROM Observation WHERE inst = 8 AND obsID = 1342191700", false, false, false, "hifi_pointed.pdf", 3f, 2.5f, true);
            //PlotRegions("SELECT region FROM Observation WHERE inst = 8 AND obsID = 1342251563", false, false, false, "hifi_map.pdf", 3f, 2.5f, true);

            //PlotRegions("SELECT region FROM Observation WHERE inst = 8 AND obsID IN (1342201114, 1342262551)", false, false, false, "hifi_both.emf", 6f, 5f, true);

            //PlotRegions("SELECT region FROM Observation WHERE inst IN (1, 2, 8) AND obsID IN (1342225752, 1342204025, 1342253169)", false, false, false, "spectro_pointed_all.emf", 6f, 5f, true);


            // Small plots for BIDS proceedings

            //PlotRegions("SELECT region FROM load.LegRegion WHERE inst = 1 AND obsID = 1342225536", false, false, false, "bids_pacs_legs.pdf", 2.3f, 2f, true);
            //PlotPoints("SELECT ra AS point_ra, dec AS point_dec FROM load.Pointing WHERE inst = 1 AND obsID = 1342225536 --AND finetime % 4 = 0", "bids_pacs_pointing.pdf", 2.3f, 2f);
            //PlotRegions("SELECT region FROM Observation WHERE inst = 1 AND obsID = 1342225536", false, false, false, "bids_pacs_full.pdf", 2.3f, 2f, true);

            //PlotRegions("SELECT region FROM Observation WHERE inst = 2 AND obsID = 1342186861", false, false, false, "bids_spire_full.pdf", 2.3f, 2.0f, true);
            //PlotRegions("SELECT region.ConvexHull(region) AS region FROM Observation WHERE inst = 2 AND obsID = 1342186861", false, false, false, "bids_spire_chull.pdf", 2.3f, 2.0f, true);
            //PlotRegions("SELECT region FROM Observation WHERE inst = 2 AND obsID = 1342186861", false, false, true, "bids_spire_reduce.pdf", 2.3f, 2.0f, true);

            //PlotRegions("SELECT region FROM Observation WHERE inst IN (1, 2, 8) AND obsID IN (1342225752, 1342204025, 1342253169)", false, false, false, "bids_spectro_pointed_all.pdf", 3.3f, 3f, true);

            //PlotRegions("SELECT region FROM Observation WHERE inst = 2 AND obsID = 1342186861", true, false, false, "bids_spire_htm.pdf", 3.3f, 3f, true);
            //PlotRegions("SELECT region FROM Observation WHERE inst = 2 AND obsID = 1342250523", false, false, false, "bids_spire_spectro_map.pdf", 3.3f, 3f, true);
        }

        static void PlotRegions(string sql, bool htmcover, bool chull, bool reduce, string filename, float w, float h, bool borders)
        {
            var plot = InitPlot(w, h, borders);

            var csb = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["Herschel"].ConnectionString);

            using (var cn = new SqlConnection(csb.ConnectionString))
            {
                cn.Open();

                using (var cmd = new SqlCommand(sql, cn))
                {
                    AppendRegionsLayer(plot, cmd, htmcover, chull, reduce);
                }

                FinishPlot(plot, filename);
            }
        }

        static void PlotPoints(string sql, string filename, float w, float h)
        {
            var plot = InitPlot(w, h, true);
            AppendGridLayer(plot);

            var csb = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["Herschel"].ConnectionString);

            using (var cn = new SqlConnection(csb.ConnectionString))
            {
                cn.Open();

                using (var cmd = new SqlCommand(sql, cn))
                {
                    AppendPointsLayer(plot, cmd);
                }

                FinishPlot(plot, filename);
            }
        }

        static void PlotPacsRaster(string filename, long obsid, float w, float h)
        {
            var plot = InitPlot(w, h, true);

            var csb = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["Herschel"].ConnectionString);

            using (var cn = new SqlConnection(csb.ConnectionString))
            {
                cn.Open();

                using (var cmd = new SqlCommand("SELECT region FROM Herschel_3..Observation WHERE inst = 1 AND obsID = " + obsid.ToString(), cn))
                {
                    AppendRegionsLayer(plot, cmd, false, false, false);
                }

                using (var cmd = new SqlCommand("SELECT ra point_ra, dec point_dec FROM Herschel_3.load.PointingCluster WHERE inst = 1 AND obsID = " + obsid.ToString() + " AND isRotated = 0", cn))
                {
                    var ds = new SqlQueryDataSource(cmd);

                    var points = new PointsLayer()
                    {
                        DataSource = ds,
                        PointDataField = "point",
                        Figure = FigureType.Cross,
                        Size = new SizeF(2f, 2f)
                    };
                    points.Outline.Visible = true;
                    points.Outline.Pens = new Pen[] { Pens.Red };
                    points.Fill.Visible = false;

                    plot.Layers.Add(points);
                }

                plot.AutoZoomFactor = 1.2f;

                FinishPlot(plot, filename);
            }
        }


        static void AppendRegionsLayer(Jhu.Spherical.Visualizer.Plot plot, SqlCommand cmd, bool htmcover, bool chull, bool reduce)
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

            var outlinepen = new Pen(Brushes.Red, 2.0f)
            {
                StartCap = LineCap.Flat,
                EndCap = LineCap.Flat,
                LineJoin = System.Drawing.Drawing2D.LineJoin.Bevel
            };

            var outlines = new RegionsLayer()
            {
                DataSource = ds,
                RegionDataField = "region",
            };
            outlines.Outline.Pens = new Pen[] { outlinepen };
            //outlines.Outline.Pens = new Pen[] { Pens.Red, Pens.Green, Pens.Blue };
            outlines.Outline.PaletteSelection = PaletteSelection.Rotate;
            outlines.Reduce = reduce;


            outlines.ReduceEpsilon = regions.ReduceEpsilon = 50.0 / 3600 * Constant.Degree2Radian;

            var htminnerpen = new Pen(Brushes.Black, 2.0f)
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
                MaxDepth = 16,
            };
            htminner.Markup = Jhu.Spherical.Htm.Markup.Inner;
            htminner.Outline.Pens = new Pen[] { htminnerpen };
            htminner.Fill.Visible = false;

            var htmpartialpen = new Pen(Brushes.Blue, 2.0f)
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
                MaxDepth = 16,
            };
            htmpartial.Markup = Jhu.Spherical.Htm.Markup.Partial;
            htmpartial.Outline.Pens = new Pen[] { htmpartialpen };
            htmpartial.Fill.Visible = false;


            outlines.Fill.Visible = false;

            plot.Layers.Add(regions);

            AppendGridLayer(plot);

            plot.Layers.Add(outlines);

            if (htmcover)
            {
                plot.Layers.Add(htminner);
                plot.Layers.Add(htmpartial);
            }
        }



        static void AppendPointsLayerPoints(Jhu.Spherical.Visualizer.Plot plot, SqlCommand cmd)
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

            plot.Layers.Add(points);
        }

        static Jhu.Spherical.Visualizer.Plot InitPlot(float w, float h, bool borders)
        {
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
                Resolution = 1.0f
            };

            if (borders)
            {
                plot.Margins.Left = 32f;
                plot.Margins.Bottom = 32f;
                plot.Margins.Right = 10f;
                plot.Margins.Top = 1f;
            }
            else
            {
                plot.Margins.Left = 1f;
                plot.Margins.Right = 1f;
                plot.Margins.Top = 1f;
                plot.Margins.Bottom = 1f;
            }

            var b = new BorderLayer();
            b.Line.Pen = new Pen(Brushes.Black, 2.0f);

            plot.Layers.Add(b);

            return plot;
        }

        static void AppendGridLayer(Jhu.Spherical.Visualizer.Plot plot)
        {
            var grid = new GridLayer();
            grid.RaScale.Density = 200f;
            grid.DecScale.Density = 200f;
            grid.Line.Pen = Pens.LightGray;

            plot.Layers.Add(grid);
        }

        static void FinishPlot(Jhu.Spherical.Visualizer.Plot plot, string filename)
        {
            var font = new Font("Consolas", 14f);

            var axes = new AxesLayer();
            axes.X1Axis.Title.Text = "right ascension";
            axes.X1Axis.Title.Font = font;
            axes.X1Axis.Labels.Font = font;
            axes.X1Axis.Scale.Density = 200f;
            axes.X1Axis.Scale.DegreeFormat.DegreeWrapAroundStyle = DegreeWrapAroundStyle.ZeroTo360;
            axes.X2Axis.Scale.DegreeFormat.DegreeStyle = DegreeStyle.Decimal;
            axes.X2Axis.Labels.Visible = false;
            axes.X2Axis.Scale.DegreeFormat.DegreeWrapAroundStyle = DegreeWrapAroundStyle.ZeroTo360;
            axes.Y1Axis.Title.Text = "declination";
            axes.Y1Axis.Title.Font = font;
            axes.Y1Axis.Labels.Font = font;
            axes.Y1Axis.Scale.Density = 200f;
            axes.Y2Axis.Scale.DegreeFormat.DegreeStyle = DegreeStyle.Decimal;
            axes.Y2Axis.Labels.Visible = false;
            axes.X1Axis.Line.Pen = axes.X2Axis.Line.Pen = axes.Y1Axis.Line.Pen = axes.Y2Axis.Line.Pen = new Pen(Brushes.Black, 2.0f);

            plot.Layers.Add(axes);

            plot.Projection.InvertX = true;

            if (filename.EndsWith(".pdf"))
            {
                plot.RenderToPdf(filename);
            }
            else if (filename.EndsWith(".eps"))
            {
                plot.RenderToEps(filename);
            }
            else if (filename.EndsWith(".emf"))
            {
                plot.RenderToEmf(filename);
            }
            else
            {
                plot.RenderToBitmap(filename, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        static void AppendPointsLayer(Jhu.Spherical.Visualizer.Plot plot, SqlCommand cmd)
        {
            var ds = new SqlQueryDataSource(cmd);

            var points = new PointsLayer()
            {
                DataSource = ds,
                PointDataField = "point",
                Figure = FigureType.Dot,
                Size = new SizeF(1.2f, 1.2f)
            };
            points.Outline.Visible = false;
            points.Fill.Visible = true;
            points.Fill.Brushes = new Brush[] { Brushes.Red };

            plot.Layers.Add(points);
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
