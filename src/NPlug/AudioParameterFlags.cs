// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug;

/// <summary>
/// Flags associated with a parameter in <see cref="AudioParameter.Flags"/>.
/// </summary>
[Flags]
public enum AudioParameterFlags
{
    /// <summary>
    /// no flags wanted
    /// </summary>
    NoFlags = 0,

    /// <summary>
    /// parameter can be automated
    /// </summary>
    CanAutomate = 1 << 0,

    /// <summary>
    /// parameter cannot be changed from outside the plug-in (implies that kCanAutomate is NOT set)
    /// </summary>
    IsReadOnly = 1 << 1,

    /// <summary>
    /// attempts to set the parameter value out of the limits will result in a wrap around [SDK 3.0.2]
    /// </summary>
    IsWrapAround = 1 << 2,

    /// <summary>
    /// parameter should be displayed as list in generic editor or automation editing [SDK 3.1.0]
    /// </summary>
    IsList = 1 << 3,

    /// <summary>
    /// parameter should be NOT displayed and cannot be changed from outside the plug-in 
    /// (implies that kCanAutomate is NOT set and kIsReadOnly is set) [SDK 3.7.0]
    /// </summary>
    IsHidden = 1 << 4,

    /// <summary>
    /// parameter is a program change (unitId gives info about associated unit 
    /// - see @ref vst3ProgramLists)
    /// </summary>
    IsProgramChange = 1 << 15,

    /// <summary>
    /// special bypass parameter (only one allowed): plug-in can handle bypass
    /// (highly recommended to export a bypass parameter for effect plug-in)
    /// </summary>
    IsBypass = 1 << 16,
}