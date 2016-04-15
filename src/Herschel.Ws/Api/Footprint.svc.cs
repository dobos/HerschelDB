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

            for (int l = 0; l < outline.LoopList.Count; l++)
            {
                var loop = outline.LoopList[l];
                var arcs = new List<Arc>(loop.ArcList);

                int q = 0;

                for (int a = 0; a < arcs.Count; a++)
                {
                    Point p;
                    var arc = arcs[a];

                    // Starting point
                    if (q == 0)
                    {
                        res.Add(arc.Point1);
                    }

                    // If a small circle arc, interpolate
                    if (arc.Circle.Cos0 != 0)
                    {
                        var n = (int)Math.Min(1000, Math.Max(6, arc.Length / resolution)) - 1;
                        var ang = arc.Angle / n;

                        for (int i = 1; i < n; i++)
                        {
                            p = (Point)arc.GetPoint(i * ang);
                            p.LoopID = l;
                            res.Add(p);
                        }
                    }

                    // Endpoint
                    p = (Point)arc.Point2;
                    p.LoopID = l;
                    res.Add(p);
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
