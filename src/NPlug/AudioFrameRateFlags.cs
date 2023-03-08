// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug;

/// <summary>
/// Flags for <see cref="AudioFrameRate"/>.
/// </summary>
[Flags]
public enum AudioFrameRateFlags : uint
{
    /// <summary>
    /// The rate should is pull down.
    /// </summary>
    PullDownRate = 1 << 0,

    /// <summary>
    /// The rate is dropped.
    /// </summary>
    DropRate = 1 << 1,
}