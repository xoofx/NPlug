// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

/// <summary>
/// Category of an audio plugin.
/// </summary>
public enum AudioPluginCategory
{
    /// <summary>Scope, FFT-Display, Loudness Processing...</summary>
    EffectAnalyzer,

    /// <summary>Delay, Multi-tap Delay, Ping-Pong Delay...</summary>
    EffectDelay,

    /// <summary>Amp Simulator, Sub-Harmonic, SoftClipper...</summary>
    EffectDistortion,

    /// <summary>Compressor, Expander, Gate, Limiter, Maximizer, Tape Simulator, EnvelopeShaper...</summary>
    EffectDynamics,

    /// <summary>Equalization, Graphical EQ...</summary>
    EffectEQ,

    /// <summary>WahWah, ToneBooster, Specific Filter,...</summary>
    EffectFilter,

    /// <summary>others type (not categorized)</summary>
    Effect,

    /// <summary>Effect which could be loaded as Instrument too</summary>
    EffectInstrument,

    /// <summary>Effect which could be loaded as Instrument too and is external (wrapped Hardware)</summary>
    EffectInstrumentExternal,

    /// <summary>MonoToStereo, StereoEnhancer,...</summary>
    EffectSpatial,

    /// <summary>Tone Generator, Noise Generator...</summary>
    EffectGenerator,

    /// <summary>Dither, Noise Shaping,...</summary>
    EffectMastering,

    /// <summary>Phaser, Flanger, Chorus, Tremolo, Vibrato, AutoPan, Rotary, Cloner...</summary>
    EffectModulation,

    /// <summary>Pitch Processing, Pitch Correction, Vocal Tuning</summary>
    EffectPitchShift,

    /// <summary>Denoiser, Declicker,...</summary>
    EffectRestoration,

    /// <summary>Reverberation, Room Simulation, Convolution Reverb...</summary>
    EffectReverb,

    /// <summary>dedicated to surround processing: LFE Splitter, Bass Manager...</summary>
    EffectSurround,

    /// <summary>Volume, Mixer, Tuner...</summary>
    EffectTools,

    /// <summary>using Network</summary>
    EffectNetwork,

    /// <summary>Effect used as instrument (sound generator), not as insert</summary>
    Instrument,

    /// <summary>Instrument for Drum sounds</summary>
    InstrumentDrum,

    /// <summary>External Instrument (wrapped Hardware)</summary>
    InstrumentExternal,

    /// <summary>Instrument for Piano sounds</summary>
    InstrumentPiano,

    /// <summary>Instrument based on Samples</summary>
    InstrumentSampler,

    /// <summary>Instrument based on Synthesis</summary>
    InstrumentSynthesizer,

    /// <summary>Instrument based on Synthesis and Samples</summary>
    InstrumentSynthesizerSampler,
}