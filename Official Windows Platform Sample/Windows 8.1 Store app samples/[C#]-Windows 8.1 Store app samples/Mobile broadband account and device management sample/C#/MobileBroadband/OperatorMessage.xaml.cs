//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;
using Windows.ApplicationModel.Background;
using Windows.UI.Core;
using Windows.Storage;
using Windows.Networking.NetworkOperators;

namespace MobileBroadband
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OperatorMessage : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        private CoreDispatcher sampleDispatcher;
        private const string OperatorNotificationTaskEntryPoint = "OperatorNotificationTask.OperatorNotification";
        private const string OperatorNotificationTaskName = "OperatorNotificationTask";

        public OperatorMessage()
        {
            this.InitializeComponent();

            sampleDispatcher = Window.Current.CoreWindow.Dispatcher;

            try
            {
                //
                // Register a background task for the network operator notification system event.
                // This event is triggered when the application is updated.
                //
                RegisterOperatorNotificationTask();
            }
            catch (Exception ex)
            {
                rootPage.NotifyUser(ex.ToString(), NotifyType.ErrorMessage);
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        //
        // Registers a background task for the operator notification system event.
        // This event occurs when the application is updated.
        //
        private void RegisterOperatorNotificationTask()
        {
            //
            // Check whether the operator notification background task is already registered.
            //
            foreach (var cur in BackgroundTaskRegistration.AllTasks)
            {
               if (cur.Value.Name == "MobileOperatorNotificationHandler")
               {
                   cur.Value.Completed += new BackgroundTaskCompletedEventHandler(OnCompleted);
                   OperatorNotificationStatus.Text = "Completion handler registered";
               }
            }

            //
            // Get all active Mobilebroadband accounts
            //
            var allAccounts = MobileBroadbandAccount.AvailableNetworkAccountIds;
            if (allAccounts.Count > 0)
            {
                //
                // Update ui to show the task is registered.
                //
                rootPage.NotifyUser("Mobile broadband account found", NotifyType.StatusMessage);
            }
            else
            {
                rootPage.NotifyUser("No Mobilebroadband accounts found", NotifyType.StatusMessage);
            }
        }

        // Handle background task completion event.
        private async void OnCompleted(IBackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs e)
        {
            // Update the UI with the completion status reported by the background task.
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
                            OperatorNotificationStatus.Text = settings.Values[key].ToString();
                            rootPage.NotifyUser("Operator Notification background task completed", NotifyType.StatusMessage);
                        }
                    }
                    catch (Exception ex)
                    {
                        OperatorNotificationStatus.Text = "Background task error";
                        rootPage.NotifyUser(ex.ToString(), NotifyType.ErrorMessage);
                    }
                });
        }
    }
}

