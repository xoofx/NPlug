// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.IO;

namespace NPlug.Interop;

/// <summary>
/// Generates a log of all interop events in the temp folder "NPlug_xxxxx.log".
/// </summary>
public class TempFileInteropTracer : IInteropTracer, IDisposable
{
    private readonly TextWriter _writer;

    /// <summary>
    /// Creates a new instance of this tracer and generates a log file in the temp folder.
    /// </summary>
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

    /// <summary>
    /// Gets the full path of the log file.
    /// </summary>
    public string FilePath { get; }


    /// <inheritdoc />
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

    /// <inheritdoc />
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

    /// <inheritdoc />
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

    /// <inheritdoc />
    public void OnExit(in NativeToManagedEvent evt)
    {
        // Don't print exit event
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
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

    /// <inheritdoc />
    public void OnExit(in ManagedToNativeEvent evt)
    {
        // Don't print exit event
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
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

    /// <inheritdoc />
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