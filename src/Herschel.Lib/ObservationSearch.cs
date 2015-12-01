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
    public class ObservationSearch
    {
        public ObservationSearchMethod SearchMethod { get; set; }
        public InstrumentModeFilter[] InstrumentModeFilters { get; set; }
        public bool? Calibration { get; set; }
        public bool? Failed { get; set; }
        public bool? Sso { get; set; }
        public long[] ObservationID { get; set; }
        public Cartesian Point { get; set; }
        public double Radius { get; set; }
        public Region Region { get; set; }
        public FineTime FineTimeStart { get; set; }
        public FineTime FineTimeEnd { get; set; }

        public IEnumerable<Observation> Find()
        {
            switch (SearchMethod)
            {
                case ObservationSearchMethod.ID:
                    return FindID();
                case ObservationSearchMethod.Point:
                    return FindEq();
                case ObservationSearchMethod.Cone:
                    return FindRegionCone();
                case ObservationSearchMethod.Intersect:
                    return FindRegionIntersect();
                case ObservationSearchMethod.Cover:
                default:
                    throw new NotImplementedException();
            }
        }

        private string GetFilterWhereConditions()
        {
            string sql = @"
((@fineTimeStart IS NULL OR @fineTimeStart <= fineTimeStart)
 AND (@fineTimeEnd IS NULL OR @fineTimeEnd >= fineTimeEnd)
 AND (calibration = @calibration OR @calibration IS NULL)
 AND (failed = @failed OR @failed IS NULL)
 AND (sso = @sso OR @sso IS NULL))
";

            return sql;
        }

        private void AppendFilterParameters(SqlCommand cmd)
        {
            cmd.Parameters.Add("@fineTimeStart", SqlDbType.Float).Value = FineTimeStart.IsUndefined ? (object)DBNull.Value : FineTimeStart.Value;
            cmd.Parameters.Add("@fineTimeEnd", SqlDbType.Float).Value = FineTimeEnd.IsUndefined ? (object)DBNull.Value : FineTimeEnd.Value;
            cmd.Parameters.Add("@calibration", SqlDbType.Bit).Value = Calibration.HasValue ? (object)Calibration.Value : DBNull.Value;
            cmd.Parameters.Add("@failed", SqlDbType.Bit).Value = Failed.HasValue ? (object)Failed.Value : DBNull.Value;
            cmd.Parameters.Add("@sso", SqlDbType.Bit).Value = Sso.HasValue ? (object)Sso.Value : DBNull.Value;

        }

        public Observation Get(ObservationID obsId)
        {
            var sql =
@"
SELECT obs.*, s.*, r.*, p.*
FROM [dbo].[Observation] obs
LEFT OUTER JOIN ScanMap s ON s.inst = obs.inst AND s.obsID = obs.obsID
LEFT OUTER JOIN RasterMap r ON r.inst = obs.inst AND r.obsID = obs.obsID
LEFT OUTER JOIN Spectro p ON p.inst = obs.inst AND p.obsID = obs.obsID
WHERE (@inst IS NULL OR (obs.inst & @inst) > 0)
      AND obs.obsID = @obsID
ORDER BY obs.inst, obs.ObsID";

            var cmd = new SqlCommand(sql);

            cmd.Parameters.Add("@inst", SqlDbType.TinyInt).Value = obsId.Instrument == Lib.Instrument.None ? (object)DBNull.Value : (byte)obsId.Instrument;
            cmd.Parameters.Add("@obsID", SqlDbType.BigInt).Value = obsId.ID;

            return DbHelper.ExecuteCommandReader<Observation>(cmd).FirstOrDefault();
        }

        /// <summary>
        /// Returns observations by observations ID, limited to a set of instruments
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Observation> FindID()
        {
            if (ObservationID.Length == 0)
            {
                return new Observation[0];
            }

            var sql =
            @"
SELECT obs.*, s.*, r.*, p.*
FROM [dbo].[Observation] obs
LEFT OUTER JOIN ScanMap s ON s.inst = obs.inst AND s.obsID = obs.obsID
LEFT OUTER JOIN RasterMap r ON r.inst = obs.inst AND r.obsID = obs.obsID
LEFT OUTER JOIN Spectro p ON p.inst = obs.inst AND p.obsID = obs.obsID
WHERE {0}
      AND obs.obsID IN ({1})
      {2}
ORDER BY obs.inst, obs.ObsID";

            sql = String.Format(
                sql,
                GetFilterWhereConditions(),
                String.Join(", ", ObservationID),
                InstrumentModeFilter.GetSqlWhereConditions(InstrumentModeFilters));

            var cmd = new SqlCommand(sql);
            AppendFilterParameters(cmd);

            return DbHelper.ExecuteCommandReader<Observation>(cmd);
        }

        /// <summary>
        /// Returns observations by instrument and observation ID
        /// </summary>
        /// <param name="obsIds"></param>
        /// <returns></returns>
        public IEnumerable<Observation> FindID(IList<ObservationID> obsIds)
        {
            if (obsIds.Count == 0)
            {
                return new Observation[0];
            }

            var sql =
@"
SELECT obs.*, s.*, r.*, p.*
FROM [dbo].[Observation] obs
LEFT OUTER JOIN ScanMap s ON s.inst = obs.inst AND s.obsID = obs.obsID
LEFT OUTER JOIN RasterMap r ON r.inst = obs.inst AND r.obsID = obs.obsID
LEFT OUTER JOIN Spectro p ON p.inst = obs.inst AND p.obsID = obs.obsID
WHERE {0}
ORDER BY obs.inst, obs.ObsID";

            var idlist = String.Empty;
            for (int i = 0; i < obsIds.Count; i++)
            {
                if (i > 0)
                {
                    idlist += "OR";
                }

                idlist += String.Format("(obs.inst = {0} AND obs.obsID = {1})", (byte)obsIds[i].Instrument, obsIds[i].ID);
            }

            sql = String.Format(sql, idlist);

            var cmd = new SqlCommand(sql);

            return DbHelper.ExecuteCommandReader<Observation>(cmd);
        }

        public IEnumerable<Observation> FindEq()
        {
            var sql =
@"
SELECT obs.*, s.*, r.*, p.*
FROM [dbo].[FindObservationEq](@ra, @dec) ids
INNER JOIN [dbo].[Observation] obs WITH (FORCESEEK)
      ON obs.inst = ids.inst AND obs.obsID = ids.obsID
LEFT OUTER JOIN ScanMap s ON s.inst = obs.inst AND s.obsID = obs.obsID
LEFT OUTER JOIN RasterMap r ON r.inst = obs.inst AND r.obsID = obs.obsID
LEFT OUTER JOIN Spectro p ON p.inst = obs.inst AND p.obsID = obs.obsID
WHERE {0}
      {1}
ORDER BY obs.inst, obs.ObsID";

            sql = String.Format(
                sql, 
                GetFilterWhereConditions(),
                InstrumentModeFilter.GetSqlWhereConditions(InstrumentModeFilters));

            var cmd = new SqlCommand(sql);
            AppendFilterParameters(cmd);
            cmd.Parameters.Add("@ra", SqlDbType.Float).Value = Point.RA;
            cmd.Parameters.Add("@dec", SqlDbType.Float).Value = Point.Dec;

            return DbHelper.ExecuteCommandReader<Observation>(cmd);
        }

        public IEnumerable<Observation> FindRegionCone()
        {
            var sb = new ShapeBuilder();
            var circle = sb.CreateCircle(Point, Radius);
            this.Region = new Region(circle, false);

            return FindRegionIntersect();
        }

        private IEnumerable<Observation> FindRegionIntersect()
        {
            var sql =
@"
SELECT obs.*, s.*, r.*, p.*
FROM [dbo].[FindObservationRegionIntersect](@region) ids
INNER JOIN [dbo].[Observation] obs
    ON obs.inst = ids.inst AND obs.obsID = ids.obsID
LEFT OUTER JOIN ScanMap s ON s.inst = obs.inst AND s.obsID = obs.obsID
LEFT OUTER JOIN RasterMap r ON r.inst = obs.inst AND r.obsID = obs.obsID
LEFT OUTER JOIN Spectro p ON p.inst = obs.inst AND p.obsID = obs.obsID
WHERE {0}
      {1}
ORDER BY obs.inst, obs.ObsID";

            sql = String.Format(
                sql, 
                GetFilterWhereConditions(),
                InstrumentModeFilter.GetSqlWhereConditions(InstrumentModeFilters));

            var cmd = new SqlCommand(sql);
            AppendFilterParameters(cmd);
            cmd.Parameters.Add("@region", SqlDbType.VarBinary).Value = Region.ToSqlBytes().Value;

            return DbHelper.ExecuteCommandReader<Observation>(cmd);
        }
    }
}
