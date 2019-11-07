/******************************** Module Header *********************************\
* Module Name:	MainForm.cs
* Project:		CSWindowsHook
* Copyright (c) Microsoft Corporation.
* 
* This example demonstrates how to set a hook that is specific to a thread as well 
* as the global hook by using the low-level mouse and keyboard hooks in .NET. You 
* can use hooks to monitor certain types of events. You can associate these events 
* with a specific thread or with all the threads in the same desktop as the 
* calling thread.
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.Reflection;
using System.Diagnostics;
#endregion


namespace CSWindowsHook
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private const int WM_KEYDOWN = 0x100;
        private const int WM_SYSKEYDOWN = 0x104;


        #region Local Mouse Hook

        // Handle to the local mouse hook procedure
        private IntPtr hLocalMouseHook = IntPtr.Zero;
        private HookProc localMouseHookCallback = null;

        /// <summary>
        /// Set local mouse hook
        /// </summary>
        /// <returns></returns>
        private bool SetLocalMouseHook()
        {
            // Create an instance of HookProc.
            localMouseHookCallback = new HookProc(this.MouseProc);

            hLocalMouseHook = NativeMethods.SetWindowsHookEx(
                HookType.WH_MOUSE,
                localMouseHookCallback,
                IntPtr.Zero,
                NativeMethod.GetCurrentThreadId());
            return hLocalMouseHook != IntPtr.Zero;
        }

        /// <summary>
        /// Remove the local mouse hook
        /// </summary>
        /// <returns></returns>
        private bool RemoveLocalMouseHook()
        {
            if (hLocalMouseHook != IntPtr.Zero)
            {
                // Unhook the mouse hook
                if (!NativeMethods.UnhookWindowsHookEx(hLocalMouseHook))
                    return false;

                hLocalMouseHook = IntPtr.Zero;
            }
            return true;
        }

        /// <summary>
        /// Mouse hook procedure
        /// The system calls this function whenever an application calls the 
        /// GetMessage or PeekMessage function and there is a mouse message to be 
        /// processed. 
        /// </summary>
        /// <param name="nCode">
        /// The hook code passed to the current hook procedure.
        /// When nCode equals HC_ACTION, the wParam and lParam parameters contain 
        /// information about a mouse message.
        /// When nCode equals HC_NOREMOVE, the wParam and lParam parameters 
        /// contain information about a mouse message, and the mouse message has 
        /// not been removed from the message queue. (An application called the 
        /// PeekMessage function, specifying the PM_NOREMOVE flag.)
        /// </param>
        /// <param name="wParam">
        /// Specifies the identifier of the mouse message. 
        /// </param>
        /// <param name="lParam">Pointer to a MOUSEHOOKSTRUCT structure.</param>
        /// <returns></returns>
        /// <see cref="http://msdn.microsoft.com/en-us/library/ms644988.aspx"/>
        private int MouseProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode == HookCodes.HC_ACTION)
            {
                // Marshal the MOUSEHOOKSTRUCT data from the callback lParam
                MOUSEHOOKSTRUCT mouseHookStruct = (MOUSEHOOKSTRUCT)
                    Marshal.PtrToStructure(lParam, typeof(MOUSEHOOKSTRUCT));

                // Get the mouse WM from the wParam parameter
                MouseMessage wmMouse = (MouseMessage)wParam;

                // Display the current mouse coordinates and the message
                String log = String.Format("X = {0} Y = {1}  ({2})\r\n",
                    mouseHookStruct.pt.x, mouseHookStruct.pt.y, wmMouse);
                this.tbLog.AppendText(log);
            }

            // Pass the hook information to the next hook procedure in chain
            return NativeMethods.CallNextHookEx(hLocalMouseHook, nCode, wParam, lParam);
        }

        #endregion


        #region Global Low-level Mouse Hook

        // Handle to the global low-level mouse hook procedure
        private IntPtr hGlobalLLMouseHook = IntPtr.Zero;
        private HookProc globalLLMouseHookCallback = null;

        /// <summary>
        /// Set global low-level mouse hook
        /// </summary>
        /// <returns></returns>
        private bool SetGlobalLLMouseHook()
        {
            // Create an instance of HookProc.
            globalLLMouseHookCallback = new HookProc(this.LowLevelMouseProc);

            hGlobalLLMouseHook = NativeMethods.SetWindowsHookEx(
                HookType.WH_MOUSE_LL,  // Must be LL for the global hook
                globalLLMouseHookCallback,
                // Get the handle of the current module
                Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]),
                // The hook procedure is associated with all existing threads running 
                // in the same desktop as the calling thread.
                0);
            return hGlobalLLMouseHook != IntPtr.Zero;
        }

        /// <summary>
        /// Remove the global low-level mouse hook
        /// </summary>
        /// <returns></returns>
        private bool RemoveGlobalLLMouseHook()
        {
            if (hGlobalLLMouseHook != IntPtr.Zero)
            {
                // Unhook the low-level mouse hook
                if (!NativeMethods.UnhookWindowsHookEx(hGlobalLLMouseHook))
                    return false;

                hGlobalLLMouseHook = IntPtr.Zero;
            }
            return true;
        }

        /// <summary>
        /// Low-level mouse hook procedure
        /// The system call this function every time a new mouse input event is 
        /// about to be posted into a thread input queue. The mouse input can come 
        /// from the local mouse driver or from calls to the mouse_event function. 
        /// If the input comes from a call to mouse_event, the input was 
        /// "injected". However, the WH_MOUSE_LL hook is not injected into another 
        /// process. Instead, the context switches back to the process that 
        /// installed the hook and it is called in its original context. Then the 
        /// context switches back to the application that generated the event. 
        /// </summary>
        /// <param name="nCode">
        /// The hook code passed to the current hook procedure.
        /// When nCode equals HC_ACTION, the wParam and lParam parameters contain 
        /// information about a mouse message.
        /// </param>
        /// <param name="wParam">
        /// This parameter can be one of the following messages: 
        /// WM_LBUTTONDOWN, WM_LBUTTONUP, WM_MOUSEMOVE, WM_MOUSEWHEEL, 
        /// WM_MOUSEHWHEEL, WM_RBUTTONDOWN, or WM_RBUTTONUP. 
        /// </param>
        /// <param name="lParam">Pointer to an MSLLHOOKSTRUCT structure.</param>
        /// <returns></returns>
        /// <see cref="http://msdn.microsoft.com/en-us/library/ms644986.aspx"/>
        public int LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                // Marshal the MSLLHOOKSTRUCT data from the callback lParam
                MSLLHOOKSTRUCT mouseLLHookStruct = (MSLLHOOKSTRUCT)
                    Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));

                // Get the mouse WM from the wParam parameter
                MouseMessage wmMouse = (MouseMessage)wParam;

                // Display the current mouse coordinates and the message
                String log = String.Format("X = {0} Y = {1}  ({2})\r\n",
                    mouseLLHookStruct.pt.x, mouseLLHookStruct.pt.y, wmMouse);
                this.tbLog.AppendText(log);
            }

            // Pass the hook information to the next hook procedure in chain
            return NativeMethods.CallNextHookEx(hGlobalLLMouseHook, nCode, wParam, lParam);
        }

        #endregion


        #region Local Keyboard Hook

        // Handle to the local keyboard hook procedure
        private IntPtr hLocalKeyboardHook = IntPtr.Zero;
        private HookProc localKeyboardHookCallback = null;

        /// <summary>
        /// Set local keyboard hook
        /// </summary>
        /// <returns></returns>
        private bool SetLocalKeyboardHook()
        {
            // Create an instance of HookProc.
            localKeyboardHookCallback = new HookProc(this.KeyboardProc);

            hLocalKeyboardHook = NativeMethods.SetWindowsHookEx(
                HookType.WH_KEYBOARD,
                localKeyboardHookCallback,
                IntPtr.Zero,
                NativeMethod.GetCurrentThreadId());
            return hLocalKeyboardHook != IntPtr.Zero;
        }

        /// <summary>
        /// Remove the local keyboard hook
        /// </summary>
        /// <returns></returns>
        private bool RemoveLocalKeyboardHook()
        {
            if (hLocalKeyboardHook != IntPtr.Zero)
            {
                // Unhook the mouse hook
                if (!NativeMethods.UnhookWindowsHookEx(hLocalKeyboardHook))
                    return false;

                hLocalKeyboardHook = IntPtr.Zero;
            }
            return true;
        }

        /// <summary>
        /// Keyboard hook procedure
        /// The system calls this function whenever an application calls the 
        /// GetMessage or PeekMessage function and there is a keyboard message 
        /// (WM_KEYUP or WM_KEYDOWN) to be processed.
        /// </summary>
        /// <param name="nCode">
        /// The hook code passed to the current hook procedure.
        /// When nCode equals HC_ACTION, the wParam and lParam parameters contain 
        /// information about a keystroke message.
        /// When nCode equals HC_NOREMOVE, the wParam and lParam parameters 
        /// contain information about a keystroke message, and the keystroke 
        /// message has not been removed from the message queue. (An application 
        /// called the PeekMessage function, specifying the PM_NOREMOVE flag.)
        /// </param>
        /// <param name="wParam">
        /// Specifies the virtual-key code of the key that generated the keystroke 
        /// message. http://msdn.microsoft.com/en-us/library/dd375731.aspx
        /// </param>
        /// <param name="lParam">
        /// Specifies the repeat count, scan code, extended-key flag, context code, 
        /// previous key-state flag, and transition-state flag.
        /// http://msdn.microsoft.com/en-us/library/ms646267.aspx#_win32_Keystroke_Message_Flags
        /// </param>
        /// <returns></returns>
        /// <see cref="http://msdn.microsoft.com/en-us/library/ms644984.aspx"/>
        public int KeyboardProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode == HookCodes.HC_ACTION)
            {
                // Get the virtual key code from wParam
                // http://msdn.microsoft.com/en-us/library/dd375731.aspx
                Keys vkCode = (Keys)wParam;

                // Get the keystroke message flags
                // http://msdn.microsoft.com/en-us/library/ms646267.aspx#_win32_Keystroke_Message_Flags
                string flag = lParam.ToString("X8");

                string log = String.Format("Virtual-Key code: {0}  Flag: {1}\r\n",
                    vkCode, flag);
                this.tbLog.AppendText(log);
            }

            // Pass the hook information to the next hook procedure in chain
            return NativeMethods.CallNextHookEx(hLocalKeyboardHook, nCode, wParam, lParam);
        }

        #endregion


        #region Global Low-level Keyboard Hook

        // Handle to the global low-level keyboard hook procedure
        private IntPtr hGlobalLLKeyboardHook = IntPtr.Zero;
        private HookProc globalLLKeyboardHookCallback = null;

        /// <summary>
        /// Set global low-level keyboard hook
        /// </summary>
        /// <returns></returns>
        private bool SetGlobalLLKeyboardHook()
        {
            // Create an instance of HookProc.
            globalLLKeyboardHookCallback = new HookProc(this.LowLevelKeyboardProc);

            hGlobalLLKeyboardHook = NativeMethods.SetWindowsHookEx(
                HookType.WH_KEYBOARD_LL,  // Must be LL for the global hook
                globalLLKeyboardHookCallback,
                // Get the handle of the current module
                Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]),
                // The hook procedure is associated with all existing threads running 
                // in the same desktop as the calling thread.
                0);
            return hGlobalLLKeyboardHook != IntPtr.Zero;
        }

        /// <summary>
        /// Remove the global low-level keyboard hook
        /// </summary>
        /// <returns></returns>
        private bool RemoveGlobalLLKeyboardHook()
        {
            if (hGlobalLLKeyboardHook != IntPtr.Zero)
            {
                // Unhook the mouse hook
                if (!NativeMethods.UnhookWindowsHookEx(hGlobalLLKeyboardHook))
                    return false;

                hGlobalLLKeyboardHook = IntPtr.Zero;
            }
            return true;
        }

        /// <summary>
        /// Low-level keyboard hook procedure.
        /// The system calls this function every time a new keyboard input event 
        /// is about to be posted into a thread input queue. The keyboard input 
        /// can come from the local keyboard driver or from calls to the 
        /// keybd_event function. If the input comes from a call to keybd_event, 
        /// the input was "injected". However, the WH_KEYBOARD_LL hook is not 
        /// injected into another process. Instead, the context switches back 
        /// to the process that installed the hook and it is called in its 
        /// original context. Then the context switches back to the application 
        /// that generated the event. 
        /// </summary>
        /// <param name="nCode">
        /// The hook code passed to the current hook procedure.
        /// When nCode equals HC_ACTION, it means that the wParam and lParam 
        /// parameters contain information about a keyboard message.
        /// </param>
        /// <param name="wParam">
        /// The parameter can be one of the following messages: 
        /// WM_KEYDOWN, WM_KEYUP, WM_SYSKEYDOWN, or WM_SYSKEYUP.
        /// </param>
        /// <param name="lParam">Pointer to a KBDLLHOOKSTRUCT structure.</param>
        /// <returns></returns>
        /// <see cref="http://msdn.microsoft.com/en-us/library/ms644985.aspx"/>
        public int LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                // Marshal the KeyboardHookStruct data from the callback lParam
                KBDLLHOOKSTRUCT keyboardLLHookStruct = (KBDLLHOOKSTRUCT)
                    Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));

                // Get the virtual key code from KBDLLHOOKSTRUCT.vkCode
                // http://msdn.microsoft.com/en-us/library/dd375731.aspx
                Keys vkCode = (Keys)keyboardLLHookStruct.vkCode;

                // Get the keyboard WM from the wParam parameter
                KeyboardMessage wmKeyboard = (KeyboardMessage)wParam;

                // Display the current mouse coordinates and the message
                String log = String.Format("Virtual-Key code: {0}  ({1})\r\n",
                    vkCode, wmKeyboard);
                this.tbLog.AppendText(log);
            }

            // Pass the hook information to the next hook procedure in chain
            return NativeMethods.CallNextHookEx(hGlobalLLKeyboardHook, nCode, wParam, lParam);
        }

        #endregion


        #region Form Event Handlers

        private void btnLocalMouseHook_Click(object sender, EventArgs e)
        {
            if (hLocalMouseHook == IntPtr.Zero)
            {
                // Set the local mouse hook
                if (SetLocalMouseHook())
                {
                    btnLocalMouseHook.Text = "Unhook Local Mouse Hook";
                    btnGlobalLLMouseHook.Enabled = false;
                }
                else
                {
                    MessageBox.Show("SetWindowsHookEx(Mouse) failed");
                }
            }
            else
            {
                // Remove the local mouse hook
                if (RemoveLocalMouseHook())
                {
                    btnLocalMouseHook.Text = "Set Local Mouse Hook";
                    btnGlobalLLMouseHook.Enabled = true;
                }
                else
                {
                    MessageBox.Show("UnhookWindowsHookEx(Mouse) failed");
                }
            }
        }

        private void btnGlobalLLMouseHook_Click(object sender, EventArgs e)
        {
            if (hGlobalLLMouseHook == IntPtr.Zero)
            {
                // Set the global low-level mouse hook
                if (SetGlobalLLMouseHook())
                {
                    btnGlobalLLMouseHook.Text = "Unhook Global LL Mouse Hook";
                    btnLocalMouseHook.Enabled = false;
                }
                else
                {
                    MessageBox.Show("SetWindowsHookEx(LL Mouse) failed");
                }
            }
            else
            {
                // Remove the global low-level mouse hook
                if (RemoveGlobalLLMouseHook())
                {
                    btnGlobalLLMouseHook.Text = "Set Global LL Mouse Hook";
                    btnLocalMouseHook.Enabled = true;
                }
                else
                {
                    MessageBox.Show("UnhookWindowsHookEx(LL Mouse) failed");
                } 
            }
        }

        private void btnLocalKeyboardHook_Click(object sender, EventArgs e)
        {
            if (hLocalKeyboardHook == IntPtr.Zero)
            {
                // Set the local keyboard hook
                if (SetLocalKeyboardHook())
                {
                    btnLocalKeyboardHook.Text = "Unhook Local Keyboard Hook";
                    btnGlobalLLKeyboardHook.Enabled = false;
                }
                else
                {
                    MessageBox.Show("SetWindowsHookEx(Keyboard) failed");
                }
            }
            else
            {
                // Remove the local keyboard hook
                if (RemoveLocalKeyboardHook())
                {
                    btnLocalKeyboardHook.Text = "Set Local Keyboard Hook";
                    btnGlobalLLKeyboardHook.Enabled = true;
                }
                else
                {
                    MessageBox.Show("UnhookWindowsHookEx(Keyboard) failed");
                }   
            }
        }      

        private void btnGlobalLLKeyboardHook_Click(object sender, EventArgs e)
        {
            if (hGlobalLLKeyboardHook == IntPtr.Zero)
            {
                // Set the global low-level keyboard hook
                if (SetGlobalLLKeyboardHook())
                {
                    btnGlobalLLKeyboardHook.Text = "Unhook Global LL Keyboard Hook";
                    btnLocalKeyboardHook.Enabled = false;
                }
                else
                {
                    MessageBox.Show("SetWindowsHookEx(LL KeyBoard) failed");
                }
            }
            else
            {
                // Remove the global low-level keyboard hook
                if (RemoveGlobalLLKeyboardHook())
                {
                    btnGlobalLLKeyboardHook.Text = "Set Global LL Keyboard Hook";
                    btnLocalKeyboardHook.Enabled = true;
                }
                else
                {
                    MessageBox.Show("UnhookWindowsHookEx(LL Keyboard) failed");
                }
            }
        }

        /// <summary>
        /// Remove all hooks if set when closing form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            RemoveLocalMouseHook();
            RemoveGlobalLLMouseHook();
            RemoveLocalKeyboardHook();
            RemoveGlobalLLKeyboardHook();
        }

        #endregion
    }
}