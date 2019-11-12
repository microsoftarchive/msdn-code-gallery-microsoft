//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;

namespace LockScreenCall
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
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var ignored = Windows.ApplicationModel.Background.BackgroundExecutionManager.RequestAccessAsync();
        }


        /// <summary>
        /// This is the click handler for the buttons that raise toasts.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Toast_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b != null)
            {
                // Get some data from the button the user clicked on.
                string toastTemplate = b.Name;

                // Decide what buttons are enabled in this scenario.
                string videoButtonXml = "";
                string voiceButtonXml = "";
                string infoString;
                string callerType = ((FrameworkElement)CallerType.SelectedItem).Name;
                string callDuration = ((FrameworkElement)CallDuration.SelectedItem).Name.Substring(1);

                if (toastTemplate.Contains("Video"))
                {
                    videoButtonXml = "  <command id=\"video\" arguments=\"Video " + callerType + " " + callDuration + "\"/>\n";
                    infoString = "Incoming video call";
                }
                else
                {
                    infoString = "Incoming voice call";
                }
                if (toastTemplate.Contains("Voice"))
                {
                    voiceButtonXml = "  <command id=\"voice\" arguments=\"Voice " + callerType + " " + callDuration + "\"/>\n";
                }

                // Create the toast contnet by direct string manipulation.
                // See the Toasts SDK Sample for other ways of generating toasts.
                string xmlPayload =
                  "<toast duration=\"long\">\n"
                + " <audio loop=\"true\" src=\"ms-winsoundevent:Notification.Looping.Call3\"/>\n"
                + "  <visual>\n"
                + "   <binding template=\"ToastText02\">\n"
                + "    <text id=\"1\">" + callerType + "</text>\n"
                + "    <text id=\"2\">" + infoString + "</text>\n"
                + "   </binding>\n"
                + "  </visual>\n"
                + " <commands scenario=\"incomingCall\">\n"
                + videoButtonXml
                + voiceButtonXml
                + "  <command id=\"decline\"/>\n"
                + " </commands>\n"
                + "</toast>";

                // Display the generated XML for demonstration purposes.
                rootPage.NotifyUser(xmlPayload, NotifyType.StatusMessage);

                // Create an XML document from the XML.
                var toastDOM = new Windows.Data.Xml.Dom.XmlDocument();
                toastDOM.LoadXml(xmlPayload);

                // Prepare to raise the toast.
                var toastNotifier = ToastNotificationManager.CreateToastNotifier();

                int delay = int.Parse(((FrameworkElement)ToastDelay.SelectedItem).Name.Substring(1));
                if (delay > 0)
                {
                    // Schedule the toast in the future.
                    DateTime dueTime = DateTime.Now.AddSeconds(delay);
                    var scheduledToast = new ScheduledToastNotification(toastDOM, dueTime);
                    toastNotifier.AddToSchedule(scheduledToast);
                }
                else
                {
                    // Raise the toast immediately.
                    var toast = new ToastNotification(toastDOM);
                    toastNotifier.Show(toast);
                }                
            }
        }
    }
}
