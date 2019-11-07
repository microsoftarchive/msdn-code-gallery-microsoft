using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Networking.Proximity;
using Windows.System.UserProfile;
using Windows.Networking.Sockets;
using Windows.UI.Popups;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Controls;

namespace CritterStomp
{
    public class ConnectedPeer
    {
        public ConnectedPeer(string displayName)
        {
            DisplayName = displayName;
        }

        public string DisplayName { get; private set; }
    }

    public sealed partial class InvitePlayersPage
    {
        private List<PeerInformation> selectedPeers;
        private SocketReaderWriter socket;
        private ObservableCollection<AvailablePeer> availablePeers;
        private Dictionary<ConnectedPeer, SocketReaderWriter> connectedPeers;
        private CoreDispatcher messageDispatcher = Window.Current.CoreWindow.Dispatcher;
        private PeerWatcher peerWatcher;

        public string SendInvitations
        {
            get { return sendInvitationsText.Text; }
            set { sendInvitationsText.Text = value; }
        }

        public InvitePlayersPage()
        {
            selectedPeers = new List<PeerInformation>();
            availablePeers = new ObservableCollection<AvailablePeer>();
            connectedPeers = new Dictionary<ConnectedPeer, SocketReaderWriter>();

            InitializeComponent();

            SizeChanged += OnWindowSizeChanged;

            // Update the list of selected peers whenever one is tapped/clicked.
            foundPeers.SelectionChanged += PeersSelectionChanged;
        }

        void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width < Constants.MinimumScreenWidth)
            {
                VisualStateManager.GoToState(this, "NarrowSizeState", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "FullSizeState", false);
            }
        }

        void StartPeerWatcher()
        {
            if (peerWatcher == null)
            {
                peerWatcher = PeerFinder.CreateWatcher();
                // Hook up events, this should only be done once
                peerWatcher.Added += PeerWatcher_Added;
                peerWatcher.Removed += PeerWatcher_Removed;
                peerWatcher.Updated += PeerWatcher_Updated;
                peerWatcher.EnumerationCompleted += PeerWatcher_EnumerationCompleted;
                peerWatcher.Stopped += PeerWatcher_Stopped;
            }

            PeerWatcherStatus status = peerWatcher.Status;
            if (status == PeerWatcherStatus.Created || status == PeerWatcherStatus.Stopped || status == PeerWatcherStatus.Aborted)
            {
                try
                {
                    foundPeers.SelectionChanged -= PeersSelectionChanged;
                    availablePeers.Clear();
                    foundPeers.ItemsSource = availablePeers;
                    noPeersFound.Visibility = Visibility.Collapsed;
                    foundPeers.SelectionChanged += PeersSelectionChanged;

                    peerWatcher.Start();

                    progressBar.Visibility = Visibility.Visible;
                }
                catch (Exception err)
                {
                    proximityStatus.Text = "Error starting PeerWatcher: " + err.ToString();
                }
            }
        }

        void StopPeerWatcher()
        {
            try
            {
                peerWatcher.Stop();
            }
            catch (Exception err)
            {
                proximityStatus.Text = "Error stopping PeerWatcher: " + err.ToString();
            }
        }

        #region PeerWatcherEvents

        private void PeerWatcher_Added(PeerWatcher sender, PeerInformation peerInfo)
        {
            var ignored = messageDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                lock (availablePeers)
                {
                    noPeersFound.Visibility = Visibility.Collapsed;
                    availablePeers.Add(new AvailablePeer(peerInfo));
                    // Don't overlap NFC text, only show this if the ListView is enabled
                    if (foundPeers.IsEnabled)
                    {
                        directions.Visibility = Visibility.Visible;
                    }
                }
            });
        }

        private void PeerWatcher_Removed(PeerWatcher sender, PeerInformation peerInfo)
        {
            var ignored = messageDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                lock (availablePeers)
                {
                    for (int i = 0; i < availablePeers.Count; i++)
                    {
                        if (availablePeers[i].Peer.Id == peerInfo.Id)
                        {
                            availablePeers.RemoveAt(i);
                        }
                    }
                    if (availablePeers.Count == 0)
                    {
                        directions.Visibility = Visibility.Collapsed;
                    }
                }
            });
        }

        private void PeerWatcher_Updated(PeerWatcher sender, PeerInformation peerInfo)
        {
            var ignored = messageDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                lock (availablePeers)
                {
                    for (int i = 0; i < availablePeers.Count; i++)
                    {
                        if (availablePeers[i].Peer.Id == peerInfo.Id)
                        {
                            availablePeers[i] = new AvailablePeer(peerInfo);
                        }
                    }
                }
            });
        }

        private void PeerWatcher_EnumerationCompleted(PeerWatcher sender, object o)
        {
            var ignored = messageDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                lock (availablePeers)
                {
                    // Notify the user that no peers were found after we have done an initial scan
                    if (availablePeers.Count == 0 && connectedPeers.Count == 0)
                    {
                        noPeersFound.Visibility = Visibility.Visible;
                        directions.Visibility = Visibility.Collapsed;
                    }
                }
            });
        }

        private void PeerWatcher_Stopped(PeerWatcher sender, object o)
        {
            var ignored = messageDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                progressBar.Visibility = Visibility.Collapsed;
            });
        }

        #endregion

        // Used to display state of the connection (i.e. connecting, connected, disconnected) when two PCs are tapped.
        private async void TriggeredConnectionStateChanged(object sender, TriggeredConnectionStateChangedEventArgs e)
        {
            await messageDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                bool tapIsProcessed = false;
                switch (e.State)
                {
                    case TriggeredConnectState.PeerFound:
                        directions.Visibility = Visibility.Collapsed;
                        nfcStatusText.Visibility = Visibility.Visible;
                        foundPeers.IsEnabled = false;

                        progressBar.Visibility = Visibility.Visible;
                        backButton.Visibility = Visibility.Collapsed;

                        break;
                    case TriggeredConnectState.Completed:
                        // Associate socket handed back by TriggeredConnectionStateChanged event with player connectedPeers.
                        socket = new SocketReaderWriter(e.Socket, this);
                        socket.ReadMessage();
                        socket.WriteMessage(string.Format("{0} {1}", Constants.OpCodeSendDisplayName, PeerFinder.DisplayName));
                        startGameButton.Visibility = Visibility.Visible;
                        tapIsProcessed = true;

                        break;
                    case TriggeredConnectState.Failed:
                        tapIsProcessed = true;
                        
                        break;
                }

                // Reset UI
                if (tapIsProcessed)
                {
                    nfcStatusText.Visibility = Visibility.Collapsed;
                    foundPeers.IsEnabled = true;

                    lock (availablePeers)
                    {
                        if (availablePeers.Count > 0)
                        {
                            directions.Visibility = Visibility.Visible;
                        }
                    }

                    // Progress bar should be visible as long as PeerWatcher is running
                    progressBar.Visibility = Visibility.Collapsed;
                    if (peerWatcher != null)
                    {
                        PeerWatcherStatus status = peerWatcher.Status;
                        if (status == PeerWatcherStatus.EnumerationCompleted || status == PeerWatcherStatus.Started)
                        {
                            progressBar.Visibility = Visibility.Visible;
                        }
                    }
                }
            });
        }

        public void AddConnectedPeer(string displayName)
        {
            ConnectedPeer temp = new ConnectedPeer(displayName);
            connectedPeers[temp] = socket;
            ConnectedPlayers.Items.Add(displayName);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            LoadState(e.Parameter, null);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (peerWatcher != null)
            {
                StopPeerWatcher();
            }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        private async void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            progressBar.Visibility = Visibility.Collapsed;
            noPeersFound.Visibility = Visibility.Collapsed;
            string displayName = await UserInformation.GetDisplayNameAsync();
            if (!string.IsNullOrEmpty(displayName))
            {
                PeerFinder.DisplayName = displayName;
            }
            ConnectedPlayers.Items.Add(PeerFinder.DisplayName + "'s Game");

            //Checks whether the PC supports Wi-Fi Direct, NFC or both.  
            //Depending on the hardware supported, different messages are displayed to the user.
            if (((PeerFinder.SupportedDiscoveryTypes & PeerDiscoveryTypes.Triggered) == PeerDiscoveryTypes.Triggered) && 
                ((PeerFinder.SupportedDiscoveryTypes & PeerDiscoveryTypes.Browse) == PeerDiscoveryTypes.Browse))
            {
                PeerFinder.Role = PeerRole.Host;
                PeerFinder.TriggeredConnectionStateChanged += TriggeredConnectionStateChanged;
                PeerFinder.AllowBluetooth = false;
                PeerFinder.AllowInfrastructure = false;
                PeerFinder.AllowWiFiDirect = true;
                PeerFinder.Start();
            }
            else if ((PeerFinder.SupportedDiscoveryTypes & PeerDiscoveryTypes.Browse) == PeerDiscoveryTypes.Browse)
            {
                PeerFinder.Role = PeerRole.Host;
                PeerFinder.Start();
            }
            else
            {
                proximityStatus.Text = "Neither Wi-Fi Direct nor NFC are supported :( Use another machine";
                return;
            }
            
            foundPeers.ItemsSource = availablePeers;
            foundPeers.SelectionMode = ListViewSelectionMode.Single;
            foundPeers.IsEnabled = true;

            StartPeerWatcher();
        }

        private void PeersSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var clickedItems = e.AddedItems;
            var unclickedItems = e.RemovedItems;

            foreach (object item in clickedItems)
            {
                selectedPeers.Add((item as AvailablePeer).Peer);
            }

            foreach (object item in unclickedItems)
            {
                selectedPeers.Remove((item as AvailablePeer).Peer);
            }

            InvitePlayer(sender, e);
        }

        private async void InvitePlayer(object sender, RoutedEventArgs e)
        {
            foundPeers.IsEnabled = false;
            List<PeerInformation> names = new List<PeerInformation>();

            if (selectedPeers.Count > 0)
            {
                foreach (PeerInformation peer in selectedPeers)
                {
                    names.Add(peer);
                    try
                    {
                        await ConnectToPeers(peer);
                    }
                    catch (Exception)
                    {
                        sendInvitationsText.Text = "Cannot connect to " + peer.DisplayName;
                    }
                }
            }
            selectedPeers.Clear();
            foundPeers.IsEnabled = true;
        }

        private void StartGameClick(object sender, RoutedEventArgs e)
        {
            PeerFinder.TriggeredConnectionStateChanged -= TriggeredConnectionStateChanged;
            if (connectedPeers.Count > 0)
            {
                foreach (ConnectedPeer peer in connectedPeers.Keys)
                {
                    connectedPeers[peer].WriteMessage(Message.NavigateToGame);
                }
                NavigateToGame();
            }
        }

        public void NavigateToGame()
        {
            GamePageParameters parameters = new GamePageParameters()
            {
                GameMode = Constants.MultiPlayer,
                Role = PeerFinder.Role,
                ConnectedPeers = connectedPeers,
                PlayerCount = connectedPeers.Count
            };

            Frame.Navigate(typeof(GamePage), parameters);
        }

        private async Task ConnectToPeers(PeerInformation peer)
        {            
            try
            {
                progressBar.Visibility = Visibility.Visible;

                UpdatePlayerStatus(peer, " - connecting...");                  
                StreamSocket s = await PeerFinder.ConnectAsync(peer);
                ConnectedPeer temp = new ConnectedPeer(peer.DisplayName);
                connectedPeers[temp] = new SocketReaderWriter(s, this);
                connectedPeers[temp].ReadMessage();

                UpdatePlayerStatus(peer, " - ready");
                ConnectedPlayers.Items.Add(peer.DisplayName);
                
                startGameButton.Visibility = Visibility.Visible;
            }
            catch (Exception)
            {
                sendInvitationsText.Text = "Cannot connect to " + peer.DisplayName;
            }
        
            // PeerFinder.ConnectAsync aborts PeerWatcher
            // restart PeerWatcher whether connect failed or succeeded
            StartPeerWatcher();
        }
        
        private void UpdatePlayerStatus(PeerInformation peer, string status)
        {
            try
            {
                for (int i = 0; i < availablePeers.Count; i++)
                {
                    if (availablePeers[i].Peer.Id == peer.Id)
                    {
                        if (status.Equals(" - connecting..."))
                        {
                            availablePeers[i].Status = status;
                        }
                        else if (status.Equals(" - ready"))
                        {
                            foundPeers.SelectionChanged -= PeersSelectionChanged;
                            availablePeers.RemoveAt(i);
                            foundPeers.SelectionChanged += PeersSelectionChanged;
                        }
                    }
                }
            }
            catch (Exception err)
            {
                sendInvitationsText.Text = err.ToString();
            }
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            PeerFinder.Stop();
            if (Frame != null && Frame.CanGoBack) Frame.GoBack();
        }
    }
}
