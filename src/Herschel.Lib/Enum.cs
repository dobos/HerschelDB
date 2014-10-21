using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Herschel.Lib
{
    public enum DetectorFootprint
    {
        None = 0,
        PacsPhoto = 1,
        PacsSpectro = 2,
        SpirePhoto = 3,
        SpireSpectro = 4
        // TODO: Hifi
    }

    [Flags]
    public enum Instrument : byte
    {
        None = 0,
        PacsPhoto = 1,
        PacsSpectro = 2,
        SpirePhoto = 4,
        SpireSpectro = 8,
        Parallel = 16,
        Hifi = 32
    }

    public enum ObservationSearchMethod
    {
        Point,
        Intersect,
        Cover
    }
}
