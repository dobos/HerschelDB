using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Configuration;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Globalization;
using Herschel.Lib;
using Jhu.Spherical;

namespace Herschel.Ws.Api
{
    public class Point
    {
        public double Ra { get; set; }
        public double Dec { get; set; }

        public static implicit operator Point(Cartesian c)
        {
            return new Point()
            {
                Ra = c.RA,
                Dec = c.Dec
            };
        }
    }

    [ServiceContract]
    [Description("This service queries the observation database and returns the footprints in various formats.")]
    public interface ISearch
    {
        [OperationContract]
        [DynamicResponseFormat]
        [Description("Finds observations by J2000 equatorial coordinates.")]
        [WebGet(BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "Observations?findby=eq&inst={inst}&ra={ra}&dec={dec}&start={start}&end={end}")]
        IEnumerable<Observation> FindObservationEq(
            [Description("A comma separated list of instrument identifiers.")]
            string inst,
            [Description("Right ascension, J2000, degrees.")]
            double ra,
            [Description("Declination, J2000, degrees.")]
            double dec,
            [Description("Start of search interval, fine time.")]
            long start,
            [Description("End of search interval, fine time.")]
            long end);

        [OperationContract]
        [DynamicResponseFormat]
        [Description("Finds observations by an intersecting region.")]
        [WebGet(UriTemplate = "Observations?findby=intersect&inst={inst}&region={region}&start={start}&end={end}")]
        IEnumerable<Observation> FindObservationIntersect(
            [Description("A comma separated list of instrument identifiers.")]
            string inst,
            [Description("A region description string, see documentation.")]
            string region,
            [Description("Start of search interval, fine time.")]
            long start,
            [Description("End of search interval, fine time.")]
            long end);

        [OperationContract]
        [WebGet(UriTemplate = "Observations/{inst}/{obsID}")]
        [DynamicResponseFormat]
        [Description("Returns the details of a single observation by obsID.")]
        Observation GetObservation(
            [Description("An instrument identifier.")]
            string inst,
            [Description("Observation ID.")]
            string obsID);

        [OperationContract]
        [DynamicResponseFormat]
        [WebGet(UriTemplate = "Observations/{inst}/{obsID}/Footprint")]
        [Description("Returns the footprint of an observation.")]
        string GetObservationFootprint(
            [Description("An instrument identifier.")]
            string inst,
            [Description("Observation ID.")]
            string obsID);

        [OperationContract]
        [DynamicResponseFormat]
        [WebGet(UriTemplate = "Observations/{inst}/{obsID}/Footprint/Outline")]
        [Description("Returns the outline of the footprint of an observation.")]
        string GetObservationOutline(
            [Description("An instrument identifier.")]
            string inst,
            [Description("Observation ID.")]
            string obsID);

        [OperationContract]
        [DynamicResponseFormat]
        [WebGet(UriTemplate = "Observations/{inst}/{obsID}/Footprint/Outline/Points?res={resolution}")]
        [Description("Returns the arc endpoints of the outline of the footprint of an observation.")]
        IEnumerable<Point> GetObservationOutlinePoints(
            [Description("An instrument identifier.")]
            string inst,
            [Description("Observation ID.")]
            string obsID,
            [Description("Resolution in arc sec to interpolate small circle arcs")]
            double resolution);

        [OperationContract]
        [DynamicResponseFormat]
        [WebGet(UriTemplate = "Observations/{inst}/{obsID}/Footprint/Outline/Reduced?limit={limit}")]
        [Description("Returns the reduced outline of the footprint of an observation.")]
        string GetObservationOutlineReduced(
            [Description("An instrument identifier.")]
            string inst,
            [Description("Observation ID.")]
            string obsID,
            [Description("Limit of the reduction algorithm, arc sec.")]
            double limit);

        [OperationContract]
        [DynamicResponseFormat]
        [WebGet(UriTemplate = "Observations/{inst}/{obsID}/Footprint/Outline/Reduced/Points?res={resolution}&limit={limit}")]
        [Description("Returns the arc endpoints of the reduced outline of the footprint of an observation.")]
        IEnumerable<Point> GetObservationOutlineReducedPoints(
            [Description("An instrument identifier.")]
            string inst,
            [Description("Observation ID.")]
            string obsID,
            [Description("Resolution in arc sec to interpolate small circle arcs")]
            double resolution,
            [Description("Limit of the reduction algorithm, arc sec.")]
            double limit);

        [OperationContract]
        [DynamicResponseFormat]
        [WebGet(UriTemplate = "Observations/{inst}/{obsID}/Footprint/ConvexHull")]
        [Description("Returns the convex hull of the footprint of an observation.")]
        string GetObservationConvexHull(
            [Description("An instrument identifier.")]
            string inst,
            [Description("Observation ID.")]
            string obsID);

        [OperationContract]
        [DynamicResponseFormat]
        [WebGet(UriTemplate = "Observations/{inst}/{obsID}/Footprint/ConvexHull/Outline")]
        [Description("Returns the outline of the convex hull of the footprint of an observation.")]
        string GetObservationConvexHullOutline(
            [Description("An instrument identifier.")]
            string inst,
            [Description("Observation ID.")]
            string obsID);

        [OperationContract]
        [DynamicResponseFormat]
        [WebGet(UriTemplate = "Observations/{inst}/{obsID}/Footprint/ConvexHull/Outline/Points?res={resolution}")]
        [Description("Returns the arc endpoints of the outline of the convex hull of the footprint of an observation.")]
        IEnumerable<Point> GetObservationConvexHullOutlinePoints(
            [Description("An instrument identifier.")]
            string inst,
            [Description("Observation ID.")]
            string obsID,
            [Description("Resolution in arc sec to interpolate small circle arcs")]
            double resolution);
    }

    public class Footprint : ISearch
    {
        #region Private utility functions

        private Observation GetObservation(ObservationID obsID)
        {
            var s = new ObservationSearch();

            var obs = s.Get(obsID);

            if (obs == null)
            {
                ThrowNotFoundException();
            }

            return obs;
        }

        private void ThrowNotFoundException()
        {
            throw new WebFaultException<string>("Observation not found", HttpStatusCode.NotFound);
        }

        private IEnumerable<Point> InterpolateOutlinePoints(Outline outline, double resolution)
        {
            if (resolution == 0)
            {
                resolution = 0.1;
            }

            resolution = resolution / 3600.0 / 180.0 * Math.PI;

            var res = new List<Point>();

            foreach (var loop in outline.LoopList)
            {
                int q = 0;
                foreach (var arc in loop.ArcList)
                {
                    // Starting point
                    if (q == 0)
                    {
                        res.Add(arc.Point1);
                    }

                    // If a small circle arc, interpolate
                    if (arc.Circle.Cos0 != 0)
                    {
                        var n = (int)Math.Min(1000, Math.Max(6, arc.Length / resolution));
                        var a = arc.Angle / n;

                        for (int i = 1; i < n - 1; i++)
                        {
                            var p = arc.GetPoint(i * a);
                            res.Add(p);
                        }
                    }

                    res.Add(arc.Point2);
                    q++;
                }
            }

            return res;
        }
        
        #endregion
        #region Interface implementation

        public IEnumerable<Observation> FindObservationEq(string inst, double ra, double dec, long start, long end)
        {
            var obsid = ObservationID.Parse(inst, "0");

            var s = new ObservationSearch()
            {
                InstrumentModeFilters = new[] { new InstrumentModeFilter(obsid.Instrument) },
                Point = new Jhu.Spherical.Cartesian(ra, dec),
                FineTimeStart = start,
                FineTimeEnd = end,
            };

            return s.FindEq();
        }

        public IEnumerable<Observation> FindObservationIntersect(string inst, string region, long start, long end)
        {
            return null;
        }

        public Observation GetObservation(string instrument, string obsID)
        {
            var obs = GetObservation(ObservationID.Parse(instrument, obsID));
            return obs;
        }

        public string GetObservationFootprint(string instrument, string obsID)
        {
            var obs = GetObservation(ObservationID.Parse(instrument, obsID));

            return obs.Region == null ? null : obs.Region.ToString();
        }

        public string GetObservationOutline(string instrument, string obsID)
        {
            var obs = GetObservation(ObservationID.Parse(instrument, obsID));
            return obs.Region.Outline.ToString();
        }

        public IEnumerable<Point> GetObservationOutlinePoints(string instrument, string obsID, double resolution)
        {
            var obs = GetObservation(ObservationID.Parse(instrument, obsID));
            return InterpolateOutlinePoints(obs.Region.Outline, resolution);
        }

        public string GetObservationOutlineReduced(string instrument, string obsID, double limit)
        {
            var obs = GetObservation(ObservationID.Parse(instrument, obsID));
            obs.Region.Outline.Reduce(limit / 648000.0 * Math.PI);
            return obs.Region.Outline.ToString();
        }

        public IEnumerable<Point> GetObservationOutlineReducedPoints(string instrument, string obsID, double resolution, double limit)
        {
            var obs = GetObservation(ObservationID.Parse(instrument, obsID));
            obs.Region.Outline.Reduce(limit / 648000.0 * Math.PI);
            return InterpolateOutlinePoints(obs.Region.Outline, resolution);
        }

        public string GetObservationConvexHull(string instrument, string obsID)
        {
            var obs = GetObservation(ObservationID.Parse(instrument, obsID));
            return obs.Region.Outline.GetConvexHull().ToString();
        }

        public string GetObservationConvexHullOutline(string instrument, string obsID)
        {
            var obs = GetObservation(ObservationID.Parse(instrument, obsID));

            var chull = obs.Region.Outline.GetConvexHull();
            chull.Simplify();

            return chull.Outline.ToString();
        }

        public IEnumerable<Point> GetObservationConvexHullOutlinePoints(string instrument, string obsID, double resolution)
        {
            var obs = GetObservation(ObservationID.Parse(instrument, obsID));

            var chull = obs.Region.Outline.GetConvexHull();
            chull.Simplify();

            return InterpolateOutlinePoints(chull.Outline, resolution);
        }

        #endregion
    }
}
