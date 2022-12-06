// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace NPlug;

public sealed class AudioPluginFactory : IAudioPluginFactory
{
    private readonly List<AudioPluginClassInfo> _plugins;
    private readonly Dictionary<Guid, Func<IAudioPluginComponent>> _mapGuidToComponentFactory;

    public AudioPluginFactory(AudioPluginFactoryInfo factoryInfo)
    {
        FactoryInfo = factoryInfo;
        _plugins = new List<AudioPluginClassInfo>();
        _mapGuidToComponentFactory = new Dictionary<Guid, Func<IAudioPluginComponent>>();
    }

    public AudioPluginFactoryInfo FactoryInfo { get; }
    
    public void RegisterPlugin<TPlugin>(AudioPluginClassInfo pluginClassInfo) where TPlugin: class, IAudioPluginComponent, new()
    {
        RegisterPlugin(pluginClassInfo, static () => new TPlugin());
    }

    public void RegisterPlugin(AudioPluginClassInfo pluginClassInfo, Func<IAudioPluginComponent> factory)
    {
        if (_mapGuidToComponentFactory.ContainsKey(pluginClassInfo.Id))
        {
            throw new InvalidOperationException($"The plugin {pluginClassInfo.Name} with id: {pluginClassInfo.Id} has been already registered");
        }
        _mapGuidToComponentFactory.Add(pluginClassInfo.Id, factory);
        _plugins.Add(pluginClassInfo);
    }

    /// <summary>
    /// Copy all registered plugins from an existing factory to this instance.
    /// </summary>
    /// <param name="factory">The factory to copy registered plugins.</param>
    public void CopyFrom(AudioPluginFactory factory)
    {
        foreach (var pluginInfo in factory._plugins)
        {
            var componentFactory = _mapGuidToComponentFactory[pluginInfo.Id];
            RegisterPlugin(pluginInfo, componentFactory);
        }
    }

    int IAudioPluginFactory.PluginClassInfoCount => _plugins.Count;

    AudioPluginClassInfo IAudioPluginFactory.GetPluginClassInfo(int index)
    {
        if ((uint)index >= (uint)_plugins.Count) throw new ArgumentOutOfRangeException(nameof(index));
        return _plugins[index];
    }

    IAudioPluginComponent? IAudioPluginFactory.CreateInstance(Guid pluginId)
    {
        return _mapGuidToComponentFactory.TryGetValue(pluginId, out var factory) ? factory() : null;
    }
}