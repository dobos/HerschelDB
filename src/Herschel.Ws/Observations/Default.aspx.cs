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
        static Brush[] HerschelBrushes;

        static Default()
        {
            HerschelBrushes = new Brush[(int)Instrument.Hifi + 1];

            for (int i = 0; i < HerschelBrushes.Length; i++)
            {
                HerschelBrushes[i] = Brushes.White;
            }

            HerschelBrushes[0] =
                new SolidBrush(Color.FromArgb(64, Color.Yellow));

            HerschelBrushes[(int)Instrument.Pacs] =
                new SolidBrush(Color.FromArgb(64, Color.Blue));

            HerschelBrushes[(int)Instrument.Spire] =
                new SolidBrush(Color.FromArgb(64, Color.Red));

            HerschelBrushes[(int)Instrument.PacsSpireParallel] =
                new SolidBrush(Color.FromArgb(64, Color.Orange));

            HerschelBrushes[(int)Instrument.Hifi] =
                new SolidBrush(Color.FromArgb(64, Color.Green));

        }

        private enum RenderMode
        {
            Normal,
            SaveToPdf
        }

        private RenderMode renderMode = RenderMode.Normal;

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
        
        protected void search_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
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
            e.ObjectInstance = searchForm.GetSearchObject();
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
            var searchObject = searchForm.GetSearchObject();

            switch (searchObject.SearchMethod)
            {
                case ObservationSearchMethod.ID:
                    break;
                case ObservationSearchMethod.Point:
                    {
                        var qp = new PointsLayer();
                        qp.DataSource = new ObjectListDataSource(new Cartesian[] { searchObject.Point });
                        qp.Figure = FigureType.CrossHair;
                        qp.Size = new SizeF(15, 15);
                        qp.Fill.Visible = false;
                        qp.Outline.Pens = new[] { Pens.Red };
                        queryLayer = qp;
                    }
                    break;
                case ObservationSearchMethod.Cone:
                case ObservationSearchMethod.Intersect:
                case ObservationSearchMethod.Cover:
                    var qr = new RegionsLayer();
                    qr.DataSource = new ObjectListDataSource(new Jhu.Spherical.Region[] { searchObject.Region });
                    qr.Fill.Visible = false;
                    qr.Outline.Pens = new[] { Pens.Red };
                    queryLayer = qr;
                    break;
                default:
                    throw new NotImplementedException();
            }

            // Find regions
            var s = new Lib.ObservationSearch();
            var ids = observationList.SelectedDataKeys.Select(id => ObservationID.Parse(id)).ToArray();
            var observations = new List<Observation>(searchObject.FindID(ids));

            // Find crossing SSOs
            var ssos = new List<Sso>();

            foreach (var obs in observations)
            {
                var ss = new Lib.SsoSearch();
                ss.ObservationID = new ObservationID()
                {
                    Instrument = obs.Instrument,
                    ID = obs.ObsID,
                };

                ssos.AddRange(ss.Find());
            }

            var ssods = new ObjectListDataSource(ssos);

            // Apply region transformations
            double epsilon = 0;
            if (plotReduce.Checked)
            {
                epsilon = double.Parse(plotReduceEpsilon.Text) / 3600.0 * Constant.Degree2Radian;
            }

            if (plotConvexHull.Checked)
            {
                for (int i = 0; i < observations.Count; i++)
                {
                    observations[i].Region = observations[i].Region.Outline.GetConvexHull();
                }
            }

            var regionds = new ObjectListDataSource(observations);

            // Create plot
            canvas.Plot.Projection = new OrthographicProjection();
            canvas.Plot.Projection.InvertX = true;

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
                r1.RegionDataField = "Region";
                r1.Fill.Brushes = HerschelBrushes;
                r1.Fill.PaletteSelection = PaletteSelection.Field;
                r1.Fill.Field = "Instrument";
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
                r2.RegionDataField = "Region";
                r2.Fill.Visible = false;
                r2.Outline.Pens = new[] { Pens.Black };
                canvas.Plot.Layers.Add(r2);

                if (plotReduce.Checked)
                {
                    r2.Reduce = true;
                    r2.ReduceEpsilon = epsilon;
                }
            }

            // SSO
            if (plotSsos.Checked)
            {
                var ssoal = new ArcsLayer();
                ssoal.DataSource = ssods;
                ssoal.ArcDataField = "Trajectory";
                ssoal.Outline.Pens = new[] { Pens.Red };
                canvas.Plot.Layers.Add(ssoal);
            
                var ssopl = new PointsLayer();
                ssopl.DataSource = ssods;
                ssopl.PointDataField = "Position";
                ssopl.Size = new SizeF(3, 3);
                ssopl.Figure = FigureType.Circle;
                ssopl.Fill.Brushes = new[] { Brushes.Red };
                ssopl.Outline.Pens = new[] { Pens.Red };
                canvas.Plot.Layers.Add(ssopl);
            }

            // Query

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