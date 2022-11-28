// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

/// <summary>
/// 
/// </summary>
public enum AudioFrameRateFlags : uint
{
    PullDownRate = 1 << 0,

    DropRate = 1 << 1,
}