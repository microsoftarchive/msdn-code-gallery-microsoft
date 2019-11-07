/****************************** Module Header ******************************\
 * Module Name:  NativeMethods.cs
 * Project:      CSSoftKeyboard
 * Copyright (c) Microsoft Corporation.
 * 
 * This class contains the structures that are used in SendInput method. 
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

namespace CSSoftKeyboard.UserInteraction
{
    internal static class NativeMethods
    {

        // The constants used in the INPUT structure.
        public const int INPUT_MOUSE = 0;
        public const int INPUT_KEYBOARD = 1;
        public const int INPUT_HARDWARE = 2;

        // The constants used in the KEYBDINPUT structure.
        public const int KEYEVENTF_EXTENDEDKEY = 0x0001;
        public const int KEYEVENTF_KEYUP = 0x0002;

        /// <summary>
        /// Used by SendInput to store information for synthesizing input events such 
        /// as keystrokes, mouse movement, and mouse clicks.
        /// http://msdn.microsoft.com/en-us/library/ms646270(VS.85).aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct INPUT
        {
            /// <summary>
            /// INPUT_MOUSE    0
            /// INPUT_KEYBOARD 1
            /// INPUT_HARDWARE 2
            /// </summary>
            public int type;
            public NativeMethods.INPUTUNION inputUnion;
        }

        /// <summary>
        /// An INPUTUNION structure only contains one field. 
        /// http://msdn.microsoft.com/en-us/library/ms646270(VS.85).aspx
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        public struct INPUTUNION
        {         
            [FieldOffset(0)]
            public NativeMethods.HARDWAREINPUT hi;
            [FieldOffset(0)]
            public NativeMethods.KEYBDINPUT ki;
            [FieldOffset(0)]
            public NativeMethods.MOUSEINPUT mi;
        }

        /// <summary>
        /// The information about a simulated hardware event.
        /// http://msdn.microsoft.com/en-us/library/ms646269(VS.85).aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct HARDWAREINPUT
        {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        }

        /// <summary>
        /// The information about a simulated keyboard event.
        /// http://msdn.microsoft.com/en-us/library/ms646271(VS.85).aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT
        {
            public short wVk;
            public short wScan;

            // KEYEVENTF_EXTENDEDKEY 0x0001
            // KEYEVENTF_KEYUP 0x0002
            // KEYEVENTF_SCANCODE 0x0008
            // KEYEVENTF_UNICODE 0x0004
            public int dwFlags;

            public int time;
            public IntPtr dwExtraInfo;
        }

        /// <summary>
        /// The information about a simulated mouse event.
        /// http://msdn.microsoft.com/en-us/library/ms646273(VS.85).aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }
    }


}
