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
using Windows.ApplicationModel.Background;
using Windows.Data.Json;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Connectivity;
using Windows.Security.Authentication.Web;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Notifications;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

using Geofencing4SqSampleTasks;

namespace Geofencing4SqSample
{
    public sealed partial class MainPage : Page
    {
        private HttpClient _httpClient = null;
        private Geolocator _geolocator = null;
        private Geoposition _geoposition = null;
        private bool _isConnectedToInternet = false;
        private Bing.Maps.Pushpin _currentLocationPushpin = null;

        // Caching
        private BasicGeoposition _venuesCachePosition;
        private List<Venue> _venuesCache = null;
        private List<Checkin> _checkinsCache = null;

        private List<Venue> _deferredCheckins = null;
        private Dictionary<string, Venue> _geofenceIdsToVenues = null;

        private IBackgroundTaskRegistration _geofenceTask = null;
        private const string _backgroundTaskName = "Geofencing4SqBackgroundTask";
        private const string _backgroundTaskEntryPoint = "Geofencing4SqSampleTasks.GeofencingBackgroundTask";

        private enum ListViewMode
        {
            Places = 0,
            MyCheckins,
            MyGeofences
        }
        private ListViewMode _currentListViewMode;
        private int _currentSelectedIndex;

        public MainPage()
        {
            this.InitializeComponent();
            _deferredCheckins = new List<Venue>();
            _geofenceIdsToVenues = new Dictionary<string, Venue>();
        }

        public void Dispose()
        {
            Settings.Changed -= OnSettingsChanged;

            // Cleanup in reverse order of creation
            if (null != _geolocator)
            {
                _geolocator.PositionChanged -= OnGeopositionChanged;
                _geolocator = null;
            }

            if (null != _httpClient)
            {
                _httpClient.Dispose();
                _httpClient = null;
            }
        }

        #region State Persistence
        public async Task SaveStateAsync()
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

        public async Task RestoreStateAsync()
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
                // Ignore this warning because it only happens on first launch after install
            }
            catch (System.Xml.XmlException)
            {
                Debug.Assert(false, "State data file corrupted");
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
                // Ignore this warning because it only happens on first launch after install
            }
            catch (System.Xml.XmlException)
            {
                Debug.Assert(false, "State data flie corrupted");
                Logger.Trace(TraceLevel.Warn, "State data file corrupted, not restoring geofenceIds to venues map.");
            }
        }

        private async Task DeleteStateAsync()
        {
            if (_venuesCache != null)
            {
                _venuesCache.Clear();
                _venuesCache = null;
            }

            if (_checkinsCache != null)
            {
                _checkinsCache.Clear();
                _checkinsCache = null;
            }

            var file = await ApplicationData.Current.LocalFolder.TryGetItemAsync(Constants.DeferredCheckinsFilename) as StorageFile;
            if (file != null)
            {
                await file.DeleteAsync();
            }

            file = await ApplicationData.Current.LocalFolder.TryGetItemAsync(Constants.GeofenceIdsToVenuesStateFilename) as StorageFile;
            if (file != null)
            {
                await file.DeleteAsync();
            }

            file = await ApplicationData.Current.LocalFolder.TryGetItemAsync(Constants.BackgroundEventsLogFilename) as StorageFile;
            if (file != null)
            {
                await file.DeleteAsync();
            }
        }

        private async Task LogBackgroundEventsAsync()
        {
            var file = await ApplicationData.Current.LocalFolder.TryGetItemAsync(Constants.BackgroundEventsLogFilename) as StorageFile;
            if (file != null)
            {
                try
                {
                    IList<string> backgroundEventsLog = await FileIO.ReadLinesAsync(file);
                    await file.DeleteAsync();

                    if (backgroundEventsLog.Count > 0)
                    {
                        Logger.Trace(TraceLevel.Info, "=== Recent Background Events ===");
                        foreach (var line in backgroundEventsLog)
                        {
                            Logger.Trace(TraceLevel.Info, line);
                        }
                        Logger.Trace(TraceLevel.Info, "=== End Recent Background Events ===");
                    }
                }
                catch (FileNotFoundException)
                {
                    // ignore
                }
                catch (Exception ex)
                {
                    Logger.Trace(TraceLevel.Warn, "Could not load recent background events: " + Logger.FormatException(ex));
                }
            }
        }

        #endregion State Persistence

        private void ShowStatus(string value) { Status.Text = value; }
        private void ClearStatus() { Status.Text = ""; }
        private void ClearItemsListView() { ItemsListView.ItemsSource = null; }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            IHttpFilter filter = new AuthFilter(new HttpBaseProtocolFilter());
            _httpClient = new HttpClient(filter);

            _isConnectedToInternet = (null != NetworkInformation.GetInternetConnectionProfile());
            NetworkInformation.NetworkStatusChanged += OnNetworkStatusChanged;

            Logger.Initialize(LoggerTextBox, LoggerScrollViewer);
            await LogBackgroundEventsAsync();

            ShowStatus("Waiting for location...");

            try
            {
                _geolocator = new Geolocator();
                _geoposition = await _geolocator.GetGeopositionAsync();
                ClearStatus();

                var position = _geoposition.Coordinate.Point.Position;
                Logger.Trace(TraceLevel.Info, String.Format("Current Location: {0}, Accuracy: {1} meters",
                    Logger.FormatLatLong(position.Latitude, position.Longitude),
                    _geoposition.Coordinate.Accuracy));

                // Initialize map
                var mapCenter = new Bing.Maps.Location(position.Latitude, position.Longitude);
                Map.SetView(mapCenter, Constants.DefaultMapZoomLevel);

                _currentLocationPushpin = new Bing.Maps.Pushpin();
                CurrentPositionMapLayer.Children.Add(_currentLocationPushpin);
                Bing.Maps.MapLayer.SetPosition(_currentLocationPushpin, mapCenter);

                // Register for position changed events
                _geolocator.PositionChanged += OnGeopositionChanged;

                // Loop through all background tasks to see if our background task is already registered
                foreach (var cur in BackgroundTaskRegistration.AllTasks)
                {
                    if (cur.Value.Name == _backgroundTaskName)
                    {
                        _geofenceTask = cur.Value;
                        break;
                    }
                }

                if (_geofenceTask != null)
                {
                    // Associate an event handler with the existing background task
                    _geofenceTask.Completed += new BackgroundTaskCompletedEventHandler(OnCompleted);
                }
                else
                {
                    // Get permission for a background task from the user. If the user has already answered once,
                    // this does nothing and the user must manually update their preference via PC Settings.
                    var backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();

                    switch (backgroundAccessStatus)
                    {
                        case BackgroundAccessStatus.Unspecified:
                        case BackgroundAccessStatus.Denied:
                            ShowStatus("This application must be added to the lock screen in order to do automatic checkins.");
                            break;

                        default:
                            break;
                    }

                    // Register the background task.
                    var geofenceTaskBuilder = new BackgroundTaskBuilder();

                    geofenceTaskBuilder.Name = _backgroundTaskName;
                    geofenceTaskBuilder.TaskEntryPoint = _backgroundTaskEntryPoint;

                    // Create a new location trigger
                    var trigger = new LocationTrigger(LocationTriggerType.Geofence);

                    // Associate the location trigger with the background task builder
                    geofenceTaskBuilder.SetTrigger(trigger);

                    // Ensure there is an internet connection before the background task is launched.
                    var condition = new SystemCondition(SystemConditionType.InternetAvailable);
                    geofenceTaskBuilder.AddCondition(condition);

                    // Register the background task
                    _geofenceTask = geofenceTaskBuilder.Register();

                    // Associate an event handler with the new background task
                    _geofenceTask.Completed += new BackgroundTaskCompletedEventHandler(OnCompleted);

                    Logger.Trace(TraceLevel.Debug, "Background registration succeeded");
                }
            }
            catch (System.UnauthorizedAccessException)
            {
                ShowStatus("Location access denied, please go to Settings -> Permissions to grant this app access to your location.");
                Logger.Trace(TraceLevel.Warn, "Access denied when getting location");
            }
            catch (TaskCanceledException)
            {
                ShowStatus("Location acquisition was canceled. Close and re-launch this app if you would like to try again.");
                Logger.Trace(TraceLevel.Warn, "Canceled getting location");
            }
            catch (Exception ex)
            {
                Logger.Trace(TraceLevel.Error, "Could not acquire location or register background trigger: " + Logger.FormatException(ex));
            }

            Settings.Changed += OnSettingsChanged;
            Logger.Trace(TraceLevel.Info, "Foursquare query limit setting: " + Settings.QueryLimit + " places");
            Logger.Trace(TraceLevel.Info, "Geofence creation radius setting: " + Settings.GeofenceRadiusMeters + " meters");
        }

        /// <summary>
        /// Invoked when we are about to navigate away from this page.  Clean up resources here.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Dispose();
        }

        private async void OnGeopositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            _geoposition = args.Position;
            var position = _geoposition.Coordinate.Point.Position;
            Logger.Trace(TraceLevel.Info, String.Format("Position Changed: {0}, Accuracy: {1} meters",
                Logger.FormatLatLong(position.Latitude, position.Longitude),
                _geoposition.Coordinate.Accuracy));

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Bing.Maps.MapLayer.SetPosition(_currentLocationPushpin, new Bing.Maps.Location(position.Latitude, position.Longitude));
            });
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

        private async void OnNetworkStatusChanged(object sender)
        {
            await AttemptDeferredCheckinsAsync();
        }

        private async Task AttemptDeferredCheckinsAsync()
        {
            if (Settings.AutoCheckinEnabled)
            {
                _isConnectedToInternet = (null != NetworkInformation.GetInternetConnectionProfile());
                if (_isConnectedToInternet)
                {
                    for (int i = 0; i < _deferredCheckins.Count; i++)
                    {
                        if (await CheckinAsync(_deferredCheckins[i]))
                        {
                            _deferredCheckins.RemoveAt(i);
                        }
                        else
                        {
                            _isConnectedToInternet = (null != NetworkInformation.GetInternetConnectionProfile());
                            if (!_isConnectedToInternet)
                            {
                                // No longer connected, leave
                                Logger.Trace(TraceLevel.Warn, "Network disconnected, stop checking in");
                                break;
                            }
                        }
                    }
                    await SaveStateAsync();
                }
            }
        }

        /// <summary>
        /// This is the callback when background event has been handled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnCompleted(IBackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs e)
        {
            try
            {
                // Update the UI with progress reported by the background task
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    try
                    {
                        // If the background task threw an exception, display the exception in
                        // the status text box.
                        e.CheckResult();
                    }
                    catch (Exception ex)
                    {
                        // The background task had an error
                        ShowStatus(ex.ToString());
                    }
                });

                await RestoreStateAsync();
                await AttemptDeferredCheckinsAsync();
                await LogBackgroundEventsAsync();
            }
            catch (Exception exception)
            {
                Logger.Trace(TraceLevel.Error, "Background Task: OnCompleted exception " + Logger.FormatException(exception));
            }
        }

        #region REST API wrappers
        // Note: these should not catch exceptions, let them propagate up to the UI handlers

        /// <summary>
        /// Retrieves the list of venues based on the current location.
        /// </summary>
        private async Task GetNearbyVenuesAsync()
        {
            _currentListViewMode = ListViewMode.Places;
            var position = _geoposition.Coordinate.Point.Position;
            bool accessDenied = false;

            // If selected index is valid (i.e. the user clicked on an exiting item), don't re-query and just use the cache.
            if ((_venuesCache != null) && (_currentSelectedIndex == -1))
            {
                // Re-query if we moved far enough
                if (GetDistanceInMetersBetweenPositions(position, _venuesCachePosition) >= Constants.VenuesQueryDistanceThresholdMeters)
                {
                    _venuesCache = null;
                }
            }

            if (_venuesCache == null)
            {
                Title.Text = "Finding Places from Foursquare ...";
 
                var resourceUri = String.Format("https://api.foursquare.com/v2/venues/explore?ll={0},{1}&limit={2}&v={3}", 
                                        Uri.EscapeDataString(position.Latitude.ToString()),
                                        Uri.EscapeDataString(position.Longitude.ToString()),
                                        Settings.QueryLimit,
                                        Constants.ApiVerifiedDate);

                try
                {
                    HttpResponseMessage response = await _httpClient.GetAsync(new Uri(resourceUri));
                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.Ok:
                            var responseBodyAsText = await response.Content.ReadAsStringAsync();
                            var responseBodyAsJson = JsonObject.Parse(responseBodyAsText);

                            _venuesCache = Venue.ListFromJson(responseBodyAsJson["response"].GetObject()) as List<Venue>;
                            _venuesCachePosition = position;
                            break;
                        case HttpStatusCode.Unauthorized: // token was revoked
                            Logger.Trace(TraceLevel.Error, "User disconnected this app from the Foursquare Settings page: " + response.StatusCode.ToString());
                            accessDenied = true;
                            break;
                        default:
                            Logger.Trace(TraceLevel.Error, "Failed to query venues from Foursquare: " + response.StatusCode.ToString());
                            break;
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    accessDenied = true;
                }
            }

            if (accessDenied)
            {
                Logger.Trace(TraceLevel.Info, "Access denied, deleting state.");
                await DeleteStateAsync();
            }

            if (_venuesCache != null)
            {
                Title.Text = _venuesCache.Count + " Recommended Places";
                ItemsListView.ItemsSource = _venuesCache;
                ScrollListViewToCurrentSelection();

                // Update map
                int index = 0;
                VenuesMapLayer.Children.Clear();
                foreach (var v in _venuesCache)
                {
                    var pushpin = new Bing.Maps.Pushpin();
                    pushpin.Text = index.ToString();
                    pushpin.Tapped += VenuePushpin_Tapped;
                    Bing.Maps.MapLayer.SetPosition(pushpin, new Bing.Maps.Location(v.Latitude, v.Longitude));
                    VenuesMapLayer.Children.Add(pushpin);
                    index++;
                }
            }
            else
            {
                Title.Text = "Places Unavailable";
            }
        }

        /// <summary>
        /// Retrieves the list of recent checkins.
        /// </summary>
        private async Task GetRecentCheckinsAsync()
        {
            _currentListViewMode = ListViewMode.MyCheckins;
            bool accessDenied = false;

            if (_checkinsCache == null)
            {
                Title.Text = "Getting Checkins...";
                var resourceUri = String.Format("https://api.foursquare.com/v2/users/self/checkins?limit={0}&v={1}",
                                    Settings.QueryLimit,
                                    Constants.ApiVerifiedDate);

                try
                {
                    HttpResponseMessage response = await _httpClient.GetAsync(new Uri(resourceUri));

                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.Ok:
                            var responseBodyAsText = await response.Content.ReadAsStringAsync();
                            var responseBodyAsJson = JsonObject.Parse(responseBodyAsText);
                            _checkinsCache = Checkin.ListFromJson(responseBodyAsJson["response"].GetObject());
                            break;
                        case HttpStatusCode.Unauthorized: // token was revoked
                            Logger.Trace(TraceLevel.Error, "User disconnected this app from the Foursquare Settings page: " + response.StatusCode.ToString());
                            accessDenied = true;
                            break;
                        default:
                            Logger.Trace(TraceLevel.Error, "Failed to get checkins from Foursquare: " + response.StatusCode.ToString());
                            break;
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    accessDenied = true;
                }
            }

            if (accessDenied)
            {
                Logger.Trace(TraceLevel.Info, "Access denied, deleting state.");
                await DeleteStateAsync();
            }

            if (_checkinsCache != null)
            {
                Title.Text = _checkinsCache.Count + " Latest Checkins";
                ItemsListView.ItemsSource = _checkinsCache;
            }
            else
            {
                Title.Text = "Checkins Unavailable";
            }
        }

        /// <summary>
        /// Checks in at a given venue
        /// </summary>
        /// <param name="v">The venue to check-in</param>
        private async Task<bool> CheckinAsync(Venue v)
        {
            bool checkedIn = false;
            bool accessDenied = false;

            var form = new HttpMultipartFormDataContent();
            form.Add(new HttpStringContent(v.Id), "venueId");
            form.Add(new HttpStringContent(v.Latitude.ToString() + "," + v.Longitude.ToString()), "ll");
            form.Add(new HttpStringContent("Geofencing 4Sq Sample auto-checkin"), "shout");
            form.Add(new HttpStringContent(Constants.ApiVerifiedDate), "v");

            try
            {
                var response = await _httpClient.PostAsync(new Uri("https://api.foursquare.com/v2/checkins/add"), form);

                switch (response.StatusCode)
                {
                    case HttpStatusCode.Ok:
                        checkedIn = true;
                        ShowToast(v.Name + " checked in");
                        Logger.Trace(TraceLevel.Debug, v.Name + " checked in");
                        _checkinsCache = null; // clear cache
                        break;
                    case HttpStatusCode.Unauthorized: // token was revoked
                        Logger.Trace(TraceLevel.Error, "User disconnected this app from the Foursquare Settings page: " + response.StatusCode.ToString());
                        accessDenied = true;
                        break;
                    default:
                        Logger.Trace(TraceLevel.Error, "Failed to checkin to Foursquare: " + response.StatusCode.ToString());
                        break;
                }
            }
            catch (UnauthorizedAccessException)
            {
                accessDenied = true;
            }

            if (accessDenied)
            {
                Logger.Trace(TraceLevel.Info, "Access denied, deleting state.");
                await DeleteStateAsync();
            }

            return checkedIn;
        }

        #endregion REST API wrappers

        #region UI control handlers

        private async void GetCheckins_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClearStatus();
                ClearItemsListView();
                AddGeofencesButton.Visibility = Visibility.Collapsed;
                AddGeofenceHereButton.Visibility = Visibility.Collapsed;
                DeleteGeofencesButton.Visibility = Visibility.Collapsed;
                CheckinButton.Visibility = Visibility.Collapsed;
                await GetRecentCheckinsAsync();
            }
            catch (Exception ex)
            {
                Logger.Trace(TraceLevel.Error, "Error displaying checkins: " + Logger.FormatException(ex));
                ShowStatus("Could not display checkins: " + ex.Message);
            }
        }

        private async void GetPlaces_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClearStatus();
                ClearItemsListView();
                AddGeofencesButton.Visibility = Visibility.Visible;
                AddGeofenceHereButton.Visibility = Visibility.Collapsed;
                DeleteGeofencesButton.Visibility = Visibility.Collapsed;
                CheckinButton.Visibility = Visibility.Visible;
                await GetNearbyVenuesAsync();
            }
            catch (Exception ex)
            {
                Logger.Trace(TraceLevel.Error, "Error returned by GetVenuesAsync() : " + Logger.FormatException(ex));
                ShowStatus("Could not get places from FourSquare: " + ex.Message);
            }
        }

        private async void AddGeofences_Click(object sender, RoutedEventArgs e)
        {
            ClearStatus();
            await RequestAutoCheckinPermissionIfNeverSetAsync();
            try
            {
                double radiusInMeters = Settings.GeofenceRadiusMeters;
                var selectedItems = ItemsListView.SelectedItems;
                var selectedCount = selectedItems.Count;
                if (selectedCount > 0)
                {
                    ShowStatus("Adding " + selectedCount + " geofences...");

                    var geofences = GeofenceMonitor.Current.Geofences;
                    foreach (Venue v in selectedItems)
                    {
                        BasicGeoposition position;
                        position.Altitude = 0;
                        position.Latitude = v.Latitude;
                        position.Longitude = v.Longitude;
                        var geocircle = new Geocircle(position, radiusInMeters);
                        var geofence = new Geofence(v.Id, geocircle);

                        try
                        {
                            geofences.Add(geofence);

                            Venue venue;
                            if (!_geofenceIdsToVenues.TryGetValue(v.Id, out venue))
                            {
                                _geofenceIdsToVenues.Add(v.Id, v);
                            }
                            Logger.Trace(TraceLevel.Info, "Added geofence for '" + v.Title + "' " + Logger.FormatLatLong(position.Latitude, position.Longitude) + ", radius: " + radiusInMeters + " meters, id: " + v.Id);
                        }
                        catch (Exception addEx)
                        {
                            if (addEx.HResult == -2147024198) // HRESULT_FROM_WIN32(ERROR_OBJECT_NAME_EXISTS)
                            {
                                Logger.Trace(TraceLevel.Info, "Geofence already exists for: " + v.Title);
                            }
                            else
                            {
                                throw (addEx);
                            }
                        }
                    }
                    ShowStatus("Added " + selectedCount + " geofences.");
                    GetGeofences_Click(this, null);
                }
                else
                {
                    ShowStatus("No places selected.");
                }
            }
            catch (Exception ex)
            {
                Logger.Trace(TraceLevel.Error, "Error adding geofences: " + Logger.FormatException(ex));
                ShowStatus("Could not add geofences: " + ex.Message);
            }

            await SaveStateAsync();
        }

        private void DeleteGeofences_Click(object sender, RoutedEventArgs e)
        {
            ClearStatus();
            try
            {
                var selectedItems = ItemsListView.SelectedItems;
                var selectedCount = selectedItems.Count;
                if (selectedCount > 0)
                {
                    ShowStatus("Deleting " + selectedCount + " geofences...");

                    if (selectedCount == ItemsListView.Items.Count)
                    {
                        GeofenceMonitor.Current.Geofences.Clear();
                    }
                    else
                    {
                        foreach (GeofenceItem gi in selectedItems)
                        {
                            gi.GeofenceCollection.Remove(gi.Geofence);
                            Logger.Trace(TraceLevel.Info, "Deleted geofence: " + gi.Title);
                        }
                    }
                    ShowStatus(selectedCount + " geofences deleted.");
                    GetGeofences_Click(this, null);
                }
                else
                {
                    ShowStatus("No geofences selected.");
                }
            }
            catch (Exception ex)
            {
                Logger.Trace(TraceLevel.Error, "Error deleting geofences: " + Logger.FormatException(ex));
                ShowStatus("Could not delete geofences: " + ex.Message);
            }
        }

        private async void DoCheckin_Click(object sender, RoutedEventArgs e)
        {
            ClearStatus();
            Title.Text = "Checking in ...";
            try
            {
                var selectedItems = ItemsListView.SelectedItems;
                if (selectedItems.Count > 0)
                {
                    foreach (Venue v in selectedItems)
                    {
                        if (await CheckinAsync(v))
                        {
                            ShowStatus("Checkin successful!");
                            Logger.Trace(TraceLevel.Debug, "Checked in at: " + v.Title);
                        }
                        else
                        {
                            ShowStatus("Checkin failed, please try again later");
                        }
                    }
                }
                else
                {
                    ShowStatus("No places selected.");
                }
            }
            catch (Exception ex)
            {
                Logger.Trace(TraceLevel.Error, "Error checking in: " + Logger.FormatException(ex));
                ShowStatus("Could not check-in: " + ex.Message);
            }
            Title.Text = "";
        }

        private void GetGeofences_Click(object sender, RoutedEventArgs e)
        {
            ClearStatus();
            ClearItemsListView();
            AddGeofencesButton.Visibility = Visibility.Collapsed;
            AddGeofenceHereButton.Visibility = Visibility.Visible;
            DeleteGeofencesButton.Visibility = Visibility.Collapsed;
            CheckinButton.Visibility = Visibility.Collapsed;
            GeofencesMapLayer.Children.Clear();
            _currentListViewMode = ListViewMode.MyGeofences;

            try
            {
                var manager = GeofenceMonitor.Current;
                var geofences = manager.Geofences;
                var geofenceCount = geofences.Count;

                if (geofenceCount > 0)
                {
                    int index = 0;
                    List<GeofenceItem> displayGeofences = new List<GeofenceItem>();
                    var count = displayGeofences.Count;
                    foreach (var geofence in geofences)
                    {
                        Venue venue = null;
                        if (_geofenceIdsToVenues.TryGetValue(geofence.Id, out venue))
                        {
                            // Foursquare venue (added from Foursquare places)
                            displayGeofences.Add(new GeofenceItem(geofence, venue, geofences));
                        }
                        else
                        {
                            // Custom geofence (added by "Geofence Here")
                            displayGeofences.Add(new GeofenceItem(geofence, geofences));
                        }

                        // Update map
                        var geocircle = geofence.Geoshape as Geocircle;
                        var geofenceMapControl = new GeofenceMapControl(Map)
                        {
                            ListViewIndex = index,
                            RadiusInMeters = geocircle.Radius
                        };

                        geofenceMapControl.Tapped += GeofenceMapControl_Tapped;
                        Bing.Maps.MapLayer.SetPosition(geofenceMapControl, new Bing.Maps.Location(geocircle.Center.Latitude, geocircle.Center.Longitude));
                        GeofencesMapLayer.Children.Add(geofenceMapControl);

                        index++;
                    }
                    ItemsListView.ItemsSource = displayGeofences;
                    ScrollListViewToCurrentSelection();

                    DeleteGeofencesButton.Visibility = Visibility.Visible;
                }
                Title.Text = geofenceCount + " Geofences";
            }
            catch (Exception ex)
            {
                Logger.Trace(TraceLevel.Error, "Error getting geofences: " + Logger.FormatException(ex));
                ShowStatus("Could not get geofences: " + ex.Message);
            }
        }

        private void AddGeofenceHere_Click(object sender, RoutedEventArgs e)
        {
            ClearStatus();

            try
            {
                double radiusInMeters = Settings.GeofenceRadiusMeters;
                ShowStatus("Adding geofence...");

                var geofences = GeofenceMonitor.Current.Geofences;

                var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff");
                BasicGeoposition position;
                position.Altitude = 0;
                position.Latitude = _geoposition.Coordinate.Point.Position.Latitude;
                position.Longitude = _geoposition.Coordinate.Point.Position.Longitude;

                var geocircle = new Geocircle(position, radiusInMeters);
                var geofence = new Geofence(timestamp, geocircle);
                geofences.Add(geofence);
                Logger.Trace(TraceLevel.Info, String.Format("Added geofence for current location: {0} radius: {1} meters, id {2}",
                    Logger.FormatLatLong(position.Latitude, position.Longitude), radiusInMeters, timestamp));

                // Re-center the map since the user wanted a geofence at the current location
                Map.SetView(new Bing.Maps.Location(position.Latitude, position.Longitude));

                GetGeofences_Click(this, null);
            }
            catch (Exception ex)
            {
                Logger.Trace(TraceLevel.Error, "Error adding geofence for current location: " + Logger.FormatException(ex));
                ShowStatus("Could not add geofence for current location: " + ex.Message);
            }
        }

        private void VenuePushpin_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var pushpin = sender as Bing.Maps.Pushpin;
            _currentSelectedIndex = Int32.Parse(pushpin.Text);
            if (_currentListViewMode == ListViewMode.Places)
            {
                ScrollListViewToCurrentSelection();
            }
            else
            {
                GetPlaces_Click(this, null);
            }
        }

        private void GeofenceMapControl_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var geofenceControl = sender as GeofenceMapControl;
            _currentSelectedIndex = geofenceControl.ListViewIndex;
            if (_currentListViewMode == ListViewMode.MyGeofences)
            {
                ScrollListViewToCurrentSelection();
            }
            else
            {
                GetGeofences_Click(this, null);
            }
        }

        private void ScrollListViewToCurrentSelection()
        {
            if ((_currentSelectedIndex >= 0) && (_currentSelectedIndex < ItemsListView.Items.Count))
            {
                ItemsListView.SelectedIndex = _currentSelectedIndex;
                ItemsListView.ScrollIntoView(ItemsListView.SelectedItem);
                _currentSelectedIndex = -1;
            }
        }

        private void ItemsListView_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            SelectionsMapLayer.Children.Clear();
            if (ItemsListView.SelectedItems.Count > 0)
            {
                if ((ListViewMode.MyGeofences == _currentListViewMode) || (ListViewMode.Places == _currentListViewMode))
                {
                    Bing.Maps.Location itemLocation = null;

                    // Add pushpins for all selected items
                    foreach (var item in ItemsListView.SelectedItems)
                    {
                        if (ListViewMode.MyGeofences == _currentListViewMode)
                        {
                            GeofenceItem g = item as GeofenceItem;
                            Geocircle geocircle = g.Geofence.Geoshape as Geocircle;
                            itemLocation = new Bing.Maps.Location(geocircle.Center.Latitude, geocircle.Center.Longitude);
                        }
                        else
                        {
                            Venue v = item as Venue;
                            itemLocation = new Bing.Maps.Location(v.Latitude, v.Longitude);
                        }

                        var pushPin = new Bing.Maps.Pushpin()
                        {
                            Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(100, 255, 23, 23)),
                        };
                        SelectionsMapLayer.Children.Add(pushPin);
                        Bing.Maps.MapLayer.SetPosition(pushPin, itemLocation);
                    }

                    // Center the map on the last seleced item
                    Map.SetView(itemLocation);
                }
            }
        }

        private void OnSettingsChanged(SettingsChangedEventArgs e)
        {
            switch (e.Type)
            {
                case SettingsType.QueryLimit:
                    _venuesCache = null;
                    Logger.Trace(TraceLevel.Info, "Foursquare query limit setting: " + (uint)e.NewValue + " places");
                    break;
                case SettingsType.GeofenceRadius:
                    Logger.Trace(TraceLevel.Info, "Geofence creation radius setting: " + (double)e.NewValue + " meters");
                    break;
                case SettingsType.AutoCheckinEnabled:
                    Logger.Trace(TraceLevel.Info, "Auto-checkin at Foursquare venue: " + ((bool)e.NewValue ? "on" : "off"));
                    break;
                default:
                    break;
            }
        }

        private async Task RequestAutoCheckinPermissionIfNeverSetAsync()
        {
            // Only request for permissions if it was never set
            if (!Settings.AutoCheckinStateSet)
            {
                var messageDialog = new MessageDialog("Automatically check-in when entering a Foursquare geofence? (You can change this later in App Settings.)");
                messageDialog.Commands.Add(new UICommand("Allow", new UICommandInvokedHandler(AutoCheckinPermissionsInvokedHandler)));
                messageDialog.Commands.Add(new UICommand("Deny", new UICommandInvokedHandler(AutoCheckinPermissionsInvokedHandler)));
                messageDialog.DefaultCommandIndex = 0;
                await messageDialog.ShowAsync();
            }
        }

        private void AutoCheckinPermissionsInvokedHandler(IUICommand command)
        {
            Settings.AutoCheckinEnabled = (command.Label == "Allow");
        }

        #endregion UI control handlers

        #region Utilities
        public double GetDistanceInMetersBetweenPositions(BasicGeoposition position1, BasicGeoposition position2)
        {
            // Computes distance between two positions using the Haversine formula
            double earthRadiusMeters = 6378137.0;

            double lat1 = (position1.Latitude) * Math.PI / 180.0;
            double lon1 = (position1.Longitude) * Math.PI / 180.0;
            double lat2 = (position2.Latitude) * Math.PI / 180.0;
            double lon2 = (position2.Longitude) * Math.PI / 180.0;

            double latDelta = lat2 - lat1;
            double lonDelta = lon2 - lon1;

            double a = Math.Pow(Math.Sin(latDelta / 2.0), 2.0) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(lonDelta / 2.0), 2.0);
            double c = 2.0 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1.0 - a));

            return earthRadiusMeters * c;
        }
        #endregion Utilities
    }
}
