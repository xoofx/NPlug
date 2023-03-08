// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

/// <summary>
/// Extended plug-in interface for note expression event support:
/// </summary>
/// <remarks>
/// vstIPlug vst350 (Vst::INoteExpressionController)
/// - [plug imp]
/// - [extends IEditController]
/// - [released: 3.5.0]
/// - [optional]
/// With this plug-in interface, the host can retrieve all necessary note expression information supported by the plug-in.
/// Note expression information (@ref NoteExpressionTypeInfo) are specific for given channel and event bus.
/// Note that there is only one NoteExpressionTypeID per given channel of an event bus.
/// The method getNoteExpressionStringByValue allows conversion from a normalized value to a string representation
/// and the getNoteExpressionValueByString method from a string to a normalized value.When the note expression state changes (for example when switching presets) the plug-in needs
/// to inform the host about it via <see cref="IAudioControllerHandler.RestartComponent"/> (<see cref="AudioRestartFlags.NoteExpressionChanged"/>).
/// </remarks>
public interface IAudioControllerNoteExpression : IAudioController
{
    /// <summary>
    /// Returns number of supported note change types for event bus index and channel.
    /// </summary>
    int GetNoteExpressionCount(int busIndex, short channel);

    /// <summary>
    /// Returns note change type info.
    /// </summary>
    AudioNoteExpressionTypeInfo GetNoteExpressionInfo(int busIndex, short channel, int noteExpressionIndex);

    /// <summary>
    /// Gets a user readable representation of the normalized note change value.
    /// </summary>
    string GetNoteExpressionStringByValue(int busIndex, short channel, AudioNoteExpressionTypeId id, double valueNormalized);

    /// <summary>
    /// Converts the user readable representation to the normalized note change value.
    /// </summary>
    double GetNoteExpressionValueByString(int busIndex, short channel, AudioNoteExpressionTypeId id, string valueAsString);
}