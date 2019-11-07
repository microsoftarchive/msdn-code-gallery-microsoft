// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

using NotificationsExtensions.ToastContent;
using System;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplateCS;

namespace ToastsSampleCS
{
    public sealed partial class ScenarioInput6 : Page
    {
        // A pointer back to the main page which is used to gain access to the input and output frames and their content.
        MainPage rootPage = null;
        ToastNotification scenario6Toast = null;

        public ScenarioInput6()
        {
            InitializeComponent();
            Scenario6Looping.Click += (sender, e) => { DisplayLongToast(true); };
            Scenario6NoLooping.Click += (sender, e) => { DisplayLongToast(false); };
            Scenario6LoopingString.Click += (sender, e) => { DisplayLongToast(true); };
            Scenario6NoLoopingString.Click += (sender, e) => { DisplayLongToast(false); };
            Scenario6HideToast.Click += HideToast_Click;
        }

        void DisplayLongToast(bool loopAudio)
        {
            IToastText02 toastContent = ToastContentFactory.CreateToastText02();

            // Toasts can optionally be set to long duration
            toastContent.Duration = ToastDuration.Long;

            toastContent.TextHeading.Text = "Long Duration Toast";

            if (loopAudio)
            {
                toastContent.Audio.Loop = true;
                toastContent.Audio.Content = ToastAudioContent.LoopingAlarm;
                toastContent.TextBodyWrap.Text = "Looping audio";
            }
            else
            {
                toastContent.Audio.Content = ToastAudioContent.IM;
            }

            scenario6Toast = toastContent.CreateNotification();
            ToastNotificationManager.CreateToastNotifier().Show(scenario6Toast);

            rootPage.NotifyUser(toastContent.GetContent(), NotifyType.StatusMessage);
        }

        void DisplayLongToastWithStringManipulation(bool loopAudio)
        {
            string toastXmlString = String.Empty;
            if (loopAudio)
            {
                toastXmlString = "<toast duration='long'>"
                            + "<visual version='1'>"
                            + "<binding template='ToastText02'>"
                            + "<text id='1'>Long Duration Toast</text>"
                            + "<text id='2'>Looping audio</text>"
                            + "</binding>"
                            + "</visual>"
                            + "<audio loop='true' src='ms-winsoundevent:Notification.Looping.Alarm'/>"
                            + "</toast>";
            }
            else
            {
                toastXmlString = "<toast duration='long'>"
                         + "<visual version='1'>"
                         + "<binding template='ToastText02'>"
                         + "<text id='1'>Long Toast</text>"
                         + "</binding>"
                         + "</visual>"
                         + "<audio loop='true' src='ms-winsoundevent:Notification.IM'/>"
                         + "</toast>";
            }

            Windows.Data.Xml.Dom.XmlDocument toastDOM = new Windows.Data.Xml.Dom.XmlDocument();
            toastDOM.LoadXml(toastXmlString);

            // Create a toast, then create a ToastNotifier object to show
            // the toast
            scenario6Toast = new ToastNotification(toastDOM);
            ToastNotificationManager.CreateToastNotifier().Show(scenario6Toast);
            rootPage.NotifyUser(toastDOM.GetXml(), NotifyType.StatusMessage);
        }

        void HideToast_Click(object sender, RoutedEventArgs e)
        {
            if (scenario6Toast != null)
            {
                ToastNotificationManager.CreateToastNotifier().Hide(scenario6Toast);
                scenario6Toast = null;
            }
            else
            {
                rootPage.NotifyUser("No toast has been displayed from Scenario 6", NotifyType.StatusMessage);
            }
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
