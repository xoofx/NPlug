// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.IO;

namespace NPlug.Vst3;

internal static unsafe partial class LibVst
{
    /// <summary>
    /// Managed equivalent is <see cref="Stream"/>.
    /// </summary>
    public partial struct IBStream
    {
        private static partial ComResult read_ccw(ComObject* self, void* buffer, int numBytes, int* numBytesRead)
        {
            var stream = (Stream)self->Handle.Target!;
            try
            {
                *numBytesRead = stream.Read(new Span<byte>(buffer, numBytes));
                return ComResult.Ok;
            }
            catch
            {
                return ComResult.InternalError;
            }
        }

        private static partial ComResult write_ccw(ComObject* self, void* buffer, int numBytes, int* numBytesWritten)
        {
            var stream = (Stream)self->Handle.Target!;
            try
            {
                stream.Write(new Span<byte>(buffer, numBytes));
                *numBytesWritten = numBytes;
                return ComResult.Ok;
            }
            catch
            {
                return ComResult.InternalError;
            }
        }

        private static partial ComResult seek_ccw(ComObject* self, long pos, int mode, long* result)
        {
            var stream = (Stream)self->Handle.Target!;
            try
            {
                *result = stream.Seek(pos, mode == 0 ? SeekOrigin.Begin : mode == 1 ? SeekOrigin.Current : SeekOrigin.End);
                return ComResult.Ok;
            }
            catch
            {
                return ComResult.InternalError;
            }
        }

        private static partial ComResult tell_ccw(ComObject* self, long* pos)
        {
            var stream = (Stream)self->Handle.Target!;
            try
            {
                *pos = stream.Position;
                return ComResult.Ok;
            }
            catch
            {
                return ComResult.InternalError;
            }
        }
    }
}