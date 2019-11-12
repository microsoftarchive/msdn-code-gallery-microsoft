// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//
//
// Abstract:
//
//     This file contains helper methods that provide a friendly way to access win32 window functions.

using System;
using System.Runtime.InteropServices;

namespace Microsoft.Samples.Printing.PrinterExtension.Helpers
{
    class WindowHelper
    {
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("User32", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern bool SetForegroundWindow(IntPtr hwnd);
    }
}
