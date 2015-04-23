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
        public Int64 ObsID { get; set; }
        public ObservationType Type { get; set; }
        public ObservationLevel Level { get; set; }
        public InstrumentMode InstrumentMode { get; set; }
        public PointingMode PointingMode { get; set; }
        public string @Object { get; set; }
        public bool Calibration { get; set; }
        public double RA { get; set; }
        public double Dec { get; set; }
        public double PA { get; set; }
        public double Aperture { get; set; }
        public FineTime FineTimeStart { get; set; }
        public FineTime FineTimeEnd { get; set; }
        public int Repetition { get; set; }
        
        public string AOR { get; set; }
        public string AOT { get; set; }

        [IgnoreDataMember]
        public ScanMap ScanMap { get; set; }

        [IgnoreDataMember]
        public RasterMap RasterMap { get; set; }

        [IgnoreDataMember]
        public Spectro Spectro { get; set; }
                
        [IgnoreDataMember]
        public Region Region { get; set; }

        public Observation()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            Instrument = Lib.Instrument.None;
            ObsID = -1;

            Type = ObservationType.None;
            Level = ObservationLevel.None;
            InstrumentMode = InstrumentMode.None;
            PointingMode = PointingMode.None;
            Object = null;
            Calibration = false;

            RA = Double.NaN;
            Dec = Double.NaN;
            PA = Double.NaN;
            Aperture = Double.NaN;
            FineTimeStart = -1;
            FineTimeEnd = -1;
            Repetition = -1;

            AOR = null;
            AOT = null;

            ScanMap = new Lib.ScanMap();
            RasterMap = new Lib.RasterMap();
            Spectro = new Lib.Spectro();
            Region = null;
        }

        public void LoadFromDataReader(SqlDataReader reader)
        {
            Instrument = (Instrument)reader.GetByte(reader.GetOrdinal("inst"));
            ObsID = reader.GetInt64(reader.GetOrdinal("obsID"));

            Type = (ObservationType)reader.GetByte(reader.GetOrdinal("obsType"));
            Level = (ObservationLevel)reader.GetByte(reader.GetOrdinal("obsLevel"));
            InstrumentMode = (InstrumentMode)reader.GetInt32(reader.GetOrdinal("instMode"));
            PointingMode = (PointingMode)reader.GetByte(reader.GetOrdinal("pointingMode"));
            Object = reader.GetString(reader.GetOrdinal("object"));
            Calibration = reader.GetBoolean(reader.GetOrdinal("calibration"));

            RA = reader.GetDouble(reader.GetOrdinal("ra"));
            Dec = reader.GetDouble(reader.GetOrdinal("dec"));
            PA = reader.GetDouble(reader.GetOrdinal("pa"));
            Aperture = reader.GetDouble(reader.GetOrdinal("aperture"));
            FineTimeStart = reader.GetInt64(reader.GetOrdinal("fineTimeStart"));
            FineTimeEnd = reader.GetInt64(reader.GetOrdinal("fineTimeEnd"));
            Repetition = reader.GetInt32(reader.GetOrdinal("repetition"));

            AOR = reader.GetString(reader.GetOrdinal("aor"));
            AOT = reader.GetString(reader.GetOrdinal("aot"));

            ScanMap.LoadFromDataReader(reader);
            RasterMap.LoadFromDataReader(reader);
            Spectro.LoadFromDataReader(reader);

            Region = Region.FromSqlBytes(reader.GetSqlBytes(reader.GetOrdinal("region")));
        }
    }
}
