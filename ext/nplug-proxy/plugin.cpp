// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

// Standard headers
#include <stdio.h>
#include <stdint.h>
#include <stdlib.h>
#include <string.h>
#include <assert.h>
#include <iostream>

// Provided by the AppHost NuGet package and installed as an SDK pack
#include <nethost.h>

// Header files copied from https://github.com/dotnet/core-setup
#include <coreclr_delegates.h>
#include <hostfxr.h>

#ifdef WIN32
#define WINDOWS 1
#endif

#ifdef WINDOWS
#include <Windows.h>

#define STR(s) L ## s
#define CH(c) L ## c
#define DIR_SEPARATOR L'\\'
#else
#include <dlfcn.h>
#include <limits.h>
#define STR(s) s
#define CH(c) c
#define DIR_SEPARATOR '/'
#define MAX_PATH PATH_MAX

#endif

#if defined(__GNUC__) || defined(__clang__)
    #define NPLUG_NATIVE_DLL_EXPORT __attribute__((__visibility__("default")))
    #define NPLUG_CDECL __attribute__((cdecl))
#elif defined(_MSC_VER) || defined(__INTEL_COMPILER)
    #define NPLUG_NATIVE_DLL_EXPORT __declspec(dllexport)
    #define NPLUG_CDECL __cdecl
#else
    #define NPLUG_NATIVE_DLL_EXPORT
    #define NPLUG_CDECL
#endif

using string_t = std::basic_string<char_t>;

namespace
{
    // Globals to hold hostfxr exports
    hostfxr_initialize_for_runtime_config_fn init_fptr;
    hostfxr_get_runtime_delegate_fn get_delegate_fptr;
    hostfxr_close_fn close_fptr;

    // Forward declarations
    bool load_hostfxr();
    load_assembly_and_get_function_pointer_fn get_dotnet_load_assembly(const char_t *assembly);
}

namespace
{
    // Forward declarations
    void *load_library(const char_t *);
    void *get_export(void *, const char *);

#ifdef WINDOWS
    void *load_library(const char_t *path)
    {
        HMODULE h = ::LoadLibraryW(path);
        assert(h != nullptr);
        return (void*)h;
    }
    void *get_export(void *h, const char *name)
    {
        void *f = ::GetProcAddress((HMODULE)h, name);
        assert(f != nullptr);
        return f;
    }
#else
    void *load_library(const char_t *path)
    {
        void *h = dlopen(path, RTLD_LAZY | RTLD_LOCAL);
        assert(h != nullptr);
        return h;
    }
    void *get_export(void *h, const char *name)
    {
        void *f = dlsym(h, name);
        assert(f != nullptr);
        return f;
    }
#endif

    // <SnippetLoadHostFxr>
    // Using the nethost library, discover the location of hostfxr and get exports
    bool load_hostfxr()
    {
        // Pre-allocate a large buffer for the path to hostfxr
        char_t buffer[MAX_PATH];
        size_t buffer_size = sizeof(buffer) / sizeof(char_t);
        int rc = get_hostfxr_path(buffer, &buffer_size, nullptr);
        if (rc != 0)
            return false;

        // Load hostfxr and get desired exports
        void *lib = load_library(buffer);
        init_fptr = (hostfxr_initialize_for_runtime_config_fn)get_export(lib, "hostfxr_initialize_for_runtime_config");
        get_delegate_fptr = (hostfxr_get_runtime_delegate_fn)get_export(lib, "hostfxr_get_runtime_delegate");
        close_fptr = (hostfxr_close_fn)get_export(lib, "hostfxr_close");

        return (init_fptr && get_delegate_fptr && close_fptr);
    }
    // </SnippetLoadHostFxr>

    // <SnippetInitialize>
    // Load and initialize .NET Core and get desired function pointer for scenario
    load_assembly_and_get_function_pointer_fn get_dotnet_load_assembly(const char_t *config_path)
    {
        // Load .NET Core
        void *load_assembly_and_get_function_pointer = nullptr;
        hostfxr_handle cxt = nullptr;
        int rc = init_fptr(config_path, nullptr, &cxt);
        //    rc == 0, Success                            - Hosting components were successfully initialized
        //    rc == 1, Success_HostAlreadyInitialized     - Config is compatible with already initialized hosting components
        //    rc == 2, Success_DifferentRuntimeProperties - Config has runtime properties that differ from already initialized hosting components
        //    rc == 3, CoreHostIncompatibleConfig         - Config is incompatible with already initialized hosting components
        if ((rc != 0 && rc != 1 && rc != 2) || cxt == nullptr)
        {
            close_fptr(cxt);
            return nullptr;
        }

        // Get the load assembly function pointer
        rc = get_delegate_fptr(
            cxt,
            hdt_load_assembly_and_get_function_pointer,
            &load_assembly_and_get_function_pointer);

        close_fptr(cxt);
        return (load_assembly_and_get_function_pointer_fn)load_assembly_and_get_function_pointer;
    }
    // </SnippetInitialize>
}

void* execute()
{
    // Get the current executable's directory
    // This sample assumes the managed assembly to load and its runtime configuration file are next to the host
    char_t host_path[MAX_PATH];
#if WINDOWS
    HMODULE hm = NULL;
    GetModuleHandleEx(GET_MODULE_HANDLE_EX_FLAG_FROM_ADDRESS | GET_MODULE_HANDLE_EX_FLAG_UNCHANGED_REFCOUNT, (LPCSTR)&execute, &hm);
    auto size = GetModuleFileNameW(hm, host_path, MAX_PATH);
    //auto size = ::GetFullPathNameW(argv[0], sizeof(host_path) / sizeof(char_t), host_path, nullptr);
    assert(size != 0);
#else
    Dl_info dl_info;
    dladdr((void *)execute, &dl_info);
    auto resolved = realpath(dl_info.dli_fname, host_path);
    assert(resolved != nullptr);
#endif

    //
    // STEP 1: Load HostFxr and get exported hosting functions
    //
    if (!load_hostfxr())
    {
        assert(false && "Failure: load_hostfxr()");
        return nullptr;
    }

    string_t root_path = host_path;
    auto pos = root_path.find_last_of(DIR_SEPARATOR);
    assert(pos != string_t::npos);
    auto posOfExtension = root_path.find_last_of('.');
    assert(posOfExtension != string_t::npos);
    auto fileName = root_path.substr(pos + 1, posOfExtension - pos - 1);
    //posOfExtension = fileName.find_last_of('.');
    //fileName = fileName.substr(0, posOfExtension);
    root_path = root_path.substr(0, pos + 1);

    //
    // STEP 2: Initialize and start the .NET Core runtime
    //
    const string_t config_path = root_path + fileName + STR(".runtimeconfig.json");
    load_assembly_and_get_function_pointer_fn load_assembly_and_get_function_pointer = nullptr;
    load_assembly_and_get_function_pointer = get_dotnet_load_assembly(config_path.c_str());
    // If we were not able to initialize, don't try to locate the plugin
    if (load_assembly_and_get_function_pointer == nullptr)
    {
        return nullptr;
    }

    //
    // STEP 3: Load managed assembly and get function pointer to a managed method
    //
    const string_t dotnetlib_path = root_path + fileName + STR(".dll");
    string_t dotnet_type = STR("NPlug.Interop.NPlugFactoryExport, ");
    dotnet_type += fileName;
    const char_t *dotnet_type_method = STR("GetPluginFactory");
    // Function pointer to managed delegate
    typedef void* (CORECLR_DELEGATE_CALLTYPE *get_plugin_factory_entry_point_fn)();
    get_plugin_factory_entry_point_fn get_plugin_factory = nullptr;
    int rc = load_assembly_and_get_function_pointer(
        dotnetlib_path.c_str(),
        dotnet_type.c_str(),
        dotnet_type_method,
        UNMANAGEDCALLERSONLY_METHOD  /*delegate_type_name*/,
        nullptr,
        (void**)&get_plugin_factory);
    return rc == 0 && get_plugin_factory != nullptr ? get_plugin_factory() : nullptr;
}

extern "C" {

typedef void* (NPLUG_CDECL *GetPluginFactoryFunction)();
static GetPluginFactoryFunction _factory;

NPLUG_NATIVE_DLL_EXPORT void nplug_set_plugin_factory(void* factory) {
    _factory = (GetPluginFactoryFunction)factory;
}

NPLUG_NATIVE_DLL_EXPORT void* GetPluginFactory() {
    if (_factory != nullptr) {
        return _factory();
    }
    return execute();
}

#if defined _WIN32
NPLUG_NATIVE_DLL_EXPORT bool InitDll() {
    return true;
}

NPLUG_NATIVE_DLL_EXPORT bool ExitDll() {
    return true;
}
#elif __APPLE__
NPLUG_NATIVE_DLL_EXPORT bool BundleEntry() {
    return true;
}

NPLUG_NATIVE_DLL_EXPORT bool BundleExit() {
    return true;
}
#elif __linux__

NPLUG_NATIVE_DLL_EXPORT bool ModuleEntry(void* sharedLibraryHandle) {
    return true;
}

NPLUG_NATIVE_DLL_EXPORT bool ModuleExit(void* sharedLibraryHandle) {
    return true;
}
#endif
}
