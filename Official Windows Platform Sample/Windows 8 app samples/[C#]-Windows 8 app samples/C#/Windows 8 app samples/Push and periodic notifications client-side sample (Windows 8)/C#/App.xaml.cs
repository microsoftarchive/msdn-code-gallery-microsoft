// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.Networking.PushNotifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SDKTemplateCS;

namespace PushAndPeriodicNotificationsSampleCS
{
	public partial class App
	{
        public PushNotificationChannel CurrentChannel = null;

        private const String PUSH_NOTIFICATIONS_TASK_NAME = "UpdateChannels";
        private const string PUSH_NOTIFICATIONS_TASK_ENTRY_POINT = "PushNotificationsHelper.MaintenanceTask";
        private const int MAINTENANCE_INTERVAL = 10 * 24 * 60; // Check for channels that need to be updated every 10 days

		public App()
		{
			InitializeComponent();
			this.Suspending += new SuspendingEventHandler(OnSuspending);

            if (!IsTaskRegistered())
            {
                BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder();
                MaintenanceTrigger trigger = new MaintenanceTrigger(MAINTENANCE_INTERVAL, false);
                taskBuilder.SetTrigger(trigger);
                taskBuilder.TaskEntryPoint = PUSH_NOTIFICATIONS_TASK_ENTRY_POINT;
                taskBuilder.Name = PUSH_NOTIFICATIONS_TASK_NAME;

                SystemCondition internetCondition = new SystemCondition(SystemConditionType.InternetAvailable);
                taskBuilder.AddCondition(internetCondition);

                taskBuilder.Register();
            }
		}

        private bool IsTaskRegistered()
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks.Values)
            {
                if (task.Name == PUSH_NOTIFICATIONS_TASK_NAME)
                {
                    return true;
                }
            }
            return false;
        }

		async protected void OnSuspending(object sender, SuspendingEventArgs args)
		{
			SuspendingDeferral deferral = args.SuspendingOperation.GetDeferral();
			await SuspensionManager.SaveAsync();
			deferral.Complete();
		}

		async protected override void OnLaunched(LaunchActivatedEventArgs args)
		{
			if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
			{
				//     Do an asynchronous restore
				await SuspensionManager.RestoreAsync();
			}
			var rootFrame = new Frame();
			rootFrame.Navigate(typeof(MainPage));
			Window.Current.Content = rootFrame;
            MainPage p = rootFrame.Content as MainPage;
            p.RootNamespace = this.GetType().Namespace;
			Window.Current.Activate();
		}
	}
}
