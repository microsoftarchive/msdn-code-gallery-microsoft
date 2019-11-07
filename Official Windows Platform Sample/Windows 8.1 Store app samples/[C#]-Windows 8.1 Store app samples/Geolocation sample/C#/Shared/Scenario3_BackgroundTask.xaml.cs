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
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Microsoft.Samples.Devices.Geolocation
{
    public sealed partial class Scenario3 : Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        private IBackgroundTaskRegistration _geolocTask = null;
        private CancellationTokenSource _cts = null;

        private const string SampleBackgroundTaskName = "SampleLocationBackgroundTask";
        private const string SampleBackgroundTaskEntryPoint = "BackgroundTask.LocationBackgroundTask";

        public Scenario3()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached. The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Loop through all background tasks to see if SampleBackgroundTaskName is already registered
            foreach (var cur in BackgroundTaskRegistration.AllTasks)
            {
                if (cur.Value.Name == SampleBackgroundTaskName)
                {
                    _geolocTask = cur.Value;
                    break;
                }
            }

            if (_geolocTask != null)
            {
                // Associate an event handler with the existing background task
                _geolocTask.Completed += new BackgroundTaskCompletedEventHandler(OnCompleted);

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

            if (_geolocTask != null)
            {
                // Remove the event handler
                _geolocTask.Completed -= new BackgroundTaskCompletedEventHandler(OnCompleted);
            }

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
                BackgroundTaskBuilder geolocTaskBuilder = new BackgroundTaskBuilder();

                geolocTaskBuilder.Name = SampleBackgroundTaskName;
                geolocTaskBuilder.TaskEntryPoint = SampleBackgroundTaskEntryPoint;

                // Create a new timer triggering at a 15 minute interval
                var trigger = new TimeTrigger(15, false);

                // Associate the timer trigger with the background task builder
                geolocTaskBuilder.SetTrigger(trigger);

                // Register the background task
                _geolocTask = geolocTaskBuilder.Register();

                // Associate an event handler with the new background task
                _geolocTask.Completed += new BackgroundTaskCompletedEventHandler(OnCompleted);

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
            if (null != _geolocTask)
            {
                _geolocTask.Unregister(true);
                _geolocTask = null;
            }

            rootPage.NotifyUser("Background task unregistered", NotifyType.StatusMessage);

            ScenarioOutput_Latitude.Text = "No data";
            ScenarioOutput_Longitude.Text = "No data";
            ScenarioOutput_Accuracy.Text = "No data";

            UpdateButtonStates(/*registered:*/ false);
        }

        /// <summary>
        /// Helper method to invoke Geolocator.GetGeopositionAsync.
        /// </summary>
        async private void GetGeopositionAsync()
        {
            try
            {
                // Get cancellation token
                _cts = new CancellationTokenSource();
                CancellationToken token = _cts.Token;

                // Get a geolocator object
                Geolocator geolocator = new Geolocator();

                rootPage.NotifyUser("Getting initial position...", NotifyType.StatusMessage);

                // Carry out the operation
                Geoposition pos = await geolocator.GetGeopositionAsync().AsTask(token);

                rootPage.NotifyUser("Initial position. Waiting for update...", NotifyType.StatusMessage);

                ScenarioOutput_Latitude.Text = pos.Coordinate.Point.Position.Latitude.ToString();
                ScenarioOutput_Longitude.Text = pos.Coordinate.Point.Position.Longitude.ToString();
                ScenarioOutput_Accuracy.Text = pos.Coordinate.Accuracy.ToString();
            }
#if WINDOWS_APP
            catch (UnauthorizedAccessException)
            {
                rootPage.NotifyUser("Disabled by user. Enable access through the settings charm to enable the background task.", NotifyType.StatusMessage);

                ScenarioOutput_Latitude.Text = "No data";
                ScenarioOutput_Longitude.Text = "No data";
                ScenarioOutput_Accuracy.Text = "No data";
            }
#endif
            catch (TaskCanceledException)
            {
                rootPage.NotifyUser("Initial position operation canceled. Waiting for update...", NotifyType.StatusMessage);
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
                _cts = null;
            }
        }

        /// <summary>
        /// Helper method to cancel the GetGeopositionAsync request (if any).
        /// </summary>
        private void CancelGetGeoposition()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts = null;
            }
        }

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
                        // The Run method of the background task sets this status. 
                        var settings = ApplicationData.Current.LocalSettings;
                        if (settings.Values["Status"] != null)
                        {
                            rootPage.NotifyUser(settings.Values["Status"].ToString(), NotifyType.StatusMessage);
                        }

                        // Extract and display Latitude
                        if (settings.Values["Latitude"] != null)
                        {
                            ScenarioOutput_Latitude.Text = settings.Values["Latitude"].ToString();
                        }
                        else
                        {
                            ScenarioOutput_Latitude.Text = "No data";
                        }

                        // Extract and display Longitude
                        if (settings.Values["Longitude"] != null)
                        {
                            ScenarioOutput_Longitude.Text = settings.Values["Longitude"].ToString();
                        }
                        else
                        {
                            ScenarioOutput_Longitude.Text = "No data";
                        }

                        // Extract and display Accuracy
                        if (settings.Values["Accuracy"] != null)
                        {
                            ScenarioOutput_Accuracy.Text = settings.Values["Accuracy"].ToString();
                        }
                        else
                        {
                            ScenarioOutput_Accuracy.Text = "No data";
                        }
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
    }
}

