/****************************** Module Header ******************************\
* Module Name:  MainForm.cs
* Project:	    CSDetectWindowsSessionState
* Copyright (c) Microsoft Corporation.
* 
* This is the main form of this application. It is used to initialize the UI and 
* handle the events.
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
using System.Windows.Forms;
using Microsoft.Win32;

namespace CSDetectWindowsSessionState
{
    public partial class MainForm : Form
    {
        WindowsSession session;

        System.Threading.Timer timer;

        public MainForm()
        {
            InitializeComponent();

            // Initialize the WindowsSession instance.
            session = new WindowsSession();

            // Initialize the timer, but not start it.
            timer = new System.Threading.Timer(
                new System.Threading.TimerCallback(DetectSessionState),
                null,
                System.Threading.Timeout.Infinite,
                5000);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

            // Register the StateChanged event.
            session.StateChanged += new EventHandler<SessionSwitchEventArgs>(
                session_StateChanged);

        }

        /// <summary>
        /// Handle the StateChanged event of WindowsSession.
        /// </summary>
        void session_StateChanged(object sender, SessionSwitchEventArgs e)
        {
            // Display the current state.
            lbState.Text = string.Format("Current State: {0}    Detect Time: {1} ",
                e.Reason, DateTime.Now);

            // Record the StateChanged event and add it to the list box.
            lstRecord.Items.Add(string.Format("{0}   {1} \tOccurred", 
                DateTime.Now, e.Reason));

            lstRecord.SelectedIndex = lstRecord.Items.Count - 1;
        }

        private void chkEnableTimer_CheckedChanged(object sender, EventArgs e)
        {
            if (chkEnableTimer.Checked)
            {
                timer.Change(0, 5000);
            }
            else
            {
                timer.Change(0, System.Threading.Timeout.Infinite);
            }
            
        }

        void DetectSessionState(object obj)
        {

            // Check whether the current session is locked.
            bool isCurrentLocked = session.IsLocked();

            var state = isCurrentLocked ? SessionSwitchReason.SessionLock
                : SessionSwitchReason.SessionUnlock;

            // Display the current state.
            lbState.Text = string.Format("Current State: {0}    Time: {1} ",
               state, DateTime.Now);

            //string mag=else.

            // Record the detected result and add it to the list box.
            lstRecord.Items.Add(string.Format("{0}   {1}",
                DateTime.Now, state));

            lstRecord.SelectedIndex = lstRecord.Items.Count - 1;

        }
    }
}
