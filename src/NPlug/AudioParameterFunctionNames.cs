// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

/// <summary>
/// The function names a host can ask for with <see cref="IAudioControllerParameterFunctionName.TryGetParameterIdFromFunctionName"/>
/// (<c>Vst::FunctionNameType</c>, VST 3.8.1).
/// </summary>
public static class AudioParameterFunctionNames
{
    /// <summary>
    /// Compressor gain reduction (a read-only parameter the host can show as a meter).
    /// </summary>
    public const string CompGainReduction = "Comp:GainReduction";

    /// <summary>
    /// Maximum compressor gain reduction.
    /// </summary>
    public const string CompGainReductionMax = "Comp:GainReductionMax";

    /// <summary>
    /// Compressor gain reduction peak hold.
    /// </summary>
    public const string CompGainReductionPeakHold = "Comp:GainReductionPeakHold";

    /// <summary>
    /// Resets the maximum compressor gain reduction.
    /// </summary>
    public const string CompResetGainReductionMax = "Comp:ResetGainReductionMax";

    /// <summary>
    /// Low latency mode, for live situations: 0 = disabled, 1 = enabled.
    /// </summary>
    public const string LowLatencyMode = "LowLatencyMode";

    /// <summary>
    /// Mix of the original (dry) signal with the processed (wet) one: 0 = dry only, 0.5 = half of each, 1 = wet only.
    /// </summary>
    public const string DryWetMix = "DryWetMix";

    /// <summary>
    /// Assigns randomized values across the full range of the parameters.
    /// </summary>
    public const string Randomize = "Randomize";

    /// <summary>
    /// Assigns randomized values around the current values of the parameters (for example ±5 %). VST 3.8.1.
    /// </summary>
    public const string RandomizeAroundCurrent = "RandomizeAroundCurrent";

    /// <summary>
    /// Panner gravity point on the X axis, [0, 1] = [left, right] (for stereo: middle between left and right).
    /// </summary>
    public const string PanPosCenterX = "PanPosCenterX";

    /// <summary>
    /// Panner gravity point on the Y axis, [0, 1] = [front, rear].
    /// </summary>
    public const string PanPosCenterY = "PanPosCenterY";

    /// <summary>
    /// Panner gravity point on the Z axis, [0, 1] = [bottom, top].
    /// </summary>
    public const string PanPosCenterZ = "PanPosCenterZ";
}
