// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;

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

    public static unsafe IntPtr ExportToVst3(object managed)
    {
        var comObject = LibVst.ComObjectManager.Instance.GetOrCreateComObject(managed);
        return (IntPtr)comObject.QueryInterface<LibVst.FUnknown>();
    }
}

public interface IInteropTracer
{
    void OnEnter(in NativeToManagedEvent evt);

    void OnExit(in NativeToManagedEvent evt);

    void OnExitWithError(in NativeToManagedEvent evt);

    void OnEnter(in ManagedToNativeEvent evt);

    void OnExit(in ManagedToNativeEvent evt);

    void OnExitWithError(in ManagedToNativeEvent evt);
}

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

    public readonly IntPtr NativePointer;

    public readonly string InterfaceName;

    public readonly string MethodName;

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
}

public struct ManagedToNativeEvent
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ManagedToNativeEvent(IntPtr nativePointer, string interfaceName, string methodName)
    {
        NativePointer = nativePointer;
        InterfaceName = interfaceName;
        MethodName = methodName;
        InteropHelper.Tracer?.OnEnter(this);
    }

    public readonly IntPtr NativePointer;

    public readonly string InterfaceName;

    public readonly string MethodName;

    public int Result;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Dispose()
    {
        var watcher = InteropHelper.Tracer;
        if (watcher != null)
        {
            if (Result != LibVst.ComResult.Ok && Result != LibVst.ComResult.False)
            {
                watcher.OnExitWithError(this);
            }
            else
            {
                watcher.OnExit(this);
            }
        }
    }
}