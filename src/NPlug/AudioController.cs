// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using NPlug.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace NPlug;

/// <summary>
/// Base class for implementing an audio controller <see cref="IAudioController"/>.
/// </summary>
/// <typeparam name="TAudioControllerModel">The model associated with this controller.</typeparam>
public abstract partial class AudioController<TAudioControllerModel> : AudioPluginComponent
    , IAudioController
    , IAudioControllerExtended
    , IAudioControllerUnitInfo
    where TAudioControllerModel : AudioProcessorModel, new()
{
    private PortableBinaryReader? _streamReader;
    private PortableBinaryWriter? _streamWriter;

    /// <summary>
    /// Default constructor.
    /// </summary>
    protected AudioController()
    {
        Model = new TAudioControllerModel();
        Model.Initialize();
        _selectedUnit = Model;
        Model.ParameterValueChanged += OnParameterValueChangedInternal;
        MapMidiCCToAudioParameter = new Dictionary<AudioMidiControllerNumber, AudioParameter>();
        _mapBusToUnit = new Dictionary<(BusMediaType, BusDirection, int, int), AudioUnit>();
    }

    /// <summary>
    /// Gets the instance of the model attached to this controller.
    /// </summary>
    /// <remarks>
    /// This instance is different than from the <see cref="AudioProcessor{TAudioProcessorModel}.Model"/>.
    /// Do not share this instance with a separate thread.
    /// </remarks>
    public TAudioControllerModel Model { get; }

    /// <summary>
    /// Gets the handler associated with this controller to communicate with the host.
    /// </summary>
    /// <remarks>
    /// This is set by the host after initializing by calling <see cref="IAudioController.SetControllerHandler" />
    /// </remarks>
    public IAudioControllerHandler? Handler { get; private set; }

    /// <summary>
    /// Called when the host is initializing this controller.
    /// </summary>
    /// <param name="host">The reference to the host.</param>
    /// <returns><c>true</c> if successfully initialized (This is the default implementation).</returns>
    protected virtual bool Initialize(AudioHostApplication host)
    {
        return true;
    }

    /// <summary>
    /// This method is called by the host when restoring the state of this controller from the audio processor component state.
    /// </summary>
    /// <param name="reader">The reader stream to restore data from.</param>
    /// <remarks>
    /// The default implementation is using the model to restore the state.
    /// </remarks>
    protected virtual void RestoreComponentState(PortableBinaryReader reader)
    {
        Model.Load(reader, AudioProcessorModelStorageMode.Default);
    }

    /// <summary>
    /// This method is called by the host when saving the state of this controller (different from the audio processor component state).
    /// </summary>
    /// <param name="writer">The writer stream to store specific controller data.</param>
    protected virtual void SaveState(PortableBinaryWriter writer)
    {
    }

    /// <summary>
    /// This method is called by the host when restoring the state of this controller (different from the audio processor component state).
    /// </summary>
    /// <param name="reader">The reader stream to restore specific controller data.</param>
    protected virtual void RestoreState(PortableBinaryReader reader)
    {
    }

    // IEditController2

    /// <summary>
    /// Called by the host to set the mode for knobs. This is only called if the host is supporting the <see cref="IAudioControllerExtended"/> interface.
    /// </summary>
    /// <param name="mode">The mode for knobs.</param>
    /// <returns><c>true</c> if supported. Returns <c>false</c> by default.</returns>
    protected virtual bool TrySetKnobMode(AudioControllerKnobModes mode)
    {
        return false;
    }

    /// <summary>
    /// Called by the host to check/open the help. This is only called if the host is supporting the <see cref="IAudioControllerExtended"/> interface.
    /// </summary>
    /// <param name="onlyCheck">A boolean indicating if the host is checking if help is supported.</param>
    /// <returns><c>true</c> if supported. Returns <c>false</c> by default.</returns>
    protected virtual bool TryOpenHelp(bool onlyCheck)
    {
        return false;
    }

    /// <summary>
    /// Called by the host to check/open the about box. This is only called if the host is supporting the <see cref="IAudioControllerExtended"/> interface.
    /// </summary>
    /// <param name="onlyCheck">A boolean indicating if the host is checking if help is supported.</param>
    /// <returns><c>true</c> if supported. Returns <c>false</c> by default.</returns>
    protected virtual bool TryOpenAboutBox(bool onlyCheck)
    {
        return false;
    }

    /// <summary>
    /// Create a view for this controller. This is called by <see cref="IAudioController.CreateView"/>.
    /// </summary>
    /// <returns></returns>
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

    void IAudioController.SetControllerHandler(IAudioControllerHandler? controllerHandler)
    {
        Handler = controllerHandler;
    }

    IAudioPluginView? IAudioController.CreateView()
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