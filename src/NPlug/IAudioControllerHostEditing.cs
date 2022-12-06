// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

/// <summary>
/// Parameter Editing from host.
/// </summary>
/// <remarks>
///  vstIPlug vst350 (Vst::IEditControllerHostEditing)
/// - [plug imp]
/// - [extends IEditController]
/// - [released: 3.5.0]
/// - [optional]
/// If this interface is implemented by the edit controller, and when performing edits from outside
/// the plug-in (host / remote) of a not automatable and not read-only, and not hidden flagged parameter (kind of helper parameter),
/// the host will start with a beginEditFromHost before calling setParamNormalized and end with an endEditFromHost.
/// Here the sequence that the host will call: IEditControllerExample Example
///
/// ```csharp
/// //------------------------------------------------------------------------
/// plugEditController.BeginEditFromHost(id);
/// plugEditController.SetParamNormalized(id, value);
/// plugEditController.SetParamNormalized(id, value + 0.1);
/// // ...
/// plugEditController.EndEditFromHost(id);
/// ```
/// </remarks>
/// <seealso cref="IAudioController"/>
public interface IAudioControllerHostEditing : IAudioController
{
    /// <summary>
    /// Called before a setParamNormalized sequence, a endEditFromHost will be call at the end of the editing action.
    /// </summary>
    void BeginEditFromHost(AudioParameterId parameterId);

    /// <summary>
    /// Called after a beginEditFromHost and a sequence of setParamNormalized.
    /// </summary>
    void EndEditFromHost(AudioParameterId parameterId);
}