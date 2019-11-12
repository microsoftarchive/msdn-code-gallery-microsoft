using System;
using System.Diagnostics;
using System.Threading;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.Data.Xml.Dom;
using Windows.Networking.NetworkOperators;
using Windows.UI.Notifications;
using System.Threading.Tasks;


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
            // Associate a cancellation handler with the background task.
            //
            taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(OnCanceled);

            //
            // Do the background task activity.
            //
            DisplayToast(taskInstance);

            //
            // Provide status to application via local settings storage
            //
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            NetworkOperatorNotificationEventDetails notificationEventData = (NetworkOperatorNotificationEventDetails)taskInstance.TriggerDetails;
            settings.Values[taskInstance.Task.TaskId.ToString()] = "Completed";
            settings.Values[taskInstance.Task.TaskId.ToString()+"_type"] = notificationEventData.NotificationType.ToString();

            Debug.WriteLine("Background " + taskInstance.Task.Name + " process ran");
        }

        private void DisplayToast(IBackgroundTaskInstance taskInstance)
        {
            try
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
                else if ((notificationEventData.NotificationType == NetworkOperatorEventMessageType.TetheringOperationalStateChanged) ||
                         (notificationEventData.NotificationType == NetworkOperatorEventMessageType.TetheringNumberOfClientsChanged))
                {
                    XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
                    XmlNodeList stringElements = toastXml.GetElementsByTagName("text");

                    stringElements.Item(0).AppendChild(toastXml.CreateTextNode("Tethering notification:"));
                    stringElements.Item(1).AppendChild(toastXml.CreateTextNode(notificationEventData.NotificationType.ToString()));

                    ToastNotification notification = new ToastNotification(toastXml);
                    ToastNotificationManager.CreateToastNotifier().Show(notification);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error displaying toast: " + ex.Message);
            }
        }

        //
        // Handles background task cancellation.
        //
        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            //
            // Indicate that the background task is canceled.
            //
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            settings.Values[sender.Task.TaskId.ToString()] = "Canceled";

            Debug.WriteLine("Background " + sender.Task.Name + " Cancel Requested...");
        }
    }
}
