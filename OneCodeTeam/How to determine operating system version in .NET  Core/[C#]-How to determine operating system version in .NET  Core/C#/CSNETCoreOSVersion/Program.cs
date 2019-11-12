using System;
using System.Runtime.InteropServices;

namespace CSNETCoreOSVersion
{
    class Program
    {
        static void Main(string[] args)
        {
            // OS platform
            Console.WriteLine($"OS platform: {GetOSPlatform().ToString()}");
            // OS description, contains os version
            Console.WriteLine($"OS description: {RuntimeInformation.OSDescription}");
            // OS architecture
            Console.WriteLine($"OS architecture: {RuntimeInformation.OSArchitecture}");
            // Process architecture
            Console.WriteLine($"Process architecture: {RuntimeInformation.ProcessArchitecture}");
            //Framework description
            Console.WriteLine($"Framework description: {RuntimeInformation.FrameworkDescription}");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
                
        /// <summary>
        /// Get OS platform
        /// </summary>
        /// <returns></returns>
        public static OSPlatform GetOSPlatform()
        {
            OSPlatform osPlatform = OSPlatform.Create("Other Platform");
            // Check if it's windows
            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            osPlatform = isWindows ? OSPlatform.Windows : osPlatform;
            // Check if it's osx
            bool isOSX = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
            osPlatform = isOSX ? OSPlatform.OSX : osPlatform;
            // Check if it's Linux
            bool isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            osPlatform = isLinux ? OSPlatform.Linux : osPlatform;
            return osPlatform;
        }
    }
}