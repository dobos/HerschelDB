using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Herschel.Lib;

namespace Herschel.Loader
{
    class ObservationsFileHifi : ObservationsFile
    {
        protected override bool Parse(string[] parts, out Observation observation)
        {
            var aor = parts[5];

            observation = new Observation()
            {
                Instrument = Instrument.Hifi,
                ObsID = long.Parse(parts[0]),
                Type = ObservationType.Spectroscopy,
                Level = ParseObservationLevel(parts[9]),
                InstrumentMode = ParseInstrumentMode(parts[2]),
                PointingMode = ParsePointingMode(parts[2]),
                Band = parts[1],
                Object = parts[10],
                Calibration = aor.IndexOf("cal", StringComparison.InvariantCultureIgnoreCase) >= 0,

                RA = double.Parse(parts[34]),
                Dec = double.Parse(parts[35]),
                PA = double.Parse(parts[43]),
                Aperture = double.Parse(parts[14]),     // HPBW
                FineTimeStart = long.Parse(parts[21]),
                FineTimeEnd = long.Parse(parts[22]),
                Repetition = 1,         // TODO

                ScanMap = new ScanMap()
                {
                    AV = -1,
                    Height = double.Parse(parts[12]),
                    Width = double.Parse(parts[11])
                },

                AOR = aor,
                AOT = parts[6],
            };

            // Calculate spectro range from WCS header

            var naxis3 = double.Parse(parts[27]);
            var cdelt3 = double.Parse(parts[30]);
            var crpix3 = double.Parse(parts[33]);
            var crval3 = double.Parse(parts[36]);

            var lambda1 = crval3 - crpix3 * cdelt3;
            var lambda2 = crval3 + (naxis3 - crpix3) * cdelt3;

            observation.Spectro = new Spectro()
            {
                RangeID = observation.Band,
                LambdaFrom = lambda1,
                LambdaTo = lambda2,
            };

            return true;
        }

        protected override InstrumentMode ParseInstrumentMode(string value)
        {
            var hifisingle = InstrumentMode.Hifi | InstrumentMode.Spectroscopy | InstrumentMode.HifiSingleBand;
            var hifiscan = InstrumentMode.Hifi | InstrumentMode.Spectroscopy | InstrumentMode.HifiSpectralScan;

            switch (value)
            {
                case "HifiPointModePositionSwitch":         // AOT I-1
                    return hifisingle |
                        InstrumentMode.HifiCalibrationOffPosition;
                case "HifiPointModeDBS":                    // AOT I-2
                    return hifisingle |
                        InstrumentMode.HifiCalibrationDualBeamSwitchSlow;
                case "HifiPointModeFastDBS":
                    return hifisingle |
                        InstrumentMode.HifiCalibrationDualBeamSwitchFast;
                case "HifiPointModeFSwitch":                // AOT I-3
                    return hifisingle |
                        InstrumentMode.HifiCalibrationFrequencySwitch |
                        InstrumentMode.HifiCalibrationOffPosition;
                case "HifiPointModeFSwitchNoRef":
                    return hifisingle |
                        InstrumentMode.HifiCalibrationFrequencySwitch;
                case "HifiPointModeLoadChop":               // AOT I-4
                    return hifisingle |
                        InstrumentMode.HifiCalibrationLoadChop |
                        InstrumentMode.HifiCalibrationOffPosition;
                case "HifiPointModeLoadChopNoRef":
                    return hifisingle |
                            InstrumentMode.HifiCalibrationLoadChop;

                case "HifiMappingModeOTF":                  // AOT II-1
                    return hifisingle |
                        InstrumentMode.HifiCalibrationOffPosition;
                case "HifiMappingModeDBSRaster":            // AOT II-2
                    return hifisingle |
                        InstrumentMode.HifiCalibrationDualBeamSwitchSlow |
                        InstrumentMode.HifiCalibrationDualBeamSwitchRaster;
                case "HifiMappingModeDBSCross":             // AOT II-2X
                    return hifisingle |
                        InstrumentMode.HifiCalibrationDualBeamSwitchSlow |
                        InstrumentMode.HifiCalibrationDualBeamSwitchCross;
                case "HifiMappingModeFastDBSRaster":        // AOT II-2
                    return hifisingle |
                        InstrumentMode.HifiCalibrationDualBeamSwitchFast |
                        InstrumentMode.HifiCalibrationDualBeamSwitchRaster;
                case "HifiMappingModeFastDBSCross":        // AOT II-2X
                    return hifisingle | InstrumentMode.HifiCalibrationDualBeamSwitchFast |
                        InstrumentMode.HifiCalibrationDualBeamSwitchCross;
                case "HifiMappingModeFSwitchOTF":           // AOT II-3
                    return hifisingle |
                        InstrumentMode.HifiCalibrationOffPosition |
                        InstrumentMode.HifiCalibrationFrequencySwitch;
                case "HifiMappingModeLoadChopOTF":          // AOT II-4
                    return hifisingle |
                       InstrumentMode.HifiCalibrationOffPosition |
                       InstrumentMode.HifiCalibrationLoadChop;
                case "HifiMappingModeLoadChopOTFNoRef":
                    return hifisingle |
                        InstrumentMode.HifiCalibrationLoadChop;

                case "HifiSScanModeDBS":
                    return hifiscan |
                        InstrumentMode.HifiCalibrationDualBeamSwitchSlow;
                case "HifiSScanModeFastDBS":
                    return hifiscan |
                        InstrumentMode.HifiCalibrationDualBeamSwitchFast;
                case "HifiSScanModeFSwitch":
                    return hifiscan |
                        InstrumentMode.HifiCalibrationOffPosition |
                        InstrumentMode.HifiCalibrationFrequencySwitch;
                case "HifiSScanModeFSwitchNoRef":
                    return hifiscan |
                        InstrumentMode.HifiCalibrationFrequencySwitch;
                case "HifiSScanModeLoadChop":
                    return hifiscan |
                        InstrumentMode.HifiCalibrationOffPosition |
                        InstrumentMode.HifiCalibrationLoadChop;
                case "HifiSScanModeLoadChopNoRef":
                    return hifiscan |
                        InstrumentMode.HifiCalibrationLoadChop;
                default:
                    throw new NotImplementedException();
            }
        }

        protected override PointingMode ParsePointingMode(string value)
        {
            // TODO: verify, e.g. DBS raster might be raster instead of mapping etc.

            switch (value)
            {
                case "HifiPointModePositionSwitch":
                case "HifiPointModeDBS":
                case "HifiPointModeFSwitch":
                case "HifiPointModeFSwitchNoRef":
                case "HifiPointModeFastDBS":
                case "HifiPointModeLoadChop":
                case "HifiPointModeLoadChopNoRef":
                    return PointingMode.Pointed;

                case "HifiMappingModeDBSCross":
                case "HifiMappingModeDBSRaster":
                case "HifiMappingModeFSwitchOTF":
                case "HifiMappingModeFastDBSCross":
                case "HifiMappingModeFastDBSRaster":
                case "HifiMappingModeLoadChopOTF":
                case "HifiMappingModeLoadChopOTFNoRef":
                case "HifiMappingModeOTF":
                    return PointingMode.Mapping;
                
                case "HifiSScanModeDBS":
                case "HifiSScanModeFSwitch":
                case "HifiSScanModeFSwitchNoRef":
                case "HifiSScanModeFastDBS":
                case "HifiSScanModeLoadChop":
                case "HifiSScanModeLoadChopNoRef":
                    return PointingMode.Pointed;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
