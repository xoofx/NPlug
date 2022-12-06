// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug.Backend;

public interface IAudioAttributeListBackend
{
    bool TrySetInt64(in AudioAttributeList attributeList, string attributeId, long value);
    bool TryGetInt64(in AudioAttributeList attributeList, string attributeId, out long value);
    bool TrySetFloat64(in AudioAttributeList attributeList, string attributeId, double value);
    bool TryGetFloat64(in AudioAttributeList attributeList, string attributeId, out double value);
    bool TrySetString(in AudioAttributeList attributeList, string attributeId, string value);
    bool TryGetString(in AudioAttributeList attributeList, string attributeId, out string value);
    bool TrySetBinary(in AudioAttributeList attributeList, string attributeId, ReadOnlySpan<byte> value);
    bool TryGetBinary(in AudioAttributeList attributeList, string attributeId, out ReadOnlySpan<byte> value);
}