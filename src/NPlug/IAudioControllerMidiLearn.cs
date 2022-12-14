// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

/// <summary>
/// MIDI Learn interface.
/// </summary>
/// <remarks>
///  vstIPlug vst3612, Vst::IMidiLearn
/// - [plug imp]
/// - [extends IEditController]
/// - [released: 3.6.12]
/// - [optional]
///
/// If this interface is implemented by the edit controller, the host will call this method whenever
/// there is live MIDI-CC input for the plug-in. This way, the plug-in can change its MIDI-CC parameter
/// mapping and inform the host via the IComponentHandler::restartComponent with the
/// kMidiCCAssignmentChanged flag.
/// Use this if you want to implement custom MIDI-Learn functionality in your plug-in.
/// 
/// ```cpp
/// //------------------------------------------------
/// // in MyController class declaration
/// class MyController : public Vst::EditController, public Vst::IMidiLearn
/// {
/// 	// ...
/// 	//--- IMidiLearn ---------------------------------
/// 	tresult PLUGIN_API onLiveMIDIControllerInput (int32 busIndex, int16 channel,
/// 												  CtrlNumber midiCC) SMTG_OVERRIDE;
/// 	// ...
/// 
/// 	OBJ_METHODS (MyController, Vst::EditController)
/// 	DEFINE_INTERFACES
/// 		// ...
/// 		DEF_INTERFACE (Vst::IMidiLearn)
/// 	END_DEFINE_INTERFACES (Vst::EditController)
/// 	//...
/// }
/// 
/// //------------------------------------------------
/// // in mycontroller.cpp
/// #include "pluginterfaces/vst/ivstmidilearn.h
/// 
/// namespace Steinberg {
/// 	namespace Vst {
/// 		DEF_CLASS_IID (IMidiLearn)
/// 	}
/// }
/// 
/// //------------------------------------------------------------------------
/// tresult PLUGIN_API MyController::onLiveMIDIControllerInput (int32 busIndex, 
/// 							int16 channel, CtrlNumber midiCC)
/// {
/// 	// if we are not in doMIDILearn (triggered by a UI button for example) 
/// 	// or wrong channel then return
/// 	if (!doMIDILearn || busIndex != 0 || channel != 0 || midiLearnParamID == InvalidParamID)
/// 		return kResultFalse;
/// 
/// 	// adapt our internal MIDICC -&gt; parameterID mapping
/// 	midiCCMapping[midiCC] = midiLearnParamID;
/// 
/// 	// new mapping then inform the host that our MIDI assignment has changed
/// 	if (auto componentHandler = getComponentHandler ())
/// 	{
/// 		componentHandler-&gt;restartComponent (kMidiCCAssignmentChanged);
/// 	}
/// 	return kResultTrue;
/// }
/// ```
/// </remarks>
public interface IAudioControllerMidiLearn : IAudioController
{
    /// <summary>
    /// Called on live input MIDI-CC change associated to a given bus index and MIDI channel
    /// </summary>
    bool TryOnLiveMidiControllerInput(int busIndex, short channel, AudioMidiControllerNumber midiCC);
}