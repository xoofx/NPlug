// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.
namespace NPlug.Vst3;


using System;

internal static unsafe partial class LibVst
{
    public partial record struct ParamID
    {
        public static implicit operator AudioParameterId(ParamID id) => new(unchecked((int)id.Value));
        public static implicit operator ParamID(AudioParameterId id) => new(unchecked((uint)id.Value));
    }
}
