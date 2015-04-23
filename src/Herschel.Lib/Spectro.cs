using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.ServiceModel;
using System.Runtime.Serialization;
using Jhu.Spherical;


namespace Herschel.Lib
{
    [DataContract]
    public class Spectro : IDatabaseTableObject
    {
        public Instrument Instrument { get; set; }
        public Int64 ObsID { get; set; }

        public int Num { get; set; }
        public double LambdaFrom { get; set; }
        public double LambdaTo { get; set; }
        public double Lambda2From { get; set; }
        public double Lambda2To { get; set; }
        public string RangeID { get; set; }

        public void LoadFromDataReader(SqlDataReader reader)
        {
            Instrument = (Instrument)reader.GetByte(reader.GetOrdinal("inst"));
            ObsID = reader.GetInt64(reader.GetOrdinal("obsID"));

            Num = reader.GetInt32(reader.GetOrdinal("num"));
            LambdaFrom = reader.GetDouble(reader.GetOrdinal("lambdaFrom"));
            LambdaTo = reader.GetDouble(reader.GetOrdinal("lambdaTo"));
            Lambda2From = reader.GetDouble(reader.GetOrdinal("lambda2From"));
            Lambda2To = reader.GetDouble(reader.GetOrdinal("lambda2To"));
            RangeID = reader.GetString(reader.GetOrdinal("rangeId"));
        }
    }
}
