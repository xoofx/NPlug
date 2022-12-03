// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug.Backend;

public interface IAudioMessageBackend
{
    string GetId(in AudioMessage message);
    void SetId(in AudioMessage message, string id);
    bool TrySetInt64(in AudioMessage message, string attributeId, long value);
    bool TryGetInt64(in AudioMessage message, string attributeId, out long value);
    bool TrySetFloat64(in AudioMessage message, string attributeId, double value);
    bool TryGetFloat64(in AudioMessage message, string attributeId, out double value);
    bool TrySetString(in AudioMessage message, string attributeId, string value);
    bool TryGetString(in AudioMessage message, string attributeId, out string value);
    bool TrySetBinary(in AudioMessage message, string attributeId, ReadOnlySpan<byte> value);
    bool TryGetBinary(in AudioMessage message, string attributeId, out ReadOnlySpan<byte> value);
    void Destroy(in AudioMessage message);
}