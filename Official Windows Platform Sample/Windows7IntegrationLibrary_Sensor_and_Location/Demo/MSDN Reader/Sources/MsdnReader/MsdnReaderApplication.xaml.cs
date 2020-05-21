// Copyright (c) Microsoft Corporation.  All rights reserved.

//---------------------------------------------------------------------------
//
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

using System;
using System.IO;
using System.ComponentModel;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Navigation;
using System.Windows.Threading;
using Microsoft.SceReader;
using Microsoft.SceReader.Data;
using Microsoft.Win32;
using Microsoft.SubscriptionCenter.Sync;
using System.Xml.XPath;
using System.Windows.Documents;

namespace MsdnReader
{
    public partial class MsdnReaderApplication : Application
    {
        #region Protected Methods

        /// <summary>
        /// On startup, perform initialization of services and settings, and register for appropriate notifications from
        /// SceReader services
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            Initialize();
            ProcessCommandLineArgs();
            StartDataLoad();
            RegisterConverters();
            base.OnStartup(e);
        }

        /// <summary>
        /// On application exit, saves current settings to use for next instance. Shuts down subscription service manager so
        /// communication channels between the application and subscription sync service can be shut down and resources freed
        /// </summary>
        /// <param name="e"></param>
        protected override void OnExit(ExitEventArgs e)
        {
            MsdnReader.Properties.Settings.Default.Save();

            // Dispose resources associated with DataManager.
            MsdnServiceProvider.DataManager.Dispose();

            // Shut down remoting services with subscription center
            MsdnServiceProvider.SubscriptionServiceManager.Shutdown();

            // Detach power manager from monitoring power broadcasts
            DetachPowerManager();

            // persist which stories have already been read by user:
            ((MsdnViewManager)MsdnServiceProvider.ViewManager).SaveStoryReadState(false);

            base.OnExit(e);
        }

        /// <summary>
        /// LoadCompleted override registers for power management services on main window
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoadCompleted(NavigationEventArgs e)
        {
            if (this.MainWindow != null)
            {
                InitializePowerManager();
            }
            base.OnLoadCompleted(e);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes settings and sample service provider. Initializes subscription service manager which initiates communication
        /// with the subscription sync service
        /// </summary>
        private void Initialize()
        {
            SceReaderSettings.Initialize(new MsdnReaderSettings());
            ServiceProvider.Initialize(new MsdnServiceProvider());

            // Initialize communication with subscription service
            MsdnServiceProvider.SubscriptionServiceManager.Initialize();
        }

        /// <summary>
        /// Processes command line args passed to the application
        /// </summary>
        private void ProcessCommandLineArgs()
        {
            ServiceProvider.ViewManager.ProcessCommandLineArgs(SingleInstance.GetCommandLineArgs());
        }


        /// <summary>
        /// Starts data load from cache or from design data, if enabled
        /// </summary>
        private void StartDataLoad()
        {
            bool loadFromCache = true;

#if DEBUG
            if (SceReaderSettings.UseDesignFeedInDebug)
            {
                loadFromCache = false;
            }
#endif

            if (loadFromCache)
            {
                // Begin by loading data from the cache
                // Initiate auto-sync after first data update is completed.
                // Upload log after any update is completed
                _updateAfterCachedLoad = true;
                ServiceProvider.DataManager.LoadCachedDataCompleted += OnDataManagerLoadCachedDataCompleted;
                Dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(StartCacheLoadCallback), null);
            }
            else
            {
                // Initiate auto-sync
                // Upload log after any update is completed
                _updateAfterCachedLoad = false;
                InitiateUpdate();
            }
        }

        /// <summary>
        /// Registers custom converters used by the application, sets up the FlowDocumentStyleProvider, etc.
        /// </summary>
        private void RegisterConverters()
        {
            // Set application's FlowDocumentStyleProvider as the source for flow document styling in the converter manager
            FlowDocumentStyleProvider styleProvider = Application.Current.TryFindResource("StoryFlowDocumentStyleProvider") as FlowDocumentStyleProvider;
            if (styleProvider != null)
            {
                ServiceProvider.ConverterManager.FlowDocumentStyleProvider = styleProvider;
            }

            MsdnEmailConverter emailConverter = new MsdnEmailConverter();
            ServiceProvider.ConverterManager.EmailConverter = emailConverter;
            ServiceProvider.ConverterManager.UnRegister(typeof(Story), typeof(FlowDocument));
            ServiceProvider.ConverterManager.Register(typeof(Story), typeof(FlowDocument), new MsdnStoryToFlowDocumentConverter());            
 
        }

        /// <summary>
        /// If power manager for the application is null, create one and attach it to the main window to listen for power broadcasts
        /// </summary>
        private void InitializePowerManager()
        {
            if (_powerManager == null && this.MainWindow != null)
            {
                // Find main window's handle to attach power manager
                _powerManager = new PowerManager();
                if (this.MainWindowHandle != IntPtr.Zero)
                {
                    _powerManager.AttachHwnd(this.MainWindowHandle, ServiceProvider.Logger);
                }
                _powerManager.SystemSuspending += OnPowerManagerSystemSuspending;
                _powerManager.SystemResumed += OnPowerManagerSystemResumed;
            }
        }

        /// <summary>
        /// Removes PowerManager and discontinues its monitoring of power broadcasts
        /// </summary>
        private void DetachPowerManager()
        {
            if (_powerManager != null)
            {
                _powerManager.SystemSuspending -= OnPowerManagerSystemSuspending;
                _powerManager.SystemResumed -= OnPowerManagerSystemResumed;
                _powerManager.DetachHwnd();
                _powerManager.Dispose();
                _powerManager = null;
            }
        }

        /// <summary>
        /// Event handler for DataManager's LoadCachedDataCompleted event. After cached load completes, if the application is not
        /// displaying design mode data, a full data update is initiated
        /// After the first cached load initiate automatic data update process
        /// </summary>
        private void OnDataManagerLoadCachedDataCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (_updateAfterCachedLoad)
            {
                // On startup the applicaiton automatically loads from cache.  After the first cached load, 
                // a full sync is initiated to get most recent data. Subsequent cached loads should not repeat this
                _updateAfterCachedLoad = false;
                InitiateUpdate();


                // restore which stories have previously read by user:
                ((MsdnViewManager)MsdnServiceProvider.ViewManager).LoadStoryReadState();
            }
        }

        /// <summary>
        /// Event handler for PowerManager's SystemSuspending event - queues dispatcher item to perform system suspending work
        /// </summary>
        private void OnPowerManagerSystemSuspending(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Send, new DispatcherOperationCallback(SystemSuspendingCallback), null);
        }

        /// <summary>
        /// Event handler for PowerManager's SystemResuming event - queues dispatcher item to perform system resumed work
        /// </summary>
        private void OnPowerManagerSystemResumed(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Send, new DispatcherOperationCallback(SystemResumingCallback), null);
        }

        /// <summary>
        /// Queue dispatcher item to start data update process
        /// </summary>
        private void InitiateUpdate()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new DispatcherOperationCallback(StartDataUpdateCallback), null);
        }

        /// <summary>
        /// Callback worker that initiates async data update
        /// </summary>
        private object StartDataUpdateCallback(object arg)
        {
            // Execute applications's start sync command, which checks that no subscription service update is in progress before starting
            // data update
            if (((MsdnViewManager)MsdnServiceProvider.ViewManager).MsdnCommands.MsdnStartSyncCommand.CanExecute(null))
            {
                ((MsdnViewManager)MsdnServiceProvider.ViewManager).MsdnCommands.MsdnStartSyncCommand.Execute(null);
            }
            return null;
        }

        /// <summary>
        /// Callback worker that starts the async data load from cache
        /// </summary>
        private object StartCacheLoadCallback(object arg)
        {
            ServiceProvider.DataManager.LoadCachedDataAsync();
            return null;
        }

        /// <summary>
        /// Callback worker for system suspending event. Cancel sync and terminate communication with subscription service
        /// </summary>
        private object SystemSuspendingCallback(object arg)
        {
            MsdnServiceProvider.SubscriptionServiceManager.Shutdown();
            if (ServiceProvider.DataManager.IsUpdateInProgress)
            {
                ServiceProvider.DataManager.CancelAsync(null);
            }
            return null;
        }

        /// <summary>
        /// Callback worker for system resuming event, restart remote communication with subscription service
        /// </summary>
        private object SystemResumingCallback(object arg)
        {
            MsdnServiceProvider.SubscriptionServiceManager.Initialize();
            return null;
        }

        #endregion

        #region Private Properties

        /// <summary>
        /// Handle for application's main window, required for power notification registration
        /// </summary>
        private IntPtr MainWindowHandle
        {
            get
            {
                IntPtr handle = IntPtr.Zero;
                if (this.MainWindow != null)
                {
                    WindowInteropHelper helper = new WindowInteropHelper(this.MainWindow);
                    handle = helper.Handle;
                }
                return handle;
            }
        }

        #endregion

        #region Private Fields

        private bool _updateAfterCachedLoad;
        private PowerManager _powerManager;

        #endregion
    }
}