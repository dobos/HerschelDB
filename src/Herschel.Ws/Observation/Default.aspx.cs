using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using Jhu.Spherical;
using Jhu.Spherical.Visualizer;
using Herschel.Lib;

namespace Herschel.Ws.Observation
{
    public partial class Default : PageBase
    {
        private enum RenderMode
        {
            Normal,
            SaveToPdf
        }

        private RenderMode renderMode = RenderMode.Normal;
        private Lib.ObservationSearch searchObject;

        protected Instrument SearchInstrument
        {
            get { return (Instrument)ViewState["SearchInstrument"]; }
            set { ViewState["SearchInstrument"] = value; }
        }

        protected ObservationSearchMethod SearchMethod
        {
            get { return (ObservationSearchMethod)ViewState["SearchMethod"]; }
            set { ViewState["SearchMethod"] = value; }
        }

        protected Cartesian SearchPoint
        {
            get { return (Cartesian)ViewState["SearchPoint"]; }
            set { ViewState["SearchPoint"] = value; }
        }

        protected Jhu.Spherical.Region SearchRegion
        {
            get { return (Jhu.Spherical.Region)ViewState["SearchRegion"]; }
            set { ViewState["SearchRegion"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            savePlotPdfLink.NavigateUrl = Page.ClientScript.GetPostBackClientHyperlink(savePlotPdf, null);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (observationList.Visible)
            {
                observationList.DataBind();
            }

            if (observationPlotPanel.Visible)
            {
                GeneratePlot();
            }

            switch(renderMode)
            {
                case RenderMode.SaveToPdf:
                    SavePlotPdf();
                    break;
            }
        }

        #region Search form

        protected void searchMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            pointTr.Visible = regionTr.Visible = false;

            ObservationSearchMethod method;
            Enum.TryParse<ObservationSearchMethod>(searchMethod.SelectedValue, out method);

            switch (method)
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

        protected void search_Click(object sender, EventArgs e)
        {
            // Instrument
            var instrument = Instrument.None;

            foreach (ListItem i in instrumentList.Items)
            {
                if (i.Selected)
                {
                    Lib.Instrument ii;

                    if (Enum.TryParse<Lib.Instrument>(i.Value, out ii))
                    {
                        instrument |= ii;
                    }
                }
            }

            SearchInstrument = instrument;

            // SearchMethod
            ObservationSearchMethod method;
            Enum.TryParse<ObservationSearchMethod>(searchMethod.SelectedValue, out method);

            SearchMethod = method;

            switch (method)
            {
                case ObservationSearchMethod.Point:
                    {
                        var parts = point.Text.Split(new char[] { ' ', '\t', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                        SearchPoint = new Cartesian(double.Parse(parts[0]), double.Parse(parts[1]));
                        break;
                    }
                case ObservationSearchMethod.Intersect:
                case ObservationSearchMethod.Cover:
                    {
                        SearchRegion = Jhu.Spherical.Region.Parse(region.Text);
                        break;
                    }
                default:
                    throw new NotImplementedException();
            }

            // Update panels
            observationListPanel.Visible = true;
            observationPlotPanel.Visible = false;
        }

        #endregion
        #region Search list

        protected void observationDataSource_ObjectCreating(object sender, ObjectDataSourceEventArgs e)
        {
            searchObject = new Lib.ObservationSearch();

            searchObject.SearchMethod = SearchMethod;

            switch (SearchMethod)
            {
                case ObservationSearchMethod.Point:
                    searchObject.Point = SearchPoint;
                    break;
                case ObservationSearchMethod.Intersect:
                case ObservationSearchMethod.Cover:
                    searchObject.Region = SearchRegion;
                    break;
                default:
                    throw new NotImplementedException();
            }

            e.ObjectInstance = searchObject;
        }

        protected void plot_Click(object sender, EventArgs e)
        {
            observationPlotPanel.Visible = true;
        }

        #endregion
        #region Plot

        protected void savePlotPdf_Click(object sender, EventArgs e)
        {
            renderMode = RenderMode.SaveToPdf;
        }

        protected void GeneratePlot()
        {
            // Query region of points
            Layer queryLayer = null;

            switch (searchObject.SearchMethod)
            {
                case ObservationSearchMethod.Point:
                    {
                        var qp = new PointsLayer();
                        qp.DataSource = new ListDataSource(searchObject.Point);
                        qp.Figure = FigureType.CrossHair;
                        qp.Size = new SizeF(15, 15);
                        qp.Fill.Visible = false;
                        qp.Outline.Pens = new[] { Pens.Red };
                        queryLayer = qp;
                    }
                    break;
                case ObservationSearchMethod.Intersect:
                case ObservationSearchMethod.Cover:
                    var qr = new RegionsLayer();
                    qr.DataSource = new ListDataSource(searchObject.Region);
                    qr.Fill.Visible = false;
                    qr.Outline.Pens = new[] { Pens.Red };
                    queryLayer = qr;
                    break;
                default:
                    throw new NotImplementedException();
            }

            // Find regions
            var s = new Lib.ObservationSearch();
            var ids = observationList.SelectedDataKeys.Select(id => long.Parse(id)).ToArray();
            var regions = new ListDataSource(searchObject.FindID(ids).Select(o => o.Region).ToArray());

            canvas.Plot.Projection = new OrthographicProjection();

            var grid = new GridLayer();
            grid.RaScale.Density = 100;
            grid.DecScale.Density = 100;
            canvas.Plot.Layers.Add(grid);

            canvas.Plot.Layers.Add(new BorderLayer());

            var r1 = new RegionsLayer();
            r1.DataSource = regions;
            r1.Outline.Visible = false;
            canvas.Plot.Layers.Add(r1);

            var r2 = new RegionsLayer();
            r2.DataSource = regions;
            r2.Fill.Visible = false;
            r2.Outline.Pens = new[] { Pens.Black };
            canvas.Plot.Layers.Add(r2);

            if (queryLayer != null)
            {
                canvas.Plot.Layers.Add(queryLayer);
            }

            canvas.Plot.Layers.Add(new AxesLayer());

            canvas.Plot.AutoRotate = true;
            canvas.Plot.AutoZoom = true;
        }

        private void SavePlotPdf()
        {
            Response.Clear();
            Response.AddHeader("content-disposition", string.Format("attachement; filename=\"{0}\"", "footprint.pdf"));
            Response.ContentType = "application/pdf";

            GeneratePlot();

            canvas.Plot.ImageSize = new SizeF(384, 288);

            canvas.Plot.RenderToPdf(Response.OutputStream);

            Response.End();
        }

        #endregion
    }
}