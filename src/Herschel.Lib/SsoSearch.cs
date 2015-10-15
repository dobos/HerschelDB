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
    public class SsoSearch
    {
        public ObservationID ObservationID { get; set; }

        public IEnumerable<Sso> Find()
        {
            var sql =
            @"
SELECT sso.*
FROM [dbo].[Sso] sso
WHERE (sso.inst & @inst) > 0
      AND sso.obsID = @obsID
ORDER BY sso.ssoID";

            var cmd = new SqlCommand(sql);
            cmd.Parameters.Add("@inst", SqlDbType.TinyInt).Value = (byte)ObservationID.Instrument;
            cmd.Parameters.Add("@obsID", SqlDbType.BigInt).Value = ObservationID.ID;

            return DbHelper.ExecuteCommandReader<Sso>(cmd);
        }
    }
}
