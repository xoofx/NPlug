// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using NPlug.IO;

namespace NPlug;

public abstract partial class AudioController<TAudioControllerModel>
{
    private AudioUnit _selectedUnit;
    private readonly Dictionary<(BusMediaType, BusDirection, int, int), AudioUnit> _mapBusToUnit;

    protected void ClearMapBusToUnit()
    {
        _mapBusToUnit.Clear();
    }

    protected void SetMappingBusToUnit(BusMediaType type, BusDirection dir, int busIndex, int channel, AudioUnit unit)
    {
        _mapBusToUnit[(type, dir, busIndex, channel)] = unit;
    }
    
    public AudioUnit SelectedUnit
    {
        get { return _selectedUnit; }
        set
        {
            if (value != _selectedUnit)
            {
                _selectedUnit = value;
                var handler = Handler;
                if (handler is not null && handler.IsUnitAndProgramListSupported)
                {
                    handler.NotifyUnitSelection(value.Id);
                }
            }
        }
    }

    protected virtual bool TryGetUnitByBus(BusMediaType type, BusDirection dir, int busIndex, int channel, [NotNullWhen(true)] out AudioUnit? unit)
    {
        return _mapBusToUnit.TryGetValue((type, dir, busIndex, channel), out unit);
    }

    protected virtual void SetUnitData(AudioUnit unit, Stream input)
    {
        var reader = new PortableBinaryReader(input, false);
        unit.Load(reader, AudioProcessorModelStorageMode.SkipProgramChangeParameters);
    }

    protected virtual void SetProgramData(AudioProgramList programList, int programIndex, Stream input)
    {
        var program = programList[programIndex];
        program.SetProgramDataFromStream(input);
    }

    int IAudioControllerUnitInfo.UnitCount => Model.UnitCount;

    AudioUnitInfo IAudioControllerUnitInfo.GetUnitInfo(int unitIndex) => Model.GetUnitByIndex(unitIndex).UnitInfo;

    AudioUnitId IAudioControllerUnitInfo.SelectedUnit
    {
        get => _selectedUnit.UnitInfo.Id;
        set
        {
            AudioUnit? nextUnit;
            if (value.Value == 0)
            {
                nextUnit = Model;
            }
            else if (!Model.TryGetUnitById(value, out nextUnit))
            {
                throw new ArgumentException($"Invalid Unit selected with id {value}. Unit does not exist!");
            }

            if (!ReferenceEquals(nextUnit, _selectedUnit))
            {
                _selectedUnit = nextUnit;
            }
        }
    }

    bool IAudioControllerUnitInfo.TryGetUnitByBus(BusMediaType type, BusDirection dir, int busIndex, int channel, out AudioUnitId unitId)
    {
        if (TryGetUnitByBus(type, dir, busIndex, channel, out var unit))
        {
            unitId = unit.Id;
            return true;
        }
        unitId = default;
        return false;
    }

    void IAudioControllerUnitInfo.SetUnitProgramData(int listOrUnitId, int programIndex, Stream input)
    {
        if (programIndex < 0)
        {
            SetUnitData(Model.GetUnitById(listOrUnitId), input);
        }
        else
        {
            SetProgramData(Model.GetProgramListById(listOrUnitId), programIndex, input);
        }
    }
}