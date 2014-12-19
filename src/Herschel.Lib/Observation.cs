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

        public Instrument Instrument { get; set; }
        public Int64 ID { get; set; }
        public ObservationLevel Level { get; set; }
        public ObservationMode Mode { get; set; }
        public InstrumentMode InstrumentMode { get; set; }
        public string @Object { get; set; }
        public bool Calibration { get; set; }
        public PointingMode PointingMode { get; set; }
        public FineTime FineTimeStart { get; set; }
        public FineTime FineTimeEnd { get; set; }
        public double RA { get; set; }
        public double Dec { get; set; }
        public double PA { get; set; }
        public int Repetition { get; set; }
        public double MapScanSpeed { get; set; }
        public double MapHeight { get; set; }
        public double MapWidth { get; set; }
        public int RasterNumPoint { get; set; }
        public double RasterPointStep { get; set; }
        public int RasterLine { get; set; }
        public int RasterColumn { get; set; }

        public int SpecNumLine { get; set; }

        // Temporary field, will need to be parsed further
        public string SpecRange { get; set; }
        public string AORLabel { get; set; }
        public string AOT { get; set; }
        
        [IgnoreDataMember]
        public Region Region { get; set; }

        public void LoadFromDataReader(SqlDataReader reader)
        {
            Instrument = (Instrument)reader.GetByte(reader.GetOrdinal("inst"));
            ID = reader.GetInt64(reader.GetOrdinal("obsID"));
            FineTimeStart = reader.GetInt64(reader.GetOrdinal("fineTimeStart"));
            FineTimeEnd = reader.GetInt64(reader.GetOrdinal("fineTimeEnd"));

            MapScanSpeed = reader.GetDouble(reader.GetOrdinal("av"));   // TODO

            Region = Region.FromSqlBytes(reader.GetSqlBytes(reader.GetOrdinal("region")));
        }
    }
}
