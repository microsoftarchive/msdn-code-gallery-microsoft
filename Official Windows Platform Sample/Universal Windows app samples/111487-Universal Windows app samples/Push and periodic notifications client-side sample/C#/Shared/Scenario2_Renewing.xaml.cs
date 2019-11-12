// Copyright (c) Microsoft. All rights reserved.

using PushNotificationsHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    /// Renewing push channels, both explicitly and via background tasks
    /// </summary>
    public sealed partial class Scenario2 : Page
    {
        private MainPage rootPage = null;
        private const string PUSH_NOTIFICATIONS_TASK_NAME = "UpdateChannels";
        private const string PUSH_NOTIFICATIONS_TASK_ENTRY_POINT = "PushNotificationsHelper.MaintenanceTask";
        private const int MAINTENANCE_INTERVAL = 10 * 24 * 60; // Check for channels that need to be updated every 10 days

        public Scenario2()
        {
			InitializeComponent();
        }

        private async void RenewChannelsButton_Click(Object sender, RoutedEventArgs e)
        {
            try
            {
                // The Notifier object allows us to use the same code in the maintenance task and this foreground application
                await rootPage.Notifier.RenewAllAsync(true);
                rootPage.NotifyUser("Channels renewed successfully", NotifyType.StatusMessage);
            }
            catch (Exception ex)
            {
                rootPage.NotifyUser("Channels renewal failed: " + ex.Message, NotifyType.ErrorMessage);
            }   
        }

        private void RegisterTaskButton_Click(Object sender, RoutedEventArgs e)
        {
            if (GetRegisteredTask() == null)
            {
                BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder();
                MaintenanceTrigger trigger = new MaintenanceTrigger(MAINTENANCE_INTERVAL, false);
                taskBuilder.SetTrigger(trigger);
                taskBuilder.TaskEntryPoint = PUSH_NOTIFICATIONS_TASK_ENTRY_POINT;
                taskBuilder.Name = PUSH_NOTIFICATIONS_TASK_NAME;

                SystemCondition internetCondition = new SystemCondition(SystemConditionType.InternetAvailable);
                taskBuilder.AddCondition(internetCondition);

                try
                {
                    taskBuilder.Register();
                    rootPage.NotifyUser("Task registered", NotifyType.StatusMessage);
                }
                catch (Exception ex)
                {
                    rootPage.NotifyUser("Error registering task: " + ex.Message, NotifyType.ErrorMessage);
                }
            }
            else
            {
                rootPage.NotifyUser("Task already registered", NotifyType.ErrorMessage);
            }
        }

        private void UnregisterTaskButton_Click(Object sender, RoutedEventArgs e)
        {
            IBackgroundTaskRegistration task = GetRegisteredTask();
            if (task != null)
            {
                task.Unregister(true);
                rootPage.NotifyUser("Task unregistered", NotifyType.StatusMessage);
            }
            else
            {
                rootPage.NotifyUser("Task not registered", NotifyType.ErrorMessage);
            }
        }

        private IBackgroundTaskRegistration GetRegisteredTask()
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks.Values)
            {
                if (task.Name == PUSH_NOTIFICATIONS_TASK_NAME)
                {
                    return task;
                }
            }
            return null;
        }

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			// Get a pointer to our main page
			rootPage = MainPage.Current;

			if (rootPage.Notifier == null)
			{
				rootPage.Notifier = new Notifier();
			}
		}
    }
}
