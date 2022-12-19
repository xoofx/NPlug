using NUnit.Framework;
using NPlug.Interop;
using NPlug.SimpleDelay;
using NPlug.Validator;

namespace NPlug.Tests;

public class BasicPluginValidation
{
    public static void Main()
    {
        //InteropHelper.Tracer = new InteropTracer();
        var factory = SimpleDelayPlugin.GetFactory();
        AudioPluginValidator.Validate(factory, Console.Out, Console.Error);

        if (InteropHelper.HasObjectAlive())
        {
            Console.WriteLine("=============================================================");
            Console.WriteLine(InteropHelper.DumpObjectAlive());
        }
    }

    [Test]
    public void TestSimple()
    {
        InteropHelper.Tracer = new InteropTracer();

        var factory = SimpleDelayPlugin.GetFactory();
        AudioPluginValidator.Validate(factory, Console.Out, Console.Error);
    }

    [Test]
    public void TestSimpleDelay()
    {
        //InteropHelper.Tracer = new InteropTracer();

        //var factory = HelloWorldPlugin.GetFactory();
        //AudioPluginValidator.Validate(@"C:\code\NPlug\samples\NPlug.SimpleDelay\bin\Release\net7.0\win-x64\publish\NPlug.SimpleDelay.vst3", Console.Out, Console.Error);
        AudioPluginValidator.Validate(@"C:\code\NPlug\samples\build\bin\Debug\NPlug.SimpleDelay.vst3", Console.Out, Console.Error);
    }



    private class InteropTracer : IInteropTracer
    {
        public void OnEnter(in NativeToManagedEvent evt)
        {
            Console.WriteLine($"<- 0x{evt.NativePointer:X16} {evt.InterfaceName}.{evt.MethodName} (enter)");
            Console.Out.Flush();
        }

        public void OnExit(in NativeToManagedEvent evt)
        {
            //Console.WriteLine($"<- 0x{evt.NativePointer:X16} {evt.InterfaceName}.{evt.MethodName} (exit)");
            //Console.Out.Flush();
        }

        public void OnExitWithError(in NativeToManagedEvent evt)
        {
            Console.WriteLine($"<- 0x{evt.NativePointer:X16} {evt.InterfaceName}.{evt.MethodName} (exit with exception: {evt.Exception})");
            Console.Out.Flush();
        }

        public void OnEnter(in ManagedToNativeEvent evt)
        {
            Console.WriteLine($"-> 0x{evt.NativePointer:X16} {evt.InterfaceName}.{evt.MethodName} (enter)");
            Console.Out.Flush();
        }

        public void OnExit(in ManagedToNativeEvent evt)
        {
            //Console.WriteLine($"-> 0x{evt.NativePointer:X16} {evt.InterfaceName}.{evt.MethodName} (exit)");
            //Console.Out.Flush();
        }

        public void OnExitWithError(in ManagedToNativeEvent evt)
        {
            Console.WriteLine($"-> 0x{evt.NativePointer:X16} {evt.InterfaceName}.{evt.MethodName} (exit with error {evt.Result})");
            Console.Out.Flush();
        }
    }
}
