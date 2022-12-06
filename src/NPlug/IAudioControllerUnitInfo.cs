// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.IO;

namespace NPlug;

/// <summary>
/// Edit controller extension to describe the plug-in structure.
/// </summary>
/// <remarks>
///  vstIPlug vst300
/// - [plug imp]
/// - [extends IEditController]
/// - [released: 3.0.0]
/// - [optional]IUnitInfo describes the internal structure of the plug-in.
/// - The root unit is the component itself, so getUnitCount must return 1 at least.
/// - The root unit id has to be 0 (kRootUnitId).
/// - Each unit can reference one program list - this reference must not change.
/// - Each unit, using a program list, references one program of the list.
/// </remarks>
public interface IAudioControllerUnitInfo : IAudioController
{
    // --------------------------------------------------------------
    // CCW methods
    // --------------------------------------------------------------
    /// <summary>
    /// Returns the flat count of units.
    /// </summary>
    int UnitCount { get; }

    /// <summary>
    /// Gets UnitInfo for a given index in the flat list of unit.
    /// </summary>
    AudioUnitInfo GetUnitInfo(int unitIndex);

    /// <summary>
    /// Component intern program structure.
    /// </summary>
    /// <remarks>
    /// Gets the count of Program List.
    /// </remarks>
    int ProgramListCount { get; }

    /// <summary>
    /// Gets for a given index the Program List Info.
    /// </summary>
    AudioProgramListInfo GetProgramListInfo(int listIndex);

    /// <summary>
    /// Gets for a given program list ID and program index its program name.
    /// </summary>
    string GetProgramName(AudioProgramListId listId, int programIndex);

    /// <summary>
    /// Gets for a given program list ID, program index and attributeId the associated attribute value.
    /// </summary>
    string GetProgramInfo(AudioProgramListId listId, int programIndex, string attributeId);

    /// <summary>
    /// Returns kResultTrue if the given program index of a given program list ID supports PitchNames.
    /// </summary>
    bool HasProgramPitchNames(AudioProgramListId listId, int programIndex);

    /// <summary>
    /// Gets the PitchName for a given program list ID, program index and pitch.
    /// If PitchNames are changed the plug-in should inform the host with IUnitHandler::notifyProgramListChange.
    /// </summary>
    string GetProgramPitchName(AudioProgramListId listId, int programIndex, short midiPitch);

    /// <summary>
    /// Gets or sets the current selected unit.
    /// </summary>
    AudioUnitId SelectedUnit { get; set; }

    /// <summary>
    /// Gets the according unit if there is an unambiguous relation between a channel or a bus and a unit.
    /// This method mainly is intended to find out which unit is related to a given MIDI input channel.
    /// </summary>
    void GetUnitByBus(BusMediaType type, BusDirection dir, int busIndex, int channel, out AudioUnitId unitId);

    /// <summary>
    /// Receives a preset data stream.
    /// - If the component supports program list data (IProgramListData), the destination of the data
    /// stream is the program specified by list-Id and program index (first and second parameter)
    /// - If the component supports unit data (IUnitData), the destination is the unit specified by the first
    /// parameter - in this case parameter programIndex is 
    /// &lt;
    /// 0).
    /// </summary>
    void SetUnitProgramData(int listOrUnitId, int programIndex, Stream input);
}