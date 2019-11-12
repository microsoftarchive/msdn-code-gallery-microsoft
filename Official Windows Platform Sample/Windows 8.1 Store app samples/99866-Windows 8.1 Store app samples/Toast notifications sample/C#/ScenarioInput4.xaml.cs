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
    public sealed partial class ScenarioInput4 : Page
    {
        // A pointer back to the main page which is used to gain access to the input and output frames and their content.
        MainPage rootPage = null;

        public ScenarioInput4()
        {
            InitializeComponent();
            Scenario4DisplayToastSilent.Click += (sender, e) => { DisplayAudioToast("Silent"); };
            Scenario4DisplayToastDefault.Click += (sender, e) => { DisplayAudioToast("Default"); };
            Scenario4DisplayToastMail.Click += (sender, e) => { DisplayAudioToast("Mail"); };
            Scenario4DisplayToastSMS.Click += (sender, e) => { DisplayAudioToast("SMS"); };
            Scenario4DisplayToastIM.Click += (sender, e) => { DisplayAudioToast("IM"); };
            Scenario4DisplayToastReminder.Click += (sender, e) => { DisplayAudioToast("Reminder"); };
            Scenario4DisplayToastSilentString.Click += (sender, e) => { DisplayAudioToastWithStringManipulation("Silent"); };
            Scenario4DisplayToastDefaultString.Click += (sender, e) => { DisplayAudioToastWithStringManipulation("Default"); };
            Scenario4DisplayToastMailString.Click += (sender, e) => { DisplayAudioToastWithStringManipulation("Mail"); };
        }

        void DisplayAudioToast(string audioSrc)
        {
            IToastText02 toastContent = ToastContentFactory.CreateToastText02();
            toastContent.TextHeading.Text = "Sound:";
            toastContent.TextBodyWrap.Text = audioSrc;
            toastContent.Audio.Content = (ToastAudioContent)Enum.Parse(typeof(ToastAudioContent), audioSrc);

            rootPage.NotifyUser(toastContent.GetContent(), NotifyType.StatusMessage);

            // Create a toast, then create a ToastNotifier object to show
            // the toast
            ToastNotification toast = toastContent.CreateNotification();

            // If you have other applications in your package, you can specify the AppId of
            // the app to create a ToastNotifier for that application
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        void DisplayAudioToastWithStringManipulation(string audioSrc)
        {
            string toastXmlString = String.Empty;

            if (audioSrc.Equals("Silent"))
            {
                toastXmlString = "<toast>"
                               + "<visual version='1'>"
                               + "<binding template='ToastText02'>"
                               + "<text id='1'>Sound:</text>"
                               + "<text id='2'>" + audioSrc + "</text>"
                               + "</binding>"
                               + "</visual>"
                               + "<audio silent='true'/>"
                               + "</toast>";
            }
            else if (audioSrc.Equals("Default"))
            {
                toastXmlString = "<toast>"
                           + "<visual version='1'>"
                           + "<binding template='ToastText02'>"
                           + "<text id='1'>Sound:</text>"
                           + "<text id='2'>" + audioSrc + "</text>"
                           + "</binding>"
                           + "</visual>"
                           + "</toast>";
            }
            else
            {
                toastXmlString = "<toast>"
                           + "<visual version='1'>"
                           + "<binding template='ToastText02'>"
                           + "<text id='1'>Sound:</text>"
                           + "<text id='2'>" + audioSrc + "</text>"
                           + "</binding>"
                           + "</visual>"
                           + "<audio src='ms-winsoundevent:Notification." + audioSrc + "'/>"
                           + "</toast>";
            }

            Windows.Data.Xml.Dom.XmlDocument toastDOM = new Windows.Data.Xml.Dom.XmlDocument();
            try
            {
                toastDOM.LoadXml(toastXmlString);

                rootPage.NotifyUser(toastDOM.GetXml(), NotifyType.StatusMessage);

                // Create a toast, then create a ToastNotifier object to show
                // the toast
                ToastNotification toast = new ToastNotification(toastDOM);

                // If you have other applications in your package, you can specify the AppId of
                // the app to create a ToastNotifier for that application
                ToastNotificationManager.CreateToastNotifier().Show(toast);
            }
            catch (Exception)
            {
                rootPage.NotifyUser("Error loading the xml, check for invalid characters in the input", NotifyType.ErrorMessage);
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
