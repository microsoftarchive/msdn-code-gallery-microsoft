//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using SDKTemplate;
using Windows.UI.Xaml.Controls;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Json;
using Windows.Devices.Geolocation;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace Microsoft.Samples.Devices.Geolocation
{
    public sealed partial class Scenario5 : Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        private IBackgroundTaskRegistration geofenceTask = null;
        private CancellationTokenSource cts = null;
        private ItemCollection geofenceBackgroundEvents = null;
        private Geolocator geolocator = null;
        private const long oneHundredNanosecondsPerSecond = 10000000;    // conversion from 100 nano-second resolution to seconds

        private class ItemCollection : System.Collections.ObjectModel.ObservableCollection<string>
        {
        }

        private const string SampleBackgroundTaskName = "SampleGeofenceBackgroundTask";
        private const string SampleBackgroundTaskEntryPoint = "BackgroundTask.GeofenceBackgroundTask";

        public Scenario5()
        {
            this.InitializeComponent();

#if WINDOWS_APP
            GeofenceBackgroundEventsListBox.Width = 480;
            GeofenceBackgroundEventsListBox.Height = 240;
#endif
            var settings = ApplicationData.Current.LocalSettings;

            // Get a geolocator object
            geolocator = new Geolocator();

            geofenceBackgroundEvents = new ItemCollection();

            // using data binding to the root page collection of GeofenceItems associated with events
            GeofenceBackgroundEventsListBox.DataContext = geofenceBackgroundEvents;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached. The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Loop through all background tasks to see if SampleGeofenceBackgroundTask is already registered
            foreach (var cur in BackgroundTaskRegistration.AllTasks)
            {
                if (cur.Value.Name == SampleBackgroundTaskName)
                {
                    geofenceTask = cur.Value;
                    break;
                }
            }

            if (geofenceTask != null)
            {
                FillEventListBoxWithExistingEvents();
                
                // Associate an event handler with the existing background task
                geofenceTask.Completed += new BackgroundTaskCompletedEventHandler(OnCompleted);

                try
                {
                    BackgroundAccessStatus backgroundAccessStatus = BackgroundExecutionManager.GetAccessStatus();

                    switch (backgroundAccessStatus)
                    {
                        case BackgroundAccessStatus.Unspecified:
                        case BackgroundAccessStatus.Denied:
#if WINDOWS_APP
                            rootPage.NotifyUser("This application must be added to the lock screen before the background task will run.", NotifyType.ErrorMessage);
#else
                            rootPage.NotifyUser("Not able to run in background.", NotifyType.ErrorMessage);
#endif
                            break;

                        default:
                            rootPage.NotifyUser("Background task is already registered. Waiting for next update...", NotifyType.ErrorMessage);
                            break;
                    }
                }
                catch (Exception ex)
                {
#if WINDOWS_APP
                    // HRESULT_FROM_WIN32(ERROR_NOT_SUPPORTED) == 0x80070032
                    const int RequestNotSupportedHResult = unchecked((int)0x80070032);

                    if (ex.HResult == RequestNotSupportedHResult)
                    {
                        rootPage.NotifyUser("Location Simulator not supported.  Could not determine lock screen status, be sure that the application is added to the lock screen.", NotifyType.StatusMessage);
                    }
                    else
#endif
                    {
                        rootPage.NotifyUser(ex.ToString(), NotifyType.ErrorMessage);
                    }
                }

                UpdateButtonStates(/*registered:*/ true);
            }
            else
            {
                UpdateButtonStates(/*registered:*/ false);
            }
        }

        /// <summary>
        /// Invoked immediately before the Page is unloaded and is no longer the current source of a parent Frame.
        /// </summary>
        /// <param name="e">
        /// Event data that can be examined by overriding code. The event data is representative
        /// of the navigation that will unload the current Page unless canceled. The
        /// navigation can potentially be canceled by setting e.Cancel to true.
        /// </param>
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            // Just in case the original GetGeopositionAsync call is still active
            CancelGetGeoposition();

            base.OnNavigatingFrom(e);
        }

        /// <summary>
        /// This is the click handler for the 'Register' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async private void RegisterBackgroundTask(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get permission for a background task from the user. If the user has already answered once,
                // this does nothing and the user must manually update their preference via PC Settings.
                BackgroundAccessStatus backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();

                // Regardless of the answer, register the background task. If the user later adds this application
                // to the lock screen, the background task will be ready to run.
                // Create a new background task builder
                BackgroundTaskBuilder geofenceTaskBuilder = new BackgroundTaskBuilder();

                geofenceTaskBuilder.Name = SampleBackgroundTaskName;
                geofenceTaskBuilder.TaskEntryPoint = SampleBackgroundTaskEntryPoint;

                // Create a new location trigger
                var trigger = new LocationTrigger(LocationTriggerType.Geofence);

                // Associate the locationi trigger with the background task builder
                geofenceTaskBuilder.SetTrigger(trigger);

                // If it is important that there is user presence and/or
                // internet connection when OnCompleted is called
                // the following could be called before calling Register()
                // SystemCondition condition = new SystemCondition(SystemConditionType.UserPresent | SystemConditionType.InternetAvailable);
                // geofenceTaskBuilder.AddCondition(condition);

                // Register the background task
                geofenceTask = geofenceTaskBuilder.Register();

                // Associate an event handler with the new background task
                geofenceTask.Completed += new BackgroundTaskCompletedEventHandler(OnCompleted);

                UpdateButtonStates(/*registered:*/ true);

                switch (backgroundAccessStatus)
                {
                    case BackgroundAccessStatus.Unspecified:
                    case BackgroundAccessStatus.Denied:
#if WINDOWS_APP
                        rootPage.NotifyUser("This application must be added to the lock screen before the background task will run.", NotifyType.ErrorMessage);
#else
                        rootPage.NotifyUser("Not able to run in background.", NotifyType.ErrorMessage);
#endif
                        break;

                    default:
                        // Ensure we have presented the location consent prompt (by asynchronously getting the current
                        // position). This must be done here because the background task cannot display UI.
                        GetGeopositionAsync();
                        break;
                }
            }
            catch (Exception ex)
            {
#if WINDOWS_APP
                // HRESULT_FROM_WIN32(ERROR_NOT_SUPPORTED) == 0x80070032
                const int RequestNotSupportedHResult = unchecked((int)0x80070032);

                if (ex.HResult == RequestNotSupportedHResult)
                {
                    rootPage.NotifyUser("Location Simulator not supported.  Could not get permission to add application to the lock screen, this application must be added to the lock screen before the background task will run.", NotifyType.StatusMessage);
                }
                else
#endif
                {
                    rootPage.NotifyUser(ex.ToString(), NotifyType.ErrorMessage);
                }

                UpdateButtonStates(/*registered:*/ false);
            }
        }

        /// <summary>
        /// This is the click handler for the 'Unregister' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UnregisterBackgroundTask(object sender, RoutedEventArgs e)
        {
            // Unregister the background task
            if (null != geofenceTask)
            {
                geofenceTask.Unregister(true);
                geofenceTask = null;
            }

            rootPage.NotifyUser("Background task unregistered", NotifyType.StatusMessage);

            UpdateButtonStates(/*registered:*/ false);
        }

        /// <summary>
        /// Helper method to invoke Geolocator.GetGeopositionAsync.
        /// </summary>
        async private void GetGeopositionAsync()
        {
            rootPage.NotifyUser("Checking permissions...", NotifyType.StatusMessage);

            try
            {
                // Get cancellation token
                cts = new CancellationTokenSource();
                CancellationToken token = cts.Token;

                // Carry out the operation
                Geoposition pos = await geolocator.GetGeopositionAsync().AsTask(token);

                // got permissions so clear the status string
                rootPage.NotifyUser("", NotifyType.StatusMessage);
            }
#if WINDOWS_APP
            catch (UnauthorizedAccessException)
            {
                rootPage.NotifyUser("Location Permissions disabled by user. Enable access through the settings charm to enable the background task.", NotifyType.StatusMessage);
            }
#endif
            catch (TaskCanceledException)
            {
                rootPage.NotifyUser("Permission check operation canceled.", NotifyType.StatusMessage);
            }
            catch (Exception ex)
            {
#if WINDOWS_APP
                // If there are no location sensors GetGeopositionAsync()
                // will timeout -- that is acceptable.
                const int WaitTimeoutHResult = unchecked((int)0x80070102);

                if (ex.HResult == WaitTimeoutHResult) // WAIT_TIMEOUT
                {
                    rootPage.NotifyUser("Operation accessing location sensors timed out. Possibly there are no location sensors.", NotifyType.StatusMessage);
                }
                else
#endif
                {
                    rootPage.NotifyUser(ex.ToString(), NotifyType.ErrorMessage);
                }
            }
            finally
            {
                cts = null;
            }
        }

        /// <summary>
        /// Helper method to cancel the GetGeopositionAsync request (if any).
        /// </summary>
        private void CancelGetGeoposition()
        {
            if (cts != null)
            {
                cts.Cancel();
                cts = null;
            }
        }

        /// <summary>
        /// This is the callback when background event has been handled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async private void OnCompleted(IBackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs e)
        {
            if (sender != null)
            {
                // Update the UI with progress reported by the background task
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    try
                    {
                        // If the background task threw an exception, display the exception in
                        // the error text box.
                        e.CheckResult();

                        // Update the UI with the completion status of the background task
                        // The Run method of the background task sets the LocalSettings. 
                        var settings = ApplicationData.Current.LocalSettings;

                        // get status
                        if (settings.Values.ContainsKey("Status"))
                        {
                            rootPage.NotifyUser(settings.Values["Status"].ToString(), NotifyType.StatusMessage);
                        }

                        FillEventListBoxWithExistingEvents();
                    }
                    catch (Exception ex)
                    {
                        // The background task had an error
                        rootPage.NotifyUser(ex.ToString(), NotifyType.ErrorMessage);
                    }
                });
            }
        }

        /// <summary>
        /// Update the enable state of the register/unregister buttons.
        /// </summary>
        /// 
        private async void UpdateButtonStates(bool registered)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    RegisterBackgroundTaskButton.IsEnabled = !registered;
                    UnregisterBackgroundTaskButton.IsEnabled = registered;
                });
        }

        /// <summary>
        /// Populate events list box with entries from BackgroundGeofenceEventCollection.
        /// </summary>
        /// 
        private void FillEventListBoxWithExistingEvents()
        {
            var settings = ApplicationData.Current.LocalSettings;
            if (settings.Values.ContainsKey("BackgroundGeofenceEventCollection"))
            {
                string geofenceEvent = settings.Values["BackgroundGeofenceEventCollection"].ToString();

                if (0 != geofenceEvent.Length)
                {
                    // remove all entries and repopulate
                    geofenceBackgroundEvents.Clear();

                    var events = JsonValue.Parse(geofenceEvent).GetArray();

                    // NOTE: the events are accessed in reverse order
                    // because the events were added to JSON from
                    // newer to older.  geofenceBackgroundEvents.Insert() adds
                    // each new entry to the beginning of the collection.
                    for (int pos = events.Count - 1; pos >= 0; pos--)
                    {
                        var element = events.GetStringAt((uint)pos);
                        geofenceBackgroundEvents.Insert(0, element);
                    }
                }
            }
        }
    }
}

