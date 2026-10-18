// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace NPlug;

public abstract partial class AudioController<TAudioControllerModel>
{
    /// <summary>
    /// Property set when SetParameterNormalized is being called and is going to change a parameter
    /// </summary>
    private bool ParameterValueChangedFromHost { get; set; }
    
    /// <summary>
    /// Parameters currently being edited, in the order their edit started. VST3 allows several parameters to be
    /// edited at the same time (e.g. a XY pad, or a mouse wheel on one control while dragging another).
    /// </summary>
    private readonly List<AudioParameter> _editedParameters = new();

    /// <summary>
    /// The parameter whose edit started most recently (set/unset via <see cref="BeginEditParameter"/> and <see cref="EndEditParameter()"/>),
    /// or <c>null</c> if no parameter is being edited.
    /// </summary>
    public AudioParameter? EditedParameter => _editedParameters.Count > 0 ? _editedParameters[^1] : null;

    /// <summary>
    /// Gets the number of parameters currently being edited.
    /// </summary>
    public int EditedParameterCount => _editedParameters.Count;

    /// <summary>
    /// Returns <c>true</c> if <paramref name="parameter"/> is currently being edited (between <see cref="BeginEditParameter"/> and <see cref="EndEditParameter(AudioParameter)"/>).
    /// </summary>
    /// <param name="parameter">The parameter to test.</param>
    /// <returns><c>true</c> if an edit of this parameter is in progress; otherwise <c>false</c>.</returns>
    public bool IsEditingParameter(AudioParameter parameter) => IndexOfEditedParameter(parameter) >= 0;

    /// <summary>
    /// Begins to edit several parameters. The host will keep the current timestamp at this call and will use it for all <see cref="BeginEditParameter"/> and <see cref="EndEditParameter(AudioParameter)"/>.
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
    /// <remarks>Several parameters can be edited at the same time; each edit is ended with <see cref="EndEditParameter(AudioParameter)"/>.</remarks>
    /// <exception cref="ArgumentNullException"><paramref name="parameter"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException">The parameter is already being edited, or no controller handler is connected.</exception>
    public void BeginEditParameter(AudioParameter parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter);
        // Resolve the handler first: if it is missing, no edit state is left behind.
        var handler = GetHandler();
        if (IsEditingParameter(parameter)) throw new InvalidOperationException($"The parameter {parameter.Id}/{parameter.Title} is already being edited.");
        handler.BeginEdit(parameter.Id);
        _editedParameters.Add(parameter);
    }

    /// <summary>
    /// Ends to edit the specified parameter. This method must be called after an equivalent <see cref="BeginEditParameter"/> for the same parameter.
    /// </summary>
    /// <param name="parameter">The parameter being edited.</param>
    /// <exception cref="ArgumentNullException"><paramref name="parameter"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException">The parameter is not being edited, or no controller handler is connected.</exception>
    public void EndEditParameter(AudioParameter parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter);
        var index = IndexOfEditedParameter(parameter);
        if (index < 0) throw new InvalidOperationException($"The parameter {parameter.Id}/{parameter.Title} is not being edited. Must have a {nameof(BeginEditParameter)}");
        _editedParameters.RemoveAt(index);
        GetHandler().EndEdit(parameter.Id);
    }

    /// <summary>
    /// Ends to edit the current <see cref="EditedParameter"/> (the most recently started edit). This method must be called after an equivalent <see cref="BeginEditParameter"/> for the same parameter.
    /// </summary>
    /// <exception cref="InvalidOperationException">No parameter is being edited, or no controller handler is connected.</exception>
    public void EndEditParameter()
    {
        var editedParameter = EditedParameter;
        if (editedParameter is null) throw new InvalidOperationException("No parameter is being edited. Must have a BeginEditParameter");
        EndEditParameter(editedParameter);
    }

    private int IndexOfEditedParameter(AudioParameter parameter)
    {
        var editedParameters = _editedParameters;
        for (int i = 0; i < editedParameters.Count; i++)
        {
            if (ReferenceEquals(editedParameters[i], parameter)) return i;
        }
        return -1;
    }

    /// <summary>
    /// Instructs host to restart the component. This must be called in the UI-Thread context!
    /// </summary>
    /// <param name="flags">is a combination of RestartFlags</param>
    public void RestartComponent(AudioRestartFlags flags)
    {
        GetHandler().RestartComponent(flags);
    }

    /// <summary>
    /// This method is called whenever a parameter value is changed in the model. It will replicate the change to the host via the <see cref="Handler"/>.
    /// </summary>
    /// <param name="parameter">The audio parameter that changed.</param>
    /// <param name="parameterValueChangedFromHost"><c>true</c> if the value was changed by the host.</param>
    /// <exception cref="InvalidOperationException"></exception>
    protected virtual void OnParameterValueChanged(AudioParameter parameter, bool parameterValueChangedFromHost)
    {
        if (parameterValueChangedFromHost)
        {
            if (parameter.IsProgramChange)
            {
                var unit = parameter.Unit!;
                unit.LoadProgram(((AudioStringListParameter)parameter).SelectedItem);
                // Notify the host that the all parameter values have changed due to a change of program
                RestartComponent(AudioRestartFlags.ParamValuesChanged);
            }
        }
        else
        {
            if (!IsEditingParameter(parameter)) throw new InvalidOperationException($"The parameter {parameter.Id}/{parameter.Title} is being edited without a call to {nameof(BeginEditParameter)}/{nameof(EndEditParameter)}.");
            GetHandler().PerformEdit(parameter.Id, parameter.NormalizedValue);
        }
    }

    private void OnParameterValueChangedInternal(AudioParameter parameter)
    {
        OnParameterValueChanged(parameter, ParameterValueChangedFromHost);
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
        // The event OnParameterValueChanged will be triggered with this parameter
        ParameterValueChangedFromHost = true;
        try
        {
            // Will trigger an event
            Model.GetParameterById(id).NormalizedValue = valueNormalized;
        }
        finally
        {
            ParameterValueChangedFromHost = false;
        }
    }
}