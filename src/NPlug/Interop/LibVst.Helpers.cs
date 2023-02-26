// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Buffers.Binary;
using System.Numerics;
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

    private static Guid ConvertToPlatform(this in Guid guid)
    {
        // GUID in VST are identical to the Windows Version
        if (OperatingSystem.IsWindows() || !BitConverter.IsLittleEndian) return guid;

        // But on non COM Compatible OS, the leading u32, u16, u16, u8[8]
        // are big endian instead of little endian, so we need to reverse them
        var newGuid = guid;
        var pInt0 = (int*)&newGuid;
        *pInt0 = BinaryPrimitives.ReverseEndianness(*pInt0);
        var pShort1 = (short*)pInt0 + 2;
        *pShort1 = BinaryPrimitives.ReverseEndianness(*pShort1);
        var pShort2 = (short*)pInt0 + 3;
        *pShort2 = BinaryPrimitives.ReverseEndianness(*pShort2);

        return newGuid;
    }

    private static void CopyStringToUTF16(string text, ref String128 str128)
    {
        str128.CopyFrom(text);
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
            _ => ""
        };
    }
}