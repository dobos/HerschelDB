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
    public class ScanMap : IDatabaseTableObject
    {
        public Instrument Instrument { get; set; }
        public Int64 ObsID { get; set; }

        public double AV { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }

        public ScanMap()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            Instrument = Lib.Instrument.None;
            ObsID = -1;

            AV = double.NaN;
            Height = double.NaN;
            Width = double.NaN;            
        }

        public void LoadFromDataReader(SqlDataReader reader)
        {
            Instrument = (Instrument)reader.GetByte(reader.GetOrdinal("inst"));
            ObsID = reader.GetInt64(reader.GetOrdinal("obsID"));
            AV = reader.GetDouble(reader.GetOrdinal("AV"));
            Height = reader.GetDouble(reader.GetOrdinal("Height"));
            Width = reader.GetDouble(reader.GetOrdinal("Width"));
        }
    }
}
