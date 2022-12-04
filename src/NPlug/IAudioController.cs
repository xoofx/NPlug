// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.IO;

namespace NPlug;

public interface IAudioController : IAudioPluginComponent
{
    void SetComponentState(Stream state);
    void SetState(Stream state);
    void GetState(Stream state);
    int ParameterCount { get; }
    AudioParameterInfo GetParameterInfo(int paramIndex);
    string GetParameterStringByValue(AudioParameterId id, double valueNormalized);
    double GetParameterValueByString(AudioParameterId id, string valueAsString);
    double NormalizedParameterToPlain(AudioParameterId id, double valueNormalized);
    double PlainParameterToNormalized(AudioParameterId id, double plainValue);
    double GetParameterNormalized(AudioParameterId id);
    void SetParameterNormalized(AudioParameterId id, double valueNormalized);
    void SetControllerHost(AudioControllerHost controllerHost);
    IAudioPluginView CreateView(string name);
}