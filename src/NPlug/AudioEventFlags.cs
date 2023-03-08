// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug;

/// <summary>
/// Describes the flags of an audio event.
/// </summary>
[Flags]
public enum AudioEventFlags : ushort
{
    /// <summary>
    /// Flags is not set.
    /// </summary>
    None = 0,

    /// <summary>
    /// Indicates that the event is played live (directly from keyboard)
    /// </summary>
    IsLive = 1 << 0,

    /// <summary>
    /// reserved for user (for internal use)
    /// </summary>
    UserReserved1 = 1 << 14,

    /// <summary>
    /// reserved for user (for internal use)
    /// </summary>
    UserReserved2 = 1 << 15,
}