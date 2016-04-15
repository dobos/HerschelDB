using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Jhu.Spherical;

namespace Herschel.Debug
{
    class Program
    {
        static void Main(string[] args)
        {
            LoopFinderProblem();
        }

        static void LoopFinderProblem()
        {
            string sql;

            using (var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["Herschel"].ConnectionString))
            {
                cn.Open();

                sql = @"
SELECT inst, obsID, region 
FROM Observation
WHERE region IS NOT NULL";

                /*
                sql = @"
SELECT inst, obsID, region 
FROM Observation
WHERE inst = 1 AND obsID = 1342247648";*/

                using (var cmd = new SqlCommand(sql, cn))
                {
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var inst = dr.GetByte(0);
                            var obsID = dr.GetInt64(1);

                            Console.Write("{0} {1} ", inst, obsID);

                            var region = Region.FromSqlBytes(dr.GetSqlBytes(2));

                            try
                            {
                                var outline = region.Outline;

                                for (int j = 0; j < outline.LoopList.Count; j++)
                                {
                                    var arcs = new List<Arc>(outline.LoopList[j].ArcList);
                                }

                                Console.WriteLine(" OK");
                            }
                            catch (Exception)
                            {
                                File.AppendAllText("error.txt",
                                    String.Format("{0} {1}\r\n", inst, obsID));

                                Console.WriteLine(" ERROR");
                            }
                        }
                    }
                }
            }
        }
    }
}
