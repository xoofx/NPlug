// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

#if HAVE_VISIBILITY
#define NPLUG_NATIVE_DLL_EXPORT __attribute__((__visibility__("default")))
#elif (defined WIN32 && !defined __CYGWIN__)
#define NPLUG_NATIVE_DLL_EXPORT __declspec(dllexport)
#else
#define NPLUG_NATIVE_DLL_EXPORT
#endif

extern "C" {

typedef void* (*GetPluginFactoryFunction)();
static GetPluginFactoryFunction _factory;

NPLUG_NATIVE_DLL_EXPORT void nplug_set_plugin_factory(void* factory) {
    _factory = (GetPluginFactoryFunction)factory;
}

NPLUG_NATIVE_DLL_EXPORT void* GetPluginFactory() {
    if (_factory != nullptr) {
        return _factory();
    }
    return nullptr;
}

#if defined _WIN32
NPLUG_NATIVE_DLL_EXPORT void InitDll() {
}

NPLUG_NATIVE_DLL_EXPORT void ExitDll() {
}
#elif __APPLE__
NPLUG_NATIVE_DLL_EXPORT void BundleEntry() {
}

NPLUG_NATIVE_DLL_EXPORT void BundleExit() {
}
#elif __linux__
NPLUG_NATIVE_DLL_EXPORT void ModuleEntry() {
}

NPLUG_NATIVE_DLL_EXPORT void ModuleExit() {
}
#endif
}
