// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

using NotificationsExtensions.ToastContent;
using System;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplateCS;

namespace ToastsSampleCS
{
    public sealed partial class ScenarioInput5 : Page
    {
        // A pointer back to the main page which is used to gain access to the input and output frames and their content.
        MainPage rootPage = null;
        ToastNotification scenario5Toast;
        CoreDispatcher dispatcher;

        public ScenarioInput5()
        {
            InitializeComponent();
            dispatcher = Window.Current.Dispatcher;
            Scenario5DisplayToastWithCallbacks.Click += Scenario5DisplayToastWithCallbacks_Click;
            Scenario5HideToast.Click += Scenario5HideToast_Click;
        }

        void Scenario5DisplayToastWithCallbacks_Click(object sender, RoutedEventArgs e)
        {
            IToastText02 toastContent = ToastContentFactory.CreateToastText02();

            // Set the launch activation context parameter on the toast.
            // The context can be recovered through the app Activation event
            toastContent.Launch = "Context123";

            toastContent.TextHeading.Text = "Tap toast";
            toastContent.TextBodyWrap.Text = "Or swipe to dismiss";

            // You can listen for the "Activated" event provided on the toast object
            // or listen to the "OnLaunched" event off the Windows.UI.Xaml.Application
            // object to tell when the user clicks the toast.
            //
            // The difference is that the OnLaunched event will
            // be raised by local, scheduled and cloud toasts, while the event provided by the 
            // toast object will only be raised by local toasts. 
            //
            // In this example, we'll use the event off the CoreApplication object.
            scenario5Toast = toastContent.CreateNotification();
            scenario5Toast.Dismissed += toast_Dismissed;
            scenario5Toast.Failed += toast_Failed;

            ToastNotificationManager.CreateToastNotifier().Show(scenario5Toast);
        }

        void Scenario5HideToast_Click(object sender, RoutedEventArgs e)
        {
            if (scenario5Toast != null)
            {
                ToastNotificationManager.CreateToastNotifier().Hide(scenario5Toast);
                scenario5Toast = null;
            }
            else
            {
                rootPage.NotifyUser("No toast has been displayed from Scenario 5", NotifyType.StatusMessage);
            }
        }

        async void toast_Failed(ToastNotification sender, ToastFailedEventArgs e)
        {
            
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                rootPage.NotifyUser("The toast encountered an error", NotifyType.ErrorMessage);
            });
        }

        async void toast_Dismissed(ToastNotification sender, ToastDismissedEventArgs e)
        {
            String outputText = "";

            switch (e.Reason)
            {
                case ToastDismissalReason.ApplicationHidden:
                    outputText = "The app hid the toast using ToastNotifier.Hide(toast)";
                    break;
                case ToastDismissalReason.UserCanceled:
                    outputText = "The user dismissed this toast";
                    break;
                case ToastDismissalReason.TimedOut:
                    outputText = "The toast has timed out";
                    break;
            }

            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                rootPage.NotifyUser(outputText, NotifyType.StatusMessage);
            });
        }

        public void LaunchedFromToast(String arguments)
        {
            rootPage.NotifyUser("A toast was clicked on with activation arguments: " + arguments, NotifyType.StatusMessage);
        }


        #region Template-Related Code - Do not remove
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Get a pointer to our main page
            rootPage = e.Parameter as MainPage;
        }
        #endregion
    }
}
