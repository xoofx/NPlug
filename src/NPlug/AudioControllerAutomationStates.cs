// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug;

/// <summary>
/// The automation states used by <see cref="IAudioControllerAutomationState.SetAutomationState"/>
/// </summary>
[Flags]
public enum AudioControllerAutomationStates
{
    /// <summary>
    /// Not Read and not Write
    /// </summary>
    NoAutomation = 0,

    /// <summary>
    /// Read state
    /// </summary>
    ReadState = 1 << 0,

    /// <summary>
    /// Write state
    /// </summary>
    WriteState = 1 << 1,

    /// <summary>
    /// Read and Write enable
    /// </summary>
    ReadWriteState = ReadState | WriteState,
}