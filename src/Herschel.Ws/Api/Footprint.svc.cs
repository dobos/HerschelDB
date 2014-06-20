using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Configuration;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using Herschel.Lib;
using Jhu.Spherical;

namespace Herschel.Ws.Api
{
    [ServiceContract]
    public interface ISearch
    {
        [OperationContract]
        [WebGet(UriTemplate = "Observations?findby=eq&inst={inst}&ra={ra}&dec={dec}&start={start}&end={end}",
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        [DynamicResponseFormat]
        IEnumerable<Observation> FindObservationEq(string inst, double ra, double dec, long start, long end);

        [OperationContract]
        [WebGet(UriTemplate = "Observations?findby=intersect&inst={inst}&region={region}&start={start}&end={end}")]
        [DynamicResponseFormat]
        IEnumerable<Observation> FindObservationIntersect(string inst, string region, long start, long end);

        [OperationContract]
        [WebGet(UriTemplate = "Observations/{obsID}")]
        [DynamicResponseFormat]
        Observation GetObservation(string obsID);

        [OperationContract]
        [WebGet(UriTemplate = "Observations/{obsID}/Footprint")]
        [DynamicResponseFormat]
        string GetObservationFootprint(string obsID);

        [OperationContract]
        [WebGet(UriTemplate = "Observations/{obsID}/Footprint/Outline")]
        [DynamicResponseFormat]
        string GetObservationOutline(string obsID);
    }

    public class Footprint : ISearch
    {
        /* TODO: delete
        private string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["Herschel"].ConnectionString; }
        }

        private SqlConnection OpenConnection()
        {
            var cn = new SqlConnection(ConnectionString);
            cn.Open();

            return cn;
        }
         * */

        public IEnumerable<Observation> FindObservationEq(string inst, double ra, double dec, long start, long end)
        {
            var s = new ObservationSearch()
            {
                Instrument = Instrument.Pacs,
                Point = new Jhu.Spherical.Cartesian(ra, dec),
            };

            return s.FindEq();
        }

        public IEnumerable<Observation> FindObservationIntersect(string inst, string region, long start, long end)
        {
            return null;
        }

        public Observation GetObservation(string obsID)
        {
            var s = new ObservationSearch();

            return s.Get(long.Parse(obsID));
        }

        public string GetObservationFootprint(string obsID)
        {
            var s = new ObservationSearch();
            var obs = s.Get(long.Parse(obsID));

            if (obs == null)
            {
                ThrowNotFoundException();
            }

            return obs.Region.ToString();
        }

        public string GetObservationOutline(string obsID)
        {
            var s = new ObservationSearch();
            var obs = s.Get(long.Parse(obsID));

            if (obs == null)
            {
                ThrowNotFoundException();
            }

            return obs.Region.Outline.ToString();
        }

        private void ThrowNotFoundException()
        {
            throw new WebFaultException<string>("Observation not found", HttpStatusCode.NotFound); 
        }
    }
}
