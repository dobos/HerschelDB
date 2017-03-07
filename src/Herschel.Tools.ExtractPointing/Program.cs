using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace HerschelExtractPointing
{
    class Program
    {
        static void Main(string[] args)
        {

            var sql = @"
SELECT
	o.obsID, p.fineTime, c.x, c.y, c.z
FROM Observation o
INNER JOIN load.Pointing p
	ON o.inst = p.inst AND o.obsID = p.obsID
CROSS APPLY dbo.GetDetectorCornersXyz(p.ra, p.dec, p.pa, 'SpirePhoto') c
WHERE o.inst = 2 AND o.pointingMode IN (8, 16)
ORDER BY obsID, fineTime, c.id";

            using (var outfile = new FileStream(@"C:\data\dobos\data\herschel_pointing\spire\spire_all.dat", FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                using (var writer = new BinaryWriter(outfile))
                {
                    using (var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["Herschel"].ConnectionString))
                    {
                        cn.Open();

                        using (var cmd = new SqlCommand(sql, cn))
                        {
                            cmd.CommandTimeout = 3600;      // because of the sort

                            using (var dr = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                            {
                                int q = 0;
                                while (dr.Read())
                                {
                                    int o = 0;
                                    var obsid = dr.GetInt64(o++);
                                    var fineTime = dr.GetInt64(o++);
                                    var cx = dr.GetDouble(o++);
                                    var cy = dr.GetDouble(o++);
                                    var cz = dr.GetDouble(o++);

                                    writer.Write(obsid);
                                    writer.Write(fineTime);
                                    writer.Write(cx);
                                    writer.Write(cy);
                                    writer.Write(cz);

                                    q++;

                                    if (q % 1000 == 0)
                                    {
                                        Console.Write(".");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
