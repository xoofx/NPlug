// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

/// <summary>
///
/// </summary>
public enum AudioProgressType : uint
{
    /// <summary>
    /// plug-in state is restored async (in a background Thread)
    /// </summary>
    AsyncStateRestoration = 0,

    /// <summary>
    /// a plug-in task triggered by a UI action
    /// </summary>
    UIBackgroundTask,
}