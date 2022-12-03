// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;

namespace NPlug.Vst3;

internal static unsafe partial class LibVst
{
    public sealed class AudioParameterChangesVst : IAudioParameterChangesBackend
    {
        public static readonly AudioParameterChangesVst Instance = new AudioParameterChangesVst();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IParameterChanges* Get(IntPtr context) => (IParameterChanges*)context;

        public int GetParameterCount(IntPtr context)
        {
            return Get(context)->getParameterCount();
        }

        public AudioParameterValueQueue GetParameterData(IntPtr context, int index)
        {
            var queue = Get(context)->getParameterData(index);
            return new AudioParameterValueQueue((IntPtr)queue, AudioParameterValueQueueVst.Instance);
        }

        public AudioParameterValueQueue AddParameterData(IntPtr context, AudioParameterId parameterId, out int index)
        {
            var localIndex = 0;
            var queue = Get(context)->addParameterData((ParamID*)&parameterId, &localIndex);
            index = localIndex;
            return new AudioParameterValueQueue((IntPtr)queue, AudioParameterValueQueueVst.Instance);
        }
    }

    public sealed class AudioParameterValueQueueVst : IAudioParameterValueQueueBackend
    {
        public static readonly AudioParameterValueQueueVst Instance = new AudioParameterValueQueueVst();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IParamValueQueue* Get(IntPtr context) => (IParamValueQueue*)context;


        public AudioParameterId GetParameterId(IntPtr context)
        {
            return new AudioParameterId(Get(context)->getParameterId().Value);
        }

        public int GetPointCount(IntPtr context)
        {
            return Get(context)->getPointCount();
        }

        public AudioParameterValue GetPoint(IntPtr context, int index, out int sampleOffset)
        {
            var localSampleOffset = 0;
            ParamValue localValue = default;
            Get(context)->getPoint(index, &localSampleOffset, &localValue);
            sampleOffset = localSampleOffset;
            return new AudioParameterValue(localValue.Value);
        }

        public int AddPoint(IntPtr context, int sampleOffset, AudioParameterValue parameterValue)
        {
            var localIndex = 0;
            if (Get(context)->addPoint(sampleOffset, new ParamValue(parameterValue.Value), &localIndex))
            {
                return localIndex;
            }
            return -1;
        }
    }

    public sealed class AudioEventListVst : IAudioEventListBackend
    {
        public static readonly AudioEventListVst Instance = new AudioEventListVst();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IEventList* Get(IntPtr context) => (IEventList*)context;

        public int GetEventCount(IntPtr context)
        {
            return Get(context)->getEventCount();
        }

        public bool TryGetEvent(IntPtr context, int index, out AudioEvent evt)
        {
            fixed (void* pEvent = &evt)
            {
                return Get(context)->getEvent(index, (Event*)pEvent);
            }
        }

        public bool TryAddEvent(IntPtr context, in AudioEvent evt)
        {
            fixed (void* pEvent = &evt)
            {
                return Get(context)->addEvent((Event*)pEvent);
            }
        }
    }

    public partial struct IAudioProcessor
    {

#if DEBUG
        static IAudioProcessor()
        {
            Debug.Assert(AudioSampleSize.Float32 == (AudioSampleSize)SymbolicSampleSizes.kSample32);
            Debug.Assert(AudioSampleSize.Float64 == (AudioSampleSize)SymbolicSampleSizes.kSample64);
            Debug.Assert(NPlug.AudioBusDirection.Input == (NPlug.AudioBusDirection)BusDirections.kInput);
            Debug.Assert(NPlug.AudioBusDirection.Output == (NPlug.AudioBusDirection)BusDirections.kOutput);
            // TODO: Check SpeakerArrangement
        }
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static NPlug.IAudioProcessor Get(IAudioProcessor* self) => (NPlug.IAudioProcessor)((ComObjectHandle*)self)->Handle.Target!;

        private static partial ComResult setBusArrangements_ccw(IAudioProcessor* self, SpeakerArrangement* inputs, int numIns, SpeakerArrangement* outputs, int numOuts)
        {
            if (numIns < 0 || numOuts < 0) return ComResult.InvalidArg;
            try
            {
                return Get(self).SetBusArrangements(new Span<NPlug.SpeakerArrangement>((NPlug.SpeakerArrangement*)inputs, numIns), new Span<NPlug.SpeakerArrangement>((NPlug.SpeakerArrangement*)outputs, numOuts));
            }
            catch (Exception)
            {
                return ComResult.InternalError;
            }
        }

        private static partial ComResult getBusArrangement_ccw(IAudioProcessor* self, BusDirection dir, int index, SpeakerArrangement* arr)
        {
            if (index < 0) return ComResult.InvalidArg;
            try
            {
                *arr = (SpeakerArrangement)((ulong)Get(self).GetBusArrangement((NPlug.AudioBusDirection)dir.Value, index));
                return ComResult.Ok;
            }
            catch (ArgumentException)
            {
                return ComResult.InvalidArg;
            }
            catch (Exception)
            {
                return ComResult.InternalError;
            }
        }

        private static partial ComResult canProcessSampleSize_ccw(IAudioProcessor* self, int symbolicSampleSize)
        {
            try
            {
                return Get(self).CanProcessSampleSize((AudioSampleSize)symbolicSampleSize);
            }
            catch (Exception)
            {
                return ComResult.InternalError;
            }
        }

        private static partial uint getLatencySamples_ccw(IAudioProcessor* self)
        {
            try
            {
                return Get(self).LatencySamples;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private static partial ComResult setupProcessing_ccw(IAudioProcessor* self, ProcessSetup* setup)
        {
            try
            {
                return Get(self).SetupProcessing(*(AudioProcessSetupData*)setup);
            }
            catch (Exception)
            {
                return ComResult.InternalError;
            }
        }

        private static partial ComResult setProcessing_ccw(IAudioProcessor* self, byte state)
        {
            try
            {
                Get(self).SetProcessing(state != 0);
                return ComResult.Ok;
            }
            catch (Exception)
            {
                return ComResult.InternalError;
            }
        }

        private static partial ComResult process_ccw(IAudioProcessor* self, ProcessData* data)
        {
            try
            {
                var audioProcessor = Get(self);
                var processData = new AudioProcessData((IntPtr)data->processContext)
                {
                    SampleCount = data->numSamples,
                    SampleSize = (AudioSampleSize)data->symbolicSampleSize,
                    ProcessMode = (AudioProcessMode)data->processMode
                };
                processData.Input.Buffers = new Span<NPlug.AudioBusBuffers>(data->inputs, data->numInputs);
                processData.Input.ParameterChanges = new AudioParameterChanges((IntPtr)data->inputParameterChanges, AudioParameterChangesVst.Instance);
                processData.Input.Events = new AudioEventList((IntPtr)data->inputEvents, AudioEventListVst.Instance);

                processData.Output.Buffers = new Span<NPlug.AudioBusBuffers>(data->outputs, data->numOutputs);
                processData.Output.ParameterChanges = new AudioParameterChanges((IntPtr)data->outputParameterChanges, AudioParameterChangesVst.Instance);
                processData.Output.Events = new AudioEventList((IntPtr)data->outputEvents, AudioEventListVst.Instance);

                audioProcessor.Process(in processData);
                return ComResult.Ok;
            }
            catch (Exception)
            {
                return ComResult.InternalError;
            }
        }

        private static partial uint getTailSamples_ccw(IAudioProcessor* self)
        {
            try
            {
                return Get(self).TailSamples;
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}