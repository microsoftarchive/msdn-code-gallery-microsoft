using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Geolocation;
using Windows.Storage;

namespace BackgroundTask
{
    public sealed class LocationBackgroundTask : IBackgroundTask
    {
        CancellationTokenSource m_cts = null;

        async void IBackgroundTask.Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            try
            {
                // Associate a cancellation handler with the background task.
                taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(OnCanceled);

                // Get cancellation token
                if (m_cts == null)
                {
                    m_cts = new CancellationTokenSource();
                }
                CancellationToken token = m_cts.Token;

                // Create geolocator object
                Geolocator geolocator = new Geolocator();

                // Make the request for the current position
                Geoposition pos = await geolocator.GetGeopositionAsync().AsTask(token);

                DateTime currrentTime = DateTime.Now;

                WriteStatusToAppdata("Time: " + currrentTime.ToString());
                WriteGeolocToAppdata(pos);
            }
            catch (UnauthorizedAccessException)
            {
                WriteStatusToAppdata("Disabled");
                WipeGeolocDataFromAppdata();
            }
            finally
            {
                m_cts = null;

                deferral.Complete();
            }
        }

        private void WriteGeolocToAppdata(Geoposition pos)
        {
            var settings = ApplicationData.Current.LocalSettings;
            settings.Values["Latitude"] = pos.Coordinate.Latitude.ToString();
            settings.Values["Longitude"] = pos.Coordinate.Longitude.ToString();
            settings.Values["Accuracy"] = pos.Coordinate.Accuracy.ToString();
        }

        private void WipeGeolocDataFromAppdata()
        {
            var settings = ApplicationData.Current.LocalSettings;
            settings.Values["Latitude"] = "";
            settings.Values["Longitude"] = "";
            settings.Values["Accuracy"] = "";
        }

        private void WriteStatusToAppdata(string status)
        {
            var settings = ApplicationData.Current.LocalSettings;
            settings.Values["Status"] = status;
        }

        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            if (m_cts != null)
            {
                m_cts.Cancel();
                m_cts = null;
            }
        }
    }
}
