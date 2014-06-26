using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Globalization;
using Jhu.Spherical;
using Jhu.Spherical.Visualizer;
using Herschel.Lib;

namespace Herschel.Ws.Observations
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
        
        private double ra;
        private double dec;

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

        protected long SearchFineTimeStart
        {
            get { return (long)ViewState["SearchFineTimeStart"]; }
            set { ViewState["SearchFineTimeStart"] = value; }
        }

        protected long SearchFineTimeEnd
        {
            get { return (long)ViewState["SearchFineTimeEnd"]; }
            set { ViewState["SearchFineTimeEnd"] = value; }
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

            switch (renderMode)
            {
                case RenderMode.SaveToPdf:
                    SavePlotPdf();
                    break;
            }
        }

        #region Search form

        protected Jhu.Spherical.Region ParseSearchRegion()
        {
            return Jhu.Spherical.Region.Parse(region.Text);
        }

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

        protected void pointFormatValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (point.Visible)
            {
                if (Util.Astro.TryParseCoordinates(point.Text, out ra, out dec))
                {
                    args.IsValid = true;
                    return;
                }

                if (Util.Astro.TryResolveObject(point.Text, out ra, out dec))
                {
                    resolvedTr.Visible = true;
                    point.Text = String.Format(CultureInfo.InvariantCulture, "{0:0.0000000}, {1:0.0000000}", ra, dec);
                    args.IsValid = true;
                    return;
                }

                args.IsValid = false;
            }
            else
            {
                args.IsValid = true;
            }
        }

        protected void regionFormatValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                if (region.Visible)
                {
                    ParseSearchRegion();
                }
                args.IsValid = true;
            }
            catch (Exception)
            {
                args.IsValid = false;
            }
        }

        protected void search_Click(object sender, EventArgs e)
        {
            Validate();

            if (IsValid)
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
                        SearchPoint = new Cartesian(ra, dec);
                        break;
                    case ObservationSearchMethod.Intersect:
                    case ObservationSearchMethod.Cover:
                        SearchRegion = ParseSearchRegion();
                        break;
                    default:
                        throw new NotImplementedException();
                }

                // Clear previous selection
                observationList.SelectedDataKeys.Clear();
            }

            // Update panels
            observationListPanel.Visible = IsValid;
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

        protected void observationListValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = observationList.SelectedDataKeys.Count > 0;
        }

        protected void plot_Click(object sender, EventArgs e)
        {
            Validate();

            observationPlotPanel.Visible = IsValid;
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
            var regions = new List<Jhu.Spherical.Region>(searchObject.FindID(ids).Select(o => o.Region));

            // Apply region transformations
            double epsilon = 0;
            if (plotReduce.Checked)
            {
                epsilon = double.Parse(plotReduceEpsilon.Text) / 3600.0 * Constant.Degree2Radian;
            }

            if (plotConvexHull.Checked)
            {
                for (int i = 0; i < regions.Count; i++)
                {
                    regions[i] = regions[i].Outline.GetConvexHull();
                }
            }

            var regionds = new ListDataSource(regions);

            // Create plot
            canvas.Plot.Projection = new OrthographicProjection();

            var grid = new GridLayer();
            grid.RaScale.Density = 100;
            grid.DecScale.Density = 100;

            if (plotGrid.Checked)
            {
                canvas.Plot.Layers.Add(grid);
                canvas.Plot.Layers.Add(new BorderLayer());
            }

            if (plotFill.Checked)
            {
                var r1 = new RegionsLayer();
                r1.DataSource = regionds;
                r1.Outline.Visible = false;
                canvas.Plot.Layers.Add(r1);

                if (plotReduce.Checked)
                {
                    r1.Reduce = true;
                    r1.ReduceEpsilon = epsilon;
                }
            }

            if (plotOutline.Checked)
            {
                var r2 = new RegionsLayer();
                r2.DataSource = regionds;
                r2.Fill.Visible = false;
                r2.Outline.Pens = new[] { Pens.Black };
                canvas.Plot.Layers.Add(r2);

                if (plotReduce.Checked)
                {
                    r2.Reduce = true;
                    r2.ReduceEpsilon = epsilon;
                }
            }

            if (queryLayer != null && plotQuery.Checked)
            {
                canvas.Plot.Layers.Add(queryLayer);
            }

            var axes = new AxesLayer();
            canvas.Plot.Layers.Add(axes);

            switch (plotDegreeStyle.SelectedValue)
            {
                case "Decimal":
                    grid.RaScale.DegreeFormat.DegreeStyle = DegreeStyle.Decimal;
                    grid.DecScale.DegreeFormat.DegreeStyle = DegreeStyle.Decimal;
                    axes.X1Axis.Scale.DegreeFormat.DegreeStyle = DegreeStyle.Decimal;
                    axes.X2Axis.Scale.DegreeFormat.DegreeStyle = DegreeStyle.Decimal;
                    axes.Y1Axis.Scale.DegreeFormat.DegreeStyle = DegreeStyle.Decimal;
                    axes.Y2Axis.Scale.DegreeFormat.DegreeStyle = DegreeStyle.Decimal;
                    break;
                case "Sexagesimal":
                    grid.RaScale.DegreeFormat.DegreeStyle = DegreeStyle.Hours;
                    grid.DecScale.DegreeFormat.DegreeStyle = DegreeStyle.Symbols;
                    axes.X1Axis.Scale.DegreeFormat.DegreeStyle = DegreeStyle.Hours;
                    axes.X2Axis.Scale.DegreeFormat.DegreeStyle = DegreeStyle.Hours;
                    axes.Y1Axis.Scale.DegreeFormat.DegreeStyle = DegreeStyle.Symbols;
                    axes.Y2Axis.Scale.DegreeFormat.DegreeStyle = DegreeStyle.Symbols;
                    break;
                default:
                    throw new NotImplementedException();
            }

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