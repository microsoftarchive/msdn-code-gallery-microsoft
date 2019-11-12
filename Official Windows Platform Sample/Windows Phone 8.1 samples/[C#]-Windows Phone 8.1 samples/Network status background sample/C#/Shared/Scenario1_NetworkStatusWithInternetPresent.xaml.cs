//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using SDKTemplate;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Networking.Connectivity;

namespace NetworkStatusApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NetworkStatusWithInternetPresent : Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;
        private CoreDispatcher NetworkStatusWithInternetPresentDispatcher;

        //Save current internet profile and network adapter ID globally
        string internetProfile = "Not connected to Internet";
        string networkAdapter = "Not connected to Internet";


        public NetworkStatusWithInternetPresent()
        {
            this.InitializeComponent();
            NetworkStatusWithInternetPresentDispatcher = Window.Current.CoreWindow.Dispatcher;

        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == BackgroundTaskSample.SampleBackgroundTaskWithConditionName)
                {
                    //Associate background task completed event handler with background task
                    var isTaskRegistered = RegisterCompletedHandlerforBackgroundTask(task.Value);
                    UpdateButton(isTaskRegistered);
                }
            }

        }

#if WINDOWS_PHONE_APP
        private async void RequestBackgroundAccess()
        {
            await BackgroundExecutionManager.RequestAccessAsync();
        }
#endif

        /// <summary>
        /// Register a SampleBackgroundTaskWithCondition.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Save current internet profile and network adapter ID globally
                var connectionProfile = NetworkInformation.GetInternetConnectionProfile();
                if (connectionProfile != null)
                {
                    internetProfile = connectionProfile.ProfileName;
                    var networkAdapterInfo = connectionProfile.NetworkAdapter;
                    if (networkAdapterInfo != null)
                    {
                        networkAdapter = networkAdapterInfo.NetworkAdapterId.ToString();
                    }
                    else
                    {
                        networkAdapter = "Not connected to Internet";
                    }
                }
                else
                {
                    internetProfile = "Not connected to Internet";
                    networkAdapter = "Not connected to Internet";
                }

#if WINDOWS_PHONE_APP
                // On Windows Phone, we need to request access from the BackgroundExecutionManager in order for our background task to run
                RequestBackgroundAccess();
#endif

                var task = BackgroundTaskSample.RegisterBackgroundTask(BackgroundTaskSample.SampleBackgroundTaskEntryPoint,
                                                                       BackgroundTaskSample.SampleBackgroundTaskWithConditionName,
                                                                       new SystemTrigger(SystemTriggerType.NetworkStateChange, false),
                                                                       new SystemCondition(SystemConditionType.InternetAvailable));

                //Associate background task completed event handler with background task.
                task.Completed += new BackgroundTaskCompletedEventHandler(OnCompleted);
                rootPage.NotifyUser("Registered for NetworkStatusChange background task with Internet present condition\n", NotifyType.StatusMessage);
                UpdateButton(true);
            }
            catch (Exception ex)
            {
                rootPage.NotifyUser(ex.ToString(), NotifyType.ErrorMessage);
            }
        }

        /// <summary>
        /// Unregister a SampleBackgroundTaskWithCondition.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UnregisterButton_Click(object sender, RoutedEventArgs e)
        {
            BackgroundTaskSample.UnregisterBackgroundTasks(BackgroundTaskSample.SampleBackgroundTaskWithConditionName);
            rootPage.NotifyUser("Unregistered for NetworkStatusChange background task with Internet present condition\n", NotifyType.StatusMessage);
            UpdateButton(false);
        }

        /// <summary>
        /// Register completion handler for registered background task on application startup.
        /// </summary>
        /// <param name="task">The task to attach progress and completed handlers to.</param>
        private bool RegisterCompletedHandlerforBackgroundTask(IBackgroundTaskRegistration task)
        {
            bool taskRegistered = false;
            try
            {
                //Associate background task completed event handler with background task.
                task.Completed += new BackgroundTaskCompletedEventHandler(OnCompleted);
                taskRegistered = true;
            }
            catch (Exception ex)
            {
                rootPage.NotifyUser(ex.ToString(), NotifyType.ErrorMessage);
            }
            return taskRegistered;
        }

        /// <summary>
        /// Handle background task completion.
        /// </summary>
        /// <param name="task">The task that is reporting completion.</param>
        /// <param name="e">Arguments of the completion report.</param>
        private async void OnCompleted(IBackgroundTaskRegistration task, BackgroundTaskCompletedEventArgs args)
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

            Object profile = localSettings.Values["InternetProfile"];
            Object adapter = localSettings.Values["NetworkAdapterId"];

            Object hasNewConnectionCost = localSettings.Values["HasNewConnectionCost"];
            Object hasNewDomainConnectivityLevel = localSettings.Values["HasNewDomainConnectivityLevel"];
            Object hasNewHostNameList = localSettings.Values["HasNewHostNameList"];
            Object hasNewInternetConnectionProfile = localSettings.Values["HasNewInternetConnectionProfile"];
            Object hasNewNetworkConnectivityLevel = localSettings.Values["HasNewNetworkConnectivityLevel"];

            await NetworkStatusWithInternetPresentDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                string outputText = string.Empty;

                if ((profile != null) && (adapter != null))
                {
                    //If internet profile has changed, display the new internet profile
                    if ((string.Equals(profile.ToString(), internetProfile, StringComparison.Ordinal) == false) ||
                            (string.Equals(adapter.ToString(), networkAdapter, StringComparison.Ordinal) == false))
                    {
                        internetProfile = profile.ToString();
                        networkAdapter = adapter.ToString();
                        outputText += "Internet Profile changed\n" + "=================\n" + "Current Internet Profile : " + internetProfile + "\n\n";

                        if (hasNewConnectionCost != null)
                        {
                            outputText += hasNewConnectionCost.ToString() + "\n";
                        }
                        if (hasNewDomainConnectivityLevel != null)
                        {
                            outputText += hasNewDomainConnectivityLevel.ToString() + "\n";
                        }
                        if (hasNewHostNameList != null)
                        {
                            outputText += hasNewHostNameList.ToString() + "\n";
                        }
                        if (hasNewInternetConnectionProfile != null)
                        {
                            outputText += hasNewInternetConnectionProfile.ToString() + "\n";
                        }
                        if (hasNewNetworkConnectivityLevel != null)
                        {
                            outputText += hasNewNetworkConnectivityLevel.ToString() + "\n";
                        }

                        rootPage.NotifyUser(outputText, NotifyType.StatusMessage);
                    }
                }
            });
        }

        /// <summary>
        /// Update the Register/Unregister button.
        /// </summary>
        private void UpdateButton(bool registered)
        {
            RegisterButton.IsEnabled = !registered;
            UnregisterButton.IsEnabled = registered;
        }

    }
}

