// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

public enum AudioProcessMode
{
    /// <summary>
    /// Realtime processing.
    /// </summary>
    Realtime,

    /// <summary>
    /// Prefetch processing.
    /// </summary>
    Prefetch,

    /// <summary>
    /// Offline processing.
    /// </summary>
    Offline,
}