using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Herschel.Lib
{
    [Flags]
    public enum Instrument : sbyte
    {
        None = 0,
        Pacs = 1,
        Spire = 2,
        PacsSpireParallel = 4,
        Hifi = 8
    }

    /// <summary>
    /// Observation final processing level
    /// </summary>
    public enum ObservationLevel : sbyte
    {
        Created = -1,
        Level0 = 0,
        Level0_5 = 5,
        Level1 = 10,
        Level2 = 20,
        Level2_5 = 25,
        Level3 = 30,
    }

    /// <summary>
    /// Observation coverage mode
    /// </summary>
    public enum ObservationMode : sbyte
    {
        Pointed = 1,
        Raster = 2,
        Mapping = 3,
        Parallel = 4,
    }

    [Flags]
    public enum PointingMode : short
    {
        // 0-1: Telescope pointing mode
        // 2:   Nodding

        Pointed = 0x0000,
        LineScan = 0x0001,
        CrossScan = 0x0002,
        Raster = 0x0003,

        Nodding = 0x0004,
    }

    [Flags]
    public enum InstrumentMode : short
    {
        // 0-3: Instrument
        //      0: PACS
        //      1: SPIRE
        //      2: PACS/SPIRE Parallel
        //      3: HIFI
        // 4:   Photometry
        // 5:   Spectroscopy

        Pacs = 0x0001,
        Spire = 0x0002,
        PacsSpireParallel = Pacs | Spire | 0x0004,
        Hifi = 8,

        Photometry = 0x0010,
        Spectroscopy = 0x0020,

        // *** PACS specific settings (band, spec mode, chopper)

        // 8:   PACS blue/green photometry
        // 9:   PACS line/range spectroscopy

        PacsPhotoBlue = Pacs | Photometry | 0x0000,
        PacsPhotoGreen = Pacs | Photometry | 0x0100,
        PacsSpectroRange = Pacs | Spectroscopy | 0x0000,
        PacsSpectroLine = Pacs | Spectroscopy | 0x0200,

        PacsChopperOff = 0x0000,
        PacsChopperOn = 0x0400,

        // *** SPIRE specific settings (BSM)

        SpirePhoto = Spire | Photometry,
        SpireSpectro = Spire | Spectroscopy,

        SpireJiggleOff = 0x0000,
        SpireJiggleOn = 0x0100,

        SpireSamplingSparse = 0x0200,
        SpireSamplingIntermediate = 0x0400,
        SpireSamplingFull = 0x0600,

        // *** HIFI specific settings (DBS,)

        //  8:    Single band spectrum
        //  9:    Spectral scan
        // 10:    Calibration done with OFF position
        // 11-12: Calibration done with Dual Beam Switch
        // 13:    Calibration done with Frequency Switch
        // 14:    Calibration done with Load target
        
        HifiSingleBand = 0x0100,
        HifiSpectralScan = 0x0200,

        HifiCalibrationOffPosition = 0x0400,                // Off-point calibration reference used
        HifiCalibrationDualBeamSwitchSlow = 0x0800,         // DBS slow
        HifiCalibrationDualBeamSwitchFast = 0x1000,         // DBS fast
        HifiCalibrationFrequencySwitch = 0x2000,            // Frequency switch calibration
        HifiCalibrationLoadChop = 0x4000,                   // Load chop calibration
    }

    // ----------------------------------------

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
