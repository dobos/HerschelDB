using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Herschel.Lib
{
    public interface IDatabaseTableObject
    {
        void LoadFromDataReader(IDataReader reader);
    }
}
