//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Networking.Proximity;
using System;
using Windows.UI.Core;

namespace SDKTemplate
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProximityDeviceEvents : Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;
        private Windows.Networking.Proximity.ProximityDevice _proximityDevice;

        public ProximityDeviceEvents()
        {
            this.InitializeComponent();
            _proximityDevice = ProximityDevice.GetDefault();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (_proximityDevice != null)
            {
                _proximityDevice.DeviceArrived += DeviceArrived;
                _proximityDevice.DeviceDeparted += DeviceDeparted;
            }
            else
            {
                rootPage.NotifyUser("No proximity device found", NotifyType.ErrorMessage);
            }
        }
         // Invoked when the main page navigates to a different scenario
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (_proximityDevice != null)
            {
                _proximityDevice.DeviceArrived -= DeviceArrived;
                _proximityDevice.DeviceDeparted -= DeviceDeparted;
            }
        }

        void DeviceArrived(ProximityDevice proximityDevice)
        {
            var ignored = Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                ProximityDeviceEventsOutputText.Text += "Proximate device arrived\n";
            });
        }

        void DeviceDeparted(ProximityDevice proximityDevice)
        {
            var ignored = Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                ProximityDeviceEventsOutputText.Text += "Proximate device departed\n";
            });
        }
    }
}
