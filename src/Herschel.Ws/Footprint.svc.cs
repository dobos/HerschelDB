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

namespace Herschel.Ws
{
    [ServiceContract]
    public interface ISearch
    {
        [OperationContract]
        [WebGet(UriTemplate = "Observation/Find?ra={ra}&dec={dec}")]
        [DynamicResponseFormat]
        IEnumerable<Observation> FindObservation(double ra, double dec);
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

        public IEnumerable<Observation> FindObservation(double ra, double dec)
        {
            var ids = new List<Int64>();
            var sql = "SELECT obsID FROM FindObservationEq(@ra, @dec, NULL)";

            var cn = OpenConnection();
            var cmd = new SqlCommand(sql, cn);

            cmd.Parameters.Add("@ra", SqlDbType.Float).Value = ra;
            cmd.Parameters.Add("@dec", SqlDbType.Float).Value = dec;

            var dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            return dr.AsEnumerable<Observation>();
        }
    }
}
