// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using NPlug.Backend;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct IAudioProcessor
    {

#if DEBUG
        static IAudioProcessor()
        {
            Debug.Assert(AudioSampleSize.Float32 == (AudioSampleSize)SymbolicSampleSizes.kSample32);
            Debug.Assert(AudioSampleSize.Float64 == (AudioSampleSize)SymbolicSampleSizes.kSample64);
            Debug.Assert(NPlug.BusDirection.Input == (NPlug.BusDirection)BusDirections.kInput);
            Debug.Assert(NPlug.BusDirection.Output == (NPlug.BusDirection)BusDirections.kOutput);
            // TODO: Check SpeakerArrangement
        }
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static NPlug.IAudioProcessor Get(IAudioProcessor* self) => (NPlug.IAudioProcessor)((ComObjectHandle*)self)->Handle.Target!;

        private static partial ComResult setBusArrangements_ToManaged(IAudioProcessor* self, SpeakerArrangement* inputs, int numIns, SpeakerArrangement* outputs, int numOuts)
        {
            if (numIns < 0) throw new ArgumentOutOfRangeException(nameof(numIns));
            if (numOuts < 0) throw new ArgumentOutOfRangeException(nameof(numOuts));
            return Get(self).SetBusArrangements(new Span<NPlug.SpeakerArrangement>((NPlug.SpeakerArrangement*)inputs, numIns), new Span<NPlug.SpeakerArrangement>((NPlug.SpeakerArrangement*)outputs, numOuts));
        }

        private static partial ComResult getBusArrangement_ToManaged(IAudioProcessor* self, BusDirection dir, int index, SpeakerArrangement* arr)
        {
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
            *arr = (SpeakerArrangement)((ulong)Get(self).GetBusArrangement((NPlug.BusDirection)dir.Value, index));
            return true;
        }

        private static partial ComResult canProcessSampleSize_ToManaged(IAudioProcessor* self, int symbolicSampleSize)
        {
            return Get(self).CanProcessSampleSize((AudioSampleSize)symbolicSampleSize);
        }

        private static partial uint getLatencySamples_ToManaged(IAudioProcessor* self)
        {
            return Get(self).LatencySamples;
        }

        private static partial ComResult setupProcessing_ToManaged(IAudioProcessor* self, ProcessSetup* setup)
        {
            return Get(self).SetupProcessing(*(AudioProcessSetupData*)setup);
        }

        private static partial ComResult setProcessing_ToManaged(IAudioProcessor* self, byte state)
        {
            Get(self).SetProcessing(state != 0);
            return true;
        }

        [SkipLocalsInit]
        private static partial ComResult process_ToManaged(IAudioProcessor* self, ProcessData* data)
        {
            var audioProcessor = Get(self);
            var processData = new AudioProcessData((IntPtr)data->processContext,
                (AudioProcessMode)data->processMode,
                (AudioSampleSize)data->symbolicSampleSize,
                data->numSamples,
                new AudioBusData(data->numInputs, (NPlug.AudioBusBuffers*)data->inputs, new AudioParameterChanges(AudioParameterChangesVst.Instance, (IntPtr)data->inputParameterChanges),
                    new AudioEventList(AudioEventListVst.Instance, (IntPtr)data->inputEvents)),
                new AudioBusData(data->numOutputs, (NPlug.AudioBusBuffers*)data->outputs, new AudioParameterChanges(AudioParameterChangesVst.Instance, (IntPtr)data->outputParameterChanges),
                    new AudioEventList(AudioEventListVst.Instance, (IntPtr)data->outputEvents))
            );
            audioProcessor.Process(in processData);
            return true;
        }

        private static partial uint getTailSamples_ToManaged(IAudioProcessor* self)
        {
            return Get(self).TailSamples;
        }
    }

    public sealed class AudioParameterChangesVst : IAudioParameterChangesBackend
    {
        public static readonly AudioParameterChangesVst Instance = new AudioParameterChangesVst();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IParameterChanges* Get(in AudioParameterChanges parameterChanges) => (IParameterChanges*)parameterChanges.NativeContext;

        public int GetParameterCount(in AudioParameterChanges parameterChanges)
        {
            return Get(parameterChanges)->getParameterCount();
        }

        public AudioParameterValueQueue GetParameterData(in AudioParameterChanges parameterChanges, int index)
        {
            var queue = Get(parameterChanges)->getParameterData(index);
            return new AudioParameterValueQueue(AudioParameterValueQueueVst.Instance, (IntPtr)queue);
        }

        public AudioParameterValueQueue AddParameterData(in AudioParameterChanges parameterChanges, AudioParameterId parameterId, out int index)
        {
            var localIndex = 0;
            var queue = Get(parameterChanges)->addParameterData((ParamID*)&parameterId, &localIndex);
            index = localIndex;
            return new AudioParameterValueQueue(AudioParameterValueQueueVst.Instance, (IntPtr)queue);
        }
    }

    public sealed class AudioParameterValueQueueVst : IAudioParameterValueQueueBackend
    {
        public static readonly AudioParameterValueQueueVst Instance = new AudioParameterValueQueueVst();

        public AudioParameterId GetParameterId(in AudioParameterValueQueue parameterValueQueue)
        {
            return new AudioParameterId(unchecked((int)Get(parameterValueQueue)->getParameterId().Value));
        }

        public int GetPointCount(in AudioParameterValueQueue parameterValueQueue)
        {
            return Get(parameterValueQueue)->getPointCount();
        }

        public double GetPoint(in AudioParameterValueQueue parameterValueQueue, int index, out int sampleOffset)
        {
            var localSampleOffset = 0;
            ParamValue localValue = default;
            Get(parameterValueQueue)->getPoint(index, &localSampleOffset, &localValue);
            sampleOffset = localSampleOffset;
            return localValue.Value;
        }

        public int AddPoint(in AudioParameterValueQueue parameterValueQueue, int sampleOffset, double parameterValue)
        {
            var localIndex = 0;
            if (Get(parameterValueQueue)->addPoint(sampleOffset, new ParamValue(parameterValue), &localIndex))
            {
                return localIndex;
            }
            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IParamValueQueue* Get(in AudioParameterValueQueue parameterValueQueue) => (IParamValueQueue*)parameterValueQueue.NativeContext;
    }

    public sealed class AudioEventListVst : IAudioEventListBackend
    {
        public static readonly AudioEventListVst Instance = new AudioEventListVst();

        public int GetEventCount(in AudioEventList eventList)
        {
            return Get(eventList)->getEventCount();
        }

        public bool TryGetEvent(in AudioEventList eventList, int index, out AudioEvent evt)
        {
            fixed (void* pEvent = &evt)
            {
                return Get(eventList)->getEvent(index, (Event*)pEvent);
            }
        }

        public bool TryAddEvent(in AudioEventList eventList, in AudioEvent evt)
        {
            fixed (void* pEvent = &evt)
            {
                return Get(eventList)->addEvent((Event*)pEvent);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IEventList* Get(in AudioEventList eventList) => (IEventList*)eventList.NativeContext;
    }
}