using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Herschel.Lib
{
    [Serializable]
    public class InstrumentModeFilter
    {
        public Instrument Instrument { get; set; }

        public PointingMode PointingMode { get; set; }

        public InstrumentMode InstrumentMode { get; set; }

        public InstrumentModeFilter()
        {
            Instrument = Lib.Instrument.Any;
            PointingMode = Lib.PointingMode.Any;
            InstrumentMode = Lib.InstrumentMode.Any;
        }

        public InstrumentModeFilter(Instrument instrument)
            :this()
        {
            Instrument = instrument;
        }

        public static InstrumentModeFilter HifiSinglePoint
        {
            get
            {
                return new InstrumentModeFilter()
                {
                    Instrument = Instrument.Hifi,
                    PointingMode = PointingMode.Pointed,
                    InstrumentMode = InstrumentMode.HifiSingleBand
                };
            }
        }

        public static InstrumentModeFilter PacsPhotometry
        {
            get
            {
                return new InstrumentModeFilter()
                {
                    Instrument = Instrument.Pacs,
                    PointingMode = PointingMode.Any,
                    InstrumentMode = InstrumentMode.Photometry
                };
            }
        }

        public static InstrumentModeFilter PacsRangeSpec
        {
            get
            {
                return new InstrumentModeFilter()
                {
                    Instrument = Instrument.Pacs,
                    PointingMode = PointingMode.Any,
                    InstrumentMode = InstrumentMode.PacsSpectroRange
                };
            }
        }

        public static InstrumentModeFilter PacsLineSpec
        {
            get
            {
                return new InstrumentModeFilter()
                {
                    Instrument = Instrument.Pacs,
                    PointingMode = PointingMode.Any,
                    InstrumentMode = InstrumentMode.PacsSpectroLine
                };
            }
        }

        public string GetSqlWhereCondition()
        {
            string sql = "";

            if (Instrument != Lib.Instrument.Any)
            {
                if (sql.Length > 0)
                {
                    sql += " AND ";
                }

                sql += "(obs.inst & {0} > 0)";
            }

            if (PointingMode != Lib.PointingMode.Any)
            {
                if (sql.Length > 0)
                {
                    sql += " AND ";
                }

                sql += "(obs.pointingMode & {1} > 0)";
            }

            if (InstrumentMode != Lib.InstrumentMode.Any)
            {
                if (sql.Length > 0)
                {
                    sql += " AND ";
                }

                sql += "(obs.instMode & {2} > 0)";
            }

            if (sql.Length > 0)
            {
                sql = "(" + sql + ")";
            }

            return String.Format(sql, (sbyte)Instrument, (sbyte)PointingMode, (int)InstrumentMode);
        }

        public static string GetSqlWhereConditions(InstrumentModeFilter[] filters)
        {
            var sql = new StringBuilder();

            foreach (var filter in filters)
            {
                if (sql.Length != 0)
                {
                    sql.AppendLine(" OR ");
                }

                sql.Append(filter.GetSqlWhereCondition());
            }

            if (sql.Length > 0)
            {
                sql.Insert(0, " AND (");
                sql.Append(")");
            }

            return sql.ToString();
        }
    }
}
