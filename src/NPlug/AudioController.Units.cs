// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.IO;

namespace NPlug;

public abstract partial class AudioController<TAudioRootUnit>
{
    private AudioUnit _selectedUnit;

    public AudioUnit SelectedUnit => _selectedUnit;

    public event Action<AudioUnit>? OnSelectedUnitChanged;

    int IAudioControllerUnitInfo.UnitCount => RootUnit.TotalUnitCount;

    AudioUnitInfo IAudioControllerUnitInfo.GetUnitInfo(int unitIndex) => RootUnit.GetUnitFromRoot(unitIndex).UnitInfo;

    AudioUnitId IAudioControllerUnitInfo.SelectedUnit
    {
        get => _selectedUnit.UnitInfo.Id;
        set
        {
            AudioUnit? nextUnit;
            if (value.Value == 0)
            {
                nextUnit = RootUnit;
            }
            else if (!RootUnit.TryGetUnitById(value, out nextUnit))
            {
                throw new ArgumentException($"Invalid Unit selected with id {value}. Unit does not exist!");
            }

            if (!ReferenceEquals(nextUnit, _selectedUnit))
            {
                _selectedUnit = nextUnit;
                OnSelectedUnitChanged?.Invoke(nextUnit);
            }
        }
    }

    void IAudioControllerUnitInfo.GetUnitByBus(BusMediaType type, BusDirection dir, int busIndex, int channel, out AudioUnitId unitId)
    {
        throw new NotImplementedException();
    }

    void IAudioControllerUnitInfo.SetUnitProgramData(int listOrUnitId, int programIndex, Stream input)
    {
        throw new NotImplementedException();
    }
}