using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Herschel.Lib;

namespace Herschel.Loader
{
    class ObservationsFileSpire : ObservationsFile
    {
        protected override bool Parse(string[] parts, out Observation observation)
        {
            if (parts.Length == 19)
            {
                // Photometer
                var aor = parts[7];

                observation = new Observation()
                {
                    Instrument = Instrument.Spire,
                    ObsID = long.Parse(parts[0]),
                    Type = ObservationType.Photometry,
                    Level = ParseObservationLevel(parts[14]),
                    InstrumentMode = ParseInstrumentMode(parts[5]),
                    PointingMode = ParsePointingMode(parts[9]),
                    Band = "",
                    Object = parts[16],
                    Calibration = aor.IndexOf("cal", StringComparison.InvariantCultureIgnoreCase) >= 0,
                    RA = double.Parse(parts[1]),
                    Dec = double.Parse(parts[2]),
                    PA = double.Parse(parts[3]),
                    Aperture = -1,
                    FineTimeStart = -1,
                    FineTimeEnd = -1,
                    Repetition = (int)double.Parse(parts[17]),

                    ScanMap = new ScanMap()
                    {
                        AV = double.NaN,            // TODO: estimate from pointings
                        Height = double.Parse(parts[13]),
                        Width = double.Parse(parts[12]),
                    },

                    AOR = aor,
                    AOT = parts[8],
                };
            }
            else if (parts.Length == 20)
            {
                // Spectroscopy
                var aor = parts[7];

                observation = new Observation()
                {
                    Instrument = Instrument.Spire,
                    ObsID = long.Parse(parts[0]),
                    Type = ObservationType.Spectroscopy,
                    Level = ParseObservationLevel(parts[15]),
                    InstrumentMode = ParseInstrumentMode(parts[5]) |
                                     ParseSampling(parts[11]) |
                                     ParseResolution(parts[19]),
                    PointingMode = ParsePointingMode(parts[9]),
                    Band = "",
                    Object = parts[17],
                    Calibration = aor.IndexOf("cal", StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                                  aor.IndexOf("dark sky", StringComparison.InvariantCultureIgnoreCase) >= 0,

                    RA = double.Parse(parts[1]),
                    Dec = double.Parse(parts[2]),
                    PA = double.Parse(parts[3]),
                    Aperture = -1,      // TODO
                    FineTimeStart = -1,
                    FineTimeEnd = -1,
                    Repetition = (int)double.Parse(parts[18]),

                    ScanMap = new ScanMap()
                    {
                        AV = double.NaN,      // TODO: estimate from pointings
                        Height = double.Parse(parts[13]),
                        Width = double.Parse(parts[12]),
                    },

                    RasterMap = new RasterMap()
                    {
                        Num = int.Parse(parts[14]),
                        Step = double.NaN,
                        Line = (int)double.Parse(parts[12]),
                        Column = (int)double.Parse(parts[13]),
                    },

                    AOR = aor,
                    AOT = parts[8],
                };

            }
            else if (parts.Length == 9)
            {
                // Parallel mode

                var aor = parts[4];

                observation = new Observation()
                {
                    Instrument = Lib.Instrument.Spire,
                    ObsID = long.Parse(parts[0]),
                    Type = ObservationType.Photometry,
                    Level = ParseObservationLevel(parts[7]),
                    InstrumentMode = ParseInstrumentMode(parts[2]),
                    PointingMode = Lib.PointingMode.PacsSpireParallel,
                    Band = "",
                    Object = parts[8],
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
                        AV = -1,
                        Height = -1,
                        Width = -1,
                    },

                    AOR = aor,
                    AOT = parts[5]
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
                case "SpirePhotoSmallScan":
                    return InstrumentMode.SpirePhotoSmallScan;
                case "SpirePhotoPointJiggle":
                    return InstrumentMode.SpirePhotoPointJiggle;
                case "SpirePhotoLargeScan":
                    return InstrumentMode.SpirePhotoLargeScan;
                case "SpirePhotoSmall":         // calibration only
                case "SpirePhotoSample":        // calibration only
                    return InstrumentMode.SpirePhoto;
                case "SpirePacsParallel":
                    return InstrumentMode.SpirePhotoLargeScan | InstrumentMode.Parallel;
                case "SpireSpectroPoint":
                    return InstrumentMode.SpireSpectroPoint;
                case "SpireSpectroRaster":
                    return InstrumentMode.SpireSpectroRaster;
                case "SpireSpectroScalOn":
                case "SpireSpectroScalOff":
                case "SpireSpectroSample":
                case "SpireSpectroPeakup":
                    return InstrumentMode.SpireSpectro;     // calibration only
                default:
                    throw new NotImplementedException();
            }
        }

        private InstrumentMode ParseSampling(string value)
        {
            switch (value)
            {
                case "none":
                    return 0;
                case "sparse":
                    return InstrumentMode.SpireSpectroSamplingSparse;
                case "intermediate":
                    return InstrumentMode.SpireSpectroSamplingIntermediate;
                case "full":
                    return InstrumentMode.SpireSpectroSamplingFull;
                default:
                    throw new NotImplementedException();
            }
        }

        private InstrumentMode ParseResolution(string value)
        {
            switch (value)
            {
                case "LR":
                    return InstrumentMode.SpireSpectroResolutionLow;
                case "MR":
                    return InstrumentMode.SpireSpectroResolutionMedium;
                case "HR":
                    return InstrumentMode.SpireSpectroResolutionHigh;
                case "H+LR":
                    return InstrumentMode.SpireSpectroResolutionLowHigh;
                case "none":
                case "CR":
                    return 0;       // calibration only
                default:
                    throw new NotImplementedException();
            }
        }

        protected override PointingMode ParsePointingMode(string value)
        {
            switch (value)
            {
                case "Basic-fine":
                    return PointingMode.Pointed;
                case "Line_scan":
                    return PointingMode.ScanLine;
                case "Cross_scan":
                    return PointingMode.ScanCross;
                case "Nodding":
                    return PointingMode.Pointed | PointingMode.Nodding;
                case "Custom-map-pointing":
                    // TODO: raster mode is instrument (chopper) setting!
                    return PointingMode.Raster;
                case "No-pointing":
                    return PointingMode.None;           // calibration only
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
                case "medium":
                    return 20.0;
                case "high":
                    return 60;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
