// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

/// <summary>
/// Prefetchable Support Enum
/// </summary>
public enum AudioProcessorPrefetchableSupport
{
    /// <summary>
    /// every instance of the plug does not support prefetch processing
    /// </summary>
    IsNeverPrefetchable = 0,

    /// <summary>
    /// in the current state the plug support prefetch processing
    /// </summary>
    IsYetPrefetchable,

    /// <summary>
    /// in the current state the plug does not support prefetch processing
    /// </summary>
    IsNotYetPrefetchable,
}