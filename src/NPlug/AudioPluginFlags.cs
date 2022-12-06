// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug;

[Flags]
public enum AudioPluginFlags
{
    /// <summary>
    /// Empty flags.
    /// </summary>
    None = 0,
    
    /// <summary>
    /// Component can be run on remote computer
    /// </summary>
    Distributable = 1 << 0,

    /// <summary>
    /// Component supports simple IO mode (or works in simple mode anyway) see @ref vst3IoMode
    /// </summary>
    SimpleModeSupported = 1 << 1,
}