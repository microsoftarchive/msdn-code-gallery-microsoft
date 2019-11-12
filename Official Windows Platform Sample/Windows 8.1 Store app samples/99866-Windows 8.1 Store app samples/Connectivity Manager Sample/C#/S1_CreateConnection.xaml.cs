//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;
using Windows.Networking.Connectivity;
using Windows.Foundation;

namespace ConnectivityManager
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class S1_CreateConnection : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        IAsyncOperation<ConnectionSession> connectionResult = null;

        public S1_CreateConnection()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // register for network state change
            NetworkInformation.NetworkStatusChanged += NetworkInformation_NetworkStatusChanged;

            if (rootPage.g_ConnectionSession != null)
            {
                // Ensure that we can still disconnect, even if we switch scenarios
                UpdateOutputText("Connected to " + rootPage.g_ConnectionSession.ConnectionProfile.ProfileName + "\n" +
                    "Connectivity Level: " + rootPage.g_ConnectionSession.ConnectionProfile.GetNetworkConnectivityLevel());

                UpdateButtonText("Disconnect");
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            NetworkInformation.NetworkStatusChanged -= NetworkInformation_NetworkStatusChanged;
        }

        /// <summary>
        /// Handler for NetworkStatusChanged events
        /// </summary>
        private void NetworkInformation_NetworkStatusChanged(object sender)
        {
            if (rootPage.g_ConnectionSession != null)
            {
                UpdateOutputText("Connected to " + rootPage.g_ConnectionSession.ConnectionProfile.ProfileName + "\n" +
                    "Connectivity Level: " + rootPage.g_ConnectionSession.ConnectionProfile.GetNetworkConnectivityLevel());
            }
        }


        /// <summary>
        /// This is the click handler for the 'Default' button.  You would replace this with your own handler
        /// if you have a button or buttons on this page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            switch (ConnectButton.Content.ToString())
            {
                case "Connect":
                    Connect();
                    break;
                case "Cancel":
                    Cancel();
                    break;
                case "Disconnect":
                    Disconnect();
                    break;
            }
        }

        private void ConnectionCompletedHandler(IAsyncOperation<ConnectionSession> asyncInfo, AsyncStatus asyncStatus)
        {
            if (asyncInfo.Status == AsyncStatus.Completed)
            {
                rootPage.g_ConnectionSession = asyncInfo.GetResults();

                UpdateOutputText("Connected to " + rootPage.g_ConnectionSession.ConnectionProfile.ProfileName + "\n" +
                    "Connectivity Level: " + rootPage.g_ConnectionSession.ConnectionProfile.GetNetworkConnectivityLevel());

                // Transition button to Disconnect state
                UpdateButtonText("Disconnect");

                connectionResult.Close();
                connectionResult = null;
            }
            else
            {
                UpdateOutputText(asyncStatus.ToString() + ": " + asyncInfo.ErrorCode.Message);

                UpdateButtonText("Connect");

                connectionResult = null;
            }
        }

        private CellularApnAuthenticationType ParseCellularApnAuthenticationType(String input)
        {
            switch (input)
            {
                case "Chap":
                    return CellularApnAuthenticationType.Chap;
                case "Mschapv2":
                    return CellularApnAuthenticationType.Mschapv2;
                case "Pap":
                    return CellularApnAuthenticationType.Pap;
                default:
                    return CellularApnAuthenticationType.None;
            }
        }

        private bool ParseCompressionEnabled(String input)
        {
            if (input.Equals("Yes"))
            {
                return true;
            }

            return false;
        }

        private void Connect()
        {
            OutputText.Text = "Attempting to connect!";

            // Fill in the CellularApnContext
            CellularApnContext cellularApnContext = new CellularApnContext();
            cellularApnContext.AccessPointName = AccessPointName.Text;
            cellularApnContext.ProviderId = ProviderId.Text;
            cellularApnContext.UserName = UserName.Text;
            cellularApnContext.Password = Password.Text;
            cellularApnContext.AuthenticationType = ParseCellularApnAuthenticationType(((ComboBoxItem)Authentication.SelectedItem).Content.ToString());
            cellularApnContext.IsCompressionEnabled = ParseCompressionEnabled(((ComboBoxItem)Compression.SelectedItem).Content.ToString());

            // Call AcquireConnectionAsync with the CellularApnContext, and set the handler
            connectionResult = Windows.Networking.Connectivity.ConnectivityManager.AcquireConnectionAsync(cellularApnContext);
            connectionResult.Completed = ConnectionCompletedHandler;

            // Transition button to Cancel state
            ConnectButton.Content = "Cancel";
        }

        private void Cancel()
        {
            OutputText.Text = "Connection canceled";

            if (connectionResult != null)
            {
                connectionResult.Cancel();
                connectionResult = null;
            }

            // Transition button to Connect state
            ConnectButton.Content = "Connect";
        }

        private void Disconnect()
        {
            OutputText.Text = "Disconnected";

            // Disconnect
            if (rootPage.g_ConnectionSession != null)
            {
                rootPage.g_ConnectionSession.Dispose();
                rootPage.g_ConnectionSession = null;
            }

            // Transition button to Disconnect state
            ConnectButton.Content = "Connect";
        }

        // Disconnect, and ensure that the connectionResult IAsyncOperation is closed.
        private void CleanUp()
        {
            if (rootPage.g_ConnectionSession != null)
            {
                rootPage.g_ConnectionSession.Dispose();
                rootPage.g_ConnectionSession = null;
            }

            if (connectionResult != null)
            {
                connectionResult.Close();
                connectionResult = null;
            }
        }

        private void UpdateButtonText(String text)
        {
            ConnectButton.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                ConnectButton.Content = text;
            });
        }

        private void UpdateOutputText(String text)
        {
            OutputText.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                OutputText.Text = text;
            });
        }
    }
}
