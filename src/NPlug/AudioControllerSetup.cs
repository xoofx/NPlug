// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug;

public readonly ref struct AudioControllerSetup
{
    private readonly AudioController _controller;

    internal AudioControllerSetup(AudioController controller, AudioHostApplication host)
    {
        _controller = controller;
        Host = host;
    }

    public AudioHostApplication Host { get; }

    public void AddParameter(AudioParameterInfo parameterInfo)
    {
        AssertInitialize();
        _controller.ParameterInfos.Add(parameterInfo);
    }

    private void AssertInitialize()
    {
        if (_controller is null) throw new InvalidOperationException($"Invalid {nameof(AudioControllerSetup)}. Must be used only from {nameof(AudioController)}.Initialize");
    }
}