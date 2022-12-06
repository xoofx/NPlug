// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

/// <summary>
/// Extended plug-in interface of <see cref="IAudioController"/>.
/// </summary>
/// <remarks>
///  vstIPlug vst365: Vst::IAutomationState
/// - [plug imp]
/// - [extends IEditController]
/// - [released: 3.6.5]
/// - [optional]Hosts can inform the plug-in about its current automation state (Read/Write/Nothing).
/// </remarks>
public interface IAudioControllerAutomationState : IAudioController
{
    /// <summary>
    /// Sets the current Automation state.
    /// </summary>
    void SetAutomationState(AudioControllerAutomationStates state);
}