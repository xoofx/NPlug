// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using NPlug.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace NPlug;

public abstract class AudioController : AudioPluginComponent
    , IAudioController
    , IAudioControllerExtended
    , IAudioControllerMidiMapping
{
    private PortableBinaryReader? _streamReader;
    private PortableBinaryWriter? _streamWriter;

    internal readonly List<AudioParameterInfo> ParameterInfos;

    protected AudioController()
    {
        ParameterInfos = new List<AudioParameterInfo>();
    }

    protected abstract bool Initialize(AudioControllerSetup setup);

    protected virtual void RestoreComponentState(PortableBinaryReader reader)
    {
    }

    protected abstract void SaveState(PortableBinaryWriter writer);

    protected abstract void RestoreState(PortableBinaryReader reader);

    // IEditController2

    protected virtual bool TrySetKnobMode(AudioControllerKnobModes mode)
    {
        return false;
    }

    protected virtual bool TryOpenHelp(bool onlyCheck)
    {
        return false;
    }

    protected virtual bool TryOpenAboutBox(bool onlyCheck)
    {
        return false;
    }

    // IMidiMapping

    protected virtual bool TryGetMidiControllerAssignment(int busIndex, int channel, AudioMidiControllerNumber midiControllerNumber, out AudioParameterId id)
    {
        id = default;
        return false;
    }

    internal override bool InitializeInternal(AudioHostApplication hostApplication)
    {
        return Initialize(new AudioControllerSetup(this, hostApplication));
    }

    void IAudioController.SetComponentState(Stream streamInput)
    {
        var reader = _streamReader;
        if (reader is null)
        {
            reader = new PortableBinaryReader();
            _streamReader = reader;
        }
        reader.Stream = streamInput;
        RestoreComponentState(reader);
    }

    void IAudioController.SetState(Stream streamInput)
    {
        var reader = _streamReader;
        if (reader is null)
        {
            reader = new PortableBinaryReader(streamInput);
            _streamReader = reader;
        }
        else
        {
            reader.Stream = streamInput;
        }
        RestoreState(reader);
    }

    void IAudioController.GetState(Stream streamOutput)
    {
        var writer = _streamWriter;
        if (writer is null)
        {
            writer = new PortableBinaryWriter(streamOutput);
            _streamWriter = writer;
        }
        else
        {
            writer.Stream = streamOutput;
        }
        SaveState(writer);
    }

    int IAudioController.ParameterCount => ParameterInfos.Count;

    AudioParameterInfo IAudioController.GetParameterInfo(int paramIndex) => ParameterInfos[paramIndex];

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

    public void SetControllerHandler(IAudioControllerHandler controllerHandler)
    {
        throw new NotImplementedException();
    }

    IAudioPluginView? IAudioController.CreateView(string name)
    {
        throw new NotImplementedException();
    }

    bool IAudioControllerExtended.TrySetKnobMode(AudioControllerKnobModes mode)
    {
        return TrySetKnobMode(mode);
    }

    bool IAudioControllerExtended.TryOpenHelp(bool onlyCheck)
    {
        return TryOpenHelp(onlyCheck);
    }

    bool IAudioControllerExtended.TryOpenAboutBox(bool onlyCheck)
    {
        return TryOpenAboutBox(onlyCheck);
    }

    bool IAudioControllerMidiMapping.TryGetMidiControllerAssignment(int busIndex, int channel, AudioMidiControllerNumber midiControllerNumber, out AudioParameterId id)
    {
        return TryGetMidiControllerAssignment(busIndex, channel, midiControllerNumber, out id);
    }
}