// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug.Backend;

public interface IAudioMessageBackend
{
    string GetId(in AudioMessage message);
    void SetId(in AudioMessage message, string id);
    void Destroy(in AudioMessage message);
}