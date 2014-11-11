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
        Pacs = 1,
        Spire = 2,
        PacsSpireParallel = Pacs | Spire,
        Hifi = 4
    }

    [Flags]
    public enum PacsObsType : byte
    {
        Photo = 1,
        SpectroRange = 2,
        SpectroLine = 4,

        Spectro = SpectroRange | SpectroLine
    }

    [Flags]
    public enum SpireObsType : byte
    {
        PhotoLargeMap = 1,
        PhotoSmallMap = 2,
        
        Photo = PhotoLargeMap | PhotoSmallMap,
        
        Spectro1 = 4,
        Spectro7 = 8,
        Spectro64 = 16,

        Spectro = Spectro1 | Spectro7 | Spectro64,
    }

    [Flags]
    public enum HifiObsType : byte
    {
        Point = 1,
        SpectralScan = 2,
        Mapping = 4,
    }

    public enum ObservationSearchMethod
    {
        Point,
        Intersect,
        Cover
    }
}
