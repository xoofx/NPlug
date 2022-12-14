// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using NPlug.Interop;

namespace NPlug;

/// <summary>
/// Describes the type of Physical UI (PUI) which could be associated to a note
/// expression.
/// </summary>
/// <seealso cref="AudioPhysicalUIMap"/>
public enum AudioPhysicalUITypeId
{
    /// <summary>
    /// absolute X position when touching keys of PUIs. Range [0=left, 0.5=middle, 1=right]
    /// </summary>
    XMovement = 0,

    /// <summary>
    /// absolute Y position when touching keys of PUIs. Range [0=bottom/near, 0.5=center, 1=top/far]
    /// </summary>
    YMovement,

    /// <summary>
    /// pressing a key down on keys of PUIs. Range [0=No Pressure, 1=Full Pressure]
    /// </summary>
    Pressure,

    /// <summary>
    /// count of current defined PUIs
    /// </summary>
    TypeCount,

    /// <summary>
    /// indicates an invalid or not initialized PUI type
    /// </summary>
    Invalid = -1,
}