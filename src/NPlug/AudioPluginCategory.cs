// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

/// <summary>
/// Category of an audio processor.
/// </summary>
public enum AudioPluginCategory
{
    /// <summary>
    /// The plugin is a <see cref="AudioProcessor{TAudioProcessorModel}"/>.
    /// </summary>
    Processor,

    /// <summary>
    /// The plugin is a <see cref="AudioController{TAudioControllerModel}"/>
    /// </summary>
    Controller,

    /// <summary>
    /// The plugin is a <see cref="IAudioTestProvider"/>
    /// </summary>
    TestProvider,
}