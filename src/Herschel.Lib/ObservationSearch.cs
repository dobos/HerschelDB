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
        private ObservationSearchMethod searchMethod;
        private InstrumentModeFilter[] instrumentModeFilters;
        private bool? calibration;
        private bool? failed;
        private bool? sso;
        private long[] observationID;
        private Cartesian point;
        private double radius;
        private Region region;
        private FineTime fineTimeStart;
        private FineTime fineTimeEnd;

        public ObservationSearchMethod SearchMethod
        {
            get { return searchMethod; }
            set { searchMethod = value; }
        }

        public InstrumentModeFilter[] InstrumentModeFilters
        {
            get { return instrumentModeFilters; }
            set { instrumentModeFilters = value; }
        }

        public bool? Calibration
        {
            get { return calibration; }
            set { calibration = value; }
        }

        public bool? Failed
        {
            get { return failed; }
            set { failed = value; }
        }

        public bool? Sso
        {
            get { return sso; }
            set { sso = value; }
        }

        public long[] ObservationID
        {
            get { return observationID; }
            set { observationID = value; }
        }

        public Cartesian Point
        {
            get { return point; }
            set { point = value; }
        }

        public double Radius
        {
            get { return radius; }
            set { radius = value; }
        }

        public Region Region
        {
            get { return region; }
            set { region = value; }
        }

        public FineTime FineTimeStart
        {
            get { return fineTimeStart; }
            set { fineTimeStart = value; }
        }

        public FineTime FineTimeEnd
        {
            get { return fineTimeEnd; }
            set { fineTimeEnd = value; }
        }


        public IEnumerable<Observation> Find()
        {
            switch (searchMethod)
            {
                case ObservationSearchMethod.ID:
                    return FindID();
                case ObservationSearchMethod.Point:
                    return FindEq();
                case ObservationSearchMethod.Cone:
                    return FindRegionCone();
                case ObservationSearchMethod.Intersect:
                    return FindRegionIntersect();
                case ObservationSearchMethod.Contain:
                    return FindRegionContain();
                default:
                    throw new NotImplementedException();
            }
        }

        private string GetFilterWhereConditions()
        {
            string sql = @"
((@fineTimeStart IS NULL OR @fineTimeStart <= obs.fineTimeStart)
 AND (@fineTimeEnd IS NULL OR @fineTimeEnd >= obs.fineTimeEnd)
 AND (obs.calibration = @calibration OR @calibration IS NULL)
 AND (obs.failed = @failed OR @failed IS NULL)
 AND (obs.sso = @sso OR @sso IS NULL))
";

            return sql;
        }

        private void AppendFilterParameters(SqlCommand cmd)
        {
            cmd.Parameters.Add("@fineTimeStart", SqlDbType.Float).Value = fineTimeStart.IsUndefined ? (object)DBNull.Value : fineTimeStart.Value;
            cmd.Parameters.Add("@fineTimeEnd", SqlDbType.Float).Value = fineTimeEnd.IsUndefined ? (object)DBNull.Value : fineTimeEnd.Value;
            cmd.Parameters.Add("@calibration", SqlDbType.Bit).Value = calibration.HasValue ? (object)calibration.Value : DBNull.Value;
            cmd.Parameters.Add("@failed", SqlDbType.Bit).Value = failed.HasValue ? (object)failed.Value : DBNull.Value;
            cmd.Parameters.Add("@sso", SqlDbType.Bit).Value = sso.HasValue ? (object)sso.Value : DBNull.Value;

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
            if (observationID.Length == 0)
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
                String.Join(", ", observationID),
                InstrumentModeFilter.GetSqlWhereConditions(instrumentModeFilters));

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
                InstrumentModeFilter.GetSqlWhereConditions(instrumentModeFilters));

            var cmd = new SqlCommand(sql);
            AppendFilterParameters(cmd);
            cmd.Parameters.Add("@ra", SqlDbType.Float).Value = Point.RA;
            cmd.Parameters.Add("@dec", SqlDbType.Float).Value = Point.Dec;

            return DbHelper.ExecuteCommandReader<Observation>(cmd);
        }

        public Region GetSearchRegion()
        {
            switch (searchMethod)
            {
                case ObservationSearchMethod.Contain:
                case ObservationSearchMethod.Intersect:
                    return region;
                case ObservationSearchMethod.Cone:
                    var sb = new ShapeBuilder();
                    var circle = sb.CreateCircle(Point, radius);
                    return new Region(circle, false);
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        public IEnumerable<Observation> FindRegionCone()
        {
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
                InstrumentModeFilter.GetSqlWhereConditions(instrumentModeFilters));

            var cmd = new SqlCommand(sql);
            AppendFilterParameters(cmd);
            cmd.Parameters.Add("@region", SqlDbType.VarBinary).Value = GetSearchRegion().ToSqlBytes().Value;

            return DbHelper.ExecuteCommandReader<Observation>(cmd);
        }

        private IEnumerable<Observation> FindRegionContain()
        {
            var sql =
@"
SELECT obs.*, s.*, r.*, p.*
FROM [dbo].[FindObservationRegionContain](@region) ids
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
                InstrumentModeFilter.GetSqlWhereConditions(instrumentModeFilters));

            var cmd = new SqlCommand(sql);
            AppendFilterParameters(cmd);
            cmd.Parameters.Add("@region", SqlDbType.VarBinary).Value = GetSearchRegion().ToSqlBytes().Value;

            return DbHelper.ExecuteCommandReader<Observation>(cmd);
        }
    }
}
