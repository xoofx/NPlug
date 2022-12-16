// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

/// <summary>
/// Edit controller extension to describe the plug-in structure: Vst::IUnitInfo
/// </summary>
/// <param name="Id">unit identifier</param>
/// <remarks>
///  vstIPlug vst300, Vst::IUnitInfo
/// - [plug imp]
/// - [extends IEditController]
/// - [released: 3.0.0]
/// - [optional]IUnitInfo describes the internal structure of the plug-in.
/// - The root unit is the component itself, so getUnitCount must return 1 at least.
/// - The root unit id has to be 0 (kRootUnitId).
/// - Each unit can reference one program list - this reference must not change.
/// - Each unit, using a program list, references one program of the list.
/// </remarks>
public sealed record AudioUnitInfo(AudioUnitId Id)
{
    /// <summary>
    /// identifier of parent unit (kNoParentUnitId: does not apply, this unit is the root)
    /// </summary>
    public AudioUnitId ParentUnitId { get; init; } = AudioUnitId.NoParent;

    /// <summary>
    /// name, optional for the root component, required otherwise
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// id of program list used in unit (kNoProgramListId = no programs used in this unit)
    /// </summary>
    public AudioProgramListId ProgramListId { get; init; } = AudioProgramListId.NoPrograms;
}