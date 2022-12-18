// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.IO;
using System.Runtime.InteropServices;

namespace NPlug.Proxy;

public sealed class AudioPluginProxy
{
    public const string DefaultFileName = "nplug_proxy";

    private readonly IntPtr _nativeProxyHandle;
    private readonly IntPtr _nativeProxySetFactory;
    private Func<IntPtr>? _nativeFactory;
    private readonly InternalGetFactoryDelegate _internalGetFactoryDelegate;
    private readonly GCHandle _internalGetFactoryDelegateHandle;

    private AudioPluginProxy(IntPtr nativeProxyHandle)
    {
        _nativeProxyHandle = nativeProxyHandle;

        _nativeProxySetFactory = NativeLibrary.GetExport(nativeProxyHandle, "nplug_set_plugin_factory");
        if (_nativeProxySetFactory == IntPtr.Zero)
        {
            throw new InvalidOperationException("Missing nplug_set_plugin_factory from the proxy library");
        }

        _internalGetFactoryDelegate = InternalGetFactory;
        _internalGetFactoryDelegateHandle = GCHandle.Alloc(_internalGetFactoryDelegate, GCHandleType.Normal);
        _nativeFactory = null;
        // Setup the proxy immediately
        unsafe
        {
            ((delegate*unmanaged<void*, void>)_nativeProxySetFactory)((void*)Marshal.GetFunctionPointerForDelegate(_internalGetFactoryDelegate));
        }
    }

    public void SetNativeFactory(Func<IntPtr> nativeFactory)
    {
        _nativeFactory = nativeFactory;
    }

    private IntPtr InternalGetFactory()
    {
        return _nativeFactory?.Invoke() ?? IntPtr.Zero;
    }
        
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr InternalGetFactoryDelegate();

    public static string GetDefaultPath() => Path.Combine(Environment.CurrentDirectory, $"{DefaultFileName}.dll");

    public static AudioPluginProxy Load(string nativeProxyDll)
    {
        if (!File.Exists(nativeProxyDll))
        {
            throw new FileNotFoundException(nameof(nativeProxyDll));
        }
        var nativeImport = NativeLibrary.Load(nativeProxyDll);
        if (nativeImport != IntPtr.Zero)
        {
            return new AudioPluginProxy(nativeImport);
        }

        throw new InvalidOperationException("Unable to load native proxy");
    }
}