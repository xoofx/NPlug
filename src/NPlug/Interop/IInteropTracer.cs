// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Text;

namespace NPlug.Interop;

/// <summary>
/// Interface used to track managed &lt;-&gt; native interop for debugging purposes.
/// An implementation must be set in <see cref="InteropHelper.Tracer"/> and the tracer must be enabled at compile time. See documentation for more details.
/// </summary>
public interface IInteropTracer
{
    void OnQueryInterfaceFromHost(Guid guid, string knownInterfaceName, bool implementedByPlugin);

    void OnQueryInterfaceFromPlugin(Guid guid, string knownInterfaceName, bool implementedByHost);

    void OnEnter(in NativeToManagedEvent evt);

    void OnExit(in NativeToManagedEvent evt);

    void OnExitWithError(in NativeToManagedEvent evt);

    void OnEnter(in ManagedToNativeEvent evt);

    void OnExit(in ManagedToNativeEvent evt);

    void OnExitWithError(in ManagedToNativeEvent evt);

    void LogInfo(string message);
}