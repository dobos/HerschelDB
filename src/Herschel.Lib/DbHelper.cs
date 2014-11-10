using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Threading.Tasks;

namespace Herschel.Lib
{
    public static class DbHelper
    {
        private static string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["Herschel"].ConnectionString; }
        }

        public static SqlConnection OpenConnection()
        {
            var cn = new SqlConnection(ConnectionString);
            cn.Open();

            return cn;
        }

        public static IEnumerable<T> ExecuteCommandReader<T>(SqlCommand cmd)
            where T : IDatabaseTableObject, new()
        {
            var cn = OpenConnection();

            cmd.Connection = cn;
            cmd.CommandTimeout = 30;

            var dr = cmd.ExecuteReader(CommandBehavior.SequentialAccess | CommandBehavior.CloseConnection);

            return dr.AsEnumerable<T>();
        }

        public static void ExecuteCommandNonQuery(SqlCommand cmd)
        {
            using (var cn = OpenConnection())
            {
                cmd.Connection = cn;
                cmd.ExecuteNonQuery();
            }
        }

        public static string[] SplitQuery(string sql)
        {
            int i = 0;
            var res = new List<string>();

            while (true)
            {
                // Find the next GO statement
                var j = sql.IndexOf("GO", i, StringComparison.InvariantCultureIgnoreCase);

                if (j < 0)
                {
                    AddToScriptIfValid(res, sql.Substring(i));
                    break;
                }
                else
                {
                    AddToScriptIfValid(res, sql.Substring(i, j - i));
                }

                i = j + 2;
            }

            return res.ToArray();
        }

        private static void AddToScriptIfValid(List<string> script, string sql)
        {
            if (!String.IsNullOrWhiteSpace(sql))
            {
                script.Add(sql.Trim());
            }
        }
    }
}
