// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.IO;

namespace NPlug;

/// <summary>
/// Component extension to access unit data.
/// </summary>
/// <remarks>
///  vstIPlug vst300: IUnitData
/// - [plug imp]
/// - [extends IComponent]
/// - [released: 3.0.0]
/// - [optional]A component can support unit preset data via this interface or
/// program list data (IProgramListData).
/// </remarks>
public interface IAudioProcessorUnitData : IAudioProcessor
{
    /// <summary>
    /// Returns kResultTrue if the specified unit supports export and import of preset data.
    /// </summary>
    bool IsUnitDataSupported(AudioUnitId unitId);

    /// <summary>
    /// Gets the preset data for the specified unit.
    /// </summary>
    void GetUnitData(AudioUnitId unitId, Stream output);

    /// <summary>
    /// Sets the preset data for the specified unit.
    /// </summary>
    void SetUnitData(AudioUnitId unitId, Stream input);
}