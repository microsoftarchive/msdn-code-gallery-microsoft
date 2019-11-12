/******************************* Module Header **********************************\
* Module Name:	HookNative.cs
* Project:		CSWindowsHook
* Copyright (c) Microsoft Corporation.
* 
* This file wraps the hook APIs and the structs that are used by these APIs.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* History:
* * 3/28/2009 11:04 PM Rongchun Zhang Created
* * 4/6/2009 1:26 PM Jialiang Ge Reviewed
\********************************************************************************/

#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
#endregion


/// <summary>
/// The CallWndProc hook procedure is an application-defined or library-defined 
/// callback function used with the SetWindowsHookEx function. The HOOKPROC type 
/// defines a pointer to this callback function. CallWndProc is a placeholder for 
/// the application-defined or library-defined function name.
/// </summary>
/// <param name="nCode">
/// Specifies whether the hook procedure must process the message. 
/// </param>
/// <param name="wParam">
/// Specifies whether the message was sent by the current thread. 
/// </param>
/// <param name="lParam">
/// Pointer to a CWPSTRUCT structure that contains details about the message.
/// </param>
/// <returns>
/// If nCode is less than zero, the hook procedure must return the value returned 
/// by CallNextHookEx. If nCode is greater than or equal to zero, it is highly 
/// recommended that you call CallNextHookEx and return the value it returns; 
/// otherwise, other applications that have installed WH_CALLWNDPROC hooks will 
/// not receive hook notifications and may behave incorrectly as a result. If the 
/// hook procedure does not call CallNextHookEx, the return value should be zero. 
/// </returns>
internal delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);

internal class NativeMethods
{
    /// <summary>
    /// The SetWindowsHookEx function installs an application-defined hook 
    /// procedure into a hook chain. You would install a hook procedure to monitor 
    /// the system for certain types of events. These events are associated either 
    /// with a specific thread or with all threads in the same desktop as the 
    /// calling thread. 
    /// </summary>
    /// <param name="hookType">
    /// Specifies the type of hook procedure to be installed
    /// </param>
    /// <param name="callback">Pointer to the hook procedure.</param>
    /// <param name="hMod">
    /// Handle to the DLL containing the hook procedure pointed to by the lpfn 
    /// parameter. The hMod parameter must be set to NULL if the dwThreadId 
    /// parameter specifies a thread created by the current process and if the 
    /// hook procedure is within the code associated with the current process. 
    /// </param>
    /// <param name="dwThreadId">
    /// Specifies the identifier of the thread with which the hook procedure is 
    /// to be associated.
    /// </param>
    /// <returns>
    /// If the function succeeds, the return value is the handle to the hook 
    /// procedure. If the function fails, the return value is 0.
    /// </returns>
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr SetWindowsHookEx(HookType hookType,
        HookProc callback, IntPtr hMod, uint dwThreadId);

    /// <summary>
    /// The UnhookWindowsHookEx function removes a hook procedure installed in 
    /// a hook chain by the SetWindowsHookEx function. 
    /// </summary>
    /// <param name="hhk">Handle to the hook to be removed.</param>
    /// <returns>
    /// If the function succeeds, the return value is true.
    /// If the function fails, the return value is false.
    /// </returns>
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern bool UnhookWindowsHookEx(IntPtr hhk);

    /// <summary>
    /// The CallNextHookEx function passes the hook information to the next hook 
    /// procedure in the current hook chain. A hook procedure can call this 
    /// function either before or after processing the hook information. 
    /// </summary>
    /// <param name="idHook">Handle to the current hook.</param>
    /// <param name="nCode">
    /// Specifies the hook code passed to the current hook procedure.
    /// </param>
    /// <param name="wParam">
    /// Specifies the wParam value passed to the current hook procedure.
    /// </param>
    /// <param name="lParam">
    /// Specifies the lParam value passed to the current hook procedure.
    /// </param>
    /// <returns>
    /// This value is returned by the next hook procedure in the chain.
    /// </returns>
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern int CallNextHookEx(IntPtr hhk, int nCode,
        IntPtr wParam, IntPtr lParam);
}

internal static class HookCodes
{
    public const int HC_ACTION = 0;
    public const int HC_GETNEXT = 1;
    public const int HC_SKIP = 2;
    public const int HC_NOREMOVE = 3;
    public const int HC_NOREM = HC_NOREMOVE;
    public const int HC_SYSMODALON = 4;
    public const int HC_SYSMODALOFF = 5;
}

internal enum HookType
{
    WH_KEYBOARD = 2,
    WH_MOUSE = 7,
    WH_KEYBOARD_LL = 13,
    WH_MOUSE_LL = 14
}

[StructLayout(LayoutKind.Sequential)]
internal class POINT
{
    public int x;
    public int y;
}

/// <summary>
/// The MSLLHOOKSTRUCT structure contains information about a low-level keyboard 
/// input event. 
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct MOUSEHOOKSTRUCT
{
    public POINT pt;        // The x and y coordinates in screen coordinates
    public int hwnd;        // Handle to the window that'll receive the mouse message
    public int wHitTestCode;
    public int dwExtraInfo;
}

/// <summary>
/// The MOUSEHOOKSTRUCT structure contains information about a mouse event passed 
/// to a WH_MOUSE hook procedure, MouseProc. 
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct MSLLHOOKSTRUCT
{
    public POINT pt;        // The x and y coordinates in screen coordinates. 
    public int mouseData;   // The mouse wheel and button info.
    public int flags;
    public int time;        // Specifies the time stamp for this message. 
    public IntPtr dwExtraInfo;
}

internal enum MouseMessage
{
    WM_MOUSEMOVE = 0x0200,
    WM_LBUTTONDOWN = 0x0201,
    WM_LBUTTONUP = 0x0202,
    WM_LBUTTONDBLCLK = 0x0203,
    WM_RBUTTONDOWN = 0x0204,
    WM_RBUTTONUP = 0x0205,
    WM_RBUTTONDBLCLK = 0x0206,
    WM_MBUTTONDOWN = 0x0207,
    WM_MBUTTONUP = 0x0208,
    WM_MBUTTONDBLCLK = 0x0209,

    WM_MOUSEWHEEL = 0x020A,
    WM_MOUSEHWHEEL = 0x020E,

    WM_NCMOUSEMOVE = 0x00A0,
    WM_NCLBUTTONDOWN = 0x00A1,
    WM_NCLBUTTONUP = 0x00A2,
    WM_NCLBUTTONDBLCLK = 0x00A3,
    WM_NCRBUTTONDOWN = 0x00A4,
    WM_NCRBUTTONUP = 0x00A5,
    WM_NCRBUTTONDBLCLK = 0x00A6,
    WM_NCMBUTTONDOWN = 0x00A7,
    WM_NCMBUTTONUP = 0x00A8,
    WM_NCMBUTTONDBLCLK = 0x00A9
}

/// <summary>
/// The structure contains information about a low-level keyboard input event. 
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct KBDLLHOOKSTRUCT
{
    public int vkCode;      // Specifies a virtual-key code
    public int scanCode;    // Specifies a hardware scan code for the key
    public int flags;
    public int time;        // Specifies the time stamp for this message
    public int dwExtraInfo;
}

internal enum KeyboardMessage
{
    WM_KEYDOWN = 0x0100,
    WM_KEYUP = 0x0101,
    WM_SYSKEYDOWN = 0x0104,
    WM_SYSKEYUP = 0x0105
}