// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Runtime.InteropServices;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct String128
    {
        public void CopyFrom(string name)
        {
            var maxLength = Math.Min(name.Length, sizeof(String128));
            var localSpan = MemoryMarshal.CreateSpan(ref this.Value[0], maxLength);
            name.AsSpan().Slice(0, maxLength).CopyTo(localSpan);
            if (maxLength < sizeof(String128))
            {
                localSpan[maxLength] = (char)0;
            }
        }

        public override string ToString()
        {
            fixed (char* pValue = Value)
            {
                var span = new ReadOnlySpan<char>(pValue, sizeof(String128));
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