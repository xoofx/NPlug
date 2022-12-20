// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;

namespace NPlug.Interop;

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


    public override string ToString()
    {
        return Result == 0
            ? $"-> 0x{NativePointer:x16} {InterfaceName}.{MethodName}"
            : $"<- 0x{NativePointer:x16} {InterfaceName}.{MethodName} => Error Code: 0x{Result:x8}";
    }

}