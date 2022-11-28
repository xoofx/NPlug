// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug.Vst3;

internal static unsafe partial class LibVst
{
    public partial struct String128
    {
        public override string ToString()
        {
            fixed (char* pValue = Value)
            {
                var span = new ReadOnlySpan<char>(pValue, 128);
                var index = span.IndexOf((char)0);
                if (index >= 0)
                {
                    span = span.Slice(0, index);
                }

                return new string(span);
            }
        }
    }
}