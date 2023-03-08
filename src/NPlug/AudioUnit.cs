// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using NPlug.IO;
using System;
using System.Collections.Generic;

namespace NPlug;

/// <summary>
/// Defines an audio unit.
/// </summary>
public class AudioUnit
{
    private readonly List<AudioParameter> _parameters;
    private readonly List<AudioUnit> _children;
    private AudioUnitId _id;

    /// <summary>
    /// Creates a new instance of an audio unit.
    /// </summary>
    public AudioUnit(string unitName, AudioProgramListBuilder? programListBuilder = null, int id = 0)
    {
        Name = unitName;
        _id = id;
        _children = new List<AudioUnit>();
        _parameters = new List<AudioParameter>();
        ProgramListBuilder = programListBuilder;
    }

    /// <summary>
    /// Gets the unit info,
    /// </summary>
    public AudioUnitInfo UnitInfo => new (_id)
    {
        Name = Name,
        ParentUnitId = ParentUnit?.UnitInfo.Id ?? AudioUnitId.NoParent,
        ProgramListId = ProgramList?.Id ?? AudioProgramListId.NoPrograms
    };

    /// <summary>
    /// Gets the id of this unit.
    /// </summary>
    public AudioUnitId Id
    {
        get => _id;
        internal set => _id = value;
    }

    /// <summary>
    /// Gets the name of this unit.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets a boolean indicating whether this unit has been initialized.
    /// </summary>
    public bool Initialized
    {
        get;
        internal set;
    }

    /// <summary>
    /// Gets or initialized the specified program list builder associated with this unit.
    /// </summary>
    public AudioProgramListBuilder? ProgramListBuilder { get; init; }

    /// <summary>
    /// Gets the program list associated with this unit that was built from <see cref="ProgramListBuilder"/>.
    /// </summary>
    public AudioProgramList? ProgramList { get; internal set; }

    /// <summary>
    /// Gets the parameter attached to the program list change that was built from <see cref="ProgramListBuilder"/>.
    /// </summary>
    public AudioStringListParameter? ProgramChangeParameter { get; internal set; }

    /// <summary>
    /// Gets or sets the index of the selected program if this unit has a program list.
    /// </summary>
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

    /// <summary>
    /// Gets the number of parameter defined by this unit without including child units.
    /// </summary>
    public int LocalParameterCount => _parameters.Count;

    /// <summary>
    /// Gets the parent unit of this unit.
    /// </summary>
    public AudioUnit? ParentUnit { get; private set; }

    /// <summary>
    /// Gets the number of child unit.
    /// </summary>
    public int ChildUnitCount => _children.Count;

    /// <summary>
    /// Gets the parameter attached directly to this unit.
    /// </summary>
    public AudioParameter GetLocalParameter(int index)
    {
        return _parameters[index];
    }

    /// <summary>
    /// Gets the child unit at the specified index.
    /// </summary>
    public AudioUnit GetChildUnit(int index) => _children[index];

    /// <summary>
    /// Adds the specified unit to this unit.
    /// </summary>
    /// <typeparam name="TAudioUnit">Type of the unit.</typeparam>
    /// <param name="unit">Instance of the unit to add.</param>
    /// <returns>The unit passed.</returns>
    /// <exception cref="ArgumentException">If the unit was already attached to another unit.</exception>
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

    /// <summary>
    /// Loads the program data for the specified program index.
    /// </summary>
    /// <param name="programIndex">The index of the program.</param>
    /// <exception cref="InvalidOperationException">If the program does not have a <see cref="AudioProgram.GetProgramData"/> attached.</exception>
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

    /// <summary>
    /// Loads the data of this unit from the specified reader.
    /// </summary>
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

    /// <summary>
    /// Saves the data of this unit to the specified writer.
    /// </summary>
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

    /// <summary>
    /// Adds the specified parameter to this unit.
    /// </summary>
    /// <typeparam name="TAudioParameter">The type of the parameter</typeparam>
    /// <param name="parameter">The parameter.</param>
    /// <returns>The parameter added</returns>
    /// <exception cref="ArgumentException">If the parameter is already attached to an existing unit.</exception>
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

    /// <inheritdoc />
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