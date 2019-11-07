/****************************** Module Header ******************************\
 * Module Name:  NoActivateWindow.cs
 * Project:      CSSoftKeyboard
 * Copyright (c) Microsoft Corporation.
 * 
 * The class represents a form that will not be activated until the user
 * presses the left mouse button within its nonclient area(such as the title
 * bar, menu bar, or window frame). When the left mouse button is released,
 * this window will activate the previous foreground Window.
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
using System.Security.Permissions;
using System.Windows.Forms;

namespace CSSoftKeyboard.NoActivate
{
    public class NoActivateWindow : Form
    {
        // The WS_EX_NOACTIVATE value for dwExStyle prevents foreground
        // activation by the system.
        const long WS_EX_NOACTIVATE = 0x08000000L;

        // WM_NCMOUSEMOVE Message is posted to a window when the cursor is 
        // moved within the nonclient area of the window. 
        const int WM_NCMOUSEMOVE = 0x00A0;

        // WM_NCLBUTTONDOWN Message is posted when the user presses the left
        // mouse button while the cursor is within the nonclient area of a window. 
        const int WM_NCLBUTTONDOWN = 0x00A1;

        // The handle of the previous foreground Window.
        IntPtr previousForegroundWindow = IntPtr.Zero;

        /// <summary>
        /// Set the form style to WS_EX_NOACTIVATE so that it will not get focus. 
        /// </summary>
        protected override CreateParams CreateParams
        {
            [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= (int)WS_EX_NOACTIVATE;
                return cp;
            }
        }

        /// <summary>
        /// Process Windows messages.
        /// 
        /// When the user presses the left mouse button while the cursor is within the
        /// nonclient area of this window, the it will store the handle of previous 
        /// foreground Window, and then activate itself.
        /// 
        /// When the cursor is moved within the nonclient area of the window, which means
        /// that the left mouse button is released, this window will activate the previous 
        /// foreground Window.
        /// </summary>
        /// <param name="m"></param>
        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_NCLBUTTONDOWN:

                    // Get the current foreground window.
                    var foregroundWindow = UnsafeNativeMethods.GetForegroundWindow();

                    // If this window is not the current foreground window, then activate
                    // itself.
                    if (foregroundWindow != this.Handle)
                    {
                        UnsafeNativeMethods.SetForegroundWindow(this.Handle);

                        // Store the handle of previous foreground window.
                        if (foregroundWindow != IntPtr.Zero)
                        {
                            previousForegroundWindow = foregroundWindow;
                        }
                    }

                    break;
                case WM_NCMOUSEMOVE:

                    // Determine whether previous window still exist. If yes, then 
                    // activate it.
                    // Note: There is a scenario that the previous window is closed, but 
                    //       the same handle is assgined to a new window.
                    if ( UnsafeNativeMethods.IsWindow(previousForegroundWindow))
                    {
                        UnsafeNativeMethods.SetForegroundWindow(previousForegroundWindow);
                        previousForegroundWindow = IntPtr.Zero;
                    }

                    break;
                default:
                    break;
            }

            base.WndProc(ref m);
        }
    }
}
