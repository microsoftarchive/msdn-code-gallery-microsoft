/****************************** Module Header ******************************\
* Module Name:  NativeMethod.cs
* Project:      CSPInvokeDll
* Copyright (c) Microsoft Corporation.
* 
* The PInvoke signatures of the methods exported from the unmanaged DLLs.
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
using System.Text;
using System.Security;
#endregion


namespace CSPInvokeDll
{
    // Function delegate of the 'PFN_COMPARE' callback function. 
    // The delegate type has the UnmanagedFunctionPointerAttribute. Using 
    // this attribute, you can specify the calling convention of the native 
    // function pointer type. In the native API's header file, the callback 
    // PFN_COMPARE is defined as __stdcall (CALLBACK), so here we specify 
    // CallingConvention.StdCall.
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    delegate int CompareCallback(int a, int b);


    [SuppressUnmanagedCodeSecurity]
    class NativeMethod
    {
        [DllImport("CppDynamicLinkLibrary.dll", CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetStringLength1(string str);

        [DllImport("CppDynamicLinkLibrary.dll", CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int GetStringLength2(string str);

        [DllImport("CppDynamicLinkLibrary.dll", CharSet = CharSet.Auto)]
        public static extern int CompareInts(int a, int b, CompareCallback cmpFunc);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern MessageBoxResult MessageBox(IntPtr hWnd,
            string text, String caption, MessageBoxOptions options);

        [DllImport("msvcrt.dll", CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern int printf(string format, string arg1, string arg2);
    }


    /// <summary>
    /// Flags that define appearance and behaviour of a standard message 
    /// box displayed by a call to the MessageBox function.
    /// </summary>
    [Flags]
    enum MessageBoxOptions : uint
    {
        Ok = 0x000000,
        OkCancel = 0x000001,
        AbortRetryIgnore = 0x000002,
        YesNoCancel = 0x000003,
        YesNo = 0x000004,
        RetryCancel = 0x000005,
        CancelTryContinue = 0x000006,

        IconHand = 0x000010,
        IconQuestion = 0x000020,
        IconExclamation = 0x000030,
        IconAsterisk = 0x000040,
        UserIcon = 0x000080,

        IconWarning = IconExclamation,
        IconError = IconHand,
        IconInformation = IconAsterisk,
        IconStop = IconHand,

        DefButton1 = 0x000000,
        DefButton2 = 0x000100,
        DefButton3 = 0x000200,
        DefButton4 = 0x000300,

        ApplicationModal = 0x000000,
        SystemModal = 0x001000,
        TaskModal = 0x002000,

        Help = 0x004000, // Help Button
        NoFocus = 0x008000,

        SetForeground = 0x010000,
        DefaultDesktopOnly = 0x020000,
        Topmost = 0x040000,
        Right = 0x080000,
        RTLReading = 0x100000,
    }


    /// <summary>
    /// Represents possible values returned by the MessageBox function.
    /// </summary>
    enum MessageBoxResult : uint
    {
        Ok = 1, Cancel, Abort, Retry, Ignore,
        Yes, No, Close, Help, TryAgain, Continue,
        Timeout = 32000
    }
}