using System.Diagnostics;
using Windows.ApplicationModel.Background;
using Windows.Networking.PushNotifications;
using Windows.Storage;

namespace BackgroundTasks
{
    // You must use a sealed class, and make sure the output is a WINMD.
    public sealed class SampleBackgroundTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // Get the background task details
            ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;
            string taskName = taskInstance.Task.Name;

            Debug.WriteLine("Background " + taskName + " starting...");

            // Store the content received from the notification so it can be retrieved from the UI.
            RawNotification notification = (RawNotification)taskInstance.TriggerDetails;
            settings.Values[taskName] = notification.Content;

            Debug.WriteLine("Background " + taskName + " completed!");
        }
    }
}
