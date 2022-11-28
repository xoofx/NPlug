// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

/// <summary>
/// This is the structure used with <see cref="IAudioProcessor.GetBusInfo"/>, informing the host about what is a specific given bus.
/// </summary>
public abstract class BusInfo
{
    protected BusInfo(string name, AudioBusMediaType mediaType, AudioBusDirection direction, int channelCount, AudioBusType busType, AudioBusFlags flags)
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
    public string Name { get; }

    /// <summary>
    /// Gets a boolean indicating whether this bus is active.
    /// </summary>
    public bool IsActive { get; internal set; }
    
    /// <summary>
    /// Media type.
    /// </summary>
    public AudioBusMediaType MediaType { get; }

    /// <summary>
    /// Input or Output.
    /// </summary>
    public AudioBusDirection Direction { get; }

    /// <summary>
    /// number of channels (if used then need to be recheck after <see cref="IAudioProcessor.SetBusArrangements"/> is called).
    /// For a bus of type <see cref="AudioBusMediaType.Event"/> the ChannelCount corresponds
    /// to the number of supported MIDI channels by this bus
    /// </summary>
    public int ChannelCount { get; protected set; }

    /// <summary>
    /// Main or auxiliary.
    /// </summary>
    public AudioBusType BusType { get; }

    /// <summary>
    /// Bus flags.
    /// </summary>
    public AudioBusFlags Flags { get; }
}

/// <summary>
/// This is the structure used with <see cref="IAudioProcessor.GetBusInfo"/>, informing the host about what is a specific given bus.
/// </summary>
public sealed class AudioBusInfo : BusInfo
{
    private SpeakerArrangement _speakerArrangement;

    public AudioBusInfo(string name, SpeakerArrangement speakerArrangement, AudioBusDirection direction, AudioBusType busType, AudioBusFlags flags) : base(name, AudioBusMediaType.Audio, direction, speakerArrangement.GetChannelCount(), busType, flags)
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
}

/// <summary>
/// This is the structure used with <see cref="IAudioProcessor.GetBusInfo"/>, informing the host about what is a specific given bus.
/// </summary>
public sealed class EventBusInfo : BusInfo
{
    public EventBusInfo(string name, int channelCount, AudioBusDirection direction, AudioBusType busType, AudioBusFlags flags) : base(name, AudioBusMediaType.Event, direction, channelCount, busType, flags)
    {
    }
}
