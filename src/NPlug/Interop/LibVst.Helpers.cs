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
        if (encodedLength < maxLength)
        {
            // Null terminate
            dest[encodedLength] = 0;
        }
    }

    private static void CopyStringToUTF16(string text, char* dest, int maxLength)
    {
        var lengthToCopy = Math.Min(text.Length, maxLength);
        text.AsSpan(0, lengthToCopy).CopyTo(new Span<char>(dest, lengthToCopy));
        if (lengthToCopy < maxLength)
        {
            dest[lengthToCopy] = (char)0;
        }
    }

    private static string GetPluginSubCategory(AudioProcessorCategory category)
    {
        return category switch
        {
            AudioProcessorCategory.EffectAnalyzer => PlugType.kFxAnalyzer,
            AudioProcessorCategory.EffectDelay => PlugType.kFxDelay,
            AudioProcessorCategory.EffectDistortion => PlugType.kFxDistortion,
            AudioProcessorCategory.EffectDynamics => PlugType.kFxDynamics,
            AudioProcessorCategory.EffectEQ => PlugType.kFxEQ,
            AudioProcessorCategory.EffectFilter => PlugType.kFxFilter,
            AudioProcessorCategory.Effect => PlugType.kFx,
            AudioProcessorCategory.EffectInstrument => PlugType.kFxInstrument,
            AudioProcessorCategory.EffectInstrumentExternal => PlugType.kFxInstrumentExternal,
            AudioProcessorCategory.EffectSpatial => PlugType.kFxSpatial,
            AudioProcessorCategory.EffectGenerator => PlugType.kFxGenerator,
            AudioProcessorCategory.EffectMastering => PlugType.kFxMastering,
            AudioProcessorCategory.EffectModulation => PlugType.kFxModulation,
            AudioProcessorCategory.EffectPitchShift => PlugType.kFxPitchShift,
            AudioProcessorCategory.EffectRestoration => PlugType.kFxRestoration,
            AudioProcessorCategory.EffectReverb => PlugType.kFxReverb,
            AudioProcessorCategory.EffectSurround => PlugType.kFxSurround,
            AudioProcessorCategory.EffectTools => PlugType.kFxTools,
            AudioProcessorCategory.EffectNetwork => PlugType.kFxNetwork,
            AudioProcessorCategory.Instrument => PlugType.kInstrument,
            AudioProcessorCategory.InstrumentDrum => PlugType.kInstrumentDrum,
            AudioProcessorCategory.InstrumentExternal => PlugType.kInstrumentExternal,
            AudioProcessorCategory.InstrumentPiano => PlugType.kInstrumentPiano,
            AudioProcessorCategory.InstrumentSampler => PlugType.kInstrumentSampler,
            AudioProcessorCategory.InstrumentSynthesizer => PlugType.kInstrumentSynth,
            AudioProcessorCategory.InstrumentSynthesizerSampler => PlugType.kInstrumentSynthSampler,
            _ => throw new ArgumentOutOfRangeException(nameof(category), category, null)
        };
    }
}