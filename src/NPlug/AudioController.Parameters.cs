// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

public abstract partial class AudioController<TAudioRootUnit>
{
    int IAudioController.ParameterCount => RootUnit.TotalParameterCount;

    AudioParameterInfo IAudioController.GetParameterInfo(int paramIndex) => RootUnit.GetParameterFromRoot(paramIndex).Info;

    string IAudioController.GetParameterStringByValue(AudioParameterId id, double valueNormalized)
    {
        return RootUnit.GetParameterById(id).ToString(valueNormalized);
    }

    double IAudioController.GetParameterValueByString(AudioParameterId id, string valueAsString)
    {
        return RootUnit.GetParameterById(id).FromString(valueAsString);
    }

    double IAudioController.NormalizedParameterToPlain(AudioParameterId id, double valueNormalized)
    {
        return RootUnit.GetParameterById(id).ToPlain(valueNormalized);
    }

    double IAudioController.PlainParameterToNormalized(AudioParameterId id, double plainValue)
    {
        return RootUnit.GetParameterById(id).ToNormalized(plainValue);
    }

    double IAudioController.GetParameterNormalized(AudioParameterId id)
    {
        return RootUnit.GetParameterById(id).NormalizedValue;
    }

    void IAudioController.SetParameterNormalized(AudioParameterId id, double valueNormalized)
    {
        RootUnit.GetParameterById(id).NormalizedValue = valueNormalized;
    }
}