// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

/// <summary>
/// Extended plug-in interface IEditController for key switches support.
/// </summary>
/// <remarks>
///  vstIPlug vst350 (Vst::IKeyswitchController)
/// - [plug imp]
/// - [extends IEditController]
/// - [released: 3.5.0]
/// - [optional]When a (instrument) plug-in supports such interface, the host could get from the plug-in the current set
/// of used key switches (megatrig/articulation) for a given channel of a event bus and then automatically use them (like in Cubase 6) to
/// create VST Expression Map (allowing to associated symbol to a given articulation / key switch).
/// </remarks>
public interface IAudioControllerKeySwitch : IAudioController
{
    /// <summary>
    /// Returns number of supported key switches for event bus index and channel.
    /// </summary>
    int GetKeySwitchCount(int busIndex, int channel);

    /// <summary>
    /// Returns key switch info.
    /// </summary>
    AudioControllerKeySwitchInfo GetKeySwitchInfo(int busIndex, short channel, int keySwitchIndex);
}