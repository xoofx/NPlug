// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

public interface IAudioControllerHandler
{
    /// <summary>
    /// To be called before calling a performEdit (e.g. on mouse-click-down event).
    /// This must be called in the UI-Thread context!
    /// </summary>
    void BeginEdit(AudioParameterId id);

    /// <summary>
    /// Called between beginEdit and endEdit to inform the handler that a given parameter has a new
    /// value. This must be called in the UI-Thread context!
    /// </summary>
    void PerformEdit(AudioParameterId id, double valueNormalized);

    /// <summary>
    /// To be called after calling a performEdit (e.g. on mouse-click-up event).
    /// This must be called in the UI-Thread context!
    /// </summary>
    void EndEdit(AudioParameterId id);

    /// <summary>
    /// Instructs host to restart the component. This must be called in the UI-Thread context!
    /// </summary>
    /// <param name="flags">is a combination of RestartFlags</param>
    void RestartComponent(AudioRestartFlags flags);

    /// <summary>
    /// True if <see cref="SetDirty"/>, <see cref="RequestOpenEditor"/>, <see cref="StartGroupEdit"/> and <see cref="FinishGroupEdit"/>  are supported.
    /// </summary>
    /// <remarks>
    /// This is equivalent of checking `IComponentHandler2` for VST3.
    /// </remarks>
    bool IsAdvancedEditSupported { get; }

    /// <summary>
    /// Tells host that the plug-in is dirty (something besides parameters has changed since last save),
    /// if true the host should apply a save before quitting.
    /// </summary>
    /// <remarks>
    /// Only supported if <see cref="IsAdvancedEditSupported"/> is <c>true</c>.
    /// </remarks>
    void SetDirty(bool state);

    /// <summary>
    /// Tells host that it should open the plug-in editor the next time it's possible.
    /// You should use this instead of showing an alert and blocking the program flow (especially on loading projects).
    /// </summary>
    /// <remarks>
    /// Only supported if <see cref="IsAdvancedEditSupported"/> is <c>true</c>.
    /// </remarks>
    void RequestOpenEditor(string name);

    /// <summary>
    /// Starts the group editing (call before a @ref IComponentHandler::beginEdit), the host will keep the current timestamp at this call and will use it for all @ref IComponentHandler::beginEdit / @ref IComponentHandler::performEdit / @ref IComponentHandler::endEdit calls until a @ref finishGroupEdit ().
    /// </summary>
    /// <remarks>
    /// Only supported if <see cref="IsAdvancedEditSupported"/> is <c>true</c>.
    /// </remarks>
    void StartGroupEdit();

    /// <summary>
    /// Finishes the group editing started by a @ref startGroupEdit (call after a @ref IComponentHandler::endEdit).
    /// </summary>
    /// <remarks>
    /// Only supported if <see cref="IsAdvancedEditSupported"/> is <c>true</c>.
    /// </remarks>
    void FinishGroupEdit();

    /// <summary>
    /// True if <see cref="CreateContextMenu"/> is supported.
    /// </summary>
    /// <remarks>
    /// This is equivalent of checking `IComponentHandler3` for VST3.
    /// </remarks>
    bool IsCreateContextMenuSupported { get; }

    /// <summary>
    /// Creates a host context menu for a plug-in:
    /// - If paramID is zero, the host may create a generic context menu.
    /// - The IPlugView object must be valid.
    /// - The return IContextMenu object needs to be released afterwards by the plug-in.
    /// </summary>
    /// <remarks>
    /// Only supported if <see cref="IsCreateContextMenuSupported"/> is <c>true</c>.
    /// </remarks>
    IAudioContextMenu CreateContextMenu(IAudioPluginView plugView, AudioParameterId paramID);

    /// <summary>
    /// Returns <c>true</c> if <see cref="RequestBusActivation"/> is supported.
    /// </summary>
    /// <remarks>
    /// This is equivalent of checking `IComponentHandlerBusActivation` for VST3.
    /// </remarks>
    bool IsRequestBusActivationSupported { get; }

    /// <summary>
    /// request the host to activate or deactivate a specific bus.
    /// </summary>
    /// <remarks>
    /// Only supported if <see cref="IsRequestBusActivationSupported"/> is <c>true</c>.
    /// </remarks>
    void RequestBusActivation(BusMediaType type, BusDirection dir, int index, bool state);

    /// <summary>
    /// Returns <c>true</c> if <see cref="StartProgress"/> is supported.
    /// </summary>
    /// <remarks>
    /// This is equivalent of checking `IProgress` for VST3.
    /// </remarks>
    bool IsProgressSupported { get; }

    /// <summary>
    /// Start a new progress of a given type and optional Description. outID is as ID created by the
    /// host to identify this newly created progress (for update and finish method)
    /// </summary>
    /// <remarks>
    /// Only supported if <see cref="IsProgressSupported"/> is <c>true</c>.
    /// </remarks>
    AudioProgressId StartProgress(AudioProgressType type, string? optionalDescription);

    /// <summary>
    /// Update the progress value (normValue between [0, 1]) associated to the given id
    /// </summary>
    /// <remarks>
    /// Only supported if <see cref="IsProgressSupported"/> is <c>true</c>.
    /// </remarks>
    void UpdateProgress(AudioProgressId id, double normValue);

    /// <summary>
    /// Finish the progress associated to the given id
    /// </summary>
    /// <remarks>
    /// Only supported if <see cref="IsProgressSupported"/> is <c>true</c>.
    /// </remarks>
    void FinishProgress(AudioProgressId id);

    /// <summary>
    /// Returns <c>true</c> if <see cref="NotifyUnitSelection"/> and <see cref="NotifyProgramListChange"/> are supported.
    /// </summary>
    /// <remarks>
    /// This is equivalent of checking `IUnitHandler` for VST3.
    /// </remarks>
    bool IsUnitAndProgramListSupported { get; }

    /// <summary>
    /// Notify host when a module is selected in plug-in GUI.
    /// </summary>
    /// <remarks>
    /// Only supported if <see cref="IsUnitAndProgramListSupported"/> is <c>true</c>.
    /// </remarks>
    void NotifyUnitSelection(AudioUnitId unitId);

    /// <summary>
    /// Tell host that the plug-in controller changed a program list (rename, load, PitchName changes).
    /// </summary>
    /// <param name="listId">is the specified program list ID to inform.</param>
    /// <param name="programIndex">: when kAllProgramInvalid, all program information is invalid, otherwise only the program of given index.</param>
    /// <remarks>
    /// Only supported if <see cref="IsUnitAndProgramListSupported"/> is <c>true</c>.
    /// </remarks>
    void NotifyProgramListChange(AudioProgramListId listId, int programIndex);
}

public record struct AudioProgressId(ulong Value);

/// <summary>
///
/// </summary>
public enum AudioProgressType : uint
{
    /// <summary>
    /// plug-in state is restored async (in a background Thread)
    /// </summary>
    AsyncStateRestoration = 0,

    /// <summary>
    /// a plug-in task triggered by a UI action
    /// </summary>
    UIBackgroundTask,
}

public readonly record struct AudioProgramListId(int Value)
{
    public static readonly AudioProgramListId NoPrograms = new (-1);

    public static implicit operator AudioProgramListId(int value) => new(value);
}