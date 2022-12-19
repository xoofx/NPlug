// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using NPlug.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace NPlug;

public abstract partial class AudioController<TAudioControllerModel> : AudioPluginComponent
    , IAudioController
    , IAudioControllerExtended
    , IAudioControllerUnitInfo
    where TAudioControllerModel : AudioProcessorModel, new()
{
    private PortableBinaryReader? _streamReader;
    private PortableBinaryWriter? _streamWriter;
    


    protected AudioController()
    {
        Model = new TAudioControllerModel();
        Model.Initialize();
        _selectedUnit = Model;
        Model.ParameterValueChanged += RootUnitOnParameterValueChanged;
        MapMidiCCToAudioParameter = new Dictionary<AudioMidiControllerNumber, AudioParameter>();
        _mapBusToUnit = new Dictionary<(BusMediaType, BusDirection, int, int), AudioUnit>();
    }

    public TAudioControllerModel Model { get; }

    public IAudioControllerHandler? Handler { get; private set; }

    protected virtual bool Initialize(AudioHostApplication host)
    {
        return true;
    }

    protected virtual void RestoreComponentState(PortableBinaryReader reader)
    {
        Model.Load(reader, AudioProcessorModelStorageMode.Default);
    }

    protected virtual void SaveState(PortableBinaryWriter writer)
    {
        //RootUnit.Save(writer);
    }

    protected virtual void RestoreState(PortableBinaryReader reader)
    {
        //RootUnit.Load(reader);
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

    protected virtual IAudioPluginView? CreateView()
    {
        return null;
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
        Handler = controllerHandler;
    }

    IAudioPluginView? IAudioController.CreateView(string name)
    {
        return CreateView();
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

    private IAudioControllerHandler GetHandler([CallerMemberName] string? callerName = null)
    {
        if (Handler is null) throw new InvalidOperationException($"Unexpected error in `{callerName}`. The controller handler is null.");
        return Handler;
    }
}