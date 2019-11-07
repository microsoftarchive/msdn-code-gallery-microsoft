/****************************** Module Header ******************************\
 * Module Name:  UnsafeNativeMethods.cs
 * Project:	     CSSoftKeyboard
 * Copyright (c) Microsoft Corporation.
 * 
 * This class wraps the GetKeyState and SendInput in User32.dll.
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

using System.Runtime.InteropServices;

namespace CSSoftKeyboard.UserInteraction
{
    internal sealed class UnsafeNativeMethods
    {

        /// <summary>
        /// Retrieves the status of the specified virtual key. The status specifies whether
        /// the key is up, down, or toggled (on, off—alternating each time the key is pressed).
        /// http://msdn.microsoft.com/en-us/library/ms646301(VS.85).aspx
        /// </summary>
        /// <param name="nVirtKey">A virtual key. </param>
        /// <returns>
        /// If the high-order bit is 1, the key is down; otherwise, it is up.
        /// If the low-order bit is 1, the key is toggled. A key, such as the CAPS LOCK key,
        /// is toggled if it is turned on. The key is off and untoggled if the low-order bit
        /// is 0. A toggle key's indicator light (if any) on the keyboard will be on when the
        /// key is toggled, and off when the key is untoggled.
        /// </returns>
        [DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern short GetKeyState(int nVirtKey);

        /// <summary>
        /// Synthesizes keystrokes, mouse motions, and button clicks.
        /// http://msdn.microsoft.com/en-us/library/ms646310(VS.85).aspx
        /// </summary>
        /// <param name="nInputs">The number of structures in the pInputs array.</param>
        /// <param name="pInputs">
        /// An array of INPUT structures. Each structure represents an event to be inserted 
        /// into the keyboard or mouse input stream.
        /// </param>
        /// <param name="cbSize">
        /// The size, in bytes, of an INPUT structure. If cbSize is not the size of an 
        /// INPUT structure, the function fails.
        /// </param>
        /// <returns>
        /// If the function returns zero, the input was already blocked by another thread. 
        /// </returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern uint SendInput(uint nInputs, NativeMethods.INPUT[] pInputs,
            int cbSize);

    }
}
