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
            Instrument = (Instrument)reader.GetByte(reader.GetOrdinal("inst"));
            ObsID = reader.GetInt64(reader.GetOrdinal("obsID"));

            Step = reader.GetDouble(reader.GetOrdinal("step"));
            Line = reader.GetInt32(reader.GetOrdinal("line"));
            Column = reader.GetInt32(reader.GetOrdinal("column"));
            Num = reader.GetInt32(reader.GetOrdinal("num"));
        }
    }
}
