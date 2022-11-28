// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

public struct AudioFrameRate
{
    /// <summary>
    /// frame rate
    /// </summary>
    public uint FramesPerSecond;

    /// <summary>
    /// flags #FrameRateFlags
    /// </summary>
    public AudioFrameRateFlags Flags;
}