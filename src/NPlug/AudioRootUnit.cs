// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NPlug;

public abstract class AudioRootUnit : AudioUnit
{
    private readonly List<AudioUnit> _allUnits;
    private readonly Dictionary<AudioUnitId, int> _unitIdToIndex;
    private readonly List<AudioParameter> _allParameters;
    private readonly Dictionary<AudioParameterId, int> _parameterIdToIndex;

    protected AudioRootUnit(string unitName, AudioProgramList? programList = null) : base(unitName, 0, programList)
    {
        _allParameters = new List<AudioParameter>();
        _parameterIdToIndex = new Dictionary<AudioParameterId, int>();
        _allUnits = new List<AudioUnit>();
        _unitIdToIndex = new Dictionary<AudioUnitId, int>();

        ByPassParameter = new AudioBoolParameter("ByPass", flags: AudioParameterFlags.IsBypass | AudioParameterFlags.CanAutomate);
        AddParameter(ByPassParameter);
    }
    
    public AudioBoolParameter ByPassParameter { get; }

    public void Initialize()
    {
        _allUnits.Clear();
        _unitIdToIndex.Clear();
        _allParameters.Clear();
        _parameterIdToIndex.Clear();
        RegisterUnit(this);
    }

    public int TotalParameterCount => _allParameters.Count;

    public int TotalUnitCount => _allUnits.Count;

    public AudioParameter GetParameterFromRoot(int index) => _allParameters[index];

    public AudioUnit GetUnitFromRoot(int index) => _allUnits[index];
    

    public bool TryGetParameterById(AudioParameterId id, [NotNullWhen(true)] out AudioParameter? parameter)
    {
        parameter = null;
        if (_parameterIdToIndex.TryGetValue(id, out var index))
        {
            parameter = _allParameters[index];
            return true;
        }

        return false;
    }

    public AudioParameter GetParameterById(AudioParameterId id)
    {
        if (TryGetParameterById(id, out var parameter))
        {
            return parameter;
        }

        throw new ArgumentException($"Invalid parameter id {id}. No parameter found with this id", nameof(id));
    }

    public bool TryGetUnitById(AudioUnitId id, [NotNullWhen(true)] out AudioUnit? unit)
    {
        unit = null;
        if (_unitIdToIndex.TryGetValue(id, out var index))
        {
            unit = _allUnits[index];
            return true;
        }

        return false;
    }

    private void RegisterUnit(AudioUnit unit)
    {
        RegisterSingleUnit(unit);
        var count = unit.ChildUnitCount;
        for (int i = 0; i < count; i++)
        {
            var childUnit = unit.GetChildUnit(i);
            RegisterUnit(childUnit);
        }
    }
    
    private void RegisterSingleUnit(AudioUnit unit)
    {
        if (unit.Id == 0 && unit != this)
        {
            unit.Id = _allUnits.Count;
        }

        if (_unitIdToIndex.TryGetValue(unit.Id, out var index))
        {
            throw new InvalidOperationException($"Unable to add the unit. A unit with the same id {unit.Id} (Name: {_allUnits[index].Name} is already added");
        }

        _unitIdToIndex.Add(unit.Id, _allUnits.Count);
        _allUnits.Add(unit);

        var parameterCount = unit.ParameterCount;
        for (int i = 0; i < parameterCount; i++)
        {
            var parameter = unit.GetParameter(i);
            RegisterParameter(parameter);
        }
    }

    private void RegisterParameter(AudioParameter parameter)
    {
        if (parameter.Id == 0)
        {
            // A parameter id == 0 => we assign it dynamically
            parameter.Id = _allParameters.Count + 1;
        }

        if (_parameterIdToIndex.TryGetValue(parameter.Id, out var index))
        {
            var otherParameter = _allParameters[index];
            var otherUnit = otherParameter.Unit!;
            throw new ArgumentException($"A parameter with the same identifier {parameter.Id} (Title: {otherParameter} from Unit: {otherUnit.UnitInfo.Name}) exists. This parameter cannot be added.");
        }

        _parameterIdToIndex[parameter.Id] = _allParameters.Count;
        _allParameters.Add(parameter);
    }
}