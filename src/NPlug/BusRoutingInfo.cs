// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

public struct BusRoutingInfo
{
    /// <summary>
    /// media type see @ref MediaTypes
    /// </summary>
    public BusMediaType MediaType;

    /// <summary>
    /// bus index
    /// </summary>
    public int BusIndex;

    /// <summary>
    /// channel (-1 for all channels)
    /// </summary>
    public int Channel;
}