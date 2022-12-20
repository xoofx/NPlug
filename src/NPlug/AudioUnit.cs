// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using NPlug.IO;
using System;
using System.Collections.Generic;

namespace NPlug;

public class AudioUnit
{
    private readonly List<AudioParameter> _parameters;
    private readonly List<AudioUnit> _children;
    private AudioUnitId _id;

    public AudioUnit(string unitName, AudioProgramListBuilder? programListBuilder = null, int id = 0)
    {
        Name = unitName;
        _id = id;
        _children = new List<AudioUnit>();
        _parameters = new List<AudioParameter>();
        ProgramListBuilder = programListBuilder;
    }

    public AudioUnitInfo UnitInfo => new (_id)
    {
        Name = Name,
        ParentUnitId = ParentUnit?.UnitInfo.Id ?? AudioUnitId.NoParent,
        ProgramListId = ProgramList?.Id ?? AudioProgramListId.NoPrograms
    };

    public AudioUnitId Id
    {
        get => _id;
        internal set => _id = value;
    }

    public string Name { get; }

    public bool Initialized
    {
        get;
        internal set;
    }

    public AudioProgramListBuilder? ProgramListBuilder { get; init; }

    public AudioProgramList? ProgramList { get; internal set; }

    public AudioStringListParameter? ProgramChangeParameter { get; internal set; }

    public int SelectedProgramIndex
    {
        get => ProgramChangeParameter?.SelectedItem ?? 0;
        set
        {
            AssertProgramList();
            if (ProgramChangeParameter is null) throw new InvalidOperationException("The unit must be initialized before using the program index");

            if ((uint)value >= (uint)ProgramList!.Count) throw new ArgumentOutOfRangeException(nameof(value));
            LoadProgram(value);

            ProgramChangeParameter!.SelectedItem = value;
        }
    }

    public int LocalParameterCount => _parameters.Count;

    public AudioUnit? ParentUnit { get; private set; }

    public int ChildUnitCount => _children.Count;

    /// <summary>
    /// Gets the parameter attached directly to this unit.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public AudioParameter GetLocalParameter(int index)
    {
        return _parameters[index];
    }
    
    public AudioUnit GetChildUnit(int index) => _children[index];

    public TAudioUnit AddUnit<TAudioUnit>(TAudioUnit unit) where TAudioUnit : AudioUnit
    {
        AssertNotInitialized();
        if (unit.ParentUnit != null)
        {
            throw new ArgumentException("The unit is already attached to another container");
        }
        unit.ParentUnit = this;
        _children.Add(unit);
        return unit;
    }

    public void LoadProgram(int programIndex)
    {
        AssertProgramList();

        var program = ProgramList![programIndex];
        var stream = program.GetProgramData();
        if (stream is null)
        {
            throw new InvalidOperationException($"The program {program.Name} from unit with {Id} (Name: {Name}) does not have a program data attached");
        }
        Load(new PortableBinaryReader(stream, false), AudioProcessorModelStorageMode.SkipProgramChangeParameters);
    }

    public virtual void Load(PortableBinaryReader reader, AudioProcessorModelStorageMode mode)
    {
        //// Nothing to read
        if (reader.Stream.Length == 0) return;

        if (mode == AudioProcessorModelStorageMode.Default)
        {
            foreach (var parameter in _parameters)
            {
                parameter.RawNormalizedValue = reader.ReadFloat64();
            }
        }
        else
        {
            foreach (var parameter in _parameters)
            {
                if (parameter.IsProgramChange) continue;
                parameter.RawNormalizedValue = reader.ReadFloat64();
            }
        }

        foreach (var childUnit in _children)
        {
            childUnit.Load(reader, mode);
        }
    }

    public virtual void Save(PortableBinaryWriter writer, AudioProcessorModelStorageMode mode)
    {
        if (mode == AudioProcessorModelStorageMode.Default)
        {
            foreach (var parameter in _parameters)
            {
                writer.WriteFloat64(parameter.NormalizedValue);
            }
        }
        else
        {
            foreach (var parameter in _parameters)
            {
                if (parameter.IsProgramChange) continue;
                writer.WriteFloat64(parameter.NormalizedValue);
            }
        }
        
        foreach (var childUnit in _children)
        {
            childUnit.Save(writer, mode);
        }
    }

    internal void InsertParameter(int index, AudioParameter parameter)
    {
        _parameters.Insert(index, parameter);
        parameter.Unit = this;
    }

    public TAudioParameter AddParameter<TAudioParameter>(TAudioParameter parameter) where TAudioParameter: AudioParameter
    {
        AssertNotInitialized();
        if (parameter.Unit != null)
        {
            throw new ArgumentException($"The parameter is already attached to the unit `{parameter.Unit.UnitInfo.Name}`");
        }
        parameter.Unit = this;
        _parameters.Add(parameter);
        return parameter;
    }

    public override string ToString()
    {
        return $"Unit {UnitInfo.Name}, UnitCount = {ChildUnitCount}, ParameterCount = {LocalParameterCount}";
    }

    internal virtual void OnParameterValueChangedInternal(AudioParameter parameter)
    {
        ParentUnit?.OnParameterValueChangedInternal(parameter);
    }

    private void AssertNotInitialized()
    {
        if (Initialized) throw new InvalidOperationException("Cannot modify this unit if it is already initialized");
    }

    private void AssertProgramList()
    {
        if (ProgramList is null) throw new InvalidOperationException("No program list attached to this unit");
    }
}