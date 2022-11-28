// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

/// <summary>
/// Flags used in <see cref="AudioBusInfo.Flags"/>.
/// </summary>
public enum AudioBusFlags
{
    /// <summary>
    /// The bus should be activated by the host per default on instantiation (activateBus call is requested).
    /// By default a bus is inactive.
    /// </summary>
    DefaultActive = 1 << 0,

    /// <summary>
    /// The bus does not contain ordinary audio data, but data used for control changes at sample rate.
    /// The data is in the same format as the audio data [-1..1].
    /// A host has to prevent unintended routing to speakers to prevent damage.
    /// Only valid for audio media type buses.
    /// </summary>
    IsControlVoltage = 1 << 1
}