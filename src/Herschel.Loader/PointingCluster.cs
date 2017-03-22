using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Jhu.Spherical;
using Herschel.Lib;

namespace Herschel.Loader
{
    class PointingCluster
    {
        public Observation Observation;
        public List<Pointing> Pointings;
        public int ClusterID;
        public int GroupID;
        public bool IsRotated;
        public long FineTimeStart;
        public long FineTimeEnd;
        public Cartesian Center;
        public double PA;

        public PointingCluster()
        {
            Observation = null;
            Pointings = new List<Pointing>();
            ClusterID = -1;
            GroupID = -1;
            IsRotated = false;
            FineTimeStart = long.MaxValue;
            FineTimeEnd = long.MinValue;
            Center = Cartesian.NaN;
            PA = double.NaN;
        }

        public PointingCluster(PointingCluster old)
        {
            this.Observation = old.Observation;
            this.ClusterID = old.ClusterID;
            this.GroupID = old.GroupID;
            this.IsRotated = old.IsRotated;
            this.FineTimeStart = old.FineTimeStart;
            this.FineTimeEnd = old.FineTimeEnd;
            this.Center = old.Center;
            this.PA = old.PA;
        }

        public PointingCluster Clone()
        {
            return new PointingCluster(this);
        }

        public void CalculateAverage()
        {
            var fineTimeStart = long.MaxValue;
            var fineTimeEnd = long.MinValue;
            double cxavg = 0;
            double cyavg = 0;
            double czavg = 0;
            double paavg = 0;

            foreach (var pi in Pointings)
            {
                fineTimeStart = Math.Min(fineTimeStart, pi.FineTime);
                fineTimeEnd = Math.Max(fineTimeEnd, pi.FineTime);
                cxavg += pi.Point.X;
                cyavg += pi.Point.Y;
                czavg += pi.Point.Z;
                paavg += pi.PA;
            }

            cxavg /= Pointings.Count;
            cyavg /= Pointings.Count;
            czavg /= Pointings.Count;
            paavg /= Pointings.Count;

            Center = new Cartesian(cxavg, cyavg, czavg, true);
            PA = paavg;
            FineTimeStart = fineTimeStart;
            FineTimeEnd = fineTimeEnd;
        }

        public void Save()
        {
            var sql = @"
INSERT [load].[PointingCluster]
    (inst, obsID, groupID, clusterID, isRotated, num, fineTimeStart, fineTimeEnd, ra, dec, pa)
VALUES
    (@inst, @obsID, @groupID, @clusterID, @isRotated, @num, @fineTimeStart, @fineTimeEnd, @ra, @dec, @pa)";


            using (var cn = DbHelper.OpenConnection())
            {
                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.CommandTimeout = 120;

                    cmd.Parameters.Add("@inst", SqlDbType.TinyInt).Value = (byte)Observation.Instrument;
                    cmd.Parameters.Add("@obsID", SqlDbType.BigInt).Value = Observation.ObsID;
                    cmd.Parameters.Add("@groupID", SqlDbType.TinyInt).Value = GroupID;
                    cmd.Parameters.Add("@clusterID", SqlDbType.Int).Value = ClusterID + 1;
                    cmd.Parameters.Add("@isRotated", SqlDbType.Bit).Value = IsRotated;
                    cmd.Parameters.Add("@num", SqlDbType.Int).Value = Pointings.Count;
                    cmd.Parameters.Add("@fineTimeStart", SqlDbType.BigInt).Value = FineTimeStart;
                    cmd.Parameters.Add("@fineTimeEnd", SqlDbType.BigInt).Value = FineTimeEnd;
                    cmd.Parameters.Add("@ra", SqlDbType.Float).Value = Center.RA;
                    cmd.Parameters.Add("@dec", SqlDbType.Float).Value = Center.Dec;
                    cmd.Parameters.Add("@pa", SqlDbType.Float).Value = PA;

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
