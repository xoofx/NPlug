// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Runtime.InteropServices;

namespace NPlug;
public static class EntryPoint
{
    [UnmanagedCallersOnly(EntryPoint = nameof(GetPluginFactory))]
    public static IntPtr GetPluginFactory()
    {
        Console.WriteLine("Hello World");
        return IntPtr.Zero;
    }
}
