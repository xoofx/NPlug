// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

/// <summary>
/// Structure describing a key switch.
/// This structure is used by the method <see cref="IAudioControllerKeySwitch.GetKeySwitchInfo"/>
/// </summary>
/// <seealso cref="IAudioControllerKeySwitch"/>
public sealed class AudioControllerKeySwitchInfo
{
    /// <summary>
    /// The type id.
    /// </summary>
    public required AudioControllerKeySwitchTypeId TypeId { get; init; }

    /// <summary>
    /// name of key switch (e.g. "Accentuation")
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// short title (e.g. "Acc")
    /// </summary>
    public required string ShortTitle { get; init; }

    /// <summary>
    /// associated main key switch min (value between [0, 127])
    /// </summary>
    public required int KeySwitchMin { get; init; }

    /// <summary>
    /// associated main key switch max (value between [0, 127])
    /// </summary>
    public required int KeySwitchMax { get; init; }

    /// <summary>
    /// Associated key remapped (value between [0, 127])
    /// </summary>
    public required int KeyRemapped { get; init; }

    /// <summary>
    /// id of unit this key switch belongs to (see @ref vst3Units), -1 means no unit used.
    /// </summary>
    public AudioUnitId UnitId { get; init; } = AudioUnitId.NoUnits;
}