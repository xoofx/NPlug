// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug.Backend;

public interface IAudioParameterValueQueueBackend
{
    AudioParameterId GetParameterId(in AudioParameterValueQueue queue);
    int GetPointCount(in AudioParameterValueQueue queue);
    double GetPoint(in AudioParameterValueQueue queue, int index, out int sampleOffset);
    int AddPoint(in AudioParameterValueQueue queue, int sampleOffset, double parameterValue);
}