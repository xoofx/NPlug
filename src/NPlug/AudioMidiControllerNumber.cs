// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

/// <summary>
/// Controller Numbers (MIDI)
/// </summary>
public enum AudioMidiControllerNumber
{
    /// <summary>
    /// Bank Select MSB
    /// </summary>
    BankSelectMSB = 0,

    /// <summary>
    /// Modulation Wheel
    /// </summary>
    ModWheel = 1,

    /// <summary>
    /// Breath controller
    /// </summary>
    Breath = 2,

    /// <summary>
    /// Foot Controller
    /// </summary>
    Foot = 4,

    /// <summary>
    /// Portamento Time
    /// </summary>
    PortamentoTime = 5,

    /// <summary>
    /// Data Entry MSB
    /// </summary>
    DataEntryMSB = 6,

    /// <summary>
    /// Channel Volume (formerly Main Volume)
    /// </summary>
    Volume = 7,

    /// <summary>
    /// Balance
    /// </summary>
    Balance = 8,

    /// <summary>
    /// Pan
    /// </summary>
    Pan = 10,

    /// <summary>
    /// Expression
    /// </summary>
    Expression = 11,

    /// <summary>
    /// Effect Control 1
    /// </summary>
    Effect1 = 12,

    /// <summary>
    /// Effect Control 2
    /// </summary>
    Effect2 = 13,

    /// <summary>
    /// General Purpose Controller #1
    /// </summary>
    GeneralPurposeController1 = 16,

    /// <summary>
    /// General Purpose Controller #2
    /// </summary>
    GeneralPurposeController2 = 17,

    /// <summary>
    /// General Purpose Controller #3
    /// </summary>
    GeneralPurposeController3 = 18,

    /// <summary>
    /// General Purpose Controller #4
    /// </summary>
    GeneralPurposeController4 = 19,

    /// <summary>
    /// Bank Select LSB
    /// </summary>
    BankSelectLSB = 32,

    /// <summary>
    /// Data Entry LSB
    /// </summary>
    DataEntryLSB = 38,

    /// <summary>
    /// Damper Pedal On/Off (Sustain)
    /// </summary>
    SustainOnOff = 64,

    /// <summary>
    /// Portamento On/Off
    /// </summary>
    PortamentoOnOff = 65,

    /// <summary>
    /// Sustenuto On/Off
    /// </summary>
    SustenutoOnOff = 66,

    /// <summary>
    /// Soft Pedal On/Off
    /// </summary>
    SoftPedalOnOff = 67,

    /// <summary>
    /// Legato FootSwitch On/Off
    /// </summary>
    LegatoFootSwitchOnOff = 68,

    /// <summary>
    /// Hold 2 On/Off
    /// </summary>
    Hold2OnOff = 69,

    /// <summary>
    /// Sound Variation
    /// </summary>
    SoundVariation = 70,

    /// <summary>
    /// Filter Cutoff (Timbre/Harmonic Intensity)
    /// </summary>
    FilterCutoff = 71,

    /// <summary>
    /// Release Time
    /// </summary>
    ReleaseTime = 72,

    /// <summary>
    /// Attack Time
    /// </summary>
    AttackTime = 73,

    /// <summary>
    /// Filter Resonance (Brightness)
    /// </summary>
    FilterResonance = 74,

    /// <summary>
    /// Decay Time
    /// </summary>
    DecayTime = 75,

    /// <summary>
    /// Vibrato Rate
    /// </summary>
    VibratoRate = 76,

    /// <summary>
    /// Vibrato Depth
    /// </summary>
    VibratoDepth = 77,

    /// <summary>
    /// Vibrato Delay
    /// </summary>
    VibratoDelay = 78,

    /// <summary>
    /// undefined
    /// </summary>
    SoundController10 = 79,

    /// <summary>
    /// General Purpose Controller #5
    /// </summary>
    GeneralPurposeController5 = 80,

    /// <summary>
    /// General Purpose Controller #6
    /// </summary>
    GeneralPurposeController6 = 81,

    /// <summary>
    /// General Purpose Controller #7
    /// </summary>
    GeneralPurposeController7 = 82,

    /// <summary>
    /// General Purpose Controller #8
    /// </summary>
    GeneralPurposeController8 = 83,

    /// <summary>
    /// Portamento Control
    /// </summary>
    PortamentoControl = 84,

    /// <summary>
    /// Effect 1 Depth (Reverb Send Level)
    /// </summary>
    Effect1Depth = 91,

    /// <summary>
    /// Effect 2 Depth (Tremolo Level)
    /// </summary>
    Effect2Depth = 92,

    /// <summary>
    /// Effect 3 Depth (Chorus Send Level)
    /// </summary>
    Effect3Depth = 93,

    /// <summary>
    /// Effect 4 Depth (Delay/Variation/Detune Level)
    /// </summary>
    Effect4Depth = 94,

    /// <summary>
    /// Effect 5 Depth (Phaser Level)
    /// </summary>
    Effect5Depth = 95,

    /// <summary>
    /// Data Increment (+1)
    /// </summary>
    DataIncrement = 96,

    /// <summary>
    /// Data Decrement (-1)
    /// </summary>
    DataDecrement = 97,

    /// <summary>
    /// NRPN Select LSB
    /// </summary>
    NRPNSelectLSB = 98,

    /// <summary>
    /// NRPN Select MSB
    /// </summary>
    NRPNSelectMSB = 99,

    /// <summary>
    /// RPN Select LSB
    /// </summary>
    RPNSelectLSB = 100,

    /// <summary>
    /// RPN Select MSB
    /// </summary>
    RPNSelectMSB = 101,

    /// <summary>
    /// All Sounds Off
    /// </summary>
    AllSoundsOff = 120,

    /// <summary>
    /// Reset All Controllers
    /// </summary>
    ResetAllControllers = 121,

    /// <summary>
    /// Local Control On/Off
    /// </summary>
    LocalCtrlOnOff = 122,

    /// <summary>
    /// All Notes Off
    /// </summary>
    AllNotesOff = 123,

    /// <summary>
    /// Omni Mode Off + All Notes Off
    /// </summary>
    OmniModeOff = 124,

    /// <summary>
    /// Omni Mode On  + All Notes Off
    /// </summary>
    OmniModeOn = 125,

    /// <summary>
    /// Poly Mode On/Off + All Sounds Off
    /// </summary>
    PolyModeOnOff = 126,

    /// <summary>
    /// Poly Mode On
    /// </summary>
    PolyModeOn = 127,

    /// <summary>
    /// After Touch (associated to Channel Pressure)
    /// </summary>
    AfterTouch = 128,

    /// <summary>
    /// Pitch Bend Change
    /// </summary>
    PitchBend = 129,

    /// <summary>
    /// Count of Controller Number
    /// </summary>
    CountControllerNumber,

    /// <summary>
    /// Program Change (use LegacyMIDICCOutEvent.value only)
    /// </summary>
    ProgramChange = 130,

    /// <summary>
    /// Polyphonic Key Pressure (use LegacyMIDICCOutEvent.value for pitch and
    /// </summary>
    PolyPressure = 131,

    /// <summary>
    /// Quarter Frame ((use LegacyMIDICCOutEvent.value only)
    /// </summary>
    QuarterFrame = 132,
}