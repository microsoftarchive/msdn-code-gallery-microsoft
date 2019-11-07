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
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario2 : SDKTemplate.Common.LayoutAwarePage
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

                ScenarioOutput_Latitude.Text = pos.Coordinate.Latitude.ToString();
                ScenarioOutput_Longitude.Text = pos.Coordinate.Longitude.ToString();
                ScenarioOutput_Accuracy.Text = pos.Coordinate.Accuracy.ToString();
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
