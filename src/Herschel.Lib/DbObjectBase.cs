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
    public abstract class DbObjectBase
    {
        protected string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["everebelyi"].ConnectionString; }
        }

        protected SqlConnection OpenConnection()
        {
            var cn = new SqlConnection(ConnectionString);
            cn.Open();

            return cn;
        }

        protected IEnumerable<T> ExecuteCommandReader<T>(SqlCommand cmd)
            where T : IDatabaseTableObject, new()
        {
            var cn = OpenConnection();

            cmd.Connection = cn;
            cmd.CommandTimeout = 30;

            var dr = cmd.ExecuteReader(CommandBehavior.SequentialAccess | CommandBehavior.CloseConnection);

            return dr.AsEnumerable<T>();
        }

        protected void ExecuteCommandNonQuery(SqlCommand cmd)
        {
            using (var cn = OpenConnection())
            {
                cmd.Connection = cn;
                cmd.ExecuteNonQuery();
            }
        }
    }
}
