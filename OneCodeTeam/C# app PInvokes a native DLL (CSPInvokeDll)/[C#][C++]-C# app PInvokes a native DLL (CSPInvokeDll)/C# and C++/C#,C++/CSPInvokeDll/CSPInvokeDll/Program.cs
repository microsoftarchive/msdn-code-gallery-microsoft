/****************************** Module Header ******************************\
* Module Name:  Program.cs
* Project:      CSPInvokeDll
* Copyright (c) Microsoft Corporation.
* 
* Platform Invocation Services (P/Invoke) in .NET allows managed code to call 
* unmanaged functions that are implemented and exported in unmanaged DLLs.  
* This VC# code sample demonstrates using P/Invoke to call the functions 
* exported by the native DLLs: CppDynamicLinkLibrary.dll, user32.dll and 
* msvcrt.dll.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#region Using directives
using System;
using System.Runtime.InteropServices;
#endregion


namespace CSPInvokeDll
{
    class Program
    {
        static void Main(string[] args)
        {
            bool isLoaded = false;
            const string moduleName = "CppDynamicLinkLibrary";

            // Check whether or not the module is loaded.
            isLoaded = IsModuleLoaded(moduleName);
            Console.WriteLine("Module \"{0}\" is {1}loaded", moduleName, 
                isLoaded ? "" : "not ");

            //
            // Access the global data exported from the module.
            // 

            // P/Invoke does not allow you to access the global data exported 
            // from a DLL module.

            //
            // Call the functions exported from the module.
            //

            string str = "HelloWorld";
            int length;

            length = NativeMethod.GetStringLength1(str);
            Console.WriteLine("GetStringLength1(\"{0}\") => {1}", str, length);

            length = NativeMethod.GetStringLength2(str);
            Console.WriteLine("GetStringLength2(\"{0}\") => {1}", str, length);

            // P/Invoke the stdcall API, MessageBox, in user32.dll.
            MessageBoxResult msgResult = NativeMethod.MessageBox(
                IntPtr.Zero, "Do you want to ...", "CSPInvokeDll", 
                MessageBoxOptions.OkCancel);
            Console.WriteLine("user32!MessageBox => {0}", msgResult);

            // P/Invoke the cdecl API, printf, in msvcrt.dll.
            Console.Write("msvcrt!printf => ");
            NativeMethod.printf("%s!%s\n", "msvcrt", "printf");

            // 
            // Call the callback functions exported from the module.
            // 

            // P/Invoke a method that requires callback as one of the args.
            CompareCallback cmpFunc = new CompareCallback(Max);
            int max = NativeMethod.CompareInts(2, 3, cmpFunc);

            // Make sure the lifetime of the delegate instance covers the 
            // lifetime of the unmanaged code; otherwise, the delegate 
            // will not be available after it is garbage-collected, and you 
            // may get the Access Violation or Illegal Instruction error.
            GC.KeepAlive(cmpFunc);
            Console.WriteLine("Max(2, 3) => {0}", max);

            //
            // Use the class exported from the module.
            //

            // There is no easy way to call the classes in a native C++ DLL 
            // module through P/Invoke. Visual C++ Team Blog introduced a 
            // solution, but it is complicated: 
            // http://go.microsoft.com/?linkid=9729423.
            // The recommended way of calling native C++ class from .NET are  
            // 1) use a C++/CLI class library to wrap the native C++ module, 
            //    and your .NET code class the C++/CLI wrapper class to 
            //    indirectly access the native C++ class.
            // 2) convert the native C++ module to be a COM server and expose
            //    the native C++ class through a COM interface. Then, the 
            //    .NET code can access the class through .NET-COM interop.

            // Unload the DLL by calling GetModuleHandle and FreeLibrary. 
            if (!FreeLibrary(GetModuleHandle(moduleName)))
            {
                Console.WriteLine("FreeLibrary failed w/err {0}", 
                    Marshal.GetLastWin32Error());
            }

            // Check whether or not the module is loaded.
            isLoaded = IsModuleLoaded(moduleName);
            Console.WriteLine("Module \"{0}\" is {1}loaded", moduleName, 
                isLoaded ? "" : "not ");
        }
        
        /// <summary>
        /// This is the callback function for the method Max exported from 
        /// the DLL CppDynamicLinkLibrary.dll.
        /// </summary>
        /// <param name="a">the first integer</param>
        /// <param name="b">the second integer</param>
        /// <returns>
        /// The function returns a positive number if a > b, returns 0 if a 
        /// equals b, and returns a negative number if a &lt b.
        /// </returns>
        static int Max(int a, int b)
        {
            return (a - b);
        }


        #region Module Related Operations

        /// <summary>
        /// Check whether or not the specified module is loaded in the 
        /// current process.
        /// </summary>
        /// <param name="moduleName">the module name</param>
        /// <returns>
        /// The function returns true if the specified module is loaded in 
        /// the current process. If the module is not loaded, the function 
        /// returns false.
        /// </returns>
        static bool IsModuleLoaded(string moduleName)
        {
            // Get the module in the process according to the module name.
            IntPtr hMod = GetModuleHandle(moduleName);
            return (hMod != IntPtr.Zero);
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr GetModuleHandle(string moduleName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool FreeLibrary(IntPtr hModule);

        #endregion
    }
}