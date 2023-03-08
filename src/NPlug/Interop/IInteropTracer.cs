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
    /// <summary>
    /// Logged when a query interface is issued from the host to the plugin.
    /// </summary>
    void OnQueryInterfaceFromHost(Guid guid, string knownInterfaceName, bool implementedByPlugin);

    /// <summary>
    /// Logged when a query interface is issued from the plugin to the host.
    /// </summary>
    void OnQueryInterfaceFromPlugin(Guid guid, string knownInterfaceName, bool implementedByHost);

    /// <summary>
    /// Logged when called from the host to the plugin for the specified method.
    /// </summary>
    void OnEnter(in NativeToManagedEvent evt);

    /// <summary>
    /// Logged when exiting the call from the host to the plugin for the specified method.
    /// </summary>
    void OnExit(in NativeToManagedEvent evt);

    /// <summary>
    /// Logged when exiting the call with an error from the host to the plugin for the specified method.
    /// </summary>
    void OnExitWithError(in NativeToManagedEvent evt);

    /// <summary>
    /// Logged when calling the host from the plugin for the specified method.
    /// </summary>
    void OnEnter(in ManagedToNativeEvent evt);

    /// <summary>
    /// Logged after calling the host from the plugin for the specified method.
    /// </summary>
    void OnExit(in ManagedToNativeEvent evt);

    /// <summary>
    /// Logged after calling the host with an error from the plugin for the specified method.
    /// </summary>
    void OnExitWithError(in ManagedToNativeEvent evt);

    /// <summary>
    /// Logged for other events.
    /// </summary>
    /// <param name="message"></param>
    void LogInfo(string message);
}