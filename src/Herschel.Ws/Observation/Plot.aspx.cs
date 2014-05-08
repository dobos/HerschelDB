using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Herschel.Ws.Observation
{
    public partial class Plot : PageBase
    {
        public static string GetUrl(IEnumerable<string> ids)
        {
            return String.Format("~/Observation/Plot.aspx?ids={0}", Util.QueryString.ToList(ids));
        }

        private long[] ids;

        protected void Page_Load(object sender, EventArgs e)
        {
            ids = Util.QueryString.FromList(Request.QueryString["ids"], long.Parse);

            image.ImageUrl = RegionPlot.GetUrl(ids, (int)image.Width.Value, (int)image.Height.Value, 0, 0);
        }
    }
}