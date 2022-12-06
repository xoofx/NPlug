// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug;

[Flags]
public enum AudioPluginFactoryFlags
{
    /// <summary>
    /// Nothing
    /// </summary>
    NoFlags = 0,

    /// <summary>
    /// The number of exported classes can change each time the Module is loaded. If this flag
    /// is set, the host does not cache class information. This leads to a longer startup time
    /// because the host always has to load the Module to get the current class information.
    /// </summary>
    ClassesDiscardable = 1 << 0,

    /// <summary>
    /// This flag is deprecated, do not use anymore, resp. it will get ignored from
    /// Cubase/Nuendo 12 and later.
    /// </summary>
    LicenseCheck = 1 << 1,

    /// <summary>
    /// Component will not be unloaded until process exit
    /// </summary>
    ComponentNonDiscardable = 1 << 3,

    ///// <summary>
    ///// Components have entirely unicode encoded strings (True for VST 3 plug-ins so far).
    ///// </summary>
    ///// This is always on by default
    //Unicode = 1 << 4,
}