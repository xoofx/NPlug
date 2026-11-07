// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

/// <summary>
/// Edit controller extension that tells the host which note-on articulations of the MIDI 2.0 orchestral profile the
/// plugin supports.
/// </summary>
/// <remarks>
/// vstIPlug vst381, Vst::NoteOnOrchestralArticulation::IInfo
/// - [plug imp]
/// - [extends IEditController]
/// - [released: 3.8.1]
/// - [optional]
/// The host calls it from the UI thread, once the plugin is connected. The articulations themselves reach the
/// processor as note expressions in the <see cref="AudioNoteExpressionTypeId.Midi2NoteOnOrchestralArticulationStart"/>
/// to <see cref="AudioNoteExpressionTypeId.Midi2NoteOnOrchestralArticulationEnd"/> range.
/// </remarks>
public interface IAudioControllerNoteOnOrchestralArticulationInfo : IAudioController
{
    /// <summary>
    /// Fills the articulations supported on a channel of an event input bus.
    /// </summary>
    /// <param name="busIndex">The index of the event input bus.</param>
    /// <param name="channel">The channel of the bus.</param>
    /// <param name="variations">The articulations to fill, all 0 on entry.</param>
    /// <returns><c>true</c> if <paramref name="variations"/> was filled; <c>false</c> if the bus or the channel has no articulations.</returns>
    bool TryGetOrchestralArticulationVariations(int busIndex, short channel, ref AudioOrchestralArticulationVariations variations);
}
