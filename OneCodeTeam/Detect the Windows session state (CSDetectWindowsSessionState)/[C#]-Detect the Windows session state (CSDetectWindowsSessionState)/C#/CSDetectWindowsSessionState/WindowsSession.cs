/****************************** Module Header ******************************\
* Module Name:  WindowsSession.cs
* Project:	    CSDetectWindowsSessionState
* Copyright (c) Microsoft Corporation.
* 
* The enum WindowsSession is used to subscribe SystemEvents.SessionSwitch event.
* 
* It imports the method OpenInputDesktop to check whether current session is locked.
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
using Microsoft.Win32;
using System.Security.Permissions;

namespace CSDetectWindowsSessionState
{
    public class WindowsSession : IDisposable
    {

        // Required to read objects on the desktop.
        const int desktop_ReadObjects = 0x0001;

        // Required to write objects on the desktop.
        const int desktop_WriteObjects = 0x0080;

        /// <summary>
        /// Open the desktop that receives user input.
        /// This method is used to check whether the desktop is locked. If the function 
        /// fails, the return value is IntPtr.Zero. 
        /// Note:
        ///      If UAC popups a secure desktop, this method may also fail. There is 
        ///      no API for differentiate between Locked Desktop and UAC popup.
        /// </summary>
        /// <param name="dwFlags">
        /// This parameter can be zero or the following value.
        /// 0x0001: Allows processes running in other accounts on the desktop to set
        /// hooks in this process.
        /// </param>
        /// <param name="fInherit">
        /// If this value is TRUE, processes created by this process will inherit the 
        /// handle. Otherwise, the processes do not inherit this handle.
        /// </param>
        /// <param name="dwDesiredAccess">
        /// The access to the desktop. See 
        /// http://msdn.microsoft.com/en-us/library/ms682575(VS.85).aspx
        /// This sample will use DESKTOP_READOBJECTS (0x0001L) and 
        /// DESKTOP_WRITEOBJECTS (0x0080L)
        /// </param>
        /// <returns>If the function fails, the return value is IntPtr.Zero. </returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr OpenInputDesktop(
            int dwFlags,
            bool fInherit,
            int dwDesiredAccess);

        /// <summary>
        /// Close an open handle to a desktop object.
        /// </summary>
        /// <param name="hDesktop">
        /// A handle to the desktop to be closed.
        /// </param>
        /// <returns>
        /// If the function succeeds, then return true.
        /// </returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool CloseDesktop(IntPtr hDesktop);


        

        // Specify whether this instance is disposed.
        private bool disposed;

        public event EventHandler<SessionSwitchEventArgs> StateChanged;

        /// <summary>
        /// Initialize the instance.
        /// Register the SystemEvents.SessionSwitch event.
        /// </summary>
        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public WindowsSession()
        {
            SystemEvents.SessionSwitch +=
                new SessionSwitchEventHandler(SystemEvents_SessionSwitch);
        }

        /// <summary>
        /// Handle the SystemEvents.SessionSwitch event.
        /// </summary>
        void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            this.OnStateChanged(e);
        }

        /// <summary>
        /// Raise the StateChanged event.
        /// </summary>
        protected virtual void OnStateChanged(SessionSwitchEventArgs e)
        {
            if (StateChanged != null)
            {
                StateChanged(this, e);
            }
        }

        /// <summary>
        /// Check whether current session is locked. 
        /// Note: If UAC popups a secure desktop, this method may also fail. 
        ///       There is no API for differentiating between Locked Desktop
        ///       and UAC Secure Desktop. 
        /// </summary>
        public bool IsLocked()
        {
            IntPtr hDesktop = IntPtr.Zero; ;
            try
            {

                // Opens the desktop that receives user input.
                hDesktop = OpenInputDesktop(0, false,
                    desktop_ReadObjects | desktop_WriteObjects);

                // If hDesktop is IntPtr.Zero, then the session is locked.
                return hDesktop == IntPtr.Zero;
            }
            finally
            {

                // Close an open handle to a desktop object.
                if (hDesktop != IntPtr.Zero)
                {
                    CloseDesktop(hDesktop);
                }
            }
        }

        /// <summary>
        /// Dispose the resources.
        /// </summary>
        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        protected virtual void Dispose(bool disposing)
        {
            // Protect from being called multiple times.
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                // Clean up all managed resources.
                SystemEvents.SessionSwitch -=
                    new SessionSwitchEventHandler(SystemEvents_SessionSwitch);

            }

            disposed = true;
        }
    }
}

