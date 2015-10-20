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
        public double RA { get; set; }
        public double Dec { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public double PA { get; set; }

        public ScanMap()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            Instrument = Lib.Instrument.None;
            ObsID = -1;

            AV = double.NaN;
            RA = double.NaN;
            Dec = double.NaN;
            Height = double.NaN;
            Width = double.NaN;
            PA = double.NaN;
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
                AV = reader.GetDouble(o++);
                RA = reader.GetDouble(o++);
                Dec = reader.GetDouble(o++);
                Height = reader.GetDouble(o++);
                Width = reader.GetDouble(o++);
                PA = reader.GetDouble(o++);
            }
        }
    }
}
