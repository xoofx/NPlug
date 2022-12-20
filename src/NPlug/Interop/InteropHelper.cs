// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace NPlug.Interop;

public static class InteropHelper
{
    public static readonly bool IsTracerEnabled = GetIsTracedEnabled();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool GetIsTracedEnabled() => AppContext.TryGetSwitch("NPlug.Interop.InteropHelper.IsTracerEnabled", out var isEnabled) && isEnabled;

    /// <summary>
    /// A watcher for interop events.
    /// </summary>
    public static IInteropTracer? Tracer { get; set; }
    
    public static bool HasObjectAlive()
    {
        return LibVst.ComObjectManager.Instance.GetAliveComObjects().Length > 0;
    }
    
    public static string DumpObjectAlive()
    {
        var builder = new StringBuilder();
        foreach (var comObject in LibVst.ComObjectManager.Instance.GetAliveComObjects())
        {
            builder.AppendLine($"COM Object: {comObject.Target?.GetType().FullName} RefCount: {comObject.ReferenceCount} InterfaceCount: {comObject.InterfaceCount}");
            for (int i = 0; i < comObject.InterfaceCount; i++)
            {
                var ptr = comObject.GetInterfacePointer(i, out var guid);
                builder.AppendLine($"    [{i}] {guid} => 0x{ptr:X16}");
            }
        }

        return builder.ToString();
    }
}


