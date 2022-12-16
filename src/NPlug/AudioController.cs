// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using NPlug.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace NPlug;

public abstract partial class AudioController<TAudioRootUnit> : AudioPluginComponent
    , IAudioController
    , IAudioControllerExtended
    , IAudioControllerMidiMapping
    , IAudioControllerUnitInfo
    where TAudioRootUnit : AudioRootUnit, new()
{
    private PortableBinaryReader? _streamReader;
    private PortableBinaryWriter? _streamWriter;

    protected AudioController()
    {
        _programLists = new List<AudioProgramList>();
        _mapProgramIdToIndex = new Dictionary<AudioProgramListId, int>();
        RootUnit = new TAudioRootUnit();
        RootUnit.Initialize();
        _selectedUnit = RootUnit;
    }

    public TAudioRootUnit RootUnit { get; }

    protected virtual bool Initialize(AudioHostApplication host)
    {
        return true;
    }

    protected virtual void RestoreComponentState(PortableBinaryReader reader)
    {
        RootUnit.Load(reader);
    }

    protected virtual void SaveState(PortableBinaryWriter writer)
    {
        RootUnit.Save(writer);
    }

    protected void RestoreState(PortableBinaryReader reader)
    {
        RootUnit.Load(reader);
    }

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
        return Initialize(hostApplication);
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