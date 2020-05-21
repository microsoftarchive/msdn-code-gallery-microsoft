// Copyright (c) Microsoft Corporation.  All rights reserved.

//---------------------------------------------------------------------------
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.
//
//---------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Windows.Navigation;

namespace MsdnReader
{
    public static class SceReaderMain
    {
        /// <summary>
        /// Application Entry Point.
        /// </summary>
        /// <remarks>
        /// MsdnReader contains an explicit Main method instead of one implicitly defined in the
        /// application code to bring up a splash screen while the application starts.
        /// </remarks>
        [System.STAThreadAttribute()]
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public static void Main()
        {
            Microsoft.SceReader.SceReaderSettings.Initialize(new MsdnReaderSettings());

            // MsdnReader is a single instance application; the splash screen and application object are only
            // created for the first instance. Single instancing is controlled by the SingleInstance class which manages
            // a mutex for the application
            if (SingleInstance.InitializeAsFirstInstance())
            {
                // Open splash screen
                _splash = new SplashScreen();
                _splash.Open();

                // Start main application
                StartApplication();
            }
        }

        /// <summary>
        /// Creates and runs a new instance of the MsdnReaderApplication. Called when single instance code has
        /// established that there is no other instance of the application currently running
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void StartApplication()
        {
            MsdnReaderApplication application = new MsdnReaderApplication();

            // Listen for application's load completed and exit events, to detect when splash screen can be closed
            application.LoadCompleted += OnApplicationLoadCompleted;
            application.Exit += OnApplicationExit;

            application.InitializeComponent();
            application.Run();

            // Allow single instance code to perform cleanup operations
            SingleInstance.Cleanup();
        }

        /// <summary>
        /// EventHandler for application's LoadCompleted event. This is used to access the application's MainWindow object
        /// and listen for it's ContentRendered event. When main window content is rendered, splash screeen can be closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnApplicationLoadCompleted(object sender, NavigationEventArgs e)
        {
            // Check event args for navigator, which is the application's MainWindow
            NavigationWindow window = e.Navigator as NavigationWindow;
            if (window != null)
            {
                window.ContentRendered += OnMainWindowContentRendered;
            }
        }

        /// <summary>
        /// Event handler for application's exit event. Splash screen needs to be closed here in case application exits before
        /// main window content has rendered
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnApplicationExit(object sender, ExitEventArgs e)
        {
            CloseSplashScreen();
        }

        /// <summary>
        /// EventHandler for application's main window content rendered event. When window content is rendered, splash screen can be
        /// closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnMainWindowContentRendered(object sender, EventArgs e)
        {
            CloseSplashScreen();
        }

        /// <summary>
        /// Closes splash screen and frees its resources
        /// </summary>
        private static void CloseSplashScreen()
        {
            if (_splash != null)
            {
                _splash.Close();
                _splash = null;
            }
        }

        private static SplashScreen _splash;
    }
}