// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using NPlug.Interop;
using NUnit.Framework;

namespace NPlug.Tests;

/// <summary>
/// The public function names are the ones of the SDK (<c>Vst::FunctionNameType</c>).
/// </summary>
public class TestParameterFunctionNames
{
    [Test]
    public void FunctionNamesMatchTheSdk()
    {
        CollectionAssert.AreEqual(new[]
        {
            LibVst.FunctionNameType.kCompGainReduction, LibVst.FunctionNameType.kCompGainReductionMax,
            LibVst.FunctionNameType.kCompGainReductionPeakHold, LibVst.FunctionNameType.kCompResetGainReductionMax,
            LibVst.FunctionNameType.kLowLatencyMode, LibVst.FunctionNameType.kDryWetMix, LibVst.FunctionNameType.kRandomize,
            LibVst.FunctionNameType.kRandomizeAroundCurrent, LibVst.FunctionNameType.kPanPosCenterX,
            LibVst.FunctionNameType.kPanPosCenterY, LibVst.FunctionNameType.kPanPosCenterZ,
        }, new[]
        {
            AudioParameterFunctionNames.CompGainReduction, AudioParameterFunctionNames.CompGainReductionMax,
            AudioParameterFunctionNames.CompGainReductionPeakHold, AudioParameterFunctionNames.CompResetGainReductionMax,
            AudioParameterFunctionNames.LowLatencyMode, AudioParameterFunctionNames.DryWetMix, AudioParameterFunctionNames.Randomize,
            AudioParameterFunctionNames.RandomizeAroundCurrent, AudioParameterFunctionNames.PanPosCenterX,
            AudioParameterFunctionNames.PanPosCenterY, AudioParameterFunctionNames.PanPosCenterZ,
        });
    }
}
