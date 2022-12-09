// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug.Interop;

public static class MarshalHelper
{
    public static unsafe IntPtr ExportToVst3(object managed)
    {
        var comObject = LibVst.ComObjectManager.Instance.GetOrCreateComObject(managed);
        return (IntPtr)comObject.GetOrCreateComInterface<LibVst.FUnknown>();
    }
}