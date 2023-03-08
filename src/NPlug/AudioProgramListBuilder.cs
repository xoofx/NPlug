// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace NPlug;

/// <summary>
/// Defines an abstract builder for creating <see cref="AudioProgramList"/>.
/// </summary>
public abstract class AudioProgramListBuilder
{
    /// <summary>
    /// Creates a new instance of this builder.
    /// </summary>
    /// <param name="name">The name of the program list to build.</param>
    /// <param name="id">The id of the program list. Default is 0 and will be automatically set.</param>
    protected AudioProgramListBuilder(string name, int id = 0)
    {
        Name = name;
        Id = id;
        ProgramChangeParameterName = $"{name} Preset";
        ProgramChangeCanAutomate = false;
    }

    /// <summary>
    /// Gets the name of the program list.
    /// </summary>

    public string Name { get; }

    /// <summary>
    /// Gets the id of the program list.
    /// </summary>
    public AudioProgramListId Id { get; }

    /// <summary>
    /// Gets or sets the name of the program change parameter name.
    /// </summary>
    public string ProgramChangeParameterName { get; set; }

    /// <summary>
    /// Gets or sets the id of the program change parameter.
    /// </summary>
    public AudioParameterId ProgramChangeParameterId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the program change parameter can be automated.
    /// </summary>
    public bool ProgramChangeCanAutomate { get; set; }

    /// <summary>
    /// Build a program list from the specified unit.
    /// </summary>
    /// <param name="model">The unit model.</param>
    /// <returns>The created program list.</returns>
    public abstract AudioProgramList Build(AudioUnit model);

    /// <summary>
    /// Creates the program change parameter.
    /// </summary>
    public virtual AudioStringListParameter CreateProgramChangeParameter()
    {
        return new AudioStringListParameter(ProgramChangeParameterName, TempItems, id: ProgramChangeParameterId.Value, flags: (ProgramChangeCanAutomate ? AudioParameterFlags.CanAutomate : AudioParameterFlags.NoFlags) | AudioParameterFlags.IsList | AudioParameterFlags.IsProgramChange);
    }

    private static readonly string[] TempItems = new [] { string.Empty, string.Empty };
}

/// <summary>
/// Base class of a program list builder using the specified model.
/// </summary>
/// <typeparam name="TAudioProcessorModel">The model associated with this program list.</typeparam>
public class AudioProgramListBuilder<TAudioProcessorModel>
    : AudioProgramListBuilder
    , IEnumerable<Func<TAudioProcessorModel, AudioProgram>>
    where TAudioProcessorModel : AudioProcessorModel
{
    private readonly List<Func<TAudioProcessorModel, AudioProgram>> _dataFactories;

    /// <summary>
    /// Creates a new instance of this builder.
    /// </summary>
    /// <param name="name">The name of the program list to build.</param>
    /// <param name="id">The id of the program list. Default is 0 and will be automatically set.</param>
    public AudioProgramListBuilder(string name, int id = 0) : base(name, id)
    {
        _dataFactories = new List<Func<TAudioProcessorModel, AudioProgram>>();
    }

    /// <summary>
    /// Adds a function that can create a program from the specified model.
    /// </summary>
    /// <param name="dataProvider">The function to create a program from a specified model.</param>
    public void Add(Func<TAudioProcessorModel, AudioProgram> dataProvider)
    {
        _dataFactories.Add(dataProvider);
    }

    /// <inheritdoc />
    public override AudioProgramList Build(AudioUnit model)
    {
        var programList = new AudioProgramList(Name, Id.Value, _dataFactories.Count);

        // Initialize the program list from last to first so that the model is initialized
        // with the preset 0
        var tempList = new List<AudioProgram>(_dataFactories.Count);
        for (var i = _dataFactories.Count - 1; i >= 0; i--)
        {
            var factory = _dataFactories[i];
            var program = factory((TAudioProcessorModel)model);
            program.SetProgramDataFromUnit(model);
            tempList.Add(program);
        }

        // Add program list in order
        for (var i = tempList.Count - 1; i >= 0; i--)
        {
            var program = tempList[i];
            programList.Add(program);
        }

        // Collect program list names
        var programCount = programList.Count;
        var items = new string[programCount];
        for (int i = 0; i < programCount; i++)
        {
            items[i] = programList[i].Name;
        }

        // Set the collected names
        if (model.ProgramChangeParameter is { } presetParameters)
        {
            presetParameters.Items = items;
        }

        return programList;
    }


    IEnumerator<Func<TAudioProcessorModel, AudioProgram>> IEnumerable<Func<TAudioProcessorModel, AudioProgram>>.GetEnumerator()
    {
        return _dataFactories.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_dataFactories).GetEnumerator();
    }
}