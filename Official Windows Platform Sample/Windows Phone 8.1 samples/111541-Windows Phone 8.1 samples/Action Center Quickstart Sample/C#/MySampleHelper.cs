using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.UI.Notifications;

namespace Action_Center_Quickstart
{
    /// <summary>
    /// Helper class for common tasks in this sample. 
    /// </summary>
    public class MySampleHelper
    {
        public static bool CanSendToasts()
        {
            bool canSend = true;
            var notifier = ToastNotificationManager.CreateToastNotifier();

            if (notifier.Setting != NotificationSetting.Enabled)
            {
                string reason = "unknown";
                switch (notifier.Setting)
                {
                    case NotificationSetting.DisabledByGroupPolicy:
                        reason = "An administrator has disabled all notifications on this computer through group policy. The group policy setting overrides the user's setting.";
                        break;
                    case NotificationSetting.DisabledByManifest:
                        reason = "To be able to send a toast, set the Toast Capable option to \"Yes\" in this app's Package.appxmanifest file.";
                        break;
                    case NotificationSetting.DisabledForApplication:
                        reason = "The user has disabled notifications for this app.";
                        break;
                    case NotificationSetting.DisabledForUser:
                        reason = "The user or administrator has disabled all notifications for this user on this computer.";
                        break;
                }

                string errroMessage = String.Format("Can't send a toast.\n{0}", reason);
                MainPage.Current.NotifyUser(errroMessage, NotifyType.ErrorMessage);
                canSend = false;
            }

            return canSend;
        }

        public static void ShowSuccessMessage(string message = "Toast has been sent.")
        {
            MainPage.Current.NotifyUser(message + "\nSwipe down from the top of your screen to reveal action center.", NotifyType.StatusMessage);
        }

        // Note: All toast templates available in the Toast Template Catalog (http://msdn.microsoft.com/en-us/library/windows/apps/hh761494.aspx)
        // are treated as a ToastText02 template on Windows Phone.
        // That template defines a maximum of 2 text elements. The first text element is treated as header text and is always bold.
        // Images will never be downloaded when any of the other templates containing image elements are used, because Windows Phone will
        // not display the image. The app icon (Square 150 x 150) is displayed to the left of the toast text and is also show in the action center.
        public static ToastNotification CreateTextOnlyToast(string toastHeading, string toastBody)
        {
            // Using the ToastText02 toast template.
            ToastTemplateType toastTemplate = ToastTemplateType.ToastText02;

            // Retrieve the content part of the toast so we can change the text.
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);

            //Find the text component of the content
            XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");

            // Set the text on the toast. 
            // The first line of text in the ToastText02 template is treated as header text, and will be bold.
            toastTextElements[0].AppendChild(toastXml.CreateTextNode(toastHeading));
            toastTextElements[1].AppendChild(toastXml.CreateTextNode(toastBody));

            // Set the duration on the toast
            IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
            ((XmlElement)toastNode).SetAttribute("duration", "long");

            // Create the actual toast object using this toast specification.
            ToastNotification toast = new ToastNotification(toastXml); 

            return toast;
        }
    }
}
