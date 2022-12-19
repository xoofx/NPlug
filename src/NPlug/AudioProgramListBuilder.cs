// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace NPlug;


public abstract class AudioProgramListBuilder
{
    protected AudioProgramListBuilder(string name, int id = 0)
    {
        Name = name;
        Id = id;
        ProgramChangeParameterName = $"{name} Preset";
        ProgramChangeCanAutomate = false;
    }

    public string Name { get; }

    public AudioProgramListId Id { get; }

    public string ProgramChangeParameterName { get; set; }

    public AudioParameterId ProgramChangeParameterId { get; set; }

    public bool ProgramChangeCanAutomate { get; set; }

    public abstract AudioProgramList Build(AudioUnit model);

    public virtual AudioStringListParameter CreateProgramChangeParameter()
    {
        return new AudioStringListParameter(ProgramChangeParameterName, TempItems, id: ProgramChangeParameterId.Value, flags: (ProgramChangeCanAutomate ? AudioParameterFlags.CanAutomate : AudioParameterFlags.NoFlags) | AudioParameterFlags.IsList | AudioParameterFlags.IsProgramChange);
    }

    private static readonly string[] TempItems = new [] { string.Empty, string.Empty };
}


public class AudioProgramListBuilder<TAudioProcessorModel>
    : AudioProgramListBuilder
    , IEnumerable<Func<TAudioProcessorModel, AudioProgram>>
    where TAudioProcessorModel : AudioProcessorModel
{
    private readonly List<Func<TAudioProcessorModel, AudioProgram>> _dataFactories;

    public AudioProgramListBuilder(string name, int id = 0) : base(name, id)
    {
        _dataFactories = new List<Func<TAudioProcessorModel, AudioProgram>>();
    }
    
    public void Add(Func<TAudioProcessorModel, AudioProgram> dataProvider)
    {
        _dataFactories.Add(dataProvider);
    }

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