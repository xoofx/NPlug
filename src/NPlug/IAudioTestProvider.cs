// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug;

/// <summary>
/// This class provides access to the component and the controller of a plug-in when running a unit test.
/// </summary>
public interface IAudioTestProvider : IAudioPluginObject
{
    /// <summary>
    /// Gets the processor for this test provider.
    /// </summary>
    IAudioProcessor GetAudioProcessor();

    /// <summary>
    /// Gets the controller for this test provider.
    /// </summary>
    /// <returns></returns>
    IAudioController GetAudioController();

    /// <summary>
    /// Gets the category of the audio processor.
    /// </summary>
    /// <returns></returns>
    AudioProcessorCategory GetAudioProcessorCategory();

    /// <summary>
    /// Gets the ClassId of the audio processor.
    /// </summary>
    /// <returns></returns>
    Guid GetAudioProcessorClassId();

    /// <summary>
    /// Gets the factory used by this test provider.
    /// </summary>
    /// <returns></returns>
    IAudioPluginFactory GetPluginFactory();
}