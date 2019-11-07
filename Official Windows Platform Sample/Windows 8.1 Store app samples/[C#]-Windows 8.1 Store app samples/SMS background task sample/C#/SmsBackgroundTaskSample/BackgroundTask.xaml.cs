//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Windows.ApplicationModel.Background;
using Windows.Devices.Sms;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using SDKTemplate;
using System;


namespace SmsBackgroundTaskSample
{
    public sealed partial class BackgroundTask : SDKTemplate.Common.LayoutAwarePage
    {
        private const string BackgroundTaskEntryPoint = "SmsBackgroundTask.SampleSmsBackgroundTask";
        private const string BackgroundTaskName = "SampleSmsBackgroundTask";
        private CoreDispatcher sampleDispatcher;
        private bool hasDeviceAccess = false;

        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public BackgroundTask()
        {
            this.InitializeComponent();

            // Get dispatcher for dispatching updates to the UI thread.
            sampleDispatcher = Window.Current.CoreWindow.Dispatcher;

            try
            {
                // Initialize state-based registration of currently registered background tasks.
                InitializeRegisteredSmsBackgroundTasks();
            }
            catch (Exception ex)
            {
                rootPage.NotifyUser(ex.ToString(), NotifyType.ErrorMessage);
            }

        }

        // Update the registration status text and the button states based on the background task's
        // registration state.
        private void UpdateBackgroundTaskUIState(bool Registered)
        {
            if (Registered)
            {
                BackgroundTaskStatus.Text = "Registered";
                RegisterBackgroundTaskButton.IsEnabled = false;
                UnregisterBackgroundTaskButton.IsEnabled = true;
            }
            else
            {
                BackgroundTaskStatus.Text = "Unregistered";
                RegisterBackgroundTaskButton.IsEnabled = true;
                UnregisterBackgroundTaskButton.IsEnabled = false;
            }
        }

        // Handle request to register the background task
        private async void RegisterBackgroundTask_Click(object sender, RoutedEventArgs e)
        {
            // SMS is a sensitive capability and the user may be prompted for consent. If the app
            // does not obtain permission for the package to have access to SMS before the background
            // work item is run (perhaps after the app is suspended or terminated), the background
            // work item will not have access to SMS and will have no way to prompt the user for consent
            // without an active window. Here, any available SMS device is activated in order to ensure
            // consent. Your app will likely do something with the SMS device as part of its features.
            if (!hasDeviceAccess)
            {
                try
                {
                    SmsDevice smsDevice = (SmsDevice)await SmsDevice.GetDefaultAsync();
                    rootPage.NotifyUser("Successfully connnected to SMS device with account number: " + smsDevice.AccountPhoneNumber, NotifyType.StatusMessage);
                    hasDeviceAccess = true;
                }
                catch (Exception ex)
                {
                    rootPage.NotifyUser("Failed to find SMS device\n" + ex.Message, NotifyType.ErrorMessage);
                    return;
                }
            }

            try
            {
                // Create a new background task builder.
                BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder();

                // Create a new SmsReceived trigger.
                SystemTrigger trigger = new SystemTrigger(SystemTriggerType.SmsReceived, false);

                // Associate the SmsReceived trigger with the background task builder.
                taskBuilder.SetTrigger(trigger);

                // Specify the background task to run when the trigger fires.
                taskBuilder.TaskEntryPoint = BackgroundTaskEntryPoint;

                // Name the background task.
                taskBuilder.Name = BackgroundTaskName;

                // Register the background task.
                BackgroundTaskRegistration taskRegistration = taskBuilder.Register();

                // Associate completed event handler with the new background task.
                taskRegistration.Completed += new BackgroundTaskCompletedEventHandler(OnCompleted);

                UpdateBackgroundTaskUIState(true);
                rootPage.NotifyUser("Registered SMS background task", NotifyType.StatusMessage);
            }
            catch (Exception ex)
            {
                rootPage.NotifyUser(ex.ToString(), NotifyType.ErrorMessage);
            }
        }

        // Handle request to unregister the background task
        private void UnregisterBackgroundTask_Click(object sender, RoutedEventArgs e)
        {
            // Loop through all background tasks and unregister our background task
            foreach (var cur in BackgroundTaskRegistration.AllTasks)
            {
                if (cur.Value.Name == BackgroundTaskName)
                {
                    cur.Value.Unregister(true);
                }
            }

            UpdateBackgroundTaskUIState(false);
            rootPage.NotifyUser("Unregistered SMS background task.", NotifyType.StatusMessage);
         }

        // Initialize state based on currently registered background tasks
        public void InitializeRegisteredSmsBackgroundTasks()
        {
            try
            {
                //
                // Initialize UI elements based on currently registered background tasks
                // and associate background task completed event handler with each background task.
                //
                UpdateBackgroundTaskUIState(false);

                foreach (var item in BackgroundTaskRegistration.AllTasks)
                {
                    IBackgroundTaskRegistration task = item.Value;
                    if (task.Name == BackgroundTaskName)
                    {
                        UpdateBackgroundTaskUIState(true);
                        task.Completed += new BackgroundTaskCompletedEventHandler(OnCompleted);
                    }
                }
            }
            catch (Exception ex)
            {
                rootPage.NotifyUser(ex.ToString(), NotifyType.ErrorMessage);
            }
        }

        // Handle background task completion event.
        private async void OnCompleted(IBackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs e)
        {
            // Update the UI with the complrtion status reported by the background task.
            // Dispatch an anonymous task to the UI thread to do the update.
            await sampleDispatcher.RunAsync(CoreDispatcherPriority.Normal,
                                         () =>
            {
                try
                {
                    if ((sender != null) && (e != null))
                    {
                        // this method throws if the event is reporting an error
                        e.CheckResult();

                        // Update the UI with the background task's completion status.
                        // The task stores status in the application data settings indexed by the task ID.
                        var key = sender.TaskId.ToString();
                        var settings = ApplicationData.Current.LocalSettings;
                        BackgroundTaskStatus.Text = settings.Values[key].ToString();
                    }
                }
                catch (Exception ex)
                {
                    rootPage.NotifyUser(ex.ToString(), NotifyType.ErrorMessage);
                }
            });
        }
    }
}

