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
    public class RasterMap : IDatabaseTableObject
    {
        public Instrument Instrument { get; set; }
        public Int64 ObsID { get; set; }

        public double Step { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }
        public int Num { get; set; }

        public RasterMap()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            Instrument = Lib.Instrument.None;
            ObsID = -1;

            Step = double.NaN;
            Line = -1;
            Column = -1;
            Num = -1;
        }

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

                Step = reader.GetDouble(o++);
                Line = reader.GetInt32(o++);
                Column = reader.GetInt32(o++);
                Num = reader.GetInt32(o++);
            }
        }
    }
}
