using NPlug.Proxy;
using System.Runtime.InteropServices;
using NPlug.Interop;

namespace NPlug.Validator;

public static class AudioPluginValidator
{
    private static readonly AudioPluginProxy _proxy;

    static AudioPluginValidator()
    {
        Initialize();
        _proxy = AudioPluginProxy.Load(AudioPluginProxy.GetDefaultPath());
    }

    public static bool Validate(AudioPluginFactory factory, TextWriter outputLog, TextWriter errorLog)
    {
        _proxy.SetNativeFactory(() =>
        {
            return InteropHelper.ExportToVst3(factory);
        });

        return Validate(2, new string[]
        {
            nameof(AudioPluginValidator),
            AudioPluginProxy.GetDefaultPath()
        }, outputLog, errorLog) == 0;
    }

    public static bool Validate(string pluginPath, TextWriter outputLog, TextWriter errorLog)
    {
        return Validate(2, new string[]
        {
            nameof(AudioPluginValidator),
            pluginPath
        }, outputLog, errorLog) == 0;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void FunctionOutputDelegate(int c);

    private static int Validate(int argc, string[] args, TextWriter outputLog, TextWriter errorLog)
    {
        FunctionOutputDelegate outputLocalDelegate = c =>
        {
            outputLog.Write((char)c);
            outputLog.Flush();
        };
        FunctionOutputDelegate errorLocalDelegate = c =>
        {
            errorLog.Write((char)c);
            errorLog.Flush();
        };
        var outputLocalDelegatePtr = Marshal.GetFunctionPointerForDelegate(outputLocalDelegate);
        var outputLocalHandle = GCHandle.Alloc(outputLocalDelegate);

        var errorLocalDelegatePtr = Marshal.GetFunctionPointerForDelegate(errorLocalDelegate);
        var errorLocalHandle = GCHandle.Alloc(errorLocalDelegate);
        try
        {
            return Validate(argc, args, outputLocalDelegatePtr, errorLocalDelegatePtr);
        }
        finally
        {
            outputLocalHandle.Free();
            errorLocalHandle.Free();
        }
    }

    [DllImport("nplug_validator", EntryPoint = "nplug_validator_initialize")]
    private static extern void Initialize();

        
    [DllImport("nplug_validator", EntryPoint = "nplug_validator_validate")]
    private static extern int Validate(int argc, string[] argv, IntPtr output, IntPtr error);


    [DllImport("nplug_validator", EntryPoint = "nplug_validator_destroy")]
    private static extern void Destroy();
}