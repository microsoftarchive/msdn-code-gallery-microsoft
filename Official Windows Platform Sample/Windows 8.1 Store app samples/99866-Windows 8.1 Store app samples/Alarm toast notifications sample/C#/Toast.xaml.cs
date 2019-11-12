//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;
using Windows.ApplicationModel.Background;

namespace AlarmNotifications
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ToastScenario : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public ToastScenario()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                await AlarmApplicationManager.RequestAccessAsync().AsTask();
            }
            catch
            {
                // RequestAccessAsync may throw an exception if the app is not currently in the foreground.
            }
        }

        /// <summary>
        /// This is the click handler for the buttons that raise toasts.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendToast_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b != null)
            {
                // Get some data from the button the user clicked on.
                string toastTemplate = b.Name;
                string alarmName = "";

                if (toastTemplate.Contains("Custom"))
                {
                    alarmName = "Wake up time with custom snooze!";
                }
                else
                {
                    alarmName = "Wake up time with default snooze!";
                }

                // Create the toast content by direct string manipulation.
                // See the Toasts SDK Sample for other ways of generating toasts.
                string toastXmlString =
                    "<toast duration=\"long\">\n" +
                        "<visual>\n" +
                            "<binding template=\"ToastText02\">\n" +
                                "<text id=\"1\">Alarms Notifications SDK Sample App</text>\n" +
                                "<text id=\"2\">" + alarmName + "</text>\n" +
                            "</binding>\n" +
                        "</visual>\n" +
                        "<commands scenario=\"alarm\">\n" +
                            "<command id=\"snooze\"/>\n" +
                            "<command id=\"dismiss\"/>\n" +
                        "</commands>\n" +
                        "<audio src=\"ms-winsoundevent:Notification.Looping.Alarm2\" loop=\"true\" />\n" +
                    "</toast>\n";

                // Display the generated XML for demonstration purposes.
                rootPage.NotifyUser(toastXmlString, NotifyType.StatusMessage);

                // Create an XML document from the XML.
                var toastDOM = new Windows.Data.Xml.Dom.XmlDocument();
                toastDOM.LoadXml(toastXmlString);

                // Prepare to raise the toast.
                var toastNotifier = Windows.UI.Notifications.ToastNotificationManager.CreateToastNotifier();
                int delay = int.Parse(((FrameworkElement)ToastDelay.SelectedItem).Name.Substring(1));

                if (toastTemplate.Contains("Custom"))
                {
                    //Schedule the alarm with the custom snooze interval
                    int customSnoozeSeconds = Convert.ToInt32(CustomSnoozeTime.Text) * 60;
                    TimeSpan snoozeInterval = TimeSpan.FromSeconds(customSnoozeSeconds);
                    var customAlarmScheduledToast = new Windows.UI.Notifications.ScheduledToastNotification(toastDOM, DateTime.Now.AddSeconds(delay), snoozeInterval, 0);
                    toastNotifier.AddToSchedule(customAlarmScheduledToast);
                }
                else 
                {
                    //Schedule the alarm with the custom snooze interval
                    //The notification is scheduled in the future
                    var customAlarmScheduledToast = new Windows.UI.Notifications.ScheduledToastNotification(toastDOM, DateTime.Now.AddSeconds(delay));
                    toastNotifier.AddToSchedule(customAlarmScheduledToast);
                }                
            }
        }
    }
}
