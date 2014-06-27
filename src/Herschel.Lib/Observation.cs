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
    public class Observation : IDatabaseTableObject
    {
        [IgnoreDataMember]
        public bool Selected { get; set; }

        public Int64 ObsID { get; set; }

        public FineTimeInterval FineTime { get; set; }
        
        public double AV { get; set; }
        
        [IgnoreDataMember]
        public Region Region { get; set; }

        public void LoadFromDataReader(SqlDataReader reader)
        {
            ObsID = reader.GetInt64(reader.GetOrdinal("obsID"));
            FineTime = new FineTimeInterval()
            {
                Start = reader.GetInt64(reader.GetOrdinal("fineTimeStart")),
                End = reader.GetInt64(reader.GetOrdinal("fineTimeEnd"))
            };
            AV = reader.GetDouble(reader.GetOrdinal("av"));

            Region = Region.FromSqlBytes(reader.GetSqlBytes(reader.GetOrdinal("region")));
        }
    }
}
