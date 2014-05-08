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
using Herschel.Lib;

namespace Herschel.Ws.Api
{
    [ServiceContract]
    public interface ISearch
    {
        [OperationContract]
        [WebGet(UriTemplate = "Observation/Find?ra={ra}&dec={dec}")]
        [DynamicResponseFormat]
        IEnumerable<long> FindObservationEq(double ra, double dec);

        [OperationContract]
        [WebGet(UriTemplate = "Observation/Footprint?obsid={obsID}")]
        [DynamicResponseFormat]
        string FindObservation(long obsID);
    }

    public class Footprint : ISearch
    {
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

        public IEnumerable<long> FindObservationEq(double ra, double dec)
        {
            var s = new ObservationSearch();

            s.Instrument = Instrument.Pacs;
            s.Point = new Jhu.Spherical.Cartesian(ra, dec);

            var q = from r in s.FindEq()
                    select r.ObsID;

            return q;
        }

        public string FindObservation(long obsID)
        {
            var s = new ObservationSearch();

            var q = from r in s.FindID(new long[] { obsID })
                    select r.Region.ToString();

            return q.First();
        }
    }
}
