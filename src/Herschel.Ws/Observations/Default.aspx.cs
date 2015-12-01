using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using Jhu.Spherical;
using Herschel.Lib;

namespace Herschel.Ws.Observations
{
    public partial class Default : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void search_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                // Clear previous selection
                observationList.SelectedDataKeys.Clear();
                observationListPanel.Visible = true;
            }
            else
            {
                observationListPanel.Visible = false;
            }

            footprintPlotPanel.Visible = false;
        }

        protected void plot_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                footprintPlot.ObservationIDs = observationList.SelectedDataKeys.Select(id => ObservationID.Parse(id)).ToArray();
                footprintPlotPanel.Visible = true;
            }
            else
            {
                footprintPlotPanel.Visible = false;
            }
        }
    }
}