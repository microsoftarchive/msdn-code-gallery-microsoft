/******************************** Module Header ********************************\
Module Name:    NativeMethods.cs
Project:        CSOfficeRibbonAccessibility
Copyright (c) Microsoft Corporation.

Declares the P/Invoke signatures of native APIs.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*******************************************************************************/

using System;
using System.Runtime.InteropServices;
using Microsoft.Office.Core;
using System.Text;


namespace CSOfficeRibbonAccessibility
{
    internal class NativeMethods
    {
        // Retrieves the child ID or IDispatch of each child within an accessible 
        // container object.
        [DllImport("oleacc.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int AccessibleChildren(
            IAccessible paccContainer,
            int iChildStart,
            int cChildren,
            [Out()] [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] 
            object[] rgvarChildren,
            ref int pcObtained);

        // Retrieves the address of the specified interface for the object 
        // associated with the specified window.
        [DllImport("oleacc.dll", PreserveSig = false, CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public static extern object AccessibleObjectFromWindow(
             IntPtr hwnd, uint id, ref Guid iid);

        // Retrieves the localized string that describes the object's role for 
        // the specified role value.
        [DllImport("oleacc.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern uint GetRoleText(uint dwRole,
            [Out] StringBuilder lpszRole, uint cchRoleMax);

        // Retrieves a localized string that describes an object's state for a 
        // single predefined state bit flag. Because state values are a 
        // combination of one or more bit flags, clients call this function more 
        // than once to retrieve all state strings.
        [DllImport("oleacc.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern uint GetStateText(MSAAStateConstants dwStateBit,
            [Out] StringBuilder lpszStateBit, uint cchStateBitMax);
    }


    [Flags]
    internal enum MSAAStateConstants
    {
        STATE_SYSTEM_NORMAL = 0,
        STATE_SYSTEM_UNAVAILABLE = 1,
        STATE_SYSTEM_SELECTED = 2,
        STATE_SYSTEM_FOCUSED = 4,
        STATE_SYSTEM_PRESSED = 8,
        STATE_SYSTEM_CHECKED = 0x10,
        STATE_SYSTEM_MIXED = 0x20,
        STATE_SYSTEM_READONLY = 0x40,
        STATE_SYSTEM_HOTTRACKED = 0x80,
        STATE_SYSTEM_DEFAULT = 0x100,
        STATE_SYSTEM_EXPANDED = 0x200,
        STATE_SYSTEM_COLLAPSED = 0x400,
        STATE_SYSTEM_BUSY = 0x800,
        STATE_SYSTEM_FLOATING = 0x1000,
        STATE_SYSTEM_MARQUEED = 0x2000,
        STATE_SYSTEM_ANIMATED = 0x4000,
        STATE_SYSTEM_INVISIBLE = 0x8000,
        STATE_SYSTEM_OFFSCREEN = 0x10000,
        STATE_SYSTEM_SIZEABLE = 0x20000,
        STATE_SYSTEM_MOVEABLE = 0x40000,
        STATE_SYSTEM_SELFVOICING = 0x80000,
        STATE_SYSTEM_FOCUSABLE = 0x100000,
        STATE_SYSTEM_SELECTABLE = 0x200000,
        STATE_SYSTEM_LINKED = 0x400000,
        STATE_SYSTEM_TRAVERSED = 0x800000,
        STATE_SYSTEM_MULTISELECTABLE = 0x1000000,
        STATE_SYSTEM_EXTSELECTABLE = 0x2000000,
        STATE_SYSTEM_ALERT_LOW = 0x4000000,
        STATE_SYSTEM_ALERT_MEDIUM = 0x8000000,
        STATE_SYSTEM_ALERT_HIGH = 0x10000000,
        STATE_SYSTEM_HASPOPUP = 0x40000000,
        STATE_SYSTEM_VALID = 0x1FFFFFFF
    }
}
