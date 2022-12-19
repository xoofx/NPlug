// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Runtime.CompilerServices;

namespace NPlug.SimpleProgramChange;

public static class SimpleProgramChangePlugin
{
    public static AudioPluginFactory GetFactory()
    {
        var factory = new AudioPluginFactory(new("NPlug", "https://github.com/xoofx/NPlug", "no_reply@nplug.org"));
        factory.RegisterPlugin<SimpleProgramChangeProcessor>(new(SimpleProgramChangeProcessor.ClassId, "SimpleProgramChange", AudioProcessorCategory.Effect));
        factory.RegisterPlugin<SimpleProgramChangeController>(new(SimpleProgramChangeController.ClassId, "SimpleProgramChange Controller"));
        return factory;
    }

    [ModuleInitializer]
    internal static void ExportThisPlugin()
    {
        AudioPluginFactoryExporter.Instance = GetFactory();
    }
}