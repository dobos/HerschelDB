using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Herschel.Lib
{
    public class Observation : IDatabaseTableObject
    {
        public Int64 ObsID { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            ObsID = reader.GetInt64(reader.GetOrdinal("obsID"));
        }
    }
}
