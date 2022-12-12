// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.IO;

namespace NPlug;

/// <summary>
/// Component extension to access program list data.
/// </summary>
/// <remarks>
///  vstIPlug vst300: IProgramListData
/// - [plug imp]
/// - [extends IComponent]
/// - [released: 3.0.0]
/// - [optional]A component can support program list data via this interface or/and
/// unit preset data (IUnitData).
/// </remarks>
public interface IAudioProcessorProgramListData : IAudioProcessor
{
    /// <summary>
    /// Returns kResultTrue if the given Program List ID supports Program Data.
    /// </summary>
    bool IsProgramDataSupported(AudioProgramListId listId);

    /// <summary>
    /// Gets for a given program list ID and program index the program Data.
    /// </summary>
    void GetProgramData(AudioProgramListId listId, int programIndex, Stream output);

    /// <summary>
    /// Sets for a given program list ID and program index a program Data.
    /// </summary>
    void SetProgramData(AudioProgramListId listId, int programIndex, Stream input);
}