using NPlug.Proxy;
using System.Runtime.InteropServices;

namespace NPlug.Validator;

/// <summary>
/// Provides a way to validate a VST3 plugin.
/// </summary>
public static class AudioPluginValidator
{
    private static readonly AudioPluginProxy NativeProxy;

    /// <summary>
    /// Name of the validator plugin used to proxy native VST to managed VST.
    /// </summary>
    public const string DefaultPluginName = "nplug_validator_proxy";

    /// <summary>
    /// Path to the validator plugin folder.
    /// </summary>
    public static string DefaultPluginPath => Path.Combine(AppContext.BaseDirectory, $"{DefaultPluginName}.vst3");

    static AudioPluginValidator()
    {
        Initialize();
        NativeProxy = AudioPluginProxy.Load(Path.Combine(DefaultPluginPath, "Contents", AudioPluginProxy.GetVstArchitecture(), AudioPluginProxy.GetVstDynamicLibraryName(DefaultPluginName)));
    }

    /// <summary>
    /// Validate a plugin with the specified factory method. The factory method is the Export method of the AudioPluginFactory provided by NPlug.
    /// </summary>
    /// <param name="factory">Factory method (you can pass AudioPluginFactory.Export as an argument).</param>
    /// <param name="outputLog">A text writer to capture the output of the log.</param>
    /// <param name="errorLog">A text writer to capture the error of the log.</param>
    /// <returns><c>true</c> if the validation was successful; <c>false</c> otherwise.</returns>
    public static bool Validate(Func<IntPtr> factory, TextWriter outputLog, TextWriter errorLog)
    {
        NativeProxy.SetNativeFactory(factory);

        return Validate(2, new string[]
        {
            nameof(AudioPluginValidator),
            DefaultPluginPath
        }, outputLog, errorLog) == 0;
    }

    /// <summary>
    /// Validate a plugin with the specified path to a native plugin.
    /// </summary>
    /// <param name="pluginPath">Path to a native plugin.</param>
    /// <param name="outputLog">A text writer to capture the output of the log.</param>
    /// <param name="errorLog">A text writer to capture the error of the log.</param>
    /// <returns><c>true</c> if the validation was successful; <c>false</c> otherwise.</returns>
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
