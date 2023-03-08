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

    /// <summary>
    /// Creates a new instance of this model.
    /// </summary>
    /// <param name="unitName">the name of the unit.</param>
    /// <param name="programListBuilder">The program list builder.</param>
    /// <param name="id">An associated id. Default is 0 and will be associated automatically.</param>
    protected AudioProcessorModel(string unitName = "Root", AudioProgramListBuilder? programListBuilder = null, int id = 0) : base(unitName, programListBuilder, id)
    {
        _allParameters = new List<AudioParameter>();
        _parameterIdToIndex = new Dictionary<AudioParameterId, int>();
        _allUnits = new List<AudioUnit>();
        _unitIdToIndex = new Dictionary<AudioUnitId, int>();
        _allProgramLists = new List<AudioProgramList>();
        _programListIdToIndex = new Dictionary<AudioProgramListId, int>();
    }

    /// <summary>
    /// Gets the by-pass parameter. This is optional (and only valid for audio effects, not instruments).
    /// </summary>
    public AudioBoolParameter? ByPassParameter { get; private set; }

    /// <summary>
    /// Gets the number of parameters this model contains.
    /// </summary>
    public int ParameterCount => _allParameters.Count;

    /// <summary>
    /// Gets the number of unit this model contains.
    /// </summary>
    public int UnitCount => _allUnits.Count;

    /// <summary>
    /// Gets the number of program lists this model contains.
    /// </summary>
    public int ProgramListCount => _allProgramLists.Count;

    /// <summary>
    /// Check whether this model has program lists.
    /// </summary>
    public bool HasProgramLists => _allProgramLists.Count > 0;

    /// <summary>
    /// Check whether this model contains the specified program list.
    /// </summary>
    /// <param name="id">The id of the program list.</param>
    /// <returns><c>true</c> if this model contains this program list.</returns>
    public bool ContainsProgramList(AudioProgramListId id) => _programListIdToIndex.ContainsKey(id);

    /// <summary>
    /// Gets the parameter at the specified index.
    /// </summary>
    /// <param name="index">Index of the parameter.</param>
    /// <returns>The associated parameter at the specified index.</returns>
    public AudioParameter GetParameterByIndex(int index) => _allParameters[index];

    /// <summary>
    /// Gets the unit at the specified index.
    /// </summary>
    /// <param name="index">Index of the unit.</param>
    /// <returns>The associated unit at the specified index.</returns>
    public AudioUnit GetUnitByIndex(int index) => _allUnits[index];

    /// <summary>
    /// Gets the program list at the specified index.
    /// </summary>
    /// <param name="index">Index of the program list.</param>
    /// <returns>The associated program list at the specified index.</returns>
    public AudioProgramList GetProgramListByIndex(int index) => _allProgramLists[index];

    /// <summary>
    /// Gets the program list with the specified id.
    /// </summary>
    /// <param name="id">The id of the program list.</param>
    /// <returns>The associated program list at the specified index.</returns>
    public AudioProgramList GetProgramListById(AudioProgramListId id) => _allProgramLists[_programListIdToIndex[id]];

    /// <summary>
    /// Event when a parameter value has changed.
    /// </summary>
    public event Action<AudioParameter>? ParameterValueChanged;

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
        if (Initialized) throw new InvalidOperationException("This unit is already initialized");

        RegisterUnit(this);

        InitializeProgramListParameters();

        InitializeParameters();

        // Mark all unit initialized before we initialize the program lists
        // to make sure that we can't add any parameters or units after this point
        foreach (var unit in _allUnits)
        {
            unit.Initialized = true;
        }

        InitializeProgramLists();
    }

    /// <summary>
    /// Tries to get the parameter with the specified id.
    /// </summary>
    /// <param name="id">The id of the parameter.</param>
    /// <param name="parameter">The output parameter if this method returns true, null otherwise.</param>
    /// <returns><c>true</c> if the parameter with the specified id was found.</returns>
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

    /// <summary>
    /// Gets the parameter with the specified id.
    /// </summary>
    /// <param name="id">The id of the parameter.</param>
    /// <returns>The parameter instance.</returns>
    /// <exception cref="ArgumentException">If the parameter with the specified id was not found.</exception>
    public AudioParameter GetParameterById(AudioParameterId id)
    {
        if (TryGetParameterById(id, out var parameter))
        {
            return parameter;
        }

        throw new ArgumentException($"Invalid parameter id {id}. No parameter found with this id", nameof(id));
    }

    /// <summary>
    /// Gets the normalized value of the parameter with the specified id.
    /// </summary>
    /// <param name="id">The id of the parameter.</param>
    /// <returns>The normalized value</returns>
    /// <exception cref="ArgumentException">If the parameter with the specified id was not found.</exception>
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

        return ref _allParameters[parameterIndex].NormalizedValueInternal;
    }

    /// <summary>
    /// Gets the unit with the specified id.
    /// </summary>
    /// <param name="id">The id of the unit.</param>
    /// <param name="unit">The unit if it was found; null otherwise.</param>
    /// <returns><c>true</c> if the unit was found.</returns>
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

    /// <summary>
    /// Gets the unit with the specified id.
    /// </summary>
    /// <param name="id">The id of the unit.</param>
    /// <returns>The unity with the specified id.</returns>
    /// <exception cref="ArgumentException">If the unit was not found.</exception>
    public AudioUnit GetUnitById(AudioUnitId id)
    {
        if (!TryGetUnitById(id, out var unit))
        {
            throw new ArgumentException($"Invalid unit id {id}. No unit found with this id.", nameof(id));
        }

        return unit;
    }

    /// <summary>
    /// Loads the model/parameter values from the specified reader.
    /// </summary>
    public override unsafe void Load(PortableBinaryReader reader, AudioProcessorModelStorageMode mode)
    {
        // Don't try to read anything if the stream is empty.
        if (reader.Stream.Length == 0) return;

        // If the mode to load is not the default one, then we cannot use the optimize one below
        // and we need to use the lower mode from the base class
        if (mode != AudioProcessorModelStorageMode.Default)
        {
            base.Load(reader, mode);
            return;
        }

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
                parameter.NormalizedValueInternal = reader.ReadFloat64();
            }
        }
    }

    /// <summary>
    /// Saves the model/parameter values to the specified writer.
    /// </summary>
    public override unsafe void Save(PortableBinaryWriter writer, AudioProcessorModelStorageMode mode)
    {
        // If the mode to load is not the default one, then we cannot use the optimize one below
        // and we need to use the lower mode from the base class
        if (mode != AudioProcessorModelStorageMode.Default)
        {
            base.Save(writer, mode);
            return;
        }

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
                writer.WriteFloat64(parameter.NormalizedValueInternal);
            }
        }
    }

    internal override void OnParameterValueChangedInternal(AudioParameter parameter)
    {
        // If a program is changed, we load its content into the current unit
        if ((parameter.Flags & AudioParameterFlags.IsProgramChange) != 0 && parameter is AudioStringListParameter listParameter)
        {
            var unit = parameter.Unit!;
            unit.LoadProgram(listParameter.SelectedItem);
        }
        
        ParameterValueChanged?.Invoke(parameter);
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
   
    private unsafe void InitializeParameters()
    {
        // Register all parameters from all units
        foreach (var unit in _allUnits)
        {
            var parameterCount = unit.LocalParameterCount;
            for (int i = 0; i < parameterCount; i++)
            {
                var parameter = unit.GetLocalParameter(i);
                RegisterParameter(parameter);
            }
        }

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
    }

    private void InitializeProgramLists()
    {
        // Mark all unit initialized
        foreach (var unit in _allUnits)
        {
            InitializeProgramList(unit);
        }
    }

    private void InitializeProgramListParameters()
    {
        // Mark all unit initialized
        foreach (var unit in _allUnits)
        {
            InitializeProgramListParameters(unit);
        }
    }

    private void InitializeProgramListParameters(AudioUnit unit)
    {
        // Initialize the program list
        if (unit.ProgramListBuilder is { } builder)
        {
            var presetParameter = builder.CreateProgramChangeParameter();
            unit.ProgramChangeParameter = presetParameter;
            unit.InsertParameter(0, presetParameter);
        }
    }

    private void InitializeProgramList(AudioUnit unit)
    {
        // Initialize the program list
        if (unit.ProgramListBuilder is { } builder)
        {
            var programList = builder.Build(unit);
            unit.ProgramList = programList;
            RegisterProgramList(programList);
        }
    }

    private static readonly uint AlignedSize = (uint)(Vector256.IsHardwareAccelerated ? Vector256<byte>.Count : Vector128.IsHardwareAccelerated ? Vector128<byte>.Count : sizeof(double));

    /// <inheritdoc />
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