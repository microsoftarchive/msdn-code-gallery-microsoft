//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Windows.ApplicationModel.Background;
using Windows.Data.Json;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Authentication.Web;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

namespace Geofencing4SqSampleTasks
{
    public sealed class GeofencingBackgroundTask : IBackgroundTask
    {
        private CancellationTokenSource _cts = null;
        private List<Venue> _deferredCheckins = null;
        private Dictionary<string, Venue> _geofenceIdsToVenues = null;
        private HttpClient _httpClient = null;

        BackgroundTaskDeferral _deferral = null;
        IBackgroundTaskInstance _taskInstance = null;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // Associate a cancellation handler with the background task.
            taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(OnCanceled);

            // Get the deferral object from the task instance, and take a reference to the taskInstance;
            _deferral = taskInstance.GetDeferral();
            _taskInstance = taskInstance;
            _httpClient = new HttpClient();

            Logger.Initialize("BackgroundTraces.log");

            // Restore state
            _deferredCheckins = new List<Venue>();
            _geofenceIdsToVenues = new Dictionary<string, Venue>();
            await RestoreStateAsync();

            DoOrDeferCheckinsAsync();
        }

        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts = null;
            }

            if (null != _httpClient)
            {
                _httpClient.Dispose();
                _httpClient = null;
            }
        }

        private const bool AutoCheckinEnabledDefault = false;
        private bool AutoCheckinEnabled
        {
            get
            {
                var value = ApplicationData.Current.LocalSettings.Values[Constants.AutoCheckinEnabledKey];
                return (null == value) ? AutoCheckinEnabledDefault : (bool)value;
            }
        }

        private async void DoOrDeferCheckinsAsync()
        {
            var eventsToLog = new List<string>();

            // Read once, no need to keep checking because we are in the background
            bool autoCheckinEnabled = AutoCheckinEnabled;

            // Retrieve a vector of state change reports
            var reports = GeofenceMonitor.Current.ReadReports();
            foreach (var report in reports)
            {
                var coordinate = report.Geoposition.Coordinate;
                Venue venue = null;
                if (null != _geofenceIdsToVenues)
                {
                    _geofenceIdsToVenues.TryGetValue(report.Geofence.Id, out venue);
                }

                var name = (venue != null) ? venue.Name : report.Geofence.Id;
                var position = coordinate.Point.Position;
                var message = String.Format("Fence: {0}, NewState: {1}, Position: {2}, Timestamp: {3}",
                                name, 
                                report.NewState,
                                Logger.FormatLatLong(position.Latitude, position.Longitude),
                                coordinate.Timestamp);

                eventsToLog.Add(message);

                switch (report.NewState)
                {
                    case GeofenceState.Entered:
                        ShowToast("ENTER: " + name);
                        break;

                    case GeofenceState.Exited:
                        ShowToast("EXIT: " + name);
                        break;

                    default:
                        break;
                }

                if ((autoCheckinEnabled == true) &&
                    (report.NewState == GeofenceState.Entered) &&
                    (venue != null))
                {
                    if (!await CheckinAsync(venue))
                    {
                        eventsToLog.Add(string.Format("Could not auto-checkin at " + venue.Name + ", saving deferred checkin"));
                        _deferredCheckins.Add(venue);
                    }
                    else
                    {
                        eventsToLog.Add(string.Format("Auto checked-in at " + venue.Name + " to FourSquare!"));
                    }
                    await SaveStateAsync();
                }

                // Write the event messages for display in the foreground
                await SaveEventsLogAsync(eventsToLog);
            }

            _deferral.Complete();
        }

        private void ShowToast(string venueName)
        {
            // Create a two line toast and add audio reminder
            var ToastNotifier = ToastNotificationManager.CreateToastNotifier();
            var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);

            var toastNodeList = toastXml.GetElementsByTagName("text");
            toastNodeList.Item(0).AppendChild(toastXml.CreateTextNode("Geofencing4SqSample"));
            toastNodeList.Item(1).AppendChild(toastXml.CreateTextNode(venueName));

            var toastNode = toastXml.SelectSingleNode("/toast");
            var audio = toastXml.CreateElement("audio");
            audio.SetAttribute("src", "ms-winsoundevent:Notification.SMS");

            var toast = new ToastNotification(toastXml);
            ToastNotifier.Show(toast);
        }

        private async Task RestoreStateAsync()
        {
            try
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync(Constants.DeferredCheckinsFilename);
                using (var inStream = await file.OpenSequentialReadAsync())
                {
                    var serializer = new DataContractSerializer(_deferredCheckins.GetType());
                    _deferredCheckins = serializer.ReadObject(inStream.AsStreamForRead()) as List<Venue>;
                }
            }
            catch (FileNotFoundException)
            {
                Logger.Trace(TraceLevel.Warn, "State data file not found, not restoring deferred checkins.");
            }
            catch (System.Xml.XmlException)
            {
                Debug.Assert(false, "State data flie corrupted");
                Logger.Trace(TraceLevel.Warn, "State data file corrupted, not restoring deferred checkins.");
            }

            try
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync(Constants.GeofenceIdsToVenuesStateFilename);
                using (var inStream = await file.OpenSequentialReadAsync())
                {
                    var serializer = new DataContractSerializer(_geofenceIdsToVenues.GetType());
                    _geofenceIdsToVenues = serializer.ReadObject(inStream.AsStreamForRead()) as Dictionary<string, Venue>;
                }
            }
            catch (FileNotFoundException)
            {
                Logger.Trace(TraceLevel.Warn, "State data file not found, not restoring geofenceIds to venues map.");
            }
            catch (System.Xml.XmlException)
            {
                Debug.Assert(false, "State data flie corrupted");
                Logger.Trace(TraceLevel.Warn, "State data file corrupted, not restoring geofenceIds to venues map.");
            }
        }

        private async Task SaveStateAsync()
        {
            {
                var deferredCheckinsData = new MemoryStream();
                var serializer = new DataContractSerializer(_deferredCheckins.GetType());
                serializer.WriteObject(deferredCheckinsData, _deferredCheckins);

                var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(Constants.DeferredCheckinsFilename, CreationCollisionOption.ReplaceExisting);
                using (var fileStream = await file.OpenStreamForWriteAsync())
                {
                    deferredCheckinsData.Seek(0, SeekOrigin.Begin);
                    await deferredCheckinsData.CopyToAsync(fileStream);
                    await fileStream.FlushAsync();
                }
            }

            {
                var geofenceIdsToVenuesData = new MemoryStream();
                var serializer = new DataContractSerializer(_geofenceIdsToVenues.GetType());
                serializer.WriteObject(geofenceIdsToVenuesData, _geofenceIdsToVenues);

                var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(Constants.GeofenceIdsToVenuesStateFilename, CreationCollisionOption.ReplaceExisting);
                using (var fileStream = await file.OpenStreamForWriteAsync())
                {
                    geofenceIdsToVenuesData.Seek(0, SeekOrigin.Begin);
                    await geofenceIdsToVenuesData.CopyToAsync(fileStream);
                    await fileStream.FlushAsync();
                }
            }
        }

        private async Task SaveEventsLogAsync(List<string> eventsToLog)
        {
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(Constants.BackgroundEventsLogFilename, CreationCollisionOption.OpenIfExists);
            await FileIO.AppendLinesAsync(file, eventsToLog);
        }

        private async Task<bool> CheckinAsync(Venue v)
        {
            bool checkedIn = false;
            try
            {
                var token = ApplicationData.Current.LocalSettings.Values[Constants.OAuthTokenKey];
                if (null != token)
                {
                    var form = new HttpMultipartFormDataContent();
                    form.Add(new HttpStringContent(v.Id), "venueId");
                    form.Add(new HttpStringContent(v.Latitude.ToString() + "," + v.Longitude.ToString()), "ll");
                    form.Add(new HttpStringContent("Geofencing4Sq sample auto-checkin"), "shout");
                    form.Add(new HttpStringContent((string)token), "oauth_token");
                    form.Add(new HttpStringContent(Constants.ApiVerifiedDate), "v");

                    _cts = new CancellationTokenSource();
                    HttpResponseMessage response = await _httpClient.PostAsync(new Uri("https://api.foursquare.com/v2/checkins/add"), form).AsTask(_cts.Token);

                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.Ok:
                            checkedIn = true;
                            ShowToast(v.Name + " checked in");
                            break;
                        case HttpStatusCode.Unauthorized:
                            ApplicationData.Current.LocalSettings.Values[Constants.OAuthTokenKey] = null;
                            break;
                        default:
                            Logger.Trace(TraceLevel.Warn, "Failed to checkin to Foursquare: " + response.StatusCode.ToString());
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Trace(TraceLevel.Error, "Unable to checkin: " + Logger.FormatException(ex));
            }

            return checkedIn;
        }
    }
}
