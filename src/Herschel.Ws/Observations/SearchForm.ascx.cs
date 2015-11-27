using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using Jhu.Spherical;
using Jhu.Spherical.Visualizer;
using Herschel.Lib;

namespace Herschel.Ws.Observations
{
    public partial class SearchForm : System.Web.UI.UserControl
    {
        #region Private members

        private double ra;
        private double dec;

        #endregion
        #region Properties

        protected InstrumentModeFilter[] InstrumentModeFilters
        {
            get { return (InstrumentModeFilter[])ViewState["InstrumentModeFilters"]; }
            set { ViewState["InstrumentModeFilters"] = value; }
        }

        protected ObservationSearchMethod SearchMethod
        {
            get { return (ObservationSearchMethod)ViewState["SearchMethod"]; }
            set { ViewState["SearchMethod"] = value; }
        }

        protected FineTime FineTimeStart
        {
            get { return (FineTime)ViewState["FineTimeStart"]; }
            set { ViewState["FineTimeStart"] = value; }
        }

        protected FineTime FineTimeEnd
        {
            get { return (FineTime)ViewState["FineTimeEnd"]; }
            set { ViewState["FineTimeEnd"] = value; }
        }

        protected Cartesian Point
        {
            get { return (Cartesian)ViewState["Point"]; }
            set { ViewState["Point"] = value; }
        }

        protected double Radius
        {
            get { return (double)ViewState["Radius"]; }
            set { ViewState["Radius"] = value; }
        }

        protected Jhu.Spherical.Region Region
        {
            get { return (Jhu.Spherical.Region)ViewState["Region"]; }
            set { ViewState["Region"] = value; }
        }

        protected long[] IDList
        {
            get { return (long[])ViewState["IDList"]; }
            set { ViewState["IDList"] = value; }
        }

        #endregion
        #region Event handlers

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void searchMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            pointTr.Visible = radiusTr.Visible = regionTr.Visible = idlistTr.Visible = false;

            ObservationSearchMethod method;
            Enum.TryParse<ObservationSearchMethod>(searchMethod.SelectedValue, out method);

            switch (method)
            {
                case ObservationSearchMethod.ID:
                    idlistTr.Visible = true;
                    break;
                case ObservationSearchMethod.Point:
                    pointTr.Visible = true;
                    break;
                case ObservationSearchMethod.Cone:
                    pointTr.Visible = true;
                    radiusTr.Visible = true;
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

        protected void idlistFormatValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                if (idlist.Visible)
                {
                    ParseSearchIdList();
                }
                args.IsValid = true;
            }
            catch (Exception)
            {
                args.IsValid = false;
            }
        }

        #endregion

        protected Jhu.Spherical.Region ParseSearchRegion()
        {
            return Jhu.Spherical.Region.Parse(region.Text);
        }

        protected long[] ParseSearchIdList()
        {
            var parts = idlist.Text.Split(new char[] { ' ', '\t', '\r', '\n', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

            var ids = new long[parts.Length];
            for (int i = 0; i < ids.Length; i++)
            {
                ids[i] = Int64.Parse(parts[i]);
            }

            return ids;
        }

        private void SaveInstrumentModeFilters()
        {
            var filters = new List<InstrumentModeFilter>();

            // HIFI

            if (hifiSinglePoint.Checked)
            {
                filters.Add(InstrumentModeFilter.HifiSinglePoint);
            }

            // PACS

            if (pacsPhotometry.Checked)
            {
                filters.Add(InstrumentModeFilter.PacsPhotometry);
            }

            if (pacsLineSpec.Checked)
            {
                filters.Add(InstrumentModeFilter.PacsLineSpec);
            }

            if (pacsRangeSpec.Checked)
            {
                filters.Add(InstrumentModeFilter.PacsRangeSpec);
            }

            // SPIRE

            // Parallel

            InstrumentModeFilters = filters.ToArray();
        }

        public void SaveForm()
        {
            SaveInstrumentModeFilters();

            // Fine time interval
            if (fineTimeStart.Text.Trim() != String.Empty)
            {
                FineTimeStart = FineTime.Parse(fineTimeStart.Text);
            }
            else
            {
                FineTimeStart = FineTime.Undefined;
            }

            if (fineTimeEnd.Text.Trim() != String.Empty)
            {
                FineTimeEnd = FineTime.Parse(fineTimeEnd.Text);
            }
            else
            {
                FineTimeEnd = FineTime.Undefined;
            }

            // SearchMethod
            ObservationSearchMethod method;
            Enum.TryParse<ObservationSearchMethod>(searchMethod.SelectedValue, out method);

            SearchMethod = method;

            switch (method)
            {
                case ObservationSearchMethod.ID:
                    IDList = ParseSearchIdList();
                    break;
                case ObservationSearchMethod.Point:
                    Point = new Cartesian(ra, dec);
                    break;
                case ObservationSearchMethod.Cone:
                    Point = new Cartesian(ra, dec);
                    Radius = Double.Parse(radius.Text);
                    break;
                case ObservationSearchMethod.Intersect:
                case ObservationSearchMethod.Cover:
                    Region = ParseSearchRegion();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public Lib.ObservationSearch GetSearchObject()
        {
            var searchObject = new Lib.ObservationSearch();

            searchObject.InstrumentModeFilters = InstrumentModeFilters;
            searchObject.FineTimeStart = FineTimeStart;
            searchObject.FineTimeEnd = FineTimeEnd;
            searchObject.SearchMethod = SearchMethod;

            switch (SearchMethod)
            {
                case ObservationSearchMethod.ID:
                    searchObject.ObservationID = IDList;
                    break;
                case ObservationSearchMethod.Point:
                    searchObject.Point = Point;
                    break;
                case ObservationSearchMethod.Cone:
                    searchObject.Point = Point;
                    searchObject.Radius = Radius;
                    break;
                case ObservationSearchMethod.Intersect:
                case ObservationSearchMethod.Cover:
                    searchObject.Region = Region;
                    break;
                default:
                    throw new NotImplementedException();
            }

            return searchObject;
        }
    }
}