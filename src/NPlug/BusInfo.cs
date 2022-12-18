// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

/// <summary>
/// This is the structure used with <see cref="IAudioProcessor.GetBusInfo"/>, informing the host about what is a specific given bus.
/// </summary>
public abstract class BusInfo
{
    protected BusInfo(string name, BusMediaType mediaType, BusDirection direction, int channelCount, BusType busType, BusFlags flags)
    {
        Name = name;
        MediaType = mediaType;
        Direction = direction;
        ChannelCount = channelCount;
        BusType = busType;
        Flags = flags;
    }

    /// <summary>
    /// Gets the name of the bus.
    /// </summary>
    /// <remarks>
    /// Changing the bus name after registration requires to call <see cref="IAudioControllerHandler.RestartComponent"/> with <see cref="AudioRestartFlags.IoTitlesChanged"/>.
    /// </remarks>
    public string Name { get; set; }

    /// <summary>
    /// Gets a boolean indicating whether this bus is active.
    /// </summary>
    public bool IsActive { get; internal set; }
    
    /// <summary>
    /// Media type.
    /// </summary>
    public BusMediaType MediaType { get; }

    /// <summary>
    /// Input or Output.
    /// </summary>
    public BusDirection Direction { get; }

    /// <summary>
    /// number of channels (if used then need to be recheck after <see cref="IAudioProcessor.SetBusArrangements"/> is called).
    /// For a bus of type <see cref="BusMediaType.Event"/> the ChannelCount corresponds
    /// to the number of supported MIDI channels by this bus
    /// </summary>
    public int ChannelCount { get; protected set; }

    /// <summary>
    /// Main or auxiliary.
    /// </summary>
    public BusType BusType { get; }

    /// <summary>
    /// Bus flags.
    /// </summary>
    public BusFlags Flags { get; }
}

/// <summary>
/// This is the structure used with <see cref="IAudioProcessor.GetBusInfo"/>, informing the host about what is a specific given bus.
/// </summary>
public sealed class AudioBusInfo : BusInfo
{
    private SpeakerArrangement _speakerArrangement;

    public AudioBusInfo(string name, SpeakerArrangement speakerArrangement, BusDirection direction, BusType busType, BusFlags flags) : base(name, BusMediaType.Audio, direction, speakerArrangement.GetChannelCount(), busType, flags)
    {
        SpeakerArrangement = speakerArrangement;
    }

    public SpeakerArrangement SpeakerArrangement
    {
        get => _speakerArrangement;

        internal set
        {
            _speakerArrangement = value;
            ChannelCount = _speakerArrangement.GetChannelCount();
        }
    }

    /// <summary>
    /// Gets how long from the moment of generation/acquiring (from file or from Input)
    /// it will take for its input to arrive, and how long it will take for its output to be presented (to output or to speaker).
    /// </summary>
    public uint PresentationLatencyInSamples { get; internal set; }
}

/// <summary>
/// This is the structure used with <see cref="IAudioProcessor.GetBusInfo"/>, informing the host about what is a specific given bus.
/// </summary>
public sealed class EventBusInfo : BusInfo
{
    public EventBusInfo(string name, int channelCount, BusDirection direction, BusType busType, BusFlags flags) : base(name, BusMediaType.Event, direction, channelCount, busType, flags)
    {
    }
}
