/****************************** Module Header ******************************\
 * Module Name:  UnsafeNativeMethods.cs
 * Project:      CSSoftKeyboard
 * Copyright (c) Microsoft Corporation.
 * 
 * These methods are used to get/set the foreground window.
 * 
 * 
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/


using System;
using System.Runtime.InteropServices;

namespace CSSoftKeyboard.NoActivate
{
    internal static class UnsafeNativeMethods
    {
        /// <summary>
        /// Retrieve a handle to the foreground window.
        /// http://msdn.microsoft.com/en-us/library/ms633505(VS.85).aspx
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// Bring the thread that created the specified window into the foreground
        /// and activates the window. 
        /// http://msdn.microsoft.com/en-us/library/ms633539(VS.85).aspx
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// Determine whether the specified window handle identifies an existing window. 
        /// http://msdn.microsoft.com/en-us/library/ms633528(VS.85).aspx
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool IsWindow(IntPtr hWnd);
    }
}
