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
    private int _selectedProgramIndex;

    public AudioUnit(string unitName, int id = 0, AudioProgramList? programList = null)
    {
        Name = unitName;
        _id = id;
        _children = new List<AudioUnit>();
        _parameters = new List<AudioParameter>();
        ProgramList = programList;
        if (programList != null)
        {
            programList.ProgramDataChanged += ProgramListOnProgramDataChanged;
        }
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

    public bool IsInitialized
    {
        get;
        internal set;
    }

    public AudioProgramList? ProgramList { get; }

    public int SelectedProgramIndex
    {
        get => _selectedProgramIndex;
        set
        {
            AssertProgramList();

            if ((uint)value >= (uint)ProgramList!.Count) throw new ArgumentOutOfRangeException(nameof(value));
            LoadProgram(value);
            var previousValue = _selectedProgramIndex;
            _selectedProgramIndex = value;
            if (previousValue != value)
            {
                OnSelectedProgramChanged(this);
            }
        }
    }

    private void ProgramListOnProgramDataChanged(AudioProgram obj)
    {
        // If the selected program is on this unit and the program data has changed.
        // Load the program data into the unit
        if (obj.Index == SelectedProgramIndex)
        {
            LoadProgram(SelectedProgramIndex);
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

        var stream = ProgramList![programIndex].GetOrLoadProgramData();
        Load(new PortableBinaryReader(stream, false));
    }

    public virtual void Load(PortableBinaryReader reader)
    {
        //// Nothing to read
        if (reader.Stream.Length == 0) return;
    
        foreach (var parameter in _parameters)
        {
            parameter.NormalizedValue = reader.ReadFloat64();
        }

        foreach (var childUnit in _children)
        {
            childUnit.Load(reader);
        }
    }

    public virtual void Save(PortableBinaryWriter writer)
    {
        foreach (var parameter in _parameters)
        {
            writer.WriteFloat64(parameter.NormalizedValue);
        }

        foreach (var childUnit in _children)
        {
            childUnit.Save(writer);
        }
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

    internal virtual void OnSelectedProgramChanged(AudioUnit unit)
    {
        ParentUnit?.OnSelectedProgramChanged(unit);
    }

    private void AssertNotInitialized()
    {
        if (IsInitialized) throw new InvalidOperationException("Cannot modify this unit if it is already initialized");
    }

    private void AssertProgramList()
    {
        if (ProgramList is null) throw new InvalidOperationException("No program list attached to this unit");
    }
}