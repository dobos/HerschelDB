using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Herschel.Lib
{
    [Flags]
    public enum Instrument : byte
    {
        None = 0,
        Pacs = 1,
        Spire = 2,
        Parallel = 4,
        Hifi = 8
    }

    public enum ObservationSearchMethod
    {
        Point,
        Intersect,
        Cover
    }
}
