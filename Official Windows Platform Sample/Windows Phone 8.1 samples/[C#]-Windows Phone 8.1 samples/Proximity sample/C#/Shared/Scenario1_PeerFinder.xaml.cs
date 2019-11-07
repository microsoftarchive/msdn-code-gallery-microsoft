//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;
using Windows.Networking.Proximity;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SDKTemplate
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PeerFinderScenario : Page, System.IDisposable
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as rootPage.NotifyUser()
        MainPage rootPage = MainPage.Current;
        
        // List returned by FindAllPeersAsync
        private IReadOnlyList<PeerInformation> _peerInformationList;

        private PeerInformation _requestingPeer;
        private bool _triggeredConnectSupported = false;
        private bool _browseConnectSupported = false;
        private bool _launchByTap = false;

        // Helper to encapsulate the StreamSocket work
        private SocketHelper _socketHelper = new SocketHelper();

        public PeerFinderScenario()
        {
            this.InitializeComponent();

            this.Loaded += OnLoaded;
            _socketHelper.RaiseSocketErrorEvent += SocketErrorHandler;
            _socketHelper.RaiseMessageEvent += MessageHandler;

            // Scenario 1 init
            _triggeredConnectSupported = (PeerFinder.SupportedDiscoveryTypes & PeerDiscoveryTypes.Triggered) ==
                                         PeerDiscoveryTypes.Triggered;
            _browseConnectSupported = (PeerFinder.SupportedDiscoveryTypes & PeerDiscoveryTypes.Browse) ==
                                      PeerDiscoveryTypes.Browse;

            Window.Current.SizeChanged += Current_SizeChanged;
        }

        // Handles PeerFinder.TriggeredConnectionStateChanged event
        async private void TriggeredConnectionStateChangedEventHandler(object sender, TriggeredConnectionStateChangedEventArgs eventArgs)
        {
            var ignored = Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                PeerFinderOutputText.Text += "TriggeredConnectionStateChangedEventHandler - " + Enum.GetName(typeof(ConnectState), (int)eventArgs.State) + "\n";
            });

            if (eventArgs.State == TriggeredConnectState.PeerFound)
            {
                // Use this state to indicate to users that the tap is complete and
                // they can pull their devices away.
                ignored = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    rootPage.NotifyUser("Tap complete, socket connection starting!", NotifyType.StatusMessage);
                });
            }

            if (eventArgs.State == TriggeredConnectState.Completed)
            {
                ignored = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    rootPage.NotifyUser("Socket connect success!", NotifyType.StatusMessage);
                });
                // Start using the socket that just connected.
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    this.PeerFinder_StartSendReceive(eventArgs.Socket, null);
                });
            }

            if (eventArgs.State == TriggeredConnectState.Failed)
            {
                // The socket conenction failed
                ignored = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    rootPage.NotifyUser("Socket connect failed!", NotifyType.ErrorMessage);
                });
            }
        }

        private bool _peerFinderStarted = false;

        private void SocketErrorHandler(object sender, SocketEventArgs e)
        {
            rootPage.NotifyUser(e.Message, NotifyType.ErrorMessage);
            ToggleAdvertiseControls(true);

            // Browse and DiscoveryData controls are valid for Browse support
            if (_browseConnectSupported)
            {
                PeerFinder_BrowsePeersButton.Visibility = Visibility.Visible;
            }

            ToggleConnectedControls(false);
            // Clear the SendToPeerList
            PeerFinder_SendToPeerList.Items.Clear();

            _socketHelper.CloseSocket();
        }

        private void MessageHandler(object sender, MessageEventArgs e)
        {
            rootPage.NotifyUser(e.Message, NotifyType.StatusMessage);
        }

        // Send message to the selected peer(s)
        // Handles PeerFinder_SendButton click
        private void PeerFinder_Send(object sender, RoutedEventArgs e)
        {
            rootPage.NotifyUser("", NotifyType.ErrorMessage);
            String message = PeerFinder_MessageBox.Text;
            PeerFinder_MessageBox.Text = ""; // clear the input now that the message is being sent.
            int idx = PeerFinder_SendToPeerList.SelectedIndex - 1;

            if (message.Length > 0)
            {
                // Send message to all peers
                if (((ComboBoxItem)(PeerFinder_SendToPeerList.SelectedItem)).Content.ToString() == "All Peers")
                {
                    foreach (ConnectedPeer obj in _socketHelper.ConnectedPeers)
                    {
                        _socketHelper.SendMessageToPeer(message, obj);
                    }
                }
                else if ((idx >= 0) && (idx < _socketHelper.ConnectedPeers.Count))
                {
                    // Sned message to selected peer
                    _socketHelper.SendMessageToPeer(message, (_socketHelper.ConnectedPeers)[idx]);
                }
            }
            else
            {
                rootPage.NotifyUser("Please type a message", NotifyType.ErrorMessage);
            }
        }

        // Handles PeerFinder_AcceptButton click
        async private void PeerFinder_Accept(object sender, RoutedEventArgs e)
        {
            rootPage.NotifyUser("Connecting to " + _requestingPeer.DisplayName + "....", NotifyType.StatusMessage);
            PeerFinder_AcceptButton.Visibility = Visibility.Collapsed;
            try
            {
                // Connect to the incoming peer
                StreamSocket socket = await PeerFinder.ConnectAsync(_requestingPeer);
                rootPage.NotifyUser("Connection succeeded", NotifyType.StatusMessage);
                PeerFinder_StartSendReceive(socket, _requestingPeer);
            }
            catch (Exception err)
            {
                rootPage.NotifyUser("Connection to " + _requestingPeer.DisplayName + " failed: " + err.Message, NotifyType.ErrorMessage);
            }
        }

        // This gets called when we receive a connect request from a Peer
        private void PeerConnectionRequested(object sender, ConnectionRequestedEventArgs args)
        {
            _requestingPeer = args.PeerInformation;
            var ignored = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                rootPage.NotifyUser("Connection requested from peer " + args.PeerInformation.DisplayName, NotifyType.StatusMessage);

                HideAllControls();
                this.PeerFinder_AcceptButton.Visibility = Visibility.Visible;
            });
        }

        // Start the send receive operations
        void PeerFinder_StartSendReceive(StreamSocket socket, PeerInformation peerInformation)
        {
            ConnectedPeer connectedPeer = new ConnectedPeer(socket, false, new Windows.Storage.Streams.DataWriter(socket.OutputStream));
            _socketHelper.Add(connectedPeer);

            if (!_peerFinderStarted)
            {
                _socketHelper.CloseSocket();
                return;
            }

            HideAllControls();
            ToggleConnectedControls(true);

            if (peerInformation != null)
            {
                // Add a new peer to the list of peers.
                ComboBoxItem item = new ComboBoxItem();
                item.Content = peerInformation.DisplayName;
                PeerFinder_SendToPeerList.Items.Add(item);
                PeerFinder_SendToPeerList.SelectedIndex = 0;
            }

            _socketHelper.StartReader(connectedPeer);
        }

        // Handles PeerFinder_ConnectButton click
        async void PeerFinder_Connect(object sender, RoutedEventArgs e)
        {
            rootPage.NotifyUser("", NotifyType.ErrorMessage);
            PeerInformation peerToConnect = null;

            if (PeerFinder_FoundPeersList.Items.Count == 0)
            {
                rootPage.NotifyUser("Cannot connect, there were no peers found!", NotifyType.ErrorMessage);
            }
            else
            {
                try
                {
                    peerToConnect = (PeerInformation)((ComboBoxItem)PeerFinder_FoundPeersList.SelectedItem).Tag;
                    if (peerToConnect == null)
                    {
                        peerToConnect = (PeerInformation)((ComboBoxItem)PeerFinder_FoundPeersList.Items[0]).Tag;
                    }

                    rootPage.NotifyUser("Connecting to " + peerToConnect.DisplayName + "....", NotifyType.StatusMessage);
                    StreamSocket socket = await PeerFinder.ConnectAsync(peerToConnect);
                    rootPage.NotifyUser("Connection succeeded", NotifyType.StatusMessage);
                    PeerFinder_StartSendReceive(socket, peerToConnect);
                }
                catch (Exception err)
                {
                    rootPage.NotifyUser("Connection to " + peerToConnect.DisplayName + " failed: " + err.Message, NotifyType.ErrorMessage);
                }
            }
        }

        // Handles PeerFinder_BrowsePeersButton click
        async void PeerFinder_BrowsePeers(object sender, RoutedEventArgs e)
        {
            rootPage.NotifyUser("Finding Peers...", NotifyType.StatusMessage);
            try
            {
                // Find all discoverable peers with compatible roles
                _peerInformationList = await PeerFinder.FindAllPeersAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("FindAllPeersAsync throws exception" + ex.Message);
            }
            Debug.WriteLine("Async operation completed");

            // Clear the list containing the previous discovery results
            PeerFinder_FoundPeersList.Items.Clear();

            if ((_peerInformationList != null) && (_peerInformationList.Count > 0))
            {
                for (int i = 0; i < _peerInformationList.Count; i++)
                {
                    String DisplayName = _peerInformationList[i].DisplayName;

                    // Append the DiscoveryData text to the DisplayName
                    if (_peerInformationList[i].DiscoveryData != null)
                    {
                        String DiscoveryData = "";
                        using (DataReader discoveryDataReader = Windows.Storage.Streams.DataReader.FromBuffer(_peerInformationList[i].DiscoveryData))
                        {
                            DiscoveryData = discoveryDataReader.ReadString(_peerInformationList[i].DiscoveryData.Length);
                        }
                        DisplayName = String.Format("{0} '{1}'", DisplayName, DiscoveryData);
                    }

                    ComboBoxItem item = new ComboBoxItem();
                    item.Content = DisplayName;
                    item.Tag = _peerInformationList[i];
                    PeerFinder_FoundPeersList.Items.Add(item);
                }
                PeerFinder_FoundPeersList.SelectedIndex = 0;
                ShowPostBrowseControls(true);
                rootPage.NotifyUser("Finding Peers Done", NotifyType.StatusMessage);
            }
            else
            {
                // Indicate that no peers were found by adding a "None Found"
                // item in the peer list.
                ComboBoxItem item = new ComboBoxItem();
                item.Content = "None Found";
                PeerFinder_FoundPeersList.Items.Add(item);
                PeerFinder_FoundPeersList.SelectedIndex = 0;
                rootPage.NotifyUser("No peers found", NotifyType.StatusMessage);
                ShowPostBrowseControls(false);
            }
        }

        // Handles PeerFinder_AdvertiseButton click
        void PeerFinder_StartAdvertising(object sender, RoutedEventArgs e)
        {
            // If PeerFinder is started, stop it, so that new properties
            // selected by the user (Role/DiscoveryData) can be updated.
            if (_peerFinderStarted)
            {
                PeerFinder.Stop();
                _peerFinderStarted = false;
            }

            rootPage.NotifyUser("", NotifyType.ErrorMessage);
            if (!_peerFinderStarted)
            {
                // attach the callback handler (there can only be one PeerConnectProgress handler).
                PeerFinder.TriggeredConnectionStateChanged += new TypedEventHandler<object, TriggeredConnectionStateChangedEventArgs>(TriggeredConnectionStateChangedEventHandler);
                // attach the incoming connection request event handler
                PeerFinder.ConnectionRequested += new TypedEventHandler<object, ConnectionRequestedEventArgs>(PeerConnectionRequested);

                // Set the PeerFinder.Role property
                // NOTE: this has no effect on the Phone platform
                if (_launchByTap)
                {
                    PeerFinder.Role = rootPage.GetLaunchRole();
                }
                else
                {
                    if (PeerFinder_SelectRole.SelectionBoxItem != null)
                    {
                        switch (PeerFinder_SelectRole.SelectionBoxItem.ToString())
                        {
                            case "Peer":
                                PeerFinder.Role = PeerRole.Peer;
                                break;
                            case "Host":
                                PeerFinder.Role = PeerRole.Host;
                                break;
                            case "Client":
                                PeerFinder.Role = PeerRole.Client;
                                break;
                        }
                    }
                }

                // Set DiscoveryData property if the user entered some text
                // NOTE: this has no effect on the Phone platform
                if ((PeerFinder_DiscoveryData.Text.Length > 0) && (PeerFinder_DiscoveryData.Text != "What's happening today?"))
                {
                    using (var discoveryDataWriter = new Windows.Storage.Streams.DataWriter(new Windows.Storage.Streams.InMemoryRandomAccessStream()))
                    {
                        discoveryDataWriter.WriteString(PeerFinder_DiscoveryData.Text);
                        PeerFinder.DiscoveryData = discoveryDataWriter.DetachBuffer();
                    }
                }

                // start listening for proximate peers
                PeerFinder.Start();
                _peerFinderStarted = true;
                ToggleAdvertiseControls(true);
                ShowStartAdvertiseControls();

                if (_browseConnectSupported && _triggeredConnectSupported)
                {
                    rootPage.NotifyUser("Tap another device to connect to a peer or click Browse for Peers button.", NotifyType.StatusMessage);
                    PeerFinder_BrowsePeersButton.Visibility = Visibility.Visible;
                }
                else if (_triggeredConnectSupported)
                {
                    rootPage.NotifyUser("Tap another device to connect to a peer.", NotifyType.StatusMessage);
                }
                else if (_browseConnectSupported)
                {
                    rootPage.NotifyUser("Click Browse for Peers button.", NotifyType.StatusMessage);
                    PeerFinder_BrowsePeersButton.Visibility = Visibility.Visible;
                }
            }
        }

        // Handles PeerFinder_StopAdvertiseButton click
        void PeerFinder_StopAdvertising(object sender, RoutedEventArgs e)
        {
            if (_peerFinderStarted)
            {
                PeerFinder.Stop();
                _peerFinderStarted = false;

                rootPage.NotifyUser("Stopped Advertising.", NotifyType.StatusMessage);
                HideAllControls();
                ToggleAdvertiseControls(true);
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ChangeVisualState(rootPage.ActualWidth);
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            HideAllControls();

            if (_triggeredConnectSupported || _browseConnectSupported)
            {
                // Initially only the advertise button, Role list and DiscoveryData box should be visible.
                PeerFinder_MessageBox.Text = "Hello World";
                PeerFinder_DiscoveryData.Text = "What's happening today?";

                ToggleAdvertiseControls(true);

                PeerFinderOutputText.Text = "";

                _launchByTap = rootPage.IsLaunchedByTap();

                if (_launchByTap)
                {
                    rootPage.NotifyUser("Launched by tap", NotifyType.StatusMessage);
                    PeerFinder_StartAdvertising(null, null);
                }
                else
                {
                    if (!_triggeredConnectSupported)
                    {
                        rootPage.NotifyUser("Tap based discovery of peers not supported", NotifyType.ErrorMessage);
                    }
                    else if (!_browseConnectSupported)
                    {
                        rootPage.NotifyUser("Browsing for peers not supported", NotifyType.ErrorMessage);
                    }
                }
            }
            else
            {
                rootPage.NotifyUser("Tap based discovery of peers not supported" + Environment.NewLine + "Browsing for peers not supported", NotifyType.ErrorMessage);
            }
        }

        // Invoked when the main page navigates to a different scenario
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            // If PeerFinder was started, stop it when navigating to a different page.
            if (_peerFinderStarted)
            {
                // detach the callback handler (there can only be one PeerConnectProgress handler).
                PeerFinder.TriggeredConnectionStateChanged -= new TypedEventHandler<object, TriggeredConnectionStateChangedEventArgs>(TriggeredConnectionStateChangedEventHandler);
                // detach the incoming connection request event handler
                PeerFinder.ConnectionRequested -= new TypedEventHandler<object, ConnectionRequestedEventArgs>(PeerConnectionRequested);
                PeerFinder.Stop();
                _socketHelper.CloseSocket();
                _peerFinderStarted = false;
            }
        }

        void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            ChangeVisualState(e.Size.Width);
        }

        void ChangeVisualState(double width)
        {
            if (width < 768)
            {
                VisualStateManager.GoToState(this, "Below768Layout", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "DefaultLayout", false);
            }
        }

        // Helpers to update the UI state

        void HideAllControls()
        {
            PeerFinder_AdvertiseButton.Visibility = Visibility.Collapsed;
            PeerFinder_StopAdvertiseButton.Visibility = Visibility.Collapsed;
            PeerFinder_BrowsePeersButton.Visibility = Visibility.Collapsed;

            PeerFinder_ConnectButton.Visibility = Visibility.Collapsed;

            PeerFinder_FoundPeersList.Visibility = Visibility.Collapsed;
            PeerFinder_SendToPeerList.Visibility = Visibility.Collapsed;
            PeerFinder_SendButton.Visibility = Visibility.Collapsed;
            PeerFinder_AcceptButton.Visibility = Visibility.Collapsed;
            PeerFinder_MessageBox.Visibility = Visibility.Collapsed;

            PeerFinder_SelectRole.Visibility = Visibility.Collapsed;
            PeerFinder_DiscoveryData.Visibility = Visibility.Collapsed;
        }

        void ToggleAdvertiseControls(bool show)
        {
            Visibility visibility = (show) ? Visibility.Visible : Visibility.Collapsed;

            PeerFinder_AdvertiseButton.Visibility = visibility;
            PeerFinder_StopAdvertiseButton.Visibility = Visibility.Collapsed;

            // The Role and DiscoveryData are not supported on Phone
            // Hide UI elements because they have no effect
#if WINDOWS_APP 
            PeerFinder_SelectRole.Visibility = visibility;
            if (_browseConnectSupported)
            {
                PeerFinder_DiscoveryData.Visibility = visibility;
            }
#endif
        }

        void ShowStartAdvertiseControls()
        {
            PeerFinder_AdvertiseButton.Visibility = Visibility.Collapsed;
            PeerFinder_StopAdvertiseButton.Visibility = Visibility.Visible;
        }

        void ShowPostBrowseControls(bool found)
        {
            PeerFinder_FoundPeersList.Visibility = Visibility.Visible;
            if (found)
            {
                PeerFinder_ConnectButton.Visibility = Visibility.Visible;
            }
            else
            {
                PeerFinder_ConnectButton.Visibility = Visibility.Collapsed;
            }
        }

        void ToggleConnectedControls(bool show)
        {
            Visibility visibility = (show) ? Visibility.Visible : Visibility.Collapsed;

            PeerFinder_SendButton.Visibility = visibility;
            PeerFinder_MessageBox.Visibility = visibility;
            PeerFinder_SendToPeerList.Visibility = visibility;
        }

        public void Dispose()
        {
            _socketHelper.CloseSocket();
        }
    }
}
