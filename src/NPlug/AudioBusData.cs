// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug;

public ref struct AudioBusData
{
    public Span<AudioBusBuffers> Buffers;

    public AudioParameterChanges ParameterChanges;

    public AudioEventList Events;
}