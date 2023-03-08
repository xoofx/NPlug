// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

/// <summary>
/// Defines how a <see cref="AudioProcessorModel"/> is stored.
/// </summary>
public enum AudioProcessorModelStorageMode
{
    /// <summary>
    /// Use the optimized storage mode by default.
    /// </summary>
    Default,

    /// <summary>
    /// Do not notify of program change parameters when loading the model.
    /// </summary>
    SkipProgramChangeParameters,
}