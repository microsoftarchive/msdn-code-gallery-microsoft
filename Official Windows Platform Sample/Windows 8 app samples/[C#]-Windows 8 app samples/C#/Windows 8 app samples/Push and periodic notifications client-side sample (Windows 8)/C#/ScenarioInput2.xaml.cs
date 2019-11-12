// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

using PushNotificationsHelper;
using SDKTemplateCS;
using System;
using Windows.ApplicationModel.Background;
using Windows.Networking.PushNotifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace PushAndPeriodicNotificationsSampleCS
{
	public sealed partial class ScenarioInput2 : Page
	{
		// A pointer back to the main page which is used to gain access to the input and output frames and their content.
		MainPage rootPage = null;
        private const String PUSH_NOTIFICATIONS_TASK_NAME = "UpdateChannels";
        private const string PUSH_NOTIFICATIONS_TASK_ENTRY_POINT = "PushNotificationsHelper.MaintenanceTask";
        private const int MAINTENANCE_INTERVAL = 10 * 24 * 60; // Check for channels that need to be updated every 10 days*/

		public ScenarioInput2()
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
			rootPage = e.Parameter as MainPage;

			// We want to be notified with the OutputFrame is loaded so we can get to the content.
			rootPage.OutputFrameLoaded += new System.EventHandler(rootPage_OutputFrameLoaded);

			if (rootPage.Notifier == null)
			{
				rootPage.Notifier = new Notifier();
			}
		}

		#region Template-Related Code - Do not remove
		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			rootPage.OutputFrameLoaded -= new System.EventHandler(rootPage_OutputFrameLoaded);
		}

		#endregion

		#region Use this code if you need access to elements in the output frame - otherwise delete
		void rootPage_OutputFrameLoaded(object sender, object e)
		{
			// At this point, we know that the Output Frame has been loaded and we can go ahead
			// and reference elements in the page contained in the Output Frame.

			// Get a pointer to the content within the OutputFrame.
			Page outputFrame = (Page)rootPage.OutputFrame.Content;

			// Go find the elements that we need for this scenario.
			// ex: flipView1 = outputFrame.FindName("FlipView1") as FlipView;
		}

		#endregion
	}
}
