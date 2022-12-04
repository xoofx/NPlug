// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.IO;

namespace NPlug;

public abstract class AudioController : AudioPluginComponent, IAudioController
{
    void IAudioController.SetComponentState(Stream state)
    {
        throw new NotImplementedException();
    }

    void IAudioController.SetState(Stream state)
    {
        throw new NotImplementedException();
    }

    void IAudioController.GetState(Stream state)
    {
        throw new NotImplementedException();
    }

    int IAudioController.ParameterCount
    {
        get { throw new NotImplementedException(); }
    }

    AudioParameterInfo IAudioController.GetParameterInfo(int paramIndex)
    {
        throw new NotImplementedException();
    }

    string IAudioController.GetParameterStringByValue(AudioParameterId id, double valueNormalized)
    {
        throw new NotImplementedException();
    }

    double IAudioController.GetParameterValueByString(AudioParameterId id, string valueAsString)
    {
        throw new NotImplementedException();
    }

    double IAudioController.NormalizedParameterToPlain(AudioParameterId id, double valueNormalized)
    {
        throw new NotImplementedException();
    }

    double IAudioController.PlainParameterToNormalized(AudioParameterId id, double plainValue)
    {
        throw new NotImplementedException();
    }

    double IAudioController.GetParameterNormalized(AudioParameterId id)
    {
        throw new NotImplementedException();
    }

    void IAudioController.SetParameterNormalized(AudioParameterId id, double valueNormalized)
    {
        throw new NotImplementedException();
    }

    void IAudioController.SetControllerHost(AudioControllerHost controllerHost)
    {
        throw new NotImplementedException();
    }

    IAudioPluginView IAudioController.CreateView(string name)
    {
        throw new NotImplementedException();
    }
}