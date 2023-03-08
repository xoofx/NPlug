// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug.Backend;

/// <summary>
/// Host interface to manipulate parameter data queue.
/// </summary>
public interface IAudioParameterValueQueueBackend
{
    /// <summary>
    /// Gets the parameter id associated with this queue.
    /// </summary>
    AudioParameterId GetParameterId(in AudioParameterValueQueue queue);

    /// <summary>
    /// Gets the number of point values associated with this queue.
    /// </summary>
    int GetPointCount(in AudioParameterValueQueue queue);

    /// <summary>
    /// Gets the point value at the specified index.
    /// </summary>
    double GetPoint(in AudioParameterValueQueue queue, int index, out int sampleOffset);

    /// <summary>
    /// Adds a point value at the specified sample offset.
    /// </summary>
    int AddPoint(in AudioParameterValueQueue queue, int sampleOffset, double parameterValue);
}