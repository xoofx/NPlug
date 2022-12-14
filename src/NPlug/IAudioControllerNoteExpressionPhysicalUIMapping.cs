// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug;

/// <summary>
/// Extended plug-in interface IEditController for note expression event support.
/// </summary>
/// <remarks>
///  vstIPlug vst3611, Vst::INoteExpressionPhysicalUIMapping
/// - [plug imp]
/// - [extends IEditController]
/// - [released: 3.6.11]
/// - [optional]With this plug-in interface, the host can retrieve the preferred physical mapping associated to note
/// expression supported by the plug-in.
/// When the mapping changes (for example when switching presets) the plug-in needs
/// to inform the host about it via @ref IComponentHandler::restartComponent (kNoteExpressionChanged). INoteExpressionPhysicalUIMappingExample Example
///
/// ```cpp
/// //------------------------------------------------------------------------
/// // here an example of how a VST3 plug-in could support this INoteExpressionPhysicalUIMapping interface.
/// // we need to define somewhere the iids:
/// 
/// //in MyController class declaration
/// class MyController : public Vst::EditController, public Vst::INoteExpressionPhysicalUIMapping
/// {
/// 	// ...
/// 	//--- INoteExpressionPhysicalUIMapping ---------------------------------
/// 	tresult PLUGIN_API getPhysicalUIMapping (int32 busIndex, int16 channel, PhysicalUIMapList&amp; list) SMTG_OVERRIDE;
/// 	// ...
/// 
/// 	OBJ_METHODS (MyController, Vst::EditController)
/// 	DEFINE_INTERFACES
/// 		// ...
/// 		DEF_INTERFACE (Vst::INoteExpressionPhysicalUIMapping)
/// 	END_DEFINE_INTERFACES (Vst::EditController)
/// 	//...
/// }
/// 
/// // In mycontroller.cpp
/// #include "pluginterfaces/vst/ivstnoteexpression.h"
/// 
/// namespace Steinberg {
/// 	namespace Vst {
/// 		DEF_CLASS_IID (INoteExpressionPhysicalUIMapping)
/// 	}
/// }
/// //------------------------------------------------------------------------
/// tresult PLUGIN_API MyController::getPhysicalUIMapping (int32 busIndex, int16 channel, PhysicalUIMapList&amp; list)
/// {
/// 	if (busIndex == 0 &amp;&amp; channel == 0)
/// 	{
/// 		for (uint32 i = 0; i &lt; list.count; ++i)
/// 		{
/// 			NoteExpressionTypeID type = kInvalidTypeID;
/// 			if (kPUIXMovement == list.map[i].physicalUITypeID)
/// 				list.map[i].noteExpressionTypeID = kCustomStart + 1;
/// 			else if (kPUIYMovement == list.map[i].physicalUITypeID)
/// 				list.map[i].noteExpressionTypeID = kCustomStart + 2;
/// 		}
/// 		return kResultTrue;
/// 	}
/// 	return kResultFalse;
/// }
/// ```
/// </remarks>
public interface IAudioControllerNoteExpressionPhysicalUIMapping : IAudioController
{
    /// <summary>
    /// Fills the list of mapped [physical UI (in) - note expression (out)] for a given bus index
    /// and channel.
    /// </summary>
    bool TryGetPhysicalUIMapping(int busIndex, short channel, Span<AudioPhysicalUIMap> mapList);
}