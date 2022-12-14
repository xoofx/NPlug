// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace NPlug;

public sealed class AudioPluginFactory : IAudioPluginFactory
{
    private readonly List<AudioPluginClassInfo> _pluginClassInfos;
    private readonly Dictionary<Guid, Func<IAudioPluginObject>> _mapGuidToComponentFactory;

    public AudioPluginFactory(AudioPluginFactoryInfo factoryInfo)
    {
        FactoryInfo = factoryInfo;
        _pluginClassInfos = new List<AudioPluginClassInfo>();
        _mapGuidToComponentFactory = new Dictionary<Guid, Func<IAudioPluginObject>>();
    }

    public AudioPluginFactoryInfo FactoryInfo { get; }

    public IReadOnlyList<AudioPluginClassInfo> PluginClassInfos => _pluginClassInfos;

    public IAudioPluginObject? CreateInstance(Guid pluginId)
    {
        return _mapGuidToComponentFactory.TryGetValue(pluginId, out var factory) ? factory() : null;
    }

    public void RegisterPlugin<TPlugin>(AudioProcessorClassInfo pluginClassInfo, bool registerTests = false) where TPlugin: class, IAudioProcessor, new()
    {
        RegisterPlugin(pluginClassInfo, static () => new TPlugin());

        // Register test plugin on the fly
        if (registerTests)
        {
            var testPluginClassInfo = new AudioTestProviderClassInfo(Guid.NewGuid(), $"{pluginClassInfo.Name} Test Provider");
            int indexOfAudioProcessor = _pluginClassInfos.Count - 1;
            RegisterPlugin(testPluginClassInfo, () => new AudioTestProvider(this, indexOfAudioProcessor));
        }
    }

    public void RegisterPlugin<TPlugin>(AudioControllerClassInfo pluginClassInfo) where TPlugin : class, IAudioController, new()
    {
        RegisterPlugin(pluginClassInfo, static () => new TPlugin());
    }

    public void RegisterPlugin(AudioPluginClassInfo pluginClassInfo, Func<IAudioPluginObject> factory)
    {
        if (_mapGuidToComponentFactory.ContainsKey(pluginClassInfo.ClassId))
        {
            throw new InvalidOperationException($"The plugin {pluginClassInfo.Name} with id: {pluginClassInfo.ClassId} has been already registered");
        }
        _mapGuidToComponentFactory.Add(pluginClassInfo.ClassId, factory);
        _pluginClassInfos.Add(pluginClassInfo);
    }

    /// <summary>
    /// Copy all registered plugins from an existing factory to this instance.
    /// </summary>
    /// <param name="factory">The factory to copy registered plugins.</param>
    public void CopyFrom(AudioPluginFactory factory)
    {
        foreach (var pluginInfo in factory._pluginClassInfos)
        {
            var componentFactory = _mapGuidToComponentFactory[pluginInfo.ClassId];
            RegisterPlugin(pluginInfo, componentFactory);
        }
    }

    int IAudioPluginFactory.PluginClassInfoCount => _pluginClassInfos.Count;

    AudioPluginClassInfo IAudioPluginFactory.GetPluginClassInfo(int index)
    {
        if ((uint)index >= (uint)_pluginClassInfos.Count) throw new ArgumentOutOfRangeException(nameof(index));
        return _pluginClassInfos[index];
    }
    
    /// <summary>
    /// Base class for <see cref="IAudioTestProvider"/>.
    /// </summary>
    private class AudioTestProvider : IAudioTestProvider
    {
        private readonly AudioPluginFactory _factory;
        private readonly AudioProcessorClassInfo _audioProcessorClassInfo;
        private IAudioProcessor? _audioProcessor;
        private IAudioController? _audioController;

        public AudioTestProvider(AudioPluginFactory factory, int indexToAudioProcessor)
        {
            _factory = factory;
            _audioProcessorClassInfo = (AudioProcessorClassInfo)_factory.PluginClassInfos[indexToAudioProcessor];
        }
        
        public IAudioProcessor GetAudioProcessor()
        {
            if (_audioProcessor != null)
            {
                return _audioProcessor;
            }
            _audioProcessor = (IAudioProcessor)_factory.CreateInstance(_audioProcessorClassInfo.ClassId)!;
            return _audioProcessor;
        }

        public IAudioController GetAudioController()
        {
            if (_audioController != null)
            {
                return _audioController;
            }
            _audioController = (IAudioController)_factory.CreateInstance(GetAudioProcessor().ControllerId)!;
            return _audioController;
        }

        public AudioProcessorCategory GetAudioProcessorCategory() => _audioProcessorClassInfo.Category;

        public Guid GetAudioProcessorClassId() => _audioProcessorClassInfo.ClassId;

        public IAudioPluginFactory GetPluginFactory() => _factory;
    }
}