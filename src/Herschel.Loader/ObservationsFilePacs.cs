using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading.Tasks;
using Herschel.Lib;

namespace Herschel.Loader
{
    class ObservationsFilePacs : ObservationsFile
    {
        private static Regex SpecRangeRegex = new Regex(@"^([0-9\.]+)(?: \- ([0-9\.]+)){0,1}\ micron, ([0-9]+) repetition\(s\), ID: (.+)", RegexOptions.Multiline | RegexOptions.Compiled);
        private static Regex SpecRangePlusRegex = new Regex(@"^([0-9\.]+)-([0-9\.]+)plus([0-9\.]+)-([0-9\.]+)");

        protected override bool Parse(string[] parts, out Observation observation)
        {
            if (parts.Length == 15)
            {
                // Photometer
                var aor = parts[5];

                observation = new Observation()
                {
                    Instrument = Instrument.Pacs,
                    ObsID = long.Parse(parts[0]),
                    Type = ObservationType.Photometry,
                    Level = ParseObservationLevel(parts[12]),
                    InstrumentMode = ParseInstrumentMode(parts[1]),
                    PointingMode = ParsePointingMode(parts[10]),
                    Object = parts[13],
                    Calibration = aor.IndexOf("cal", StringComparison.InvariantCultureIgnoreCase) >= 0,

                    RA = -999,
                    Dec = -999,
                    PA = -999,
                    Aperture = -1,
                    FineTimeStart = -1,
                    FineTimeEnd = -1,
                    Repetition = (int)double.Parse(parts[9]),

                    ScanMap = new ScanMap()
                    {
                        AV = ParseMapScanSpeed(parts[14]),
                        Height = double.NaN,
                        Width = double.NaN,
                    },

                    AOR = aor,
                    AOT = parts[6],
                };
            }
            else if (parts.Length == 19)
            {
                // Spectrometer

                var aor = parts[7];

                observation = new Observation()
                {
                    Instrument = Instrument.Pacs,
                    ObsID = long.Parse(parts[0]),
                    Type = ObservationType.Spectroscopy,
                    Level = ParseObservationLevel(parts[15]),
                    InstrumentMode = ParseInstrumentMode(parts[5]) | ParseChoppingMode(parts[10]),
                    PointingMode = ParsePointingMode(parts[6]),
                    Object = parts[16],
                    Calibration = aor.IndexOf("cal", StringComparison.InvariantCultureIgnoreCase) >= 0,

                    RA = double.Parse(parts[1]),
                    Dec = double.Parse(parts[2]),
                    PA = double.Parse(parts[3]),
                    Aperture = -1,
                    FineTimeStart = -1,
                    FineTimeEnd = -1,
                    Repetition = -1,   // TODO: parse from spec info

                    RasterMap = new RasterMap()
                    {
                        Num = int.Parse(parts[12]) * int.Parse(parts[13]),
                        Step = double.Parse(parts[11]),
                        Line = int.Parse(parts[12]),
                        Column = int.Parse(parts[13]),
                    },

                    Spectro = new Spectro()
                    {
                        Num = int.Parse(parts[17]),
                        RangeID = parts[18],    
                    },

                    AOR = aor,
                    AOT = parts[8],
                };

                // --- parse spec range
                var specRange = parts[18];
                var m = SpecRangeRegex.Match(specRange);

                if (m.Success)
                {
                    // from - to
                    observation.Spectro.LambdaFrom = double.Parse(m.Groups[1].Value);

                    if (!String.IsNullOrWhiteSpace(m.Groups[2].Value))
                    {
                        observation.Spectro.LambdaTo = double.Parse(m.Groups[2].Value);
                    }
                    else
                    {
                        observation.Spectro.LambdaTo = observation.Spectro.LambdaFrom;
                    }

                    // repetitions
                    observation.Repetition = int.Parse(m.Groups[3].Value);

                    // if it covers two separate ranges
                    var mp = SpecRangePlusRegex.Match(m.Groups[4].Value);

                    if (mp.Success)
                    {
                        observation.Spectro.Lambda2From = double.Parse(mp.Groups[3].Value);
                        observation.Spectro.Lambda2To = double.Parse(mp.Groups[4].Value);

                        observation.Spectro.RangeID = "";
                    }
                    else
                    {
                        observation.Spectro.Lambda2From = -1;
                        observation.Spectro.Lambda2To = -1;
                    }
                }


            }
            else if (parts.Length == 14)
            {
                // Parallel
                var aor = parts[5];

                observation = new Observation()
                {
                    Instrument = Instrument.PacsSpireParallel,
                    ObsID = long.Parse(parts[0]),
                    Type = ObservationType.Photometry,
                    Level = ParseObservationLevel(parts[12]),
                    InstrumentMode = ParseInstrumentMode(parts[1]),
                    PointingMode = ParsePointingMode(parts[10]),
                    Object = parts[13],
                    Calibration = aor.IndexOf("cal", StringComparison.InvariantCultureIgnoreCase) >= 0,

                    RA = -999,
                    Dec = -999,
                    PA = -999,
                    Aperture = -1,
                    FineTimeStart = -1,
                    FineTimeEnd = -1,
                    Repetition = -1,       // TODO: missing

                    ScanMap = new ScanMap()
                    {
                        AV = ParseMapScanSpeed(parts[7]),
                        Height = double.Parse(parts[8]),
                        Width = double.Parse(parts[9]),
                    },

                    AOR = aor,
                    AOT = parts[6],
                };
            }
            else
            {
                throw new NotImplementedException();
            }

            return true;
        }

        protected override InstrumentMode ParseInstrumentMode(string value)
        {
            switch (value)
            {
                case "blue1":
                    return InstrumentMode.PacsPhotoBlue;
                case "blue2":
                    return InstrumentMode.PacsPhotoGreen;
                case "none":
                    return InstrumentMode.Pacs;     // TODO: calibration ?
                case "PacsRangeSpec":
                    return InstrumentMode.PacsSpectroRange;
                case "PacsLineSpec":
                    return InstrumentMode.PacsSpectroLine;
                default:
                    throw new NotImplementedException();
            }
        }

        protected override PointingMode ParsePointingMode(string value)
        {
            // TODO: raster mode is instrument (chopper) setting!

            switch (value)
            {
                case "Basic-fine":
                case "Pointed":
                    return PointingMode.Pointed;
                case "Line_scan":
                    return PointingMode.ScanLine;
                case "Nodding":
                    return PointingMode.Pointed | PointingMode.Nodding;
                case "Raster":
                    return PointingMode.Raster;
                case "Nodding-raster":
                    return PointingMode.Raster | PointingMode.Nodding;
                case "Mapping":
                    return PointingMode.Mapping;
                case "Pointed with dither":
                    return PointingMode.Pointed;    // only calibration, no special flag defined
                default:
                    throw new NotImplementedException();
            }
        }

        private InstrumentMode ParseChoppingMode(string value)
        {
            switch (value)
            {
                case "true":
                    return InstrumentMode.PacsChopperOn;
                case "false":
                    return InstrumentMode.PacsChopperOff;
                default:
                    throw new NotImplementedException();
            }
        }

        private double ParseMapScanSpeed(string value)
        {
            switch (value)
            {
                case "none":
                    return double.NaN;
                case "low":
                    return 10.0;
                case "medium":          // photo
                case "slow":            // parallel
                    return 20.0;
                case "high":            // photo
                case "fast":            // parallel
                    return 60;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
