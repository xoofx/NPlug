// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.IO;

namespace NPlug.Interop;

/// <summary>
/// Generates a log of all interop events in the temp folder "NPlug.xxxxx.log".
/// </summary>
public class TempFileInteropTracer : IInteropTracer, IDisposable
{
    private readonly TextWriter _writer;

    public TempFileInteropTracer()
    {
        var tempFile = Path.GetTempFileName();
        var directory = Path.GetDirectoryName(tempFile)!;
        var fileName = $"NPlug_{Path.GetFileNameWithoutExtension(tempFile)}.log";
        var fullFileName = Path.Combine(directory, fileName);
        FilePath = fullFileName;
        try
        {
            File.Move(tempFile, fullFileName);
            _writer = new StreamWriter(fullFileName);
        }
        catch
        {
            _writer = StreamWriter.Null;
        }
    }

    public string FilePath { get; }


    public void OnQueryInterfaceFromHost(Guid guid, string knownInterfaceName, bool implementedByPlugin)
    {
        lock (_writer)
        {
            try
            {
                _writer.WriteLine($"<- FUnknown.queryInterface Guid = {guid}, InterfaceName = {knownInterfaceName}, ProvidedByPlugin = {implementedByPlugin}");
                _writer.Flush();
            }
            catch
            {
                // ignore, never crash in the interop tracer
            }
        }
    }

    public void OnQueryInterfaceFromPlugin(Guid guid, string knownInterfaceName, bool implementedByHost)
    {
        lock (_writer)
        {
            try
            {
                _writer.WriteLine($"-> FUnknown.queryInterface Guid = {guid}, InterfaceName = {knownInterfaceName}, ProvidedByHost = {implementedByHost}");
                _writer.Flush();
            }
            catch
            {
                // ignore, never crash in the interop tracer
            }
        }
    }

    public void OnEnter(in NativeToManagedEvent evt)
    {
        lock (_writer)
        {
            try
            {
                _writer.WriteLine(evt.ToString());
                _writer.Flush();
            }
            catch
            {
                // ignore, never crash in the interop tracer
            }
        }
    }

    public void OnExit(in NativeToManagedEvent evt)
    {
        // Don't print exit event
    }

    public void OnExitWithError(in NativeToManagedEvent evt)
    {
        lock (_writer)
        {
            try
            {

                _writer.WriteLine($"Error {evt}");
                _writer.Flush();
            }
            catch
            {
                // ignore, never crash in the interop tracer
            }
        }
    }

    public void OnEnter(in ManagedToNativeEvent evt)
    {
        lock (_writer)
        {
            try
            {
                _writer.WriteLine(evt.ToString());
                _writer.Flush();
            }
            catch
            {
                // ignore, never crash in the interop tracer
            }
        }
    }

    public void OnExit(in ManagedToNativeEvent evt)
    {
        // Don't print exit event
    }

    public void OnExitWithError(in ManagedToNativeEvent evt)
    {
        lock (_writer)
        {
            try
            {
                _writer.WriteLine($"Error {evt}");
                _writer.Flush();
            }
            catch
            {
                // ignore, never crash in the interop tracer
            }
        }
    }

    public void LogInfo(string message)
    {
        lock (_writer)
        {
            try
            {
                //                  <- Message here
                //                  -> Message here
                _writer.WriteLine($"   {message}");
                _writer.Flush();
            }
            catch
            {
                // ignore, never crash in the interop tracer
            }
        }
    }

    public void Dispose()
    {
        lock (_writer)
        {
            try
            {
                _writer.Dispose();
            }
            catch
            {
                // ignore
            }
        }
    }
}