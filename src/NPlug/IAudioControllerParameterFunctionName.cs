// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

/// <summary>
/// Edit controller component interface extension.
/// </summary>
/// <remarks>
///  vstIPlug vst370, Vst::IParameterFunctionName
/// - [plug imp]
/// - [extends IEditController]
/// - [released: 3.7.0]
/// - [optional]
/// This interface allows the host to get a parameter associated to a specific meaning (a functionName)
/// for a given unit. The host can use this information, for example, for drawing a Gain Reduction meter
/// in its own UI. In order to get the plain value of this parameter, the host should use the
/// IEditController::normalizedParamToPlain. The host can automatically map parameters to dedicated UI
/// controls, such as the wet-dry mix knob or Randomize button. IParameterFunctionNameExample Example
///
/// ```cpp
/// //------------------------------------------------------------------------
/// // here an example of how a VST3 plug-in could support this IParameterFunctionName interface.
/// // we need to define somewhere the iids:
/// 
/// in MyController class declaration
/// class MyController : public Vst::EditController, public Vst::IParameterFunctionName
/// {
///     ...
///     tresult PLUGIN_API getParameterIDFromFunctionName (UnitID unitID, FIDString functionName,
///                                                     Vst::ParamID&amp; paramID) override;
///     ...
/// 
///     OBJ_METHODS (MyController, Vst::EditController)
///     DEFINE_INTERFACES
///         ...
///         DEF_INTERFACE (Vst::IParameterFunctionName)
///     END_DEFINE_INTERFACES (Vst::EditController)
///     DELEGATE_REFCOUNT (Vst::EditController)
///     ...
/// }
/// 
/// #include "ivstparameterfunctionname.h"
/// namespace Steinberg {
///     namespace Vst {
///         DEF_CLASS_IID (IParameterFunctionName)
///     }
/// }
/// 
/// //------------------------------------------------------------------------
/// tresult PLUGIN_API MyController::getParameterIDFromFunctionName (UnitID unitID, FIDString
/// functionName, Vst::ParamID&amp; paramID)
/// {
///     using namespace Vst;
/// 
///     paramID = kNoParamId;
/// 
///     if (unitID == kRootUnitId &amp;&amp; FIDStringsEqual (functionName, kCompGainReduction))
///         paramID = kMyGainReductionId;
/// 
///     return (paramID != kNoParamId) ? kResultOk : kResultFalse;
/// }
/// 
/// //--- a host implementation example: --------------------
/// ...
/// FUnknownPtr&lt;Vst::IParameterFunctionName&gt; functionName (mEditController-&gt;getIEditController ());
/// if (functionName)
/// {
///     Vst::ParamID paramID;
///     if (functionName-&gt;getParameterIDFromFunctionName (kRootUnitId,
///                                                       Vst::FunctionNameType::kCompGainReduction, paramID) == kResultTrue)
///     {
///         // paramID could be cached for performance issue
///         ParamValue norm = mEditController-&gt;getIEditController ()-&gt;getParamNormalized (paramID);
///         ParamValue plain = mEditController-&gt;getIEditController ()-&gt;normalizedParamToPlain (paramID, norm);
///         // plain is something like -6 (-6dB)
///     }
/// }
/// ```
/// </remarks>
public interface IAudioControllerParameterFunctionName : IAudioController
{
    /// <summary>
    /// Gets for the given unitID the associated paramID to a function Name. Returns false when no found parameter (paramID is set to kNoParamId in this case).
    /// </summary>
    /// <param name="unitId"></param>
    /// <param name="functionName"></param>
    /// <param name="paramId"></param>
    /// <returns></returns>
    bool TryGetParameterIdFromFunctionName(AudioUnitId unitId, string functionName, out AudioParameterId paramId);
}