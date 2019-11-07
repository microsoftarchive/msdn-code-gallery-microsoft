// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.PushNotifications;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace SDKTemplate
{
    /// <summary>
    /// Registering a background task and notification channel for raw notifications
    /// </summary>
    public sealed partial class Scenario1 : Page
    {        
        private MainPage rootPage = null;
        private const string SAMPLE_TASK_NAME = "SampleBackgroundTask";
        private const string SAMPLE_TASK_ENTRY_POINT = "BackgroundTasks.SampleBackgroundTask";
        private CoreDispatcher _dispatcher;

        public Scenario1()
        {
            this.InitializeComponent();
            _dispatcher = Window.Current.Dispatcher;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {            
            rootPage = MainPage.Current;
        }

        void OutputToTextBox(String text)
        {
            // Write text
            Scenario1ChannelOutput.Text = text;
        }

        private async void Scenario1Open_Click(object sender, RoutedEventArgs e)
        {
            // Applications must have lock screen privileges in order to receive raw notifications
            BackgroundAccessStatus backgroundStatus = await BackgroundExecutionManager.RequestAccessAsync();

            // Make sure the user allowed privileges
            if (backgroundStatus != BackgroundAccessStatus.Denied && backgroundStatus != BackgroundAccessStatus.Unspecified)
            {
                OpenChannelAndRegisterTask();
            }
            else
            {
                // This event comes back in a background thread, so we need to move to the UI thread to access any UI elements
                await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                    rootPage.NotifyUser("Lock screen access is denied", NotifyType.ErrorMessage);
                });
            }
        }

        private void Scenario1Unregister_Click(object sender, RoutedEventArgs e)
        {
            if (UnregisterBackgroundTask())
            {
                rootPage.NotifyUser("Task unregistered", NotifyType.StatusMessage);
            }
            else
            {
                rootPage.NotifyUser("No task is registered", NotifyType.ErrorMessage);
            }
        }

        private async void OpenChannelAndRegisterTask()
        {
            // Open the channel. See the "Push and Polling Notifications" sample for more detail
            try
            {
                if (rootPage.Channel == null) 
                {
                    PushNotificationChannel channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
                    string uri = channel.Uri;
                    rootPage.Channel = channel;
                    // This event comes back in a background thread, so we need to move to the UI thread to access any UI elements
                    await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                        OutputToTextBox(uri);
                        rootPage.NotifyUser("Channel request succeeded!", NotifyType.StatusMessage);
                    });
                }

                // Clean out the background task just for the purpose of this sample
                UnregisterBackgroundTask();
                RegisterBackgroundTask();
                rootPage.NotifyUser("Task registered", NotifyType.StatusMessage);
            }
            catch (Exception ex)
            {
                rootPage.NotifyUser("Could not create a channel. Error number:" + ex.Message, NotifyType.ErrorMessage);
            }
        }

        private void RegisterBackgroundTask()
        {
            BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder();
            PushNotificationTrigger trigger = new PushNotificationTrigger();
            taskBuilder.SetTrigger(trigger);

            // Background tasks must live in separate DLL, and be included in the package manifest
            // Also, make sure that your main application project includes a reference to this DLL
            taskBuilder.TaskEntryPoint = SAMPLE_TASK_ENTRY_POINT;
            taskBuilder.Name = SAMPLE_TASK_NAME;

            try
            {
                BackgroundTaskRegistration task = taskBuilder.Register();
                task.Completed += BackgroundTaskCompleted;
            }
            catch (Exception ex)
            {
                rootPage.NotifyUser("Registration error: " + ex.Message, NotifyType.ErrorMessage);
                UnregisterBackgroundTask();
            }
        }

        private bool UnregisterBackgroundTask()
        {
            foreach (var iter in BackgroundTaskRegistration.AllTasks)
            {
                IBackgroundTaskRegistration task = iter.Value;
                if (task.Name == SAMPLE_TASK_NAME)
                {
                    task.Unregister(true);
                    return true;
                }
            }
            return false;
        }

        private async void BackgroundTaskCompleted(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                rootPage.NotifyUser("Background work item triggered by raw notification with payload = " + ApplicationData.Current.LocalSettings.Values[SAMPLE_TASK_NAME] + " has completed!", NotifyType.StatusMessage);
            });
        }
    }
}
