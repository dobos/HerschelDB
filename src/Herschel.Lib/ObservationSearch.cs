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
        public FineTimeInterval FineTimeInterval { get; set; }

        public IEnumerable<Observation> Find()
        {
            switch (SearchMethod)
            {
                case ObservationSearchMethod.Point:
                    return FindEq();
                case ObservationSearchMethod.Intersect:
                    return FindIntersect();
                case ObservationSearchMethod.Cover:
                default:
                    throw new NotImplementedException();
            }
        }

        public IEnumerable<Observation> FindEq()
        {
            var sql = 
@"
SELECT obs.obsID, fineTimeStart, fineTimeEnd, av, region
FROM [dbo].[FindObservationEq](@ra, @dec, @fineTimeStart, @fineTimeEnd) ids
INNER JOIN [dbo].[Observation] obs
    ON obs.obsID = ids.obsID";

            var cmd = new SqlCommand(sql);
            cmd.Parameters.Add("@ra", SqlDbType.Float).Value = Point.RA;
            cmd.Parameters.Add("@dec", SqlDbType.Float).Value = Point.Dec;
            cmd.Parameters.Add("@fineTimeStart", SqlDbType.Float).Value = DBNull.Value;
            cmd.Parameters.Add("@fineTimeEnd", SqlDbType.Float).Value = DBNull.Value;

            return ExecuteCommandReader<Observation>(cmd);
        }

        public IEnumerable<Observation> FindIntersect()
        {
            var sql =
@"
SELECT obs.obsID, fineTimeStart, fineTimeEnd, av, obs.region
FROM [dbo].[FindObservationRegion](@region, NULL, NULL) ids
INNER JOIN [dbo].[Observation] obs WITH (FORCESEEK)
    ON obs.obsID = ids.obsID";

            var cmd = new SqlCommand(sql);
            cmd.Parameters.Add("@region", SqlDbType.VarBinary).Value = Region.ToSqlBytes().Value;
            cmd.Parameters.Add("@fineTimeStart", SqlDbType.Float).Value = DBNull.Value;
            cmd.Parameters.Add("@fineTimeEnd", SqlDbType.Float).Value = DBNull.Value;

            return ExecuteCommandReader<Observation>(cmd);
        }

        public IEnumerable<Observation> FindID(IList<long> ids)
        {
            var sql =
@"
SELECT obs.obsID, fineTimeStart, fineTimeEnd, av, region
FROM [dbo].[Observation] obs
WHERE obs.obsID IN ({0})";

            var idlist = String.Empty;
            for (int i = 0; i < ids.Count; i++)
            {
                if (i > 0)
                {
                    idlist += ",";
                }
                idlist += ids[i];
            }

            sql = String.Format(sql, idlist);

            var cmd = new SqlCommand(sql);

            return ExecuteCommandReader<Observation>(cmd);
        }
    }
}
