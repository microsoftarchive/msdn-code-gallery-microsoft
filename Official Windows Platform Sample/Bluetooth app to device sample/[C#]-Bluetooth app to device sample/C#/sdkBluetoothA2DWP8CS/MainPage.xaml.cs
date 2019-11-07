/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using sdkBluetoothA2DWP8CS.Resources;
using Windows.Networking.Proximity;
using Windows.Networking.Sockets;

namespace sdkBluetoothA2DWP8CS
{
    public partial class MainPage : PhoneApplicationPage
    {
        ObservableCollection<PairedDeviceInfo> _pairedDevices;  // a local copy of paired device information
        StreamSocket _socket; // socket object used to communicate with the device

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Bluetooth is not available in the emulator. 
            if (Microsoft.Devices.Environment.DeviceType == Microsoft.Devices.DeviceType.Emulator)
            {
                MessageBox.Show(AppResources.Msg_EmulatorMode,"Warning",MessageBoxButton.OK);
            }

            _pairedDevices = new ObservableCollection<PairedDeviceInfo>();
            PairedDevicesList.ItemsSource = _pairedDevices;
        }

        private void FindPairedDevices_Tap(object sender, GestureEventArgs e)
        {
            RefreshPairedDevicesList();
        }

        /// <summary>
        /// Asynchronous call to re-populate the ListBox of paired devices.
        /// </summary>
        private async void RefreshPairedDevicesList()
        {
            try
            {
                // Search for all paired devices
                PeerFinder.AlternateIdentities["Bluetooth:Paired"] = "";
                var peers = await PeerFinder.FindAllPeersAsync();

                // By clearing the backing data, we are effectively clearing the ListBox
                _pairedDevices.Clear();

                if (peers.Count == 0)
                {
                    MessageBox.Show(AppResources.Msg_NoPairedDevices);
                }
                else
                {
                    // Found paired devices.
                    foreach (var peer in peers)
                    {
                        _pairedDevices.Add(new PairedDeviceInfo(peer));
                    }
                }
            }
            catch (Exception ex)
            {
                if ((uint)ex.HResult == 0x8007048F)
                {
                    var result = MessageBox.Show(AppResources.Msg_BluetoothOff, "Bluetooth Off", MessageBoxButton.OKCancel);
                    if (result == MessageBoxResult.OK)
                    {
                        ShowBluetoothcControlPanel();
                    }
                }
                else if ((uint)ex.HResult == 0x80070005)
                {
                    MessageBox.Show(AppResources.Msg_MissingCaps);
                }
                else
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void ConnectToSelected_Tap_1(object sender, GestureEventArgs e)
        {
            // Because I enable the ConnectToSelected button only if the user has
            // a device selected, I don't need to check here whether that is the case.

            // Connect to the device
            PairedDeviceInfo pdi = PairedDevicesList.SelectedItem as PairedDeviceInfo;
            PeerInformation peer = pdi.PeerInfo;

            // Asynchronous call to connect to the device
            ConnectToDevice(peer);
        }

        private async void ConnectToDevice(PeerInformation peer)
        {
            if (_socket != null)
            {
                // Disposing the socket with close it and release all resources associated with the socket
                _socket.Dispose();
            }

            try
            {
                _socket = new StreamSocket();
                string serviceName = (String.IsNullOrWhiteSpace(peer.ServiceName)) ? tbServiceName.Text : peer.ServiceName;

                // Note: If either parameter is null or empty, the call will throw an exception
                await _socket.ConnectAsync(peer.HostName, serviceName);

                // If the connection was successful, the RemoteAddress field will be populated
               MessageBox.Show(String.Format(AppResources.Msg_ConnectedTo, _socket.Information.RemoteAddress.DisplayName));
            }
            catch (Exception ex)
            {
                // In a real app, you would want to take action dependent on the type of 
                // exception that occurred.
                MessageBox.Show(ex.Message);

                _socket.Dispose();
                _socket = null;
            }
        }

        private void PairedDevicesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check whether the user has selected a device
            if (PairedDevicesList.SelectedItem == null)
            {
                // No - hide these fields
                ConnectToSelected.IsEnabled = false;
                ServiceNameInput.Visibility = Visibility.Collapsed;
            }
            else
            {
                // Yes - enable the connect button
                ConnectToSelected.IsEnabled = true;

                // Show the service name field, if the ServiceName associated with this device is currently empty
                PairedDeviceInfo pdi = PairedDevicesList.SelectedItem as PairedDeviceInfo;
                ServiceNameInput.Visibility = (String.IsNullOrWhiteSpace(pdi.ServiceName)) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void ShowBluetoothcControlPanel()
        {
            ConnectionSettingsTask connectionSettingsTask = new ConnectionSettingsTask();
            connectionSettingsTask.ConnectionSettingsType = ConnectionSettingsType.Bluetooth;
            connectionSettingsTask.Show();
        }
    }

    /// <summary>
    ///  Class to hold all paired device information
    /// </summary>
    public class PairedDeviceInfo
    {
        internal PairedDeviceInfo(PeerInformation peerInformation)
        {
            this.PeerInfo = peerInformation;
            this.DisplayName = this.PeerInfo.DisplayName;
            this.HostName = this.PeerInfo.HostName.DisplayName;
            this.ServiceName = this.PeerInfo.ServiceName;
        }

        public string DisplayName { get; private set; }
        public string HostName { get; private set; }
        public string ServiceName { get; private set; }
        public PeerInformation PeerInfo { get; private set; }
    }
}
