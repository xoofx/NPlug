using NUnit.Framework;
using System;
using System.Security.Cryptography;
using NPlug;
using NPlug.Interop;
using NPlug.Validator;

namespace NPlug.Tests;

public class BasicPluginValidation
{
    public static void Main()
    {
        //InteropHelper.Tracer = new InteropTracer();
        var factory = HelloWorldPlugin.GetFactory();
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

        var factory = HelloWorldPlugin.GetFactory();
        AudioPluginValidator.Validate(factory, Console.Out, Console.Error);
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
