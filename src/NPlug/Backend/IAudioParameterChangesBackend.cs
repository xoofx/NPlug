// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug.Backend;

/// <summary>
/// Host interface to manipulate parameter changes.
/// </summary>
public interface IAudioParameterChangesBackend
{
    /// <summary>
    /// Gets the number of parameter changes.
    /// </summary>
    int GetParameterCount(in AudioParameterChanges parameterChanges);

    /// <summary>
    /// Gets the associated parameter data queue for the specified index.
    /// </summary>
    AudioParameterValueQueue GetParameterData(in AudioParameterChanges parameterChanges, int index);

    /// <summary>
    /// Adds a new parameter data queue for the specified parameter id.
    /// </summary>
    AudioParameterValueQueue AddParameterData(in AudioParameterChanges parameterChanges, AudioParameterId parameterId, out int index);
}