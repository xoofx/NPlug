// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using NPlug.Interop;

namespace NPlug;

/// <summary>
/// PhysicalUIMap describes a mapping of a noteExpression Type to a Physical UI Type.
/// It is used in PhysicalUIMapList.
/// </summary>
/// <seealso cref="LibVst.PhysicalUIMapList "/>
public struct AudioPhysicalUIMap
{
    /// <summary>
    /// This represents the physical UI. This is set by the caller of <see cref="IAudioControllerNoteExpressionPhysicalUIMapping.TryGetPhysicalUIMapping"/>
    /// </summary>
    public readonly AudioPhysicalUITypeId PhysicalUITypeId;

    /// <summary>
    /// This represents the associated noteExpression TypeID to the given physicalUITypeID. This will be filled by the plug-in in the call <see cref="IAudioControllerNoteExpressionPhysicalUIMapping.TryGetPhysicalUIMapping"/>,
    /// set it to <see cref="AudioPhysicalUITypeId.Invalid"/> if no Note Expression is associated to the given PUI.
    /// </summary>
    public AudioNoteExpressionTypeId NoteExpressionTypeId;
}