/* 
    Copyright (c) 2011 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Threading;
using System.IO;

using System.IO.IsolatedStorage;


namespace sdkSocketsCS
{
    public partial class App : Application
    {
        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {

                // Use #if DEBUG so that the below counters only show up on the
                // screen in the Debug build.
#if DEBUG
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;
#endif

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // Caution:- Use this under debug mode only. Application that disable user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {

        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            if (e.IsApplicationInstancePreserved)
            {
                return;
            }

            // Check to see if the key for the application state data is in the State dictionary.
            if (PhoneApplicationService.Current.State.ContainsKey("ApplicationDataObject"))
            {
                // If it exists, assign the data to the application member variable.
                string data = PhoneApplicationService.Current.State["ApplicationDataObject"] as string;
                DeSerializeSettings(data);
            }

        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            // Serialize the settings into a string that can be easily stored
            string settings = SerializeSettings();

            // Store it in the State dictionary.
            PhoneApplicationService.Current.State["ApplicationDataObject"] = settings;

            // Also store it in Isolated Storage, in case the application is never reactivated.
            SaveDataToIsolatedStorage("myDataFile.txt", settings);

        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            // The application will not be tombstoned, so only save to Isolated Storage

            // Serialize the settings into a string that can be easily stored
            string settings = SerializeSettings();

            // Also store it in Isolated Storage, in case the application is never reactivated.
            SaveDataToIsolatedStorage("myDataFile.txt", settings);

        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion

        #region Settings Management
        // Declare an event for when the application data changes.
        public event EventHandler ApplicationDataObjectChanged;

        // Declare a private variable to store the host (server) name
        private string _hostName;

        // Declare a public property to access the application data variable.
        public string HostName
        {
            get { return _hostName; }
            set
            {
                if (value != _hostName)
                {
                    _hostName = value;
                    OnApplicationDataObjectChanged(EventArgs.Empty);
                }
            }
        }

        // Declare a private variable to store the port number used by the application
        // NOTE: The port number in this application and the port number in the server must match.
        // Remember to open the port you choose on the computer running the server.
        private int _portNumber = 0;

        // Declare a public property to access the application data variable.
        public int PortNumber
        {
            get { return _portNumber; }
            set
            {
                if (value != _portNumber)
                {
                    _portNumber = value;
                    OnApplicationDataObjectChanged(EventArgs.Empty);
                }
            }
        }

        // Declare a private variable to store whether the client is playing a
        // 'X' or as 'O'
        private bool _playAsX = true;

        // Declare a public property to access the application data variable.
        public bool PlayAsX
        {
            get { return _playAsX; }
            set
            {
                if (value != _playAsX)
                {
                    _playAsX = value;
                    OnApplicationDataObjectChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Produce a string that contains all the settings used by the application
        /// </summary>
        /// <returns>The string containing the serialized data</returns>
        private string SerializeSettings()
        {
            // This string will be of the form
            // PlayAsX|Hostname|PortNumber

            string result = string.Empty;

            result += _playAsX.ToString();
            result += "|";
            result += _hostName;
            result += "|";
            result += _portNumber.ToString();

            return result;
        }

        /// <summary>
        /// Given a string of serialized settings, re-hydrate each settings variable
        /// </summary>
        /// <param name="data">The string of serialized settings</param>
        private void DeSerializeSettings(string data)
        {
            // Split the string using the '|' delimiter
            string[] values = data.Split("|".ToCharArray(), StringSplitOptions.None);

            // The string is of the form
            // PlayAsX|Hostname|PortNumber

            PlayAsX = Convert.ToBoolean(values[0]);
            HostName = values[1];
            PortNumber = Convert.ToInt32(values[2]);
        }

        // Create a method to raise the ApplicationDataObjectChanged event.
        protected void OnApplicationDataObjectChanged(EventArgs e)
        {
            EventHandler handler = ApplicationDataObjectChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Save data to a file in IsolatedStorage
        /// </summary>
        /// <param name="isoFileName">The name of the file</param>
        /// <param name="value">The string value to write to the given file</param>
        private void SaveDataToIsolatedStorage(string isoFileName, string value)
        {
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                StreamWriter sw = new StreamWriter(isoStore.OpenFile(isoFileName, FileMode.Create));
                sw.Write(value);
                sw.Close();
            }
        }

        /// <summary>
        /// Call GetData on a different thread, i.e, asynchronously
        /// </summary>
        public void GetDataAsync()
        {
            // Call the GetData method on a new thread.
            Thread t = new Thread(new ThreadStart(GetData));
            t.Start();
        }

        /// <summary>
        /// Retrieve data from IsolatedStorage
        /// </summary>
        private void GetData()
        {
            // Check to see if data exists in Isolated Storage 
            IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();
            if (isoStore.FileExists("myDataFile.txt"))
            {
                // This method loads the data from Isolated Storage, if it is available.
                StreamReader sr = new StreamReader(isoStore.OpenFile("myDataFile.txt", FileMode.Open));
                string data = sr.ReadToEnd();

                sr.Close();

                DeSerializeSettings(data);
            }
        }
        #endregion
    }
}
