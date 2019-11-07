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
using SDKTemplate;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Proximity
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PeerFinderScenario : SDKTemplate.Common.LayoutAwarePage, System.IDisposable
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as rootPage.NotifyUser()
        MainPage rootPage = MainPage.Current;
        
        private IReadOnlyList<PeerInformation> _peerInformationList;
        private PeerInformation _requestingPeer;
        private StreamSocket _socket = null;
        private bool _socketClosed = true;
        private DataWriter _dataWriter = null;
        private bool _triggeredConnectSupported = false;
        private bool _browseConnectSupported = false;

        public PeerFinderScenario()
        {
            this.InitializeComponent();
            // Scenario 1 init
            _triggeredConnectSupported = (PeerFinder.SupportedDiscoveryTypes & PeerDiscoveryTypes.Triggered) ==
                                         PeerDiscoveryTypes.Triggered;
            _browseConnectSupported = (PeerFinder.SupportedDiscoveryTypes & PeerDiscoveryTypes.Browse) ==
                                      PeerDiscoveryTypes.Browse;
            if (_triggeredConnectSupported || _browseConnectSupported)
            {
                // This scenario demonstrates "PeerFinder" to tap or browse for peers to connect to using a StreamSocket
                PeerFinder_StartFindingPeersButton.Click += new RoutedEventHandler(PeerFinder_StartFindingPeers);
                PeerFinder_BrowsePeersButton.Click += new RoutedEventHandler(PeerFinder_BrowsePeers);
                PeerFinder_ConnectButton.Click += new RoutedEventHandler(PeerFinder_Connect);
                PeerFinder_AcceptButton.Click += new RoutedEventHandler(PeerFinder_Accept);
                PeerFinder_SendButton.Click += new RoutedEventHandler(PeerFinder_Send);
                PeerFinder_StartFindingPeersButton.Visibility = Visibility.Visible;
            }
        }

        //connection states
        string[] rgConnectState = {"PeerFound", 
                                   "Listening",
                                   "Connecting",
                                   "Completed",
                                   "Canceled",
                                   "Failed"};

        async private void TriggeredConnectionStateChangedEventHandler(object sender, TriggeredConnectionStateChangedEventArgs eventArgs)
        {
            rootPage.UpdateLog("TriggeredConnectionStateChangedEventHandler - " + rgConnectState[(int)eventArgs.State], PeerFinderOutputText);

            if (eventArgs.State == TriggeredConnectState.PeerFound)
            {
                // Use this state to indicate to users that the tap is complete and
                // they can pull their devices away.
                rootPage.NotifyUser("Tap complete, socket connection starting!", NotifyType.StatusMessage);
            }

            if (eventArgs.State == TriggeredConnectState.Completed)
            {
                rootPage.NotifyUser("Socket connect success!", NotifyType.StatusMessage);
                // Start using the socket that just connected.
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    this.PeerFinder_StartSendReceive(eventArgs.Socket);
                });
            }

            if (eventArgs.State == TriggeredConnectState.Failed)
            {
                rootPage.NotifyUser("Socket connect failed!", NotifyType.ErrorMessage);
            }
        }

        private bool _peerFinderStarted = false;

        private void SocketError(String errMessage)
        {
            rootPage.NotifyUser(errMessage, NotifyType.ErrorMessage);
            PeerFinder_StartFindingPeersButton.Visibility = Visibility.Visible;
            if (_browseConnectSupported)
            {
                PeerFinder_BrowsePeersButton.Visibility = Visibility.Visible;
            }
            PeerFinder_SendButton.Visibility = Visibility.Collapsed;
            PeerFinder_MessageBox.Visibility = Visibility.Collapsed;
            CloseSocket();
        }

        async private void PeerFinder_Send(object sender, RoutedEventArgs e)
        {
            rootPage.NotifyUser("", NotifyType.ErrorMessage);
            String message = PeerFinder_MessageBox.Text;
            PeerFinder_MessageBox.Text = ""; // clear the input now that the message is being sent.
            if (!_socketClosed)
            {
                if (message.Length > 0)
                {
                    try
                    {
                        uint strLength = _dataWriter.MeasureString(message);
                        _dataWriter.WriteUInt32(strLength);
                        _dataWriter.WriteString(message);
                        uint numBytesWritten = await _dataWriter.StoreAsync();
                        if (numBytesWritten > 0)
                        {
                            rootPage.NotifyUser("Sent message: " + message + ", number of bytes written: " + numBytesWritten, NotifyType.StatusMessage);

                        }
                        else
                        {
                            SocketError("The remote side closed the socket");
                        }
                    }
                    catch (Exception err)
                    {
                        if (!_socketClosed)
                        {
                            SocketError("Failed to send message with error: " + err.Message);
                        }
                    }
                }
                else
                {
                    rootPage.NotifyUser("Please type a message", NotifyType.ErrorMessage);
                }
            }
            else
            {
                SocketError("The remote side closed the socket");
            }
        }

        async private void PeerFinder_Accept(object sender, RoutedEventArgs e)
        {
            rootPage.NotifyUser("Connecting to " + _requestingPeer.DisplayName + "....", NotifyType.StatusMessage);
            PeerFinder_AcceptButton.Visibility = Visibility.Collapsed;
            try
            {
                StreamSocket socket = await PeerFinder.ConnectAsync(_requestingPeer);
                rootPage.NotifyUser("Connection succeeded", NotifyType.StatusMessage);
                PeerFinder_StartSendReceive(socket);
            }
            catch (Exception err)
            {
                rootPage.NotifyUser("Connection to " + _requestingPeer.DisplayName + " failed: " + err.Message, NotifyType.ErrorMessage);
            }
        }

        private async void PeerConnectionRequested(object sender, ConnectionRequestedEventArgs args)
        {
            _requestingPeer = args.PeerInformation;
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                rootPage.NotifyUser("Connection requested from peer " + args.PeerInformation.DisplayName, NotifyType.StatusMessage);
                
                this.PeerFinder_AcceptButton.Visibility = Visibility.Visible;
                this.PeerFinder_SendButton.Visibility = Visibility.Collapsed;
                this.PeerFinder_MessageBox.Visibility = Visibility.Collapsed;
            });
        }

        async void PeerFinder_StartReader(DataReader socketReader)
        {
            try
            {
                uint bytesRead = await socketReader.LoadAsync(sizeof(uint));
                if (bytesRead > 0)
                {
                    uint strLength = (uint)socketReader.ReadUInt32();
                    bytesRead = await socketReader.LoadAsync(strLength);
                    if (bytesRead > 0)
                    {
                        String message = socketReader.ReadString(strLength);
                        rootPage.NotifyUser("Got message: " + message, NotifyType.StatusMessage);
                        PeerFinder_StartReader(socketReader); // Start another reader
                    }
                    else
                    {
                        SocketError("The remote side closed the socket");
                        socketReader.Dispose();
                    }
                }
                else
                {
                    SocketError("The remote side closed the socket");
                    socketReader.Dispose();
                }
            }
            catch (Exception e)
            {
                if (!_socketClosed)
                {
                    SocketError("Reading from socket failed: " + e.Message);
                }
                socketReader.Dispose();
            }
        }

        // Start the send receive operations
        void PeerFinder_StartSendReceive(StreamSocket socket)
        {
            _socket = socket;
            // If the scenario was switched just as the socket connection completed, just close the socket.
            if (!_peerFinderStarted)
            {
                CloseSocket();
                return;
            }

            PeerFinder_SendButton.Visibility = Visibility.Visible;
            PeerFinder_MessageBox.Visibility = Visibility.Visible;

            // Hide the controls related to setting up a connection
            PeerFinder_ConnectButton.Visibility = Visibility.Collapsed;
            PeerFinder_AcceptButton.Visibility = Visibility.Collapsed;
            PeerFinder_FoundPeersList.Visibility = Visibility.Collapsed;
            PeerFinder_BrowsePeersButton.Visibility = Visibility.Collapsed;
            PeerFinder_StartFindingPeersButton.Visibility = Visibility.Collapsed;
            _dataWriter = new DataWriter(_socket.OutputStream);
            _socketClosed = false;
            PeerFinder_StartReader(new DataReader(_socket.InputStream));
        }

        async void PeerFinder_Connect(object sender, RoutedEventArgs e)
        {
            rootPage.NotifyUser("", NotifyType.ErrorMessage);
            PeerInformation peerToConnect = null;
            try
            {
                // If nothing is selected, select the first peer
                if (PeerFinder_FoundPeersList.SelectedIndex == -1)
                {
                    peerToConnect = _peerInformationList[0];
                }
                else
                {
                    peerToConnect = _peerInformationList[PeerFinder_FoundPeersList.SelectedIndex];
                }

                rootPage.NotifyUser("Connecting to " + peerToConnect.DisplayName + "....", NotifyType.StatusMessage);
                StreamSocket socket = await PeerFinder.ConnectAsync(peerToConnect);
                rootPage.NotifyUser("Connection succeeded", NotifyType.StatusMessage);
                PeerFinder_StartSendReceive(socket);
            }
            catch (Exception err)
            {
                rootPage.NotifyUser("Connection to " + peerToConnect.DisplayName + " failed: " + err.Message, NotifyType.ErrorMessage);
            }
        }

        async void PeerFinder_BrowsePeers(object sender, RoutedEventArgs e)
        {
            rootPage.NotifyUser("Finding Peers...", NotifyType.StatusMessage);
            try
            {
                _peerInformationList = await PeerFinder.FindAllPeersAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("FindAll throws exception" + ex.Message);
            }
            Debug.WriteLine("Async operation completed");
            rootPage.NotifyUser("No peers found", NotifyType.StatusMessage);

            if (_peerInformationList.Count > 0)
            {
                PeerFinder_FoundPeersList.Items.Clear();
                for (int i = 0; i < _peerInformationList.Count; i++)
                {
                    ListBoxItem item = new ListBoxItem();
                    item.Content = _peerInformationList[i].DisplayName;
                    PeerFinder_FoundPeersList.Items.Add(item);
                }
                PeerFinder_ConnectButton.Visibility = Visibility.Visible;
                PeerFinder_FoundPeersList.Visibility = Visibility.Visible;
                rootPage.NotifyUser("Finding Peers Done", NotifyType.StatusMessage);
            }
            else
            {
                rootPage.NotifyUser("No peers found", NotifyType.StatusMessage);
                PeerFinder_ConnectButton.Visibility = Visibility.Collapsed;
                PeerFinder_FoundPeersList.Visibility = Visibility.Collapsed;
            }
        }

        void PeerFinder_StartFindingPeers(object sender, RoutedEventArgs e)
        {
            rootPage.NotifyUser("", NotifyType.ErrorMessage);
            if (!_peerFinderStarted)
            {
                // attach the callback handler (there can only be one PeerConnectProgress handler).
                PeerFinder.TriggeredConnectionStateChanged += new TypedEventHandler<object, TriggeredConnectionStateChangedEventArgs>(TriggeredConnectionStateChangedEventHandler);
                // attach the incoming connection request event handler
                PeerFinder.ConnectionRequested += new TypedEventHandler<object, ConnectionRequestedEventArgs>(PeerConnectionRequested);
                // start listening for proximate peers
                PeerFinder.Start();
                _peerFinderStarted = true;
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

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (_triggeredConnectSupported || _browseConnectSupported)
            {
                // Initially only the advertise button should be visible.
                PeerFinder_StartFindingPeersButton.Visibility = Visibility.Visible;
                PeerFinder_BrowsePeersButton.Visibility = Visibility.Collapsed;
                PeerFinder_ConnectButton.Visibility = Visibility.Collapsed;
                PeerFinder_FoundPeersList.Visibility = Visibility.Collapsed;
                PeerFinder_SendButton.Visibility = Visibility.Collapsed;
                PeerFinder_AcceptButton.Visibility = Visibility.Collapsed;
                PeerFinder_MessageBox.Visibility = Visibility.Collapsed;
                PeerFinder_MessageBox.Text = "Hello World";
                if (rootPage.IsLaunchedByTap())
                {
                    rootPage.NotifyUser("Launched by tap", NotifyType.StatusMessage);
                    PeerFinder_StartFindingPeers(null, null);
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
                rootPage.NotifyUser("Tap based discovery of peers not supported \nBrowsing for peers not supported", NotifyType.ErrorMessage);
            }
        }

        // Invoked when the main page navigates to a different scenario
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (_peerFinderStarted)
            {
                // detach the callback handler (there can only be one PeerConnectProgress handler).
                PeerFinder.TriggeredConnectionStateChanged -= new TypedEventHandler<object, TriggeredConnectionStateChangedEventArgs>(TriggeredConnectionStateChangedEventHandler);
                // detach the incoming connection request event handler
                PeerFinder.ConnectionRequested -= new TypedEventHandler<object, ConnectionRequestedEventArgs>(PeerConnectionRequested);
                PeerFinder.Stop();
                CloseSocket();
                _peerFinderStarted = false;
            }
        }

        private void CloseSocket()
        {
            if (_socket != null)
            {
                _socketClosed = true;
                _socket.Dispose();
                _socket = null;
            }

            if (_dataWriter != null)
            {
                _dataWriter.Dispose();
                _dataWriter = null;
            }
        }

        public void Dispose()
        {
            CloseSocket();
        }
    }
}
