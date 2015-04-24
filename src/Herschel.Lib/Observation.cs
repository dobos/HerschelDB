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
            int o = 0;
            LoadFromDataReader(reader, ref o);
        }

        public void LoadFromDataReader(SqlDataReader reader, ref int o)
        {
            Instrument = (Instrument)reader.GetByte(o++);
            ObsID = reader.GetInt64(o++);

            Type = (ObservationType)reader.GetByte(o++);
            Level = (ObservationLevel)reader.GetByte(o++);
            InstrumentMode = (InstrumentMode)reader.GetInt32(o++);
            PointingMode = (PointingMode)reader.GetByte(o++);
            Object = reader.GetString(o++);
            Calibration = reader.GetBoolean(o++);

            RA = reader.GetDouble(o++);
            Dec = reader.GetDouble(o++);
            PA = reader.GetDouble(o++);
            Aperture = reader.GetDouble(o++);
            FineTimeStart = reader.GetInt64(o++);
            FineTimeEnd = reader.GetInt64(o++);
            Repetition = reader.GetInt32(o++);

            AOR = reader.GetString(o++);
            AOT = reader.GetString(o++);

            var bytes = reader.GetSqlBytes(o++);
            Region = bytes.IsNull ? null : Region.FromSqlBytes(bytes);

            ScanMap.LoadFromDataReader(reader, ref o);
            RasterMap.LoadFromDataReader(reader, ref o);
            Spectro.LoadFromDataReader(reader, ref o);
        }
    }
}
