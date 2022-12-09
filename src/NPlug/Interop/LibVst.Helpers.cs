// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    private static void CopyStringToUTF8(string text, byte* dest, int maxLength)
    {
        var encodedLength = Encoding.UTF8.GetBytes(text, new Span<byte>(dest, maxLength));
        // Null terminate
        dest[Math.Min(encodedLength, maxLength - 1)] = 0;
    }

    private static void CopyStringToUTF16(string text, char* dest, int maxLength)
    {
        var lengthToCopy = Math.Min(text.Length, maxLength);
        text.AsSpan(0, lengthToCopy).CopyTo(new Span<char>(dest, lengthToCopy));
        dest[Math.Min(text.Length, maxLength - 1)] = (char)0;
    }

    private static string GetPluginSubCategory(AudioPluginCategory category)
    {
        return category switch
        {
            AudioPluginCategory.EffectAnalyzer => PlugType.kFxAnalyzer,
            AudioPluginCategory.EffectDelay => PlugType.kFxDelay,
            AudioPluginCategory.EffectDistortion => PlugType.kFxDistortion,
            AudioPluginCategory.EffectDynamics => PlugType.kFxDynamics,
            AudioPluginCategory.EffectEQ => PlugType.kFxEQ,
            AudioPluginCategory.EffectFilter => PlugType.kFxFilter,
            AudioPluginCategory.Effect => PlugType.kFx,
            AudioPluginCategory.EffectInstrument => PlugType.kFxInstrument,
            AudioPluginCategory.EffectInstrumentExternal => PlugType.kFxInstrumentExternal,
            AudioPluginCategory.EffectSpatial => PlugType.kFxSpatial,
            AudioPluginCategory.EffectGenerator => PlugType.kFxGenerator,
            AudioPluginCategory.EffectMastering => PlugType.kFxMastering,
            AudioPluginCategory.EffectModulation => PlugType.kFxModulation,
            AudioPluginCategory.EffectPitchShift => PlugType.kFxPitchShift,
            AudioPluginCategory.EffectRestoration => PlugType.kFxRestoration,
            AudioPluginCategory.EffectReverb => PlugType.kFxReverb,
            AudioPluginCategory.EffectSurround => PlugType.kFxSurround,
            AudioPluginCategory.EffectTools => PlugType.kFxTools,
            AudioPluginCategory.EffectNetwork => PlugType.kFxNetwork,
            AudioPluginCategory.Instrument => PlugType.kInstrument,
            AudioPluginCategory.InstrumentDrum => PlugType.kInstrumentDrum,
            AudioPluginCategory.InstrumentExternal => PlugType.kInstrumentExternal,
            AudioPluginCategory.InstrumentPiano => PlugType.kInstrumentPiano,
            AudioPluginCategory.InstrumentSampler => PlugType.kInstrumentSampler,
            AudioPluginCategory.InstrumentSynthesizer => PlugType.kInstrumentSynth,
            AudioPluginCategory.InstrumentSynthesizerSampler => PlugType.kInstrumentSynthSampler,
            _ => throw new ArgumentOutOfRangeException(nameof(category), category, null)
        };
    }
}