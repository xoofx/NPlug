// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial record struct ParamValue
    {
        public static implicit operator double(ParamValue paramValue) => paramValue.Value;
        public static implicit operator ParamValue(double value) => new ParamValue(value);
    }
}
