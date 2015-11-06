using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Herschel.Loader
{
    // Used during load only
    public enum PointingObservationType : sbyte
    {
        // PACS
        PacsPhoto = 1,
        PacsSpectroRange = 2,
        PacsSpectroLine = 4,

        PacsSpectro = PacsSpectroRange | PacsSpectroLine,

        // SPIRE
        SpirePhotoLargeMap = 1,
        SpirePhotoSmallMap = 2,

        SpirePhoto = SpirePhotoLargeMap | SpirePhotoSmallMap,

        SpireSpectro1 = 4,
        SpireSpectro7 = 8,
        SpireSpectro64 = 16,
        SpireSpectroRaster = 32,

        SpireSpectro = SpireSpectro1 | SpireSpectro7 | SpireSpectro64 | SpireSpectroRaster,

        // HIFI
        HifiPoint = 1,
        HifiSpectralScan = 2,
        HifiMapping = 4,
    }
}
