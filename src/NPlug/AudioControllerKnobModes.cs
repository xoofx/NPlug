// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

/// <summary>
/// Knob Mode
/// </summary>
public enum AudioControllerKnobModes
{
    /// <summary>
    /// Circular with jump to clicked position
    /// </summary>
    CircularMode = 0,

    /// <summary>
    /// Circular without jump to clicked position
    /// </summary>
    RelativeCircularMode,

    /// <summary>
    /// Linear: depending on vertical movement
    /// </summary>
    LinearMode,
}