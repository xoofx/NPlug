// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;

namespace NPlug.Interop;

/// <summary>
/// Defines the event for a native to managed call.
/// </summary>
public struct NativeToManagedEvent
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal NativeToManagedEvent(IntPtr nativePointer, string interfaceName, string methodName)
    {
        NativePointer = nativePointer;
        InterfaceName = interfaceName;
        MethodName = methodName;
        InteropHelper.Tracer?.OnEnter(this);
    }

    /// <summary>
    /// Gets the native pointer of the interface.
    /// </summary>
    public readonly IntPtr NativePointer;

    /// <summary>
    /// Gets the interface name.
    /// </summary>
    public readonly string InterfaceName;

    /// <summary>
    /// Gets the method name.
    /// </summary>
    public readonly string MethodName;

    /// <summary>
    /// Gets the exception associated after calling this method.
    /// </summary>
    public Exception? Exception;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Dispose()
    {
        var watcher = InteropHelper.Tracer;
        if (watcher != null)
        {
            if (Exception != null)
            {
                watcher.OnExitWithError(this);
            }
            else
            {
                watcher.OnExit(this);
            }
        }
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return Exception == null
            ? $"<- 0x{NativePointer:x16} {InterfaceName}.{MethodName}"
            : $"-> 0x{NativePointer:x16} {InterfaceName}.{MethodName} => Exception: {Exception}";
    }
}