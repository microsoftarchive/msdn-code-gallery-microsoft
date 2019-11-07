/****************************** Module Header ******************************\
 * Module Name:  KeyboardInput.cs
 * Project:      CSSoftKeyboard
 * Copyright (c) Microsoft Corporation.
 * 
 * This class wraps the UnsafeNativeMethods.SendInput to synthesize keystrokes.
 * 
 * There are 3 scenarios:
 * 1. A single key is pressed, such as "A".
 * 2. A key with modifier keys is pressed, such as "Ctrl+A".
 * 3. A key that could be toggled is pressed, such as Caps Lock, Num Lock or
 *    Scroll Lock. 
 * 
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace CSSoftKeyboard.UserInteraction
{
    public static class KeyboardInput
    {

        /// <summary>
        /// A single key is pressed.
        /// </summary>
        public static void SendKey(int key)
        {
            SendKey(null, key);
        }

        /// <summary>
        /// A key with modifier keys is pressed. 
        /// </summary>
        public static void SendKey(IEnumerable<int> modifierKeys, int key)
        {
            if (key <= 0)
            {
                return;
            }

            // Only a single key is pressed.
            if (modifierKeys == null || modifierKeys.Count()==0)
            {
                var inputs = new NativeMethods.INPUT[1];
                inputs[0].type = NativeMethods.INPUT_KEYBOARD;
                inputs[0].inputUnion.ki.wVk = (short)key;
                UnsafeNativeMethods.SendInput(1, inputs, Marshal.SizeOf(inputs[0]));
            }

            // A key with modifier keys is pressed. 
            else
            {
                // To simulate this scenario, the inputs contains the toggling 
                // modifier keys, pressing the key and releasing modifier keys events.
                //
                // For example, to simulate Ctrl+C, we have to send 3 inputs:
                // 1. Ctrl is pressed.
                // 2. C is pressed. 
                // 3. Ctrl is released.
                var inputs = new NativeMethods.INPUT[modifierKeys.Count()*2 + 1];
                
                int i = 0;

                // Simulate toggling the modifier keys.
                foreach (var modifierKey in modifierKeys)
                {
                    inputs[i].type = NativeMethods.INPUT_KEYBOARD;
                    inputs[i].inputUnion.ki.wVk = (short)modifierKey;
                    i++;
                }

                // Simulate pressing the key.
                inputs[i].type = NativeMethods.INPUT_KEYBOARD;
                inputs[i].inputUnion.ki.wVk = (short)key;
                i++;

                // Simulate releasing the modifier keys.
                foreach (var modifierKey in modifierKeys)
                {
                    inputs[i].type = NativeMethods.INPUT_KEYBOARD;
                    inputs[i].inputUnion.ki.wVk = (short)modifierKey; 
                    inputs[i].inputUnion.ki.dwFlags = NativeMethods.KEYEVENTF_KEYUP;
                    i++;
                }
                
                UnsafeNativeMethods.SendInput((uint)inputs.Length, inputs,
                    Marshal.SizeOf(inputs[0]));
            }
        }

        /// <summary>
        /// A key that could be toggled is pressed, such as Caps Lock, 
        /// Num Lock or Scroll Lock. To simulate it, the key should be pressed and then 
        /// released.
        /// </summary>
        public static void SendToggledKey(int key)
        {
            var inputs = new NativeMethods.INPUT[2];

            // Press the key.
            inputs[0].type = NativeMethods.INPUT_KEYBOARD;
            inputs[0].inputUnion.ki.wVk = (short)key;

            // Release the key.
            inputs[1].type = NativeMethods.INPUT_KEYBOARD;
            inputs[1].inputUnion.ki.wVk = (short)key;
            inputs[1].inputUnion.ki.dwFlags = NativeMethods.KEYEVENTF_KEYUP;

            UnsafeNativeMethods.SendInput(2, inputs, Marshal.SizeOf(inputs[0]));
        }
    }
}
