//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using SDKTemplate;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Windows.Devices.Enumeration;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Microsoft.Samples.Devices.Geolocation
{
    public sealed partial class Scenario4 : Page
    {
        private bool nameSet = false;
        private bool latitudeSet = false;
        private bool longitudeSet = false;
        private bool radiusSet = false;
        private bool permissionsChecked = false;
        private bool inGetPositionAsync = false;
        private int secondsPerMinute = 60;
        private int secondsPerHour = 60 * 60;
        private int secondsPerDay = 24 * 60 * 60;
        private int oneHundredNanosecondsPerSecond = 10000000;
        private int  defaultDwellTimeSeconds = 10;
        private const int maxEventDescriptors = 42; // Value determined by how many max length event descriptors (91 chars) 
                                                    // stored as a JSON string can fit in 8K (max allowed for local settings)
        private CancellationTokenSource cts = null;
        private Geolocator geolocator = null;
        private IList<Geofence> geofences;
        private GeofenceItemCollection geofenceCollection = null;
        private EventDescriptorCollection eventCollection = null;
#if WINDOWS_APP
        private DeviceAccessInformation accessInfo;
#endif
        private Windows.Globalization.DateTimeFormatting.DateTimeFormatter formatterShortDateLongTime;
        private Windows.Globalization.DateTimeFormatting.DateTimeFormatter formatterLongTime;
        private Windows.Globalization.Calendar calendar;
        private Windows.Globalization.NumberFormatting.DecimalFormatter decimalFormatter;
        private Windows.UI.Core.CoreWindow coreWindow;

        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        private MainPage rootPage = MainPage.Current;

        public Scenario4()
        {
            this.InitializeComponent();

            try
            {
                formatterShortDateLongTime = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("{month.integer}/{day.integer}/{year.full} {hour.integer}:{minute.integer(2)}:{second.integer(2)}", new[] { "en-US" }, "US", Windows.Globalization.CalendarIdentifiers.Gregorian, Windows.Globalization.ClockIdentifiers.TwentyFourHour);
                formatterLongTime = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("{hour.integer}:{minute.integer(2)}:{second.integer(2)}", new[] { "en-US" }, "US", Windows.Globalization.CalendarIdentifiers.Gregorian, Windows.Globalization.ClockIdentifiers.TwentyFourHour);
                calendar = new Windows.Globalization.Calendar();
                decimalFormatter = new Windows.Globalization.NumberFormatting.DecimalFormatter();

                geofenceCollection = new GeofenceItemCollection();
                eventCollection = new EventDescriptorCollection();

                // Get a geolocator object
                geolocator = new Geolocator();

                geofences = GeofenceMonitor.Current.Geofences;

                // using data binding to the root page collection of GeofenceItems
                RegisteredGeofenceListBox.DataContext = geofenceCollection;

                // using data binding to the root page collection of GeofenceItems associated with events
                GeofenceEventsListBox.DataContext = eventCollection;

                FillRegisteredGeofenceListBoxWithExistingGeofences();
                FillEventListBoxWithExistingEvents();

#if WINDOWS_APP
                accessInfo = DeviceAccessInformation.CreateFromDeviceClass(DeviceClass.Location);
                accessInfo.AccessChanged += OnAccessChanged;
#endif

                coreWindow = CoreWindow.GetForCurrentThread(); // this needs to be set before InitializeComponent sets up event registration for app visibility
                coreWindow.VisibilityChanged += OnVisibilityChanged;

                // register for state change events
                GeofenceMonitor.Current.GeofenceStateChanged += OnGeofenceStateChanged;
                GeofenceMonitor.Current.StatusChanged += OnGeofenceStatusChanged;
            }
#if WINDOWS_APP
            catch (UnauthorizedAccessException)
            {
                if (DeviceAccessStatus.DeniedByUser == accessInfo.CurrentStatus)
                {
                    rootPage.NotifyUser("Location has been disabled by the user. Enable access through the settings charm.", NotifyType.StatusMessage);
                }
                else if (DeviceAccessStatus.DeniedBySystem == accessInfo.CurrentStatus)
                {
                    rootPage.NotifyUser("Location has been disabled by the system. The administrator of the device must enable location access through the location control panel.", NotifyType.StatusMessage);
                }
                else if (DeviceAccessStatus.Unspecified == accessInfo.CurrentStatus)
                {
                    rootPage.NotifyUser("Location has been disabled by unspecified source. The administrator of the device may need to enable location access through the location control panel, then enable access through the settings charm.", NotifyType.StatusMessage);
                }
            }
#endif
            catch (Exception ex)
            {
                // GeofenceMonitor failed in adding a geofence
                // exceptions could be from out of memory, lat/long out of range,
                // too long a name, not a unique name, specifying an activation
                // time + duration that is still in the past

                // If ex.HResult is RPC_E_DISCONNECTED (0x80010108):
                // The Location Framework service event state is out of synchronization
                // with the Windows.Devices.Geolocation.Geofencing.GeofenceMonitor.
                // To recover remove all event handlers on the GeofenceMonitor or restart the application.
                // Once all event handlers are removed you may add back any event handlers and retry the operation.

                rootPage.NotifyUser(ex.ToString(), NotifyType.ErrorMessage);
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached. The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        /// <summary>
        /// Invoked immediately before the Page is unloaded and is no longer the current source of a parent Frame.
        /// </summary>
        /// <param name="e">
        /// Event data that can be examined by overriding code. The event data is representative
        /// of the navigation that will unload the current Page unless canceled. The
        /// navigation can potentially be canceled by setting e.Cancel to true.
        /// </param>
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (true == inGetPositionAsync)
            {
                if (cts != null)
                {
                    cts.Cancel();
                    cts = null;
                }
            }

            GeofenceMonitor.Current.GeofenceStateChanged -= OnGeofenceStateChanged;
            GeofenceMonitor.Current.StatusChanged -= OnGeofenceStatusChanged;

            // save off the contents of the event collection
            SaveExistingEvents();

            base.OnNavigatingFrom(e);
        }

        private void OnVisibilityChanged(CoreWindow sender, VisibilityChangedEventArgs args)
        {
            // NOTE: After the app is no longer visible on the screen and before the app is suspended
            // you might want your app to use toast notification for any geofence activity.
            // By registering for VisibiltyChanged the app is notified when the app is no longer visible in the foreground.

            if (args.Visible)
            {
                // register for foreground events
                GeofenceMonitor.Current.GeofenceStateChanged += OnGeofenceStateChanged;
                GeofenceMonitor.Current.StatusChanged += OnGeofenceStatusChanged;
            }
            else
            {
                // unregister foreground events (let background capture events)
                GeofenceMonitor.Current.GeofenceStateChanged -= OnGeofenceStateChanged;
                GeofenceMonitor.Current.StatusChanged -= OnGeofenceStatusChanged;
            }
        }

#if WINDOWS_APP
        public async void OnAccessChanged(DeviceAccessInformation sender, DeviceAccessChangedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var status = args.Status;

                string eventDescription = GetTimeStampedMessage("Device Access Status");
                
                eventDescription += " (" + status.ToString() + ")";

                AddEventDescription(eventDescription);

                if (DeviceAccessStatus.DeniedByUser == args.Status)
                {
                    rootPage.NotifyUser("Location has been disabled by the user. Enable access through the settings charm.", NotifyType.StatusMessage);
                }
                else if (DeviceAccessStatus.DeniedBySystem == args.Status)
                {
                    rootPage.NotifyUser("Location has been disabled by the system. The administrator of the device must enable location access through the location control panel.", NotifyType.StatusMessage);
                }
                else if (DeviceAccessStatus.Unspecified == args.Status)
                {
                    rootPage.NotifyUser("Location has been disabled by unspecified source. The administrator of the device may need to enable location access through the location control panel, then enable access through the settings charm.", NotifyType.StatusMessage);
                }
                else if (DeviceAccessStatus.Allowed == args.Status)
                {
                    // clear status
                    rootPage.NotifyUser("", NotifyType.StatusMessage);
                }
                else
                {
                    rootPage.NotifyUser("Unknown device access information status", NotifyType.StatusMessage);
                }
            });
        }
#endif

        public async void OnGeofenceStatusChanged(GeofenceMonitor sender, object e)
        {
            var status = sender.Status;

            string eventDescription = GetTimeStampedMessage("Geofence Status Changed");

            eventDescription += " (" + status.ToString() + ")";

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                AddEventDescription(eventDescription);
            });
        }

        public async void OnGeofenceStateChanged(GeofenceMonitor sender, object e)
        {
            var reports = sender.ReadReports();

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                foreach (GeofenceStateChangeReport report in reports)
                {
                    GeofenceState state = report.NewState;

                    Geofence geofence = report.Geofence;
                    string eventDescription = GetTimeStampedMessage(geofence.Id);

                    eventDescription += " (" + state.ToString();

                    if (state == GeofenceState.Removed)
                    {
                        eventDescription += "/" + report.RemovalReason.ToString() + ")";

                        AddEventDescription(eventDescription);

                        // remove the geofence from the client side geofences collection
                        Remove(geofence);

                        // empty the registered geofence listbox and repopulate
                        geofenceCollection.Clear();

                        FillRegisteredGeofenceListBoxWithExistingGeofences();
                    }
                    else if (state == GeofenceState.Entered || state == GeofenceState.Exited)
                    {
                        // NOTE: You might want to write your app to take particular
                        // action based on whether the app has internet connectivity.

                        eventDescription += ")";

                        AddEventDescription(eventDescription);
                    }
                }
            });
        }

        /// <summary>
        /// This method removes the geofence from the client side geofences collection
        /// </summary>
        /// <param name="geofence"></param>
        private void Remove(Geofence geofence)
        {
            try
            {
                if (!geofences.Remove(geofence))
                {
                    var strMsg = "Could not find Geofence " + geofence.Id + " in the geofences collection";

                    rootPage.NotifyUser(strMsg, NotifyType.StatusMessage);
                }
            }
            catch (Exception ex)
            {
                rootPage.NotifyUser(ex.ToString(), NotifyType.ErrorMessage);
            }
        }

        /// <summary>
        /// This is the click handler for the 'Remove Geofence Item' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRemoveGeofenceItem(object sender, RoutedEventArgs e)
        {
            if (null != RegisteredGeofenceListBox.SelectedItem)
            {
                // get selected item
                GeofenceItem itemToRemove = RegisteredGeofenceListBox.SelectedItem as GeofenceItem;

                var geofence = itemToRemove.Geofence;

                // remove the geofence from the client side geofences collection
                Remove(geofence);

                // empty the registered geofence listbox and repopulate
                geofenceCollection.Clear();

                FillRegisteredGeofenceListBoxWithExistingGeofences();
            }
        }

        private Geofence GenerateGeofence()
        {
            Geofence geofence = null;

            string fenceKey = new string(Id.Text.ToCharArray());

            BasicGeoposition position;
            position.Latitude = Double.Parse(Latitude.Text);
            position.Longitude = Double.Parse(Longitude.Text);
            position.Altitude = 0.0;
            double radius = Double.Parse(Radius.Text);

            // the geofence is a circular region
            Geocircle geocircle = new Geocircle(position, radius);

            bool singleUse = (bool)SingleUse.IsChecked;

            // want to listen for enter geofence, exit geofence and remove geofence events
            // you can select a subset of these event states
            MonitoredGeofenceStates mask = 0;

            mask |= MonitoredGeofenceStates.Entered;
            mask |= MonitoredGeofenceStates.Exited;
            mask |= MonitoredGeofenceStates.Removed;

            // setting up how long you need to be in geofence for enter event to fire
            TimeSpan dwellTime;

            if ("" != DwellTime.Text)
            {
                dwellTime = new TimeSpan(ParseTimeSpan(DwellTime.Text, defaultDwellTimeSeconds));
            }
            else
            {
                dwellTime = new TimeSpan(ParseTimeSpan("0", defaultDwellTimeSeconds));
            }

            // setting up how long the geofence should be active
            TimeSpan duration;

            if ("" != Duration.Text)
            {
                duration = new TimeSpan(ParseTimeSpan(Duration.Text, 0));
            }
            else
            {
                duration = new TimeSpan(ParseTimeSpan("0", 0));
            }

            // setting up the start time of the geofence
            DateTimeOffset startTime;

            try
            {
                if ("" != StartTime.Text)
                {
                    startTime = DateTimeOffset.Parse(StartTime.Text); 
                }
                else
                {
                    // if you don't set start time in C# the start time defaults to 1/1/1601
                    calendar.SetToNow();

                    startTime = calendar.GetDateTime();
                }
            }
            catch (ArgumentNullException)
            {
            }
            catch (FormatException)
            {
                rootPage.NotifyUser("Start Time is not a valid string representation of a date and time", NotifyType.ErrorMessage);
            }
            catch (ArgumentException)
            {
                rootPage.NotifyUser("The offset is greater than 14 hours or less than -14 hours.", NotifyType.ErrorMessage);
            }

            geofence = new Geofence(fenceKey, geocircle, mask, singleUse, dwellTime, startTime, duration);

            return geofence;
        }
    }
}
