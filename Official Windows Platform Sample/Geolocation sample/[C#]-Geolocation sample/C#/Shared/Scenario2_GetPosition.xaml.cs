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
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Microsoft.Samples.Devices.Geolocation
{
    public sealed partial class Scenario2 : Page
    {
        private Geolocator _geolocator = null;
        private CancellationTokenSource _cts = null;

        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario2()
        {
            this.InitializeComponent();

            _geolocator = new Geolocator();

#if WINDOWS_PHONE_APP
            // Desired Accuracy needs to be set
            // before polling for desired accuracy.
            _geolocator.DesiredAccuracyInMeters = 10;
#endif

            DesiredAccuracyInMeters.TextChanged += DesiredAccuracyInMeters_TextChanged;
            SetDesiredAccuracyInMeters.Click += SetDesiredAccuracyInMeters_Click;
        }

        void SetDesiredAccuracyInMeters_Click(object sender, RoutedEventArgs e)
        {
            uint desiredAccuracy = uint.Parse(DesiredAccuracyInMeters.Text);

            _geolocator.DesiredAccuracyInMeters = desiredAccuracy;

            // update get field
            ScenarioOutput_DesiredAccuracyInMeters.Text = _geolocator.DesiredAccuracyInMeters.ToString();
        }

        void DesiredAccuracyInMeters_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                uint value = uint.Parse(DesiredAccuracyInMeters.Text);

                SetDesiredAccuracyInMeters.IsEnabled = true;

                // clear out status message
                rootPage.NotifyUser("", NotifyType.StatusMessage);
            }
            catch (ArgumentNullException)
            {
                SetDesiredAccuracyInMeters.IsEnabled = false;
            }
            catch (FormatException)
            {
                rootPage.NotifyUser("Desired Accuracy must be a number", NotifyType.StatusMessage);
                SetDesiredAccuracyInMeters.IsEnabled = false;
            }
            catch (OverflowException)
            {
                rootPage.NotifyUser("Desired Accuracy is out of bounds", NotifyType.StatusMessage);
                SetDesiredAccuracyInMeters.IsEnabled = false;
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached. The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            GetGeolocationButton.IsEnabled = true;
            CancelGetGeolocationButton.IsEnabled = false;
            DesiredAccuracyInMeters.IsEnabled = false;
            SetDesiredAccuracyInMeters.IsEnabled = false;
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
            if (_cts != null)
            {
                _cts.Cancel();
                _cts = null;
            }

            base.OnNavigatingFrom(e);
        }

        /// <summary>
        /// This is the click handler for the 'getGeolocation' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async private void GetGeolocation(object sender, RoutedEventArgs e)
        {
            GetGeolocationButton.IsEnabled = false;
            CancelGetGeolocationButton.IsEnabled = true;

            try
            {
                // Get cancellation token
                _cts = new CancellationTokenSource();
                CancellationToken token = _cts.Token;

                rootPage.NotifyUser("Waiting for update...", NotifyType.StatusMessage);

                // Carry out the operation
                Geoposition pos = await _geolocator.GetGeopositionAsync().AsTask(token);

                rootPage.NotifyUser("Updated", NotifyType.StatusMessage);

                DesiredAccuracyInMeters.IsEnabled = true;

                ScenarioOutput_Latitude.Text = pos.Coordinate.Point.Position.Latitude.ToString();
                ScenarioOutput_Longitude.Text = pos.Coordinate.Point.Position.Longitude.ToString();
                ScenarioOutput_Accuracy.Text = pos.Coordinate.Accuracy.ToString();
                ScenarioOutput_Source.Text = pos.Coordinate.PositionSource.ToString();

                if (pos.Coordinate.PositionSource == PositionSource.Satellite)
                {
                    // show labels and satellite data
                    Label_PosPrecision.Visibility = Visibility.Visible;
                    ScenarioOutput_PosPrecision.Visibility = Visibility.Visible;
                    ScenarioOutput_PosPrecision.Text = pos.Coordinate.SatelliteData.PositionDilutionOfPrecision.ToString();
                    Label_HorzPrecision.Visibility = Visibility.Visible;
                    ScenarioOutput_HorzPrecision.Visibility = Visibility.Visible;
                    ScenarioOutput_HorzPrecision.Text = pos.Coordinate.SatelliteData.HorizontalDilutionOfPrecision.ToString();
                    Label_VertPrecision.Visibility = Visibility.Visible;
                    ScenarioOutput_VertPrecision.Visibility = Visibility.Visible;
                    ScenarioOutput_VertPrecision.Text = pos.Coordinate.SatelliteData.VerticalDilutionOfPrecision.ToString();
                }
                else
                {
                    // hide labels and satellite data
                    Label_PosPrecision.Visibility = Visibility.Collapsed;
                    ScenarioOutput_PosPrecision.Visibility = Visibility.Collapsed;
                    Label_HorzPrecision.Visibility = Visibility.Collapsed;
                    ScenarioOutput_HorzPrecision.Visibility = Visibility.Collapsed;
                    Label_VertPrecision.Visibility = Visibility.Collapsed;
                    ScenarioOutput_VertPrecision.Visibility = Visibility.Collapsed;
                }

                ScenarioOutput_DesiredAccuracyInMeters.Text = _geolocator.DesiredAccuracyInMeters.ToString();

            }
            catch (System.UnauthorizedAccessException)
            {
                rootPage.NotifyUser("Disabled", NotifyType.StatusMessage);

                ScenarioOutput_Latitude.Text = "No data";
                ScenarioOutput_Longitude.Text = "No data";
                ScenarioOutput_Accuracy.Text = "No data";
            }
            catch (TaskCanceledException)
            {
                rootPage.NotifyUser("Canceled", NotifyType.StatusMessage);
            }
            catch (Exception ex)
            {
#if WINDOWS_APP
                // If there are no location sensors GetGeopositionAsync()
                // will timeout -- that is acceptable.
                const int WaitTimeoutHResult = unchecked((int)0x80070102);

                if (ex.HResult == WaitTimeoutHResult) // WAIT_TIMEOUT
                {
                    rootPage.NotifyUser("Operation accessing location sensors timed out. Possibly there are no location sensors.", NotifyType.StatusMessage);
                }
                else
#endif
                {
                    rootPage.NotifyUser(ex.ToString(), NotifyType.ErrorMessage);
                }
            }
            finally
            {
                _cts = null;
            }

            GetGeolocationButton.IsEnabled = true;
            CancelGetGeolocationButton.IsEnabled = false;
        }

        /// <summary>
        /// This is the click handler for the 'CancelGetGeolocation' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelGetGeolocation(object sender, RoutedEventArgs e)
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts = null;
            }

            GetGeolocationButton.IsEnabled = true;
            CancelGetGeolocationButton.IsEnabled = false;
        }
    }
}
