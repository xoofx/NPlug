// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace NPlug;

public class AudioProgramListBuilder<TAudioProcessorModel>
    : IEnumerable<AudioProgram>
    where TAudioProcessorModel : AudioProcessorModel, new()
{
    private readonly List<AudioProgram> _programs;
    private readonly List<Action<TAudioProcessorModel>> _dataProviders;

    public AudioProgramListBuilder(string name, int id = 0)
    {
        Name = name;
        Id = id;
        _programs = new List<AudioProgram>();
        _dataProviders = new List<Action<TAudioProcessorModel>>();

    }

    public string Name { get; }

    public AudioProgramListId Id { get; }
    
    public void Add(AudioProgram program, Action<TAudioProcessorModel> dataProvider)
    {
        _programs.Add(program);
        _dataProviders.Add(dataProvider);
    }

    public AudioProgramList Build()
    {
        var model = new TAudioProcessorModel();
        model.Initialize();
        var programList = new AudioProgramList(Name, Id.Value);
        for (var i = 0; i < _programs.Count; i++)
        {
            var program = _programs[i];
            var modelProvider = _dataProviders[i];
            modelProvider(model);
            program.SetProgramDataFromModel(model);
            programList.Add(program);
        }

        return programList;
    }

    IEnumerator<AudioProgram> IEnumerable<AudioProgram>.GetEnumerator()
    {
        return _programs.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_programs).GetEnumerator();
    }
}