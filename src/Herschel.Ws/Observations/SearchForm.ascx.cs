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

        protected Jhu.Spherical.Region Region
        {
            get { return (Jhu.Spherical.Region)ViewState["Region"]; }
            set { ViewState["Region"] = value; }
        }

        protected Cartesian Point
        {
            get { return (Cartesian)ViewState["Point"]; }
            set { ViewState["Point"] = value; }
        }

        protected double Radius
        {
            get
            {
                double r;

                if (double.TryParse(radius.Text, out r))
                {
                    return r;
                }
                else if (Util.Astro.TryParseDms(radius.Text, out r))
                {
                    return r * 60;       // arc min
                }
                else
                {
                    throw new FormatException();
                }
            }
        }

        protected long[] IDList
        {
            get { return (long[])ViewState["IDList"]; }
            set { ViewState["IDList"] = value; }
        }

        protected FineTime FineTimeStart
        {
            get
            {
                if (String.IsNullOrWhiteSpace(timeStart.Text))
                {
                    return FineTime.Undefined;
                }
                else
                {
                    FineTime ft;
                    FineTime.TryParse(timeStart.Text, out ft);
                    return ft;
                }
            }
        }

        protected FineTime FineTimeEnd
        {
            get
            {
                if (String.IsNullOrWhiteSpace(timeEnd.Text))
                {
                    return FineTime.Undefined;
                }
                else
                {
                    FineTime ft;
                    FineTime.TryParse(timeEnd.Text, out ft);
                    return ft;
                }
            }
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
                case ObservationSearchMethod.Cover:
                    regionTr.Visible = true;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        protected void pointFormatValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            double ra, dec;

            if (point.Visible)
            {
                if (Util.Astro.TryParseCoordinates(point.Text, out ra, out dec))
                {
                    args.IsValid = true;
                    return;
                }

                if (Util.Astro.TryResolveObject(point.Text, out ra, out dec))
                {
                    resolved.Visible = true;
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

            if (hifiMapping.Checked)
            {
                filters.Add(InstrumentModeFilter.HifiMapping);
            }

            if (hifiSpectralScan.Checked)
            {
                filters.Add(InstrumentModeFilter.HifiSpectralScan);
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

            if (spirePhotometry.Checked)
            {
                filters.Add(InstrumentModeFilter.SpirePhotometry);
            }

            if (spireSpectroscopy.Checked)
            {
                filters.Add(InstrumentModeFilter.SpireSpectroscopy);
            }

            // Parallel

            if (parallelPhotometry.Checked)
            {
                filters.Add(InstrumentModeFilter.ParallelPhotometry);
            }

            InstrumentModeFilters = filters.ToArray();
        }

        protected void SaveForm()
        {
            SaveInstrumentModeFilters();

            // SearchMethod
            ObservationSearchMethod method;
            Enum.TryParse<ObservationSearchMethod>(searchMethod.SelectedValue, out method);

            // Coordinates
            double ra, dec;
            Util.Astro.TryParseCoordinates(point.Text, out ra, out dec);

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
            SaveForm();

            var searchObject = new Lib.ObservationSearch();

            searchObject.InstrumentModeFilters = InstrumentModeFilters;
            searchObject.Sso = !sso.Checked ? (bool?)false : null;
            searchObject.Calibration = !calibration.Checked ? (bool?)false : null;
            searchObject.Failed = !failed.Checked ? (bool?)false : null;
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