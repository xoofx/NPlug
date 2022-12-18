// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug;

public abstract partial class AudioController<TAudioControllerModel>
{
    /// <summary>
    /// The parameter being edited (set/unset via <see cref="BeginEditParameter"/> and <see cref="EndEditParameter"/>)
    /// </summary>
    public AudioParameter? EditedParameter { get; private set; }

    /// <summary>
    /// Begins to edit several parameters. The host will keep the current timestamp at this call and will use it for all <see cref="BeginEditParameter"/> and <see cref="EndEditParameter"/>.
    /// </summary>
    public void BeginGroupEditParameters()
    {
        var handler = GetHandler();
        if (handler.IsAdvancedEditSupported)
        {
            handler.StartGroupEdit();
        }
    }

    /// <summary>
    /// Ends to edit several parameters. Must be paired with a <see cref="BeginGroupEditParameters"/>.
    /// </summary>
    public void EndGroupEditParameters()
    {
        var handler = GetHandler();
        if (handler.IsAdvancedEditSupported)
        {
            handler.FinishGroupEdit();
        }
    }
    
    /// <summary>
    /// Begins to edit the specified parameter. This method must be called before editing the value of a parameter.
    /// </summary>
    /// <param name="parameter">The parameter to be edited.</param>
    public void BeginEditParameter(AudioParameter parameter)
    {
        EditedParameter = parameter;
        GetHandler().BeginEdit(parameter.Id);
    }

    /// <summary>
    /// Ends to edit the current <see cref="EditedParameter"/>. This method must be called after an equivalent <see cref="BeginEditParameter"/> for the same parameter.
    /// </summary>
    public void EndEditParameter()
    {
        var editedParameter = EditedParameter!;
        if (editedParameter is null) throw new InvalidOperationException("No parameter is being edited. Must have a BeginEditParameter");
        EditedParameter = null;
        GetHandler().EndEdit(editedParameter.Id);
    }

    /// <summary>
    /// Instructs host to restart the component. This must be called in the UI-Thread context!
    /// </summary>
    /// <param name="flags">is a combination of RestartFlags</param>
    public void RestartComponent(AudioRestartFlags flags)
    {
        GetHandler().RestartComponent(flags);
    }

    private void RootUnitOnParameterValueChanged(AudioParameter obj)
    {
        if (!ReferenceEquals(EditedParameter, obj)) throw new InvalidOperationException($"The parameter {obj.Id}/{obj.Title} is being edited without a call to {nameof(BeginEditParameter)}/{nameof(EndEditParameter)}.");
        GetHandler().PerformEdit(obj.Id, obj.NormalizedValue);
    }
    
    int IAudioController.ParameterCount => Model.ParameterCount;

    AudioParameterInfo IAudioController.GetParameterInfo(int paramIndex) => Model.GetParameterByIndex(paramIndex).GetInfo();

    string IAudioController.GetParameterStringByValue(AudioParameterId id, double valueNormalized)
    {
        return Model.GetParameterById(id).ToString(valueNormalized);
    }

    double IAudioController.GetParameterValueByString(AudioParameterId id, string valueAsString)
    {
        return Model.GetParameterById(id).FromString(valueAsString);
    }

    double IAudioController.NormalizedParameterToPlain(AudioParameterId id, double valueNormalized)
    {
        return Model.GetParameterById(id).ToPlain(valueNormalized);
    }

    double IAudioController.PlainParameterToNormalized(AudioParameterId id, double plainValue)
    {
        return Model.GetParameterById(id).ToNormalized(plainValue);
    }

    double IAudioController.GetParameterNormalized(AudioParameterId id)
    {
        return Model.GetParameterById(id).NormalizedValue;
    }

    void IAudioController.SetParameterNormalized(AudioParameterId id, double valueNormalized)
    {
        Model.GetParameterById(id).RawNormalizedValue = valueNormalized;
    }
}