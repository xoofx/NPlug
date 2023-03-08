// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace NPlug.Interop;

/// <summary>
/// Helper class to trace interop calls.
/// </summary>
public static class InteropHelper
{
    /// <summary>
    /// Gets a boolean indicating if tracing is enabled (This is setup at compile time via the `NPlugInteropTracer` MSBuild property).
    /// </summary>
    public static readonly bool IsTracerEnabled = GetIsTracedEnabled();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool GetIsTracedEnabled() => AppContext.TryGetSwitch("NPlug.Interop.InteropHelper.IsTracerEnabled", out var isEnabled) && isEnabled;

    /// <summary>
    /// Gets or sets the current tracer for interop events.
    /// </summary>
    public static IInteropTracer? Tracer { get; set; }

    /// <summary>
    /// Reports if there are any live COM objects.
    /// </summary>
    /// <returns></returns>
    public static bool HasObjectAlive()
    {
        return LibVst.ComObjectManager.Instance.GetAliveComObjects().Length > 0;
    }

    /// <summary>
    /// Dumps all live COM objects to a string.
    /// </summary>
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


