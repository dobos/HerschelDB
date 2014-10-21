using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Jhu.Spherical;

namespace Herschel.Lib
{
    public class ObservationSearch : DbObjectBase
    {
        public ObservationSearchMethod SearchMethod { get; set; }
        public Instrument Instrument { get; set; }
        public Cartesian Point { get; set; }
        public Region Region { get; set; }
        public FineTime FineTimeStart { get; set; }
        public FineTime FineTimeEnd { get; set; }

        public IEnumerable<Observation> Find()
        {
            switch (SearchMethod)
            {
                case ObservationSearchMethod.Point:
                    return FindEq();
                case ObservationSearchMethod.Intersect:
                    return FindRegionIntersect();
                case ObservationSearchMethod.Cover:
                default:
                    throw new NotImplementedException();
            }
        }

        public Observation Get(ObservationID obsId)
        {
            var sql =
@"
SELECT obs.inst, obs.obsID, fineTimeStart, fineTimeEnd, av, region
FROM [dbo].[Observation] obs
WHERE (obs.inst IS NULL OR (obs.inst & @inst) > 0)
      AND obs.obsID = @obsID
ORDER BY obs.ObsID";

            var cmd = new SqlCommand(sql);
            cmd.Parameters.Add("@inst", SqlDbType.TinyInt).Value = obsId.Instrument == Lib.Instrument.None ? (object)DBNull.Value : (byte)obsId.Instrument;
            cmd.Parameters.Add("@obsID", SqlDbType.BigInt).Value = obsId.ID;

            return ExecuteCommandReader<Observation>(cmd).FirstOrDefault();
        }

        public IEnumerable<Observation> FindID(IList<ObservationID> obsIds)
        {
            if (obsIds.Count == 0)
            {
                return new Observation[0];
            }

            var sql =
@"
SELECT obs.inst, obs.obsID, fineTimeStart, fineTimeEnd, av, region
FROM [dbo].[Observation] obs
WHERE {0}
ORDER BY inst, obs.ObsID";

            var idlist = String.Empty;
            for (int i = 0; i < obsIds.Count; i++)
            {
                if (i > 0)
                {
                    idlist += "OR";
                }

                idlist += String.Format("(inst = {0} AND obsID = {1})", (byte)obsIds[i].Instrument, obsIds[i].ID);
            }

            sql = String.Format(sql, idlist);

            var cmd = new SqlCommand(sql);

            return ExecuteCommandReader<Observation>(cmd);
        }

        public IEnumerable<Observation> FindEq()
        {
            var sql =
@"
SELECT obs.inst, obs.obsID, fineTimeStart, fineTimeEnd, av, region
FROM [dbo].[FindObservationEq](@ra, @dec) ids
INNER JOIN [dbo].[Observation] obs WITH (FORCESEEK)
      ON obs.inst = ids.inst AND obs.obsID = ids.obsID
WHERE (obs.inst IS NULL OR (obs.inst & @inst) > 0)
      AND (@fineTimeStart IS NULL OR @fineTimeStart <= fineTimeStart)
      AND (@fineTimeEnd IS NULL OR @fineTimeEnd >= fineTimeEnd)
ORDER BY obs.ObsID";

            var cmd = new SqlCommand(sql);
            cmd.Parameters.Add("@inst", SqlDbType.TinyInt).Value = Instrument == Lib.Instrument.None ? (object)DBNull.Value : (byte)Instrument;
            cmd.Parameters.Add("@ra", SqlDbType.Float).Value = Point.RA;
            cmd.Parameters.Add("@dec", SqlDbType.Float).Value = Point.Dec;
            cmd.Parameters.Add("@fineTimeStart", SqlDbType.Float).Value = FineTime.IsUndefined(FineTimeStart) ? (object)DBNull.Value : FineTimeStart.Value;
            cmd.Parameters.Add("@fineTimeEnd", SqlDbType.Float).Value = FineTime.IsUndefined(FineTimeEnd) ? (object)DBNull.Value : FineTimeEnd.Value;

            return ExecuteCommandReader<Observation>(cmd);
        }

        public IEnumerable<Observation> FindRegionIntersect()
        {
            var sql =
@"
SELECT obs.inst, obs.obsID, fineTimeStart, fineTimeEnd, av, obs.region
FROM [dbo].[FindObservationRegionIntersect](@region) ids
INNER JOIN [dbo].[Observation] obs WITH (FORCESEEK)
    ON obs.inst = ids.inst AND obs.obsID = ids.obsID
WHERE (obs.inst IS NULL OR (obs.inst & @inst) > 0)
      AND (@fineTimeStart IS NULL OR @fineTimeStart <= fineTimeStart)
      AND (@fineTimeEnd IS NULL OR @fineTimeEnd >= fineTimeEnd)
ORDER BY obs.ObsID";

            var cmd = new SqlCommand(sql);
            cmd.Parameters.Add("@inst", SqlDbType.TinyInt).Value = Instrument == Lib.Instrument.None ? (object)DBNull.Value : (byte)Instrument;
            cmd.Parameters.Add("@region", SqlDbType.VarBinary).Value = Region.ToSqlBytes().Value;
            cmd.Parameters.Add("@fineTimeStart", SqlDbType.Float).Value = FineTime.IsUndefined(FineTimeStart) ? (object)DBNull.Value : FineTimeStart.Value;
            cmd.Parameters.Add("@fineTimeEnd", SqlDbType.Float).Value = FineTime.IsUndefined(FineTimeEnd) ? (object)DBNull.Value : FineTimeEnd.Value;

            return ExecuteCommandReader<Observation>(cmd);
        }
    }
}
