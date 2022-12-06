// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug;

/// <summary>
/// Sample size (32 or 64 bit).
/// </summary>
[Flags]
public enum AudioSampleSizeSupport
{
    /// <summary>
    /// 32 bit floating point.
    /// </summary>
    Float32 = 1 << 0,

    /// <summary>
    /// 64 bit floating point.
    /// </summary>
    Float64 = 1 << 1,

    /// <summary>
    /// 32 and 64 bit floating point.
    /// </summary>
    Any = Float32 | Float64,
}