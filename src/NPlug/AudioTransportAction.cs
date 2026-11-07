// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

/// <summary>
/// A transport action that a plugin can request from the host with
/// <see cref="IAudioControllerHandler.RequestTransportAction(AudioTransportAction, AudioTransportPosition)"/>.
/// </summary>
/// <remarks>
/// This is the equivalent of <c>Vst::ITransportControl::Action</c> (VST 3.8.1). The actions whose name contains
/// <c>Locate</c> and the cycle start / end actions require a position (see <see cref="AudioTransportActionExtensions.RequiresPosition"/>).
/// </remarks>
public enum AudioTransportAction
{
    /// <summary>
    /// Locate the transport to a new position (position required).
    /// </summary>
    Locate = 0,

    /// <summary>
    /// Start playback at the current position.
    /// </summary>
    PlaybackStart,

    /// <summary>
    /// Stop playback, keeping the current position.
    /// </summary>
    PlaybackStop,

    /// <summary>
    /// Locate, then start playback (position required).
    /// </summary>
    LocateAndPlaybackStart,

    /// <summary>
    /// Stop playback, then locate (position required).
    /// </summary>
    PlaybackStopAndLocate,

    /// <summary>
    /// Start recording and playback at the current position.
    /// </summary>
    RecordOnPlaybackStart,

    /// <summary>
    /// Locate, then start recording and playback (position required).
    /// </summary>
    LocateAndRecordOnPlaybackStart,

    /// <summary>
    /// Stop recording and continue playback.
    /// </summary>
    RecordOff,

    /// <summary>
    /// Stop recording and playback, keeping the current position.
    /// </summary>
    RecordOffPlaybackStop,

    /// <summary>
    /// Stop recording and playback, then locate (position required).
    /// </summary>
    RecordOffPlaybackStopAndLocate,

    /// <summary>
    /// Set the start of the cycle (loop) region (position required).
    /// </summary>
    SetCycleStart,

    /// <summary>
    /// Set the end of the cycle (loop) region (position required).
    /// </summary>
    SetCycleEnd,

    /// <summary>
    /// Enable the cycle (loop) mode.
    /// </summary>
    CycleOn,

    /// <summary>
    /// Disable the cycle (loop) mode.
    /// </summary>
    CycleOff,
}

/// <summary>
/// Extension methods for <see cref="AudioTransportAction"/>.
/// </summary>
public static class AudioTransportActionExtensions
{
    /// <summary>
    /// Returns <c>true</c> if the action needs a <see cref="AudioTransportPosition"/>: the locate actions and the cycle start / end.
    /// </summary>
    /// <param name="action">The transport action.</param>
    /// <returns><c>true</c> if a position must be given with the action.</returns>
    public static bool RequiresPosition(this AudioTransportAction action) => action switch
    {
        AudioTransportAction.Locate or
        AudioTransportAction.LocateAndPlaybackStart or
        AudioTransportAction.PlaybackStopAndLocate or
        AudioTransportAction.LocateAndRecordOnPlaybackStart or
        AudioTransportAction.RecordOffPlaybackStopAndLocate or
        AudioTransportAction.SetCycleStart or
        AudioTransportAction.SetCycleEnd => true,
        _ => false,
    };
}
