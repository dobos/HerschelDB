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
        public List<RawPointing> Pointings;
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
            Pointings = new List<RawPointing>();
            ClusterID = -1;
            GroupID = -1;
            IsRotated = false;
            FineTimeStart = long.MaxValue;
            FineTimeEnd = long.MinValue;
            Center = Cartesian.NaN;
            PA = double.NaN;
        }

        public void Save()
        {
            var sql = @"
INSERT [load].[PointingCluster]
    (inst, obsID, clusterID, groupID, isRotated, num, fineTimeStart, fineTimeEnd, ra, dec, pa)
VALUES
    (@inst, @obsID, @clusterID, @groupID, @isRotated, @num, @fineTimeStart, @fineTimeEnd, @ra, @dec, @pa)";


            using (var cn = DbHelper.OpenConnection())
            {
                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.Add("@inst", SqlDbType.TinyInt).Value = (byte)Observation.Instrument;
                    cmd.Parameters.Add("@obsID", SqlDbType.BigInt).Value = Observation.ObsID;
                    cmd.Parameters.Add("@clusterID", SqlDbType.Int).Value = ClusterID + 1;
                    cmd.Parameters.Add("@groupID", SqlDbType.TinyInt).Value = GroupID;
                    cmd.Parameters.Add("@isRotated", SqlDbType.Bit).Value = IsRotated;
                    cmd.Parameters.Add("@num", SqlDbType.Int).Value = Points.Count;
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
