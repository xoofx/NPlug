// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

/// <summary>
/// Extended plug-in interface IEditController for Inter-App Audio Preset Management.
/// </summary>
/// <remarks>
///  vstIPlug vst360: IInterAppAudioPresetManager
/// - [plug imp]
/// - [extends IEditController]
/// - [released: 3.6.0]
/// </remarks>
public interface IAudioControllerInterAppAudioPresetManager : IAudioController
{
    /// <summary>
    /// Open the Preset Browser in order to load a preset
    /// </summary>
    void RunLoadPresetBrowser();

    /// <summary>
    /// Open the Preset Browser in order to save a preset
    /// </summary>
    void RunSavePresetBrowser();

    /// <summary>
    /// Load the next available preset
    /// </summary>
    void LoadNextPreset();

    /// <summary>
    /// Load the previous available preset
    /// </summary>
    void LoadPreviousPreset();
}