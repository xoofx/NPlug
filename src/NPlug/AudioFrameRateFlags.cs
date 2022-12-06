// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug;

/// <summary>
/// 
/// </summary>
[Flags]
public enum AudioFrameRateFlags : uint
{
    PullDownRate = 1 << 0,

    DropRate = 1 << 1,
}