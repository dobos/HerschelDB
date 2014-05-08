using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Drawing.Imaging;
using Herschel.Lib;
using Jhu.Spherical.Visualizer;

namespace Herschel.Ws
{
    public partial class RegionPlot : System.Web.UI.Page
    {
        public static string GetUrl(long[] ids, int width, int height, double ra, double dec)
        {
            return String.Format(
                "~/RegionPlot.aspx?ids={0}&width={1}&height={2}&ra={3}&dec={4}",
                Util.QueryString.ToList(ids),
                width, height, ra, dec);
        }

        private int width;
        private int height;
        private long[] ids;

        protected void Page_Load(object sender, EventArgs e)
        {
            ids = Util.QueryString.FromList(Request.QueryString["ids"], long.Parse);
            width = int.Parse(Request.QueryString["width"]);
            height = int.Parse(Request.QueryString["height"]);

            var s = new Lib.ObservationSearch();

            var bmp = new Bitmap(width, height);
            var unit = GraphicsUnit.Pixel;
            var bounds = bmp.GetBounds(ref unit);

            using (var g = Graphics.FromImage(bmp))
            {
                var proj = new Jhu.Spherical.Visualizer.Projectors.Stereographic(bounds, new Jhu.Spherical.Cartesian(0, 0))
                {
                    AutoZoom = true,
                };

                var p = new Plot(g, proj, bounds, ColorScheme.Lcd)
                {
                    PlotGrid = false,
                    PlotRegionOutline = true,
                };

                var regions = new List<Jhu.Spherical.Region>(s.FindID(ids).Select(o => o.Region));

                p.Draw(regions);
            }

            Response.ContentType = "image/png";
            bmp.Save(Response.OutputStream, ImageFormat.Png);

            Response.End();
        }
    }
}