// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.IO;
using System.Runtime.InteropServices;

namespace NPlug.Proxy;

public sealed class AudioPluginProxy : IDisposable
{
    public const string DefaultFileName = "nplug_proxy";

    private readonly IntPtr _nativeProxyHandle;
    private readonly bool _ownHandle;
    private Func<IntPtr>? _nativeFactory;
    private readonly InternalGetFactoryDelegate _internalGetFactoryDelegate;
    private GCHandle _internalGetFactoryDelegateHandle;
    private bool _disposed;

    private AudioPluginProxy(IntPtr nativeProxyHandle, bool ownHandle)
    {
        _nativeProxyHandle = nativeProxyHandle;
        _ownHandle = ownHandle;

        var nativeProxySetFactory = NativeLibrary.GetExport(nativeProxyHandle, "nplug_set_plugin_factory");
        if (nativeProxySetFactory == IntPtr.Zero)
        {
            throw new InvalidOperationException("Missing nplug_set_plugin_factory from the proxy library");
        }

        _internalGetFactoryDelegate = InternalGetFactory;
        _internalGetFactoryDelegateHandle = GCHandle.Alloc(_internalGetFactoryDelegate, GCHandleType.Normal);
        _nativeFactory = null;
        // Setup the proxy immediately
        unsafe
        {
            ((delegate*unmanaged<void*, void>)nativeProxySetFactory)((void*)Marshal.GetFunctionPointerForDelegate(_internalGetFactoryDelegate));
        }
    }

    public void SetNativeFactory(Func<IntPtr> nativeFactory)
    {
        _nativeFactory = nativeFactory;
    }

    public static string GetDefaultPath() => Path.Combine(Environment.CurrentDirectory, $"{DefaultFileName}.dll");

    public static AudioPluginProxy LoadDefault()
    {
        return Load(GetDefaultPath());
    }

    public static AudioPluginProxy Load(string nativeProxyDllFilePath)
    {
        if (!File.Exists(nativeProxyDllFilePath))
        {
            throw new FileNotFoundException(nameof(nativeProxyDllFilePath));
        }
        var nativeImport = NativeLibrary.Load(nativeProxyDllFilePath);
        if (nativeImport != IntPtr.Zero)
        {
            return new AudioPluginProxy(nativeImport, true);
        }

        throw new InvalidOperationException($"Unable to load native proxy from path {nativeProxyDllFilePath}");
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _internalGetFactoryDelegateHandle.Free();
        if (_ownHandle)
        {
            NativeLibrary.Free(_nativeProxyHandle);
        }
    }
    
    private IntPtr InternalGetFactory()
    {
        return _nativeFactory?.Invoke() ?? IntPtr.Zero;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr InternalGetFactoryDelegate();
}