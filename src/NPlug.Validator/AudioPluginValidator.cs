using System.Runtime.InteropServices;

namespace NPlug.Validator
{
    public class AudioPluginValidator
    {


        //public static bool TryValidate(AudioPluginFactory factory, TextWriter outputWriter, TextWriter errorWriter)
        //{

        //}


        static void Main(string[] args)
        {
            Initialize();
            //for (int i = 0; i < 5; i++)
            {
                Console.WriteLine("--------------------------------------------------------------------");
                //Console.WriteLine($"Test {i}");
                Console.WriteLine("--------------------------------------------------------------------");
                var result = Validate(2, new string[]
                {
                    "validate_shared.dll",
                    @"C:\code\NPlug\src\NPlug.Validator\bin\Debug\net7.0\NPlug.HelloWorldPlugin.proxy.vst3"
                });

            }
            Destroy();
        }


        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void FunctionOutputDelegate(int c);

        private static int Validate(int argc, string[] args)
        {
            FunctionOutputDelegate outputLocalDelegate = c => Console.Out.Write((char)c);
            FunctionOutputDelegate errorLocalDelegate = c => Console.Error.Write((char)c);
            var outputLocalDelegatePtr = Marshal.GetFunctionPointerForDelegate(outputLocalDelegate);
            var errorLocalDelegatePtr = Marshal.GetFunctionPointerForDelegate(errorLocalDelegate);


            return Validate(argc, args, outputLocalDelegatePtr, errorLocalDelegatePtr);
        }

        [DllImport("nplug_validator", EntryPoint = "nplug_validator_initialize")]
        private static extern void Initialize();

        
        [DllImport("nplug_validator", EntryPoint = "nplug_validator_validate")]
        private static extern int Validate(int argc, string[] argv, IntPtr output, IntPtr error);


        [DllImport("nplug_validator", EntryPoint = "nplug_validator_destroy")]
        private static extern void Destroy();

    }
}