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

        [IgnoreDataMember]
        public Instrument Instrument { get; set; }

        [DataMember(Name = "inst")]
        public string Instrument_ForXml
        {
            get { return Instrument.ToString(); }
            set { Instrument = (Instrument)Enum.Parse(typeof(Instrument), value); }
        }

        [DataMember(Name = "obsID")]
        public Int64 ObsID { get; set; }

        [DataMember(Name = "type")]
        public ObservationType Type { get; set; }

        [DataMember(Name = "obsLevel")]
        public ObservationLevel Level { get; set; }

        [IgnoreDataMember]
        public InstrumentMode InstrumentMode { get; set; }

        [DataMember(Name = "instMode")]
        public string InstrumentMode_ForXml
        {
            get { return InstrumentMode.ToString(); }
            set { InstrumentMode = (InstrumentMode)Enum.Parse(typeof(InstrumentMode), value); }
        }

        [DataMember(Name = "pointingMode")]
        public PointingMode PointingMode { get; set; }

        [DataMember(Name = "band")]
        public string Band { get; set; }

        [DataMember(Name = "object")]
        public string @Object { get; set; }

        [DataMember(Name = "calib")]
        public bool Calibration { get; set; }

        [DataMember(Name = "failed")]
        public bool Failed { get; set; }

        [DataMember(Name = "sso")]
        public bool Sso { get; set; }

        [DataMember(Name = "ra")]
        public double RA { get; set; }

        [DataMember(Name = "dec")]
        public double Dec { get; set; }

        [DataMember(Name = "pa")]
        public double PA { get; set; }

        [DataMember(Name = "aper")]
        public double Aperture { get; set; }

        [DataMember(Name = "start")]
        public FineTime FineTimeStart { get; set; }

        [DataMember(Name = "end")]
        public FineTime FineTimeEnd { get; set; }

        [DataMember(Name = "repetitions")]
        public int Repetition { get; set; }

        [DataMember(Name = "proposer")]
        public string Proposer { get; set; }

        [DataMember(Name = "aor")]
        public string AOR { get; set; }

        [DataMember(Name = "aot")]
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
            Failed = false;
            Sso = false;

            RA = Double.NaN;
            Dec = Double.NaN;
            PA = Double.NaN;
            Aperture = Double.NaN;
            FineTimeStart = -1;
            FineTimeEnd = -1;
            Repetition = -1;

            Proposer = null;
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
            Band = reader.GetString(o++);
            Object = reader.GetString(o++);
            Calibration = reader.GetBoolean(o++);
            Failed = reader.GetBoolean(o++);
            Sso = reader.GetBoolean(o++);

            RA = reader.GetDouble(o++);
            Dec = reader.GetDouble(o++);
            PA = reader.GetDouble(o++);
            Aperture = reader.GetDouble(o++);
            FineTimeStart = reader.GetInt64(o++);
            FineTimeEnd = reader.GetInt64(o++);
            Repetition = reader.GetInt32(o++);

            Proposer = reader.GetString(o++);
            AOR = reader.GetString(o++);
            AOT = reader.GetString(o++);

            if (!reader.IsDBNull(o))
            {
                var bytes = reader.GetSqlBytes(o++);
                Region = bytes.IsNull ? null : Region.FromSqlBytes(bytes);
            }
            else
            {
                o++;
                Region = null;
            }

            ScanMap.LoadFromDataReader(reader, ref o);
            RasterMap.LoadFromDataReader(reader, ref o);
            Spectro.LoadFromDataReader(reader, ref o);
        }
    }
}
