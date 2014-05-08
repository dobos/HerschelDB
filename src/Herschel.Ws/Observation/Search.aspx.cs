using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Jhu.Spherical;
using Herschel.Lib;

namespace Herschel.Ws.Observation
{
    public partial class Search : PageBase
    {
        protected ObservationSearchMethod SearchMethod
        {
            get
            {
                ObservationSearchMethod method;
                Enum.TryParse<ObservationSearchMethod>(searchMethod.SelectedValue, out method);
                return method;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void observationDataSource_ObjectCreating(object sender, ObjectDataSourceEventArgs e)
        {
            var search = new Lib.ObservationSearch();

            foreach (ListItem i in instrument.Items)
            {
                if (i.Selected)
                {
                    Lib.Instrument inst;

                    if (Enum.TryParse<Lib.Instrument>(i.Value, out inst))
                    {
                        search.Instrument |= inst;
                    }
                }
            }

            switch (SearchMethod)
            {
                case ObservationSearchMethod.Point:
                    {
                        var parts = point.Text.Split(new char[] { ' ', '\t', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                        search.Point = new Cartesian(double.Parse(parts[0]), double.Parse(parts[1]));
                    }
                    break;
                case ObservationSearchMethod.Intersect:
                    {
                        search.Region = Region.Parse(region.Text);
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }

            e.ObjectInstance = search;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (observationList.Visible)
            {
                observationList.DataBind();
            }
        }

        protected void search_Click(object sender, EventArgs e)
        {
            switch (SearchMethod)
            {
                case ObservationSearchMethod.Point:
                    observationDataSource.SelectMethod = "FindEq";
                    break;
                case ObservationSearchMethod.Intersect:
                    observationDataSource.SelectMethod = "FindIntersect";
                    break;
                default:
                    throw new NotImplementedException();
            }

            observationListPanel.Visible = true;
        }

        protected void plot_Click(object sender, EventArgs e)
        {
            Response.Redirect(Plot.GetUrl(observationList.SelectedDataKeys));
        }

        protected void searchMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            pointTr.Visible = regionTr.Visible = false;
            observationListPanel.Visible = false;

            switch (SearchMethod)
            {
                case ObservationSearchMethod.Point:
                    pointTr.Visible = true;
                    break;
                case ObservationSearchMethod.Intersect:
                    regionTr.Visible = true;
                    break;
                case ObservationSearchMethod.Cover:
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}