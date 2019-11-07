/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Microsoft.Phone.Networking.Voip;
using PhoneVoIPApp.BackEnd;
using PhoneVoIPApp.BackEnd.OutOfProcess;

namespace PhoneVoIPApp.UI
{
    /// <summary>
    /// A class used by the VoIP UI to connect to and control the VoIP background agent host process.
    /// </summary>
    /// <remarks>This class is a singleton.</remarks>
    public sealed class BackgroundProcessController
    {
        #region Properties

        /// <summary>
        /// Get the single instance of this class
        /// </summary>
        public static BackgroundProcessController Instance
        {
            get
            {
                if (BackgroundProcessController.singleton == null)
                {
                    BackgroundProcessController.singleton = new BackgroundProcessController();
                }

                return BackgroundProcessController.singleton;
            }
        }

        /// <summary>
        /// Get the object that can be used to create and control VoIP calls.
        /// </summary>
        /// <remarks>The returned object is a proxy object to the real call controller
        /// object that exists in the  VoIP background agent host process.</remarks>
        public CallController CallController
        {
            get
            {
                return this.server.CallController;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// A method that lets the VoIP background process know that the UI process is connected to it.
        /// Call this method at the beginning of the Launching and Activated event handlers.
        /// </summary>
        public void ConnectUi()
        {
            if (this.isConnected)
            {
                // Nothing more to be done
                return;
            }

            // Start the VoIP background agent host process, if it is not started already
            int backgroundProcessId;
            try
            {
                VoipBackgroundProcess.Launch(out backgroundProcessId);
            }
            catch (Exception err)
            {
                Debug.WriteLine("[App] Error launching VoIP background process. UI may no longer be in the foreground. Exception: " + err.Message);
                throw;
            }

            // Wait for the background process to become ready
            string backgroundProcessReadyEventName = Globals.GetBackgroundProcessReadyEventName((uint)backgroundProcessId);
            using (EventWaitHandle backgroundProcessReadyEvent = new EventWaitHandle(initialState: false, mode: EventResetMode.ManualReset, name: backgroundProcessReadyEventName))
            {
                TimeSpan timeout = Debugger.IsAttached ? BackgroundProcessController.indefiniteWait : BackgroundProcessController.fifteenSeconds;
                if (!backgroundProcessReadyEvent.WaitOne(timeout))
                {
                    // We timed out - something is wrong
                    throw new InvalidOperationException(string.Format("The background process did not become ready in {0} milliseconds", timeout.Milliseconds));
                }
                else
                {
                    Debug.WriteLine("[App] Background process {0} is ready", backgroundProcessId);
                }
            }
            // The background process is now ready.
            // It is possible that the background process now becomes "not ready" again, but the chances of this happening are slim,
            // and in that case, the following statement would fail - so, at this point, we don't explicitly guard against this condition.
            
            // Create an instance of the server in the background process.
            this.server = (Server)WindowsRuntimeMarshal.GetActivationFactory(typeof(Server)).ActivateInstance();

            // Un-set an event that indicates that the UI process is disconnected from the background process.
            // The VoIP background process waits for this event to get set before shutting down.
            // This ensures that the VoIP background agent host process doesn't shut down while the UI process is connected to it.
            string uiDisconnectedEventName = Globals.GetUiDisconnectedEventName((uint)backgroundProcessId);
            this.uiDisconnectedEvent = new EventWaitHandle(initialState: false, mode: EventResetMode.ManualReset, name: uiDisconnectedEventName);
            this.uiDisconnectedEvent.Reset();

            // The UI process is now connected to the background process
            this.isConnected = true;
        }

        /// <summary>
        /// A method that lets the VoIP background process know that the UI is no longer connected to it.
        /// Call this method at the end of the the Deactivated and Closing event handlers.
        /// </summary>
        public void DisconnectUi()
        {
            if (!this.isConnected)
            {
                // Nothing more to be done
                return;
            }

            // This process is no longer connected to the background process
            this.isConnected = false;

            // From this point onwards, it is no longer safe to use any objects in the VoIP background process,
            // or for the VoIP background process to call back into this process.
            this.server = null;

            // Lastly, set the event that indicates that the UI is no longer connected to the background process.
            if (this.uiDisconnectedEvent == null)
                throw new InvalidOperationException("The ConnectUi method must be called before this method is called");

            this.uiDisconnectedEvent.Set();
            this.uiDisconnectedEvent.Dispose();
            this.uiDisconnectedEvent = null;
        }

        #endregion

        #region Private members

        /// <summary>
        /// Private constructor
        /// </summary>
        private BackgroundProcessController()
        {
            // Nothing to do here
        }

        // A timespan representing fifteen seconds
        private static readonly TimeSpan fifteenSeconds = new TimeSpan(0, 0, 15);

        // A timespan representing an indefinite wait
        private static readonly TimeSpan indefiniteWait = new TimeSpan(0, 0, 0, 0, -1);

        // The single instance of this class
        private static BackgroundProcessController singleton;

        // Indicates if the UI process is in the foreground or not
        private bool isConnected;

        // An event that indicates that the UI process is no longer connected to the background process
        private EventWaitHandle uiDisconnectedEvent;

        // A proxy to the server object in the VoIP background agent host process
        private Server server;

        #endregion
    }
}
