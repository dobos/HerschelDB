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
    public class Sso : IDatabaseTableObject
    {
        [DataMember]
        public Instrument Instrument { get; set; }

        [DataMember]
        public Int64 ObsID { get; set; }

        [DataMember]
        public Int16 SsoID { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public double Coverage { get; set; }

        [DataMember]
        public double Mag { get; set; }

        [DataMember]
        public double Hh { get; set; }

        [DataMember]
        public double R0 { get; set; }

        [DataMember]
        public double Delta { get; set; }

        [DataMember]
        public double RA { get; set; }

        [DataMember]
        public double Dec { get; set; }

        [DataMember]
        public double PmRA { get; set; }

        [DataMember]
        public double PmDec { get; set; }

        [DataMember]
        public double Pm { get; set; }

        [DataMember]
        public double Alpha { get; set; }

        [DataMember]
        public double Flux { get; set; }

        [DataMember]
        public double GSlope { get; set; }

        [DataMember]
        public double Eta { get; set; }

        [DataMember]
        public double Pv { get; set; }

        public Cartesian Position
        {
            get
            {
                return new Cartesian(RA, Dec);
            }
        }

        public Arc Trajectory
        {
            get
            {
                // TODO
                // This is an ugly hack here, fix later and
                var c0 = new Cartesian(RA - PmRA, Dec - PmDec);
                var c1 = new Cartesian(RA, Dec);
                var c2 = new Cartesian(RA + PmRA, Dec + PmDec);

                var arc = new Arc(c0, c1, c2);

                if (!arc.ContainsOnEdge(c1))
                {
                    arc = new Arc(c2, c1, c0);
                }

                return arc;
            }
        }

        public Sso()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            Instrument = Lib.Instrument.None;
            ObsID = -1;
            SsoID = -1;

            Name = null;
            Coverage = Double.NaN;
            Mag = Double.NaN;
            Hh = Double.NaN;
            R0 = Double.NaN;
            Delta = Double.NaN;
            RA = Double.NaN;
            Dec = Double.NaN;
            PmRA = Double.NaN;
            PmDec = Double.NaN;
            Pm = Double.NaN;
            Alpha = Double.NaN;
            Flux = Double.NaN;
            GSlope = Double.NaN;
            Eta = Double.NaN;
            Pv = Double.NaN;
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
            SsoID = reader.GetInt16(o++);

            Name = reader.GetString(o++);
            Coverage = reader.GetFloat(o++);
            Mag = reader.GetFloat(o++);
            Hh = reader.GetFloat(o++);
            R0 = reader.GetFloat(o++);
            Delta = reader.GetFloat(o++);
            RA = reader.GetFloat(o++);
            Dec = reader.GetFloat(o++);
            PmRA = reader.GetFloat(o++);
            PmDec = reader.GetFloat(o++);
            Pm = reader.GetFloat(o++);
            Alpha = reader.GetFloat(o++);
            Flux = reader.GetFloat(o++);
            GSlope = reader.GetFloat(o++);
            Eta = reader.GetFloat(o++);
            Pv = reader.GetFloat(o++);
        }
    }
}
