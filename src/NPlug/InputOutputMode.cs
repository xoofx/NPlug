// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

public enum InputOutputMode
{
    /// <summary>
    /// 1:1 Input / Output. Only used for Instruments.
    /// </summary>
    Simple = 0,

    /// <summary>
    /// n:m Input / Output. Only used for Instruments.
    /// </summary>
    Advanced,

    /// <summary>
    /// plug-in used in an offline processing context
    /// </summary>
    OfflineProcessing,
}