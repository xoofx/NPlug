// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug.Backend;

public interface IAudioParameterChangesBackend
{
    int GetParameterCount(in AudioParameterChanges parameterChanges);
    AudioParameterValueQueue GetParameterData(in AudioParameterChanges parameterChanges, int index);
    AudioParameterValueQueue AddParameterData(in AudioParameterChanges parameterChanges, AudioParameterId parameterId, out int index);
}