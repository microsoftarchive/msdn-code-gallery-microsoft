/****************************** Module Header ******************************\
* Module Name:  KeyModifiers.cs
* Project:	    CSRegisterHotkey
* Copyright (c) Microsoft Corporation.
* 
* This enum defines the modifiers to generate the WM_HOTKEY message. 
* See http://msdn.microsoft.com/en-us/library/ms646309(VS.85).aspx.
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

namespace CSRegisterHotkey
{
    [Flags]
    public enum KeyModifiers
    {
        None = 0,
        Alt = 1,
        Control = 2,
        Shift = 4,

        // Either WINDOWS key was held down. These keys are labeled with the Windows logo.
        // Keyboard shortcuts that involve the WINDOWS key are reserved for use by the 
        // operating system.
        Windows = 8
    }
}
