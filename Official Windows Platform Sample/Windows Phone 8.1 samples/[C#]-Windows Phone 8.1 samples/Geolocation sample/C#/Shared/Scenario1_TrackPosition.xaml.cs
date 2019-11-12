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
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Microsoft.Samples.Devices.Geolocation
{
    public sealed partial class Scenario1 : Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        private Geolocator _geolocator = null;

        public Scenario1()
        {
            this.InitializeComponent();

            _geolocator = new Geolocator();

#if WINDOWS_PHONE_APP
            // You must set the MovementThreshold for 
            // distance-based tracking or ReportInterval for
            // periodic-based tracking before adding event handlers.
            //
            // Value of 1000 milliseconds (1 second)
            // isn't a requirement, it is just an example.
            _geolocator.ReportInterval = 1000;
#endif
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached. The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            StartTrackingButton.IsEnabled = true;
            StopTrackingButton.IsEnabled = false;
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
            if (StopTrackingButton.IsEnabled)
            {
                _geolocator.PositionChanged -= new TypedEventHandler<Geolocator, PositionChangedEventArgs>(OnPositionChanged);
                _geolocator.StatusChanged -= new TypedEventHandler<Geolocator, StatusChangedEventArgs>(OnStatusChanged);
            }

            base.OnNavigatingFrom(e);
        }

        /// <summary>
        /// This is the event handler for PositionChanged events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async private void OnPositionChanged(Geolocator sender, PositionChangedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Geoposition pos = e.Position;

                rootPage.NotifyUser("Updated", NotifyType.StatusMessage);

                ScenarioOutput_Latitude.Text = pos.Coordinate.Point.Position.Latitude.ToString();
                ScenarioOutput_Longitude.Text = pos.Coordinate.Point.Position.Longitude.ToString();
                ScenarioOutput_Accuracy.Text = pos.Coordinate.Accuracy.ToString();
            });
        }

        /// <summary>
        /// This is the event handler for StatusChanged events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async private void OnStatusChanged(Geolocator sender, StatusChangedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                switch (e.Status)
                {
                case PositionStatus.Ready:
                    // Location platform is providing valid data.
                    ScenarioOutput_Status.Text = "Ready";
                    break;

                case PositionStatus.Initializing:
                    // Location platform is acquiring a fix. It may or may not have data. Or the data may be less accurate.
                    ScenarioOutput_Status.Text = "Initializing";
                    break;

                case PositionStatus.NoData:
                    // Location platform could not obtain location data.
                    ScenarioOutput_Status.Text = "No data";
                    break;

                case PositionStatus.Disabled:
                    // The permission to access location data is denied by the user or other policies.
                    ScenarioOutput_Status.Text = "Disabled";

                    //Clear cached location data if any
                    ScenarioOutput_Latitude.Text = "No data";
                    ScenarioOutput_Longitude.Text = "No data";
                    ScenarioOutput_Accuracy.Text = "No data";
                    break;

                case PositionStatus.NotInitialized:
                    // The location platform is not initialized. This indicates that the application has not made a request for location data.
                    ScenarioOutput_Status.Text = "Not initialized";
                    break;

                case PositionStatus.NotAvailable:
                    // The location platform is not available on this version of the OS.
                    ScenarioOutput_Status.Text = "Not available";
                    break;

                default:
                    ScenarioOutput_Status.Text = "Unknown";
                    break;
                }
            });
        }

        /// <summary>
        /// This is the click handler for the 'StartTracking' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartTracking(object sender, RoutedEventArgs e)
        {
            rootPage.NotifyUser("Waiting for update...", NotifyType.StatusMessage);

            _geolocator.PositionChanged += new TypedEventHandler<Geolocator, PositionChangedEventArgs>(OnPositionChanged);
            _geolocator.StatusChanged += new TypedEventHandler<Geolocator, StatusChangedEventArgs>(OnStatusChanged);

            StartTrackingButton.IsEnabled = false;
            StopTrackingButton.IsEnabled = true;
        }

        /// <summary>
        /// This is the click handler for the 'StopTracking' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopTracking(object sender, RoutedEventArgs e)
        {
            _geolocator.PositionChanged -= new TypedEventHandler<Geolocator, PositionChangedEventArgs>(OnPositionChanged);
            _geolocator.StatusChanged -= new TypedEventHandler<Geolocator, StatusChangedEventArgs>(OnStatusChanged);

            StartTrackingButton.IsEnabled = true;
            StopTrackingButton.IsEnabled = false;
        }
    }
}
