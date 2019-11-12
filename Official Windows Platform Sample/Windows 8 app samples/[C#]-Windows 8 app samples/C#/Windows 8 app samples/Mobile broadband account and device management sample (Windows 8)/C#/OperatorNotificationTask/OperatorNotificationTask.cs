using System.Diagnostics;
using System.Threading;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.Networking.NetworkOperators;
using Windows.UI.Notifications;


namespace OperatorNotificationTask
{
    public sealed class OperatorNotification : IBackgroundTask
    {
        //
        // The Run method is the entry point of a background task.
        //
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            //
            // Get the notification event details
            //
            NetworkOperatorNotificationEventDetails notificationEventData = (NetworkOperatorNotificationEventDetails)taskInstance.TriggerDetails;

            //
            // This sample only handles notification types that typically have message content
            // The message is displayed in a toast
            //
            if ((notificationEventData.NotificationType == NetworkOperatorEventMessageType.Gsm) ||
                (notificationEventData.NotificationType == NetworkOperatorEventMessageType.Cdma) ||
                (notificationEventData.NotificationType == NetworkOperatorEventMessageType.Ussd))
            {
                XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);

                XmlNodeList stringElements = toastXml.GetElementsByTagName("text");

                stringElements.Item(0).AppendChild(toastXml.CreateTextNode("MobileBroadband Message:"));

                stringElements.Item(1).AppendChild(toastXml.CreateTextNode(notificationEventData.Message));

                ToastNotification notification = new ToastNotification(toastXml);
                ToastNotificationManager.CreateToastNotifier().Show(notification);
            }

            //
            // Provide status to application via local settings storage
            //
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            settings.Values[taskInstance.Task.TaskId.ToString()] = "Completed";

            Debug.WriteLine("Background " + taskInstance.Task.Name + " process ran");
        }
    }
}
