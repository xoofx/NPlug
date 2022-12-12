// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

/// <summary>
/// Extension to the <see cref="IAudioController"/> interface.
/// </summary>
/// <remarks>
/// VST Method from `IEditController2`.
/// </remarks>
public interface IAudioControllerExtended : IAudioController
{
    /// <summary>
    /// Host could set the Knob Mode for the plug-in. Return kResultFalse means not supported mode.
    /// </summary>
    /// <remarks>
    /// VST Method from `IEditController2`.
    /// </remarks>
    bool TrySetKnobMode(AudioControllerKnobModes mode);

    /// <summary>
    /// Host could ask to open the plug-in help (could be: opening a PDF document or link to a web page).
    /// The host could call it with onlyCheck set to true for testing support of open Help.
    /// Return kResultFalse means not supported function.
    /// </summary>
    /// <remarks>
    /// VST Method from `IEditController2`.
    /// </remarks>
    bool TryOpenHelp(bool onlyCheck);

    /// <summary>
    /// Host could ask to open the plug-in about box.
    /// The host could call it with onlyCheck set to true for testing support of open AboutBox.
    /// Return kResultFalse means not supported function.
    /// </summary>
    bool TryOpenAboutBox(bool onlyCheck);
}