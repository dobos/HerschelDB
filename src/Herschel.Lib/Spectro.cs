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
            int o = 0;
            LoadFromDataReader(reader, ref o);
        }

        public void LoadFromDataReader(SqlDataReader reader, ref int o)
        {
            if (o < reader.FieldCount && !reader.IsDBNull(o))
            {
                Instrument = (Instrument)reader.GetByte(o++);
                ObsID = reader.GetInt64(o++);

                Num = reader.GetInt32(o++);
                LambdaFrom = reader.GetDouble(o++);
                LambdaTo = reader.GetDouble(o++);
                Lambda2From = reader.GetDouble(o++);
                Lambda2To = reader.GetDouble(o++);
                RangeID = reader.GetString(o++);
            }
        }
    }
}
