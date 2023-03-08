// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using NPlug.Helpers;

namespace NPlug;

public abstract partial class AudioProcessor<TAudioProcessorModel>
{
    /// <summary>
    /// Gets a boolean indicating if this processor is currently processing. This is set by <see cref="IAudioProcessor.SetProcessing"/>.
    /// </summary>
    public bool IsProcessing { get; private set; }

    /// <summary>
    /// Gets a boolean indicating if the model specifies that this processor should be bypassed.
    /// </summary>
    public bool ShouldByPass => Model.ByPassParameter?.Value ?? false;

    /// <summary>
    /// This method is called before processing the data as part of the <see cref="IAudioProcessor.Process"/>.
    /// </summary>
    /// <param name="data">The process data.</param>
    protected virtual void PreProcess(in AudioProcessData data)
    {
    }

    /// <summary>
    /// This method is called by <see cref="IAudioProcessor.Process"/>.
    /// </summary>
    /// <param name="data">The process data</param>
    /// <remarks>
    /// The default implementation is calling:
    /// - <see cref="PreProcess"/>
    /// - <see cref="ProcessParameterChanges"/>
    /// - If the parameter changes have been processed, it calls <see cref="ProcessRecalculate"/>.
    /// - Then it calls <see cref="ProcessEvents"/>
    /// - If the sample count is greater than 0 and the processor is not bypassed, it calls <see cref="ProcessMain"/> and <see cref="PostProcessCheckSilence"/>
    /// </remarks>
    protected virtual void Process(in AudioProcessData data)
    {
        PreProcess(data);

        var needRecalculate = ProcessParameterChanges(data);
        if (needRecalculate)
        {
            ProcessRecalculate(data);
        }

        ProcessEvents(data);

        if (data.SampleCount > 0 && !ProcessByPass(data))
        {
            ProcessMain(data);
            PostProcessCheckSilence(data);
        }
    }

    /// <summary>
    /// Implement this method to generate the main part of the audio processing.
    /// </summary>
    protected virtual void ProcessMain(in AudioProcessData data)
    {
    }

    /// <summary>
    /// This method is called before processing the data if the parameter have changed (after calling <see cref="ProcessParameterChanges"/>).
    /// </summary>
    protected virtual void ProcessRecalculate(in AudioProcessData data)
    {
    }

    /// <summary>
    /// This method is called before processing the data and process any parameter changes.
    /// </summary>
    /// <param name="data">The input data.</param>
    /// <returns><c>true</c> if some parameters have changed.</returns>
    /// <remarks>
    /// The default implementation is taking the last point value of a parameter change.
    /// </remarks>
    protected virtual bool ProcessParameterChanges(in AudioProcessData data)
    {
        var parameterChanges = data.Input.ParameterChanges;
        var count = parameterChanges.Count;
        if (count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                var parameterData = parameterChanges.GetParameterData(i);
                if (parameterData.PointCount > 0 && Model.TryGetParameterById(parameterData.ParameterId, out var parameter))
                {
                    // Update with latest parameter
                    var value = parameterData.GetPoint(parameterData.PointCount - 1, out _);
                    parameter.RawNormalizedValue = value;
                }
            }

            return true;
        }

        return false;
    }

    /// <summary>
    /// Process any input events
    /// </summary>
    protected virtual void ProcessEvents(in AudioProcessData data)
    {
        var events = data.Input.Events;
        var count = events.Count;
        for (int i = 0; i < count; i++)
        {
            if (events.TryGetEvent(i, out var evt))
            {
                ProcessEvent(evt);
            }
        }
    }

    /// <summary>
    /// This method is called by <see cref="ProcessEvents"/> to process a single event.
    /// </summary>
    protected virtual void ProcessEvent(in AudioEvent audioEvent)
    {
    }

    /// <summary>
    /// This method is called by <see cref="Process"/> if the processor is bypassed.
    /// </summary>
    /// <returns><c>true</c> if this method has generated a bypass audio.</returns>
    /// <remarks>
    /// The default implementation for the by-pass is to copy the input audio to the output audio.
    /// </remarks>
    protected virtual bool ProcessByPass(in AudioProcessData data)
    {
        if (!ShouldByPass || data.SampleCount == 0) return false;

        ref readonly var setupData = ref ProcessSetupData;
        var inputCount = data.Input.BusCount;
        var outputCount = data.Output.BusCount;
        var busOutputs = GetAudioOutputBuses();
        for (int bus = 0; bus < inputCount && bus < outputCount; bus++)
        {
            var busOutput = busOutputs[bus];
            if (!busOutput.IsActive) continue;

            var inputBuffer = data.Input[bus];
            var outputBuffer = data.Output[bus];
            int channelCount = outputBuffer.ChannelCount;
            for (int channel = 0; channel < channelCount; channel++)
            {
                if (channel >= inputBuffer.ChannelCount)
                {
                    // Clear the output buffer
                    outputBuffer.GetChannelSpanAsBytes(setupData, data, channel).Fill(0);
                    data.Output[bus].SetChannelSilence(channel, true);
                }
                else
                {
                    // Copy the input buffer to the output buffer
                    inputBuffer.GetChannelSpanAsBytes(setupData, data, channel).CopyTo(outputBuffer.GetChannelSpanAsBytes(setupData, data, channel));
                    // Copy the silence state
                    data.Output[bus].SetChannelSilence(channel, inputBuffer.IsChannelSilence(channel));
                }
            }
        }

        return true;
    }

    /// <summary>
    /// This method is called by <see cref="Process"/> after <see cref="ProcessMain"/> to check if the output is silent.
    /// </summary>
    protected virtual void PostProcessCheckSilence(in AudioProcessData data)
    {
        var inputCount = data.Input.BusCount;
        var outputCount = data.Output.BusCount;
        var busOutputs = GetBusInfoList(BusMediaType.Audio, BusDirection.Output);
        for (int bus = 0; bus < inputCount && bus < outputCount; bus++)
        {
            data.Output[bus].SilenceFlags = 0;

            var busOutput = busOutputs[bus];
            if (!busOutput.IsActive) continue;

            var outputBuffer = data.Output[bus];
            int channelCount = outputBuffer.ChannelCount;
            for (int channel = 0; channel < channelCount; channel++)
            {
                bool isChannelSilent;
                if (ProcessSetupData.SampleSize == AudioSampleSize.Float32)
                {
                    var buffer = outputBuffer.GetChannelSpanAsFloat32(ProcessSetupData, data, channel);
                    const float silenceThreshold = 0.000132184039f; // TODO this is coming from VST SDK, not sure about this particular value
                    isChannelSilent = AudioHelper.CheckIsSilent(buffer, silenceThreshold);
                }
                else
                {
                    const double silenceThreshold = 0.000132184039; // TODO this is coming from VST SDK, not sure about this particular value
                    var buffer = outputBuffer.GetChannelSpanAsFloat64(ProcessSetupData, data, channel);
                    isChannelSilent = AudioHelper.CheckIsSilent(buffer, silenceThreshold);
                }
                data.Output[bus].SetChannelSilence(channel, isChannelSilent);
            }
        }
    }

    bool IAudioProcessor.SetupProcessing(in AudioProcessSetupData processSetupData)
    {
        if (IsSampleSizeSupported(processSetupData.SampleSize))
        {
            _processSetupData = processSetupData;
            OnSetupProcessing(processSetupData);
            return true;
        }

        return false;
    }

    void IAudioProcessor.SetProcessing(bool state)
    {
        IsProcessing = state;
    }

    void IAudioProcessor.Process(in AudioProcessData processData)
    {
        Process(in processData);
    }
}