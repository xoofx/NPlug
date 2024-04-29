// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Runtime.InteropServices;

namespace NPlug.Interop;

/// <summary>
/// This class is responsible for exporting the current registered factory in <see cref="AudioPluginFactoryExporter.Instance"/>.
/// </summary>
internal static partial class NPlugFactoryExport
{
    [UnmanagedCallersOnly(EntryPoint = nameof(bundleEntry))]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Exported function required by VST3 API")]
    // ReSharper disable once InconsistentNaming
    private static bool bundleEntry(nint bundlePointer)
    {
        return true;
    }

    [UnmanagedCallersOnly(EntryPoint = nameof(bundleExit))]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Exported function required by VST3 API")]
    // ReSharper disable once InconsistentNaming
    private static bool bundleExit(nint bundlePointer)
    {
        return true;
    }
}