// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.
namespace NPlug;

/// <summary>
/// Describes the type of a key switch
/// </summary>
/// <seealso cref="AudioControllerKeySwitchInfo"/>
public enum AudioControllerKeySwitchTypeId : uint
{
    /// <summary>
    /// press before noteOn is played
    /// </summary>
    NoteOn = 0,

    /// <summary>
    /// press while noteOn is played
    /// </summary>
    OnTheFly,

    /// <summary>
    /// press before entering release
    /// </summary>
    OnRelease,

    /// <summary>
    /// key should be maintained pressed for playing
    /// </summary>
    KeyRange,
}