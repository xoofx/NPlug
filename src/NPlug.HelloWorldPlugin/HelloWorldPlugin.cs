// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Runtime.CompilerServices;

namespace NPlug;

public static class HelloWorldPlugin
{
    public static AudioPluginFactory GetFactory()
    {
        var factory = new AudioPluginFactory(new("My Company", "https://plugin_corp.com", "contact@plugin_corp.com"));
        factory.RegisterPlugin<HelloWorldProcessor>(new(HelloWorldProcessor.ClassId, "HelloWorld", AudioProcessorCategory.Effect));
        factory.RegisterPlugin<HelloWorldController>(new(HelloWorldController.ClassId, "HelloWorld Controller"));
        return factory;
    }

    [ModuleInitializer]
    internal static void ExportThisPlugin()
    {
        AudioPluginFactoryExporter.Instance = GetFactory();
    }
}