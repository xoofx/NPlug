// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using NPlug.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using NPlug.IO;

namespace NPlug;

/// <summary>
/// The <see cref="AudioProcessor{TAudioProcessorModel}"/> model that will be shared between
/// the controller and processor. Provides a definition of units, parameters and program lists.
/// </summary>
public abstract class AudioProcessorModel : AudioUnit, IDisposable
{
    private readonly List<AudioUnit> _allUnits;
    private readonly Dictionary<AudioUnitId, int> _unitIdToIndex;
    private readonly List<AudioParameter> _allParameters;
    private readonly Dictionary<AudioParameterId, int> _parameterIdToIndex;
    private readonly List<AudioProgramList> _allProgramLists;
    private readonly Dictionary<AudioProgramListId, int> _programListIdToIndex;
    private nuint _allParameterSizeInBytes;
    private unsafe double* _pointerToBuffer;
    
    protected AudioProcessorModel(string unitName, AudioProgramList? programList = null) : base(unitName, 0, programList)
    {
        _allParameters = new List<AudioParameter>();
        _parameterIdToIndex = new Dictionary<AudioParameterId, int>();
        _allUnits = new List<AudioUnit>();
        _unitIdToIndex = new Dictionary<AudioUnitId, int>();
        _allProgramLists = new List<AudioProgramList>();
        _programListIdToIndex = new Dictionary<AudioProgramListId, int>();
    }
    
    public AudioBoolParameter? ByPassParameter { get; private set; }

    public int ParameterCount => _allParameters.Count;

    public int UnitCount => _allUnits.Count;

    public int ProgramListCount => _allProgramLists.Count;

    public bool HasProgramLists => _allProgramLists.Count > 0;

    public bool ContainsProgramList(AudioProgramListId id) => _programListIdToIndex.ContainsKey(id);

    public AudioParameter GetParameterByIndex(int index) => _allParameters[index];

    public AudioUnit GetUnitByIndex(int index) => _allUnits[index];

    public AudioProgramList GetProgramListByIndex(int index) => _allProgramLists[index];

    public AudioProgramList GetProgramListById(AudioProgramListId id) => _allProgramLists[_programListIdToIndex[id]];

    /// <summary>
    /// Event when a parameter value has changed.
    /// </summary>
    public event Action<AudioParameter>? ParameterValueChanged;

    /// <summary>
    /// Event when a program is changed for a unit.
    /// </summary>
    public event Action<AudioUnit>? SelectedProgramChanged;

    /// <summary>
    /// Add a default by-pass parameter (e.g for effects).
    /// </summary>
    /// <param name="name">The name of the parameter. Default is "ByPass".</param>
    public void AddByPassParameter(string name = "ByPass")
    {
        // Don't add a bypass parameter if it was already added.
        if (ByPassParameter is null)
        {
            ByPassParameter = new AudioBoolParameter(name, flags: AudioParameterFlags.IsBypass | AudioParameterFlags.CanAutomate);
            AddParameter(ByPassParameter);
        }
    }

    /// <summary>
    /// Initialize this model.
    /// </summary>
    /// <exception cref="InvalidOperationException">If the model has been already initialized.</exception>
    public void Initialize()
    {
        if (IsInitialized) throw new InvalidOperationException("This unit is already initialized");
        RegisterUnit(this);
        InitializeBuffer();
    }

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

    public unsafe ref double GetNormalizedValueById(AudioParameterId id)
    {
        if (!_parameterIdToIndex.TryGetValue(id, out int parameterIndex))
        {
            throw new ArgumentException($"Invalid parameter id {id}. No parameter found with this id", nameof(id));
        }

        if (_pointerToBuffer != null)
        {
            return ref _pointerToBuffer[parameterIndex];
        }

        return ref _allParameters[parameterIndex].LocalNormalizedValue;
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

    public override unsafe void Load(PortableBinaryReader reader)
    {
        // Don't try to read anything if the stream is empty.
        if (reader.Stream.Length == 0) return;

        // Fast path
        var pointerBuffer = _pointerToBuffer;
        if (pointerBuffer != null)
        {
            if (BitConverter.IsLittleEndian)
            {
                reader.Stream.ReadExactly(new Span<byte>(pointerBuffer, (int)_allParameterSizeInBytes));
            }
            else
            {
                var pValue = _pointerToBuffer;
                var endValue = _pointerToBuffer + _allParameters.Count;
                while (pValue < endValue)
                {
                    *pValue = reader.ReadFloat64();
                    pValue++;
                }
            }
        }
        else
        {
            foreach (var parameter in _allParameters)
            {
                parameter.LocalNormalizedValue = reader.ReadFloat64();
            }
        }
    }

    public override unsafe void Save(PortableBinaryWriter writer)
    {
        // Fast path
        var pointerBuffer = _pointerToBuffer;
        if (pointerBuffer != null)
        {
            // Fast path
            if (BitConverter.IsLittleEndian)
            {
                writer.Stream.Write(new ReadOnlySpan<byte>(pointerBuffer, (int)_allParameterSizeInBytes));
            }
            else
            {
                var pValue = _pointerToBuffer;
                var endValue = _pointerToBuffer + _allParameters.Count;
                while (pValue < endValue)
                {
                    writer.WriteFloat64(*pValue);
                    pValue++;
                }
            }
        }
        else
        {
            foreach (var parameter in _allParameters)
            {
                writer.WriteFloat64(parameter.LocalNormalizedValue);
            }
        }
    }

    internal override void OnParameterValueChangedInternal(AudioParameter parameter)
    {
        ParameterValueChanged?.Invoke(parameter);
    }

    internal override void OnSelectedProgramChanged(AudioUnit unit)
    {
        SelectedProgramChanged?.Invoke(unit);
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

        var parameterCount = unit.LocalParameterCount;
        for (int i = 0; i < parameterCount; i++)
        {
            var parameter = unit.GetLocalParameter(i);
            RegisterParameter(parameter);
        }

        // Initialize the program list
        if (unit.ProgramList is { } programList)
        {
            RegisterProgramList(programList);
        }
    }

    private void RegisterProgramList(AudioProgramList programList)
    {
        if (programList.Id == 0)
        {
            programList.Id = _allProgramLists.Count + 1;
        }

        // If the program list is shared, don't throw if it is multi-referenced
        if (_programListIdToIndex.TryGetValue(programList.Id, out var programListIndex))
        {
            var existingProgramList = _allProgramLists[programListIndex];
            if (existingProgramList != programList)
            {
                throw new InvalidOperationException($"Unable to add the program list with the name `{programList.Name}`. A program list with the same id {programList.Id} and name `{existingProgramList.Name}` is already added");
            }
            return;
        }

        _programListIdToIndex.Add(programList.Id, _allProgramLists.Count);
        _allProgramLists.Add(programList);
        programList.Initialized = true;
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
   
    private unsafe void InitializeBuffer()
    {
        // Allocate the memory for all parameters (double size)
        var allParameterSizeInBytes = _allParameters.Count * sizeof(double);
        var memoryToAllocate = MathHelper.AlignToUpper((nuint)allParameterSizeInBytes, AlignedSize);
        _pointerToBuffer = (double*)NativeMemory.AlignedAlloc(memoryToAllocate, (nuint)AlignedSize);
        if (_pointerToBuffer == null) throw new OutOfMemoryException("Unable to allocate native memory for the parameters");
        NativeMemory.Fill(_pointerToBuffer, memoryToAllocate, 0);
        _allParameterSizeInBytes = (nuint)allParameterSizeInBytes;

        // Assign a pointer slot to each parameter
        var pValue = _pointerToBuffer;
        for (var i = 0; i < _allParameters.Count; i++)
        {
            var audioParameter = _allParameters[i];
            audioParameter.PointerToNormalizedValueInSharedBuffer = pValue;
            *pValue = audioParameter.NormalizedValue;
            pValue++;
        }

        // Mark all unit initialized
        foreach (var unit in _allUnits)
        {
            unit.IsInitialized = true;
        }
    }

    private static readonly uint AlignedSize = (uint)(Vector256.IsHardwareAccelerated ? Vector256<byte>.Count : Vector128.IsHardwareAccelerated ? Vector128<byte>.Count : sizeof(double));

    public unsafe void Dispose()
    {
        if (_pointerToBuffer != null)
        {
            NativeMemory.Free(_pointerToBuffer);
            _pointerToBuffer = null;
            _allParameterSizeInBytes = 0;
        }
    }
}