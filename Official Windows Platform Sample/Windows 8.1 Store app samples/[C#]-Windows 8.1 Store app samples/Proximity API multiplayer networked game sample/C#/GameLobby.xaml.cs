using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.Networking.Proximity;
using Windows.Networking.Sockets;
using Windows.System.UserProfile;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Controls;

namespace CritterStomp
{
    public sealed partial class GameLobby
    {
        private Dictionary<string, PeerInformation> invitations;
        private CoreDispatcher messageDispatcher = Window.Current.CoreWindow.Dispatcher;

        private string selectedInvitation;
        private ListView receivedInvitations;

        public string AcceptInvitation
        {
            get { return acceptInvitation.Text; }
            set { acceptInvitation.Text = value; }
        }

        public GameLobby()
        {
            InitializeComponent();

            // Initialize the lists of selected ongoing games/received invitations.
            selectedInvitation = null;
            invitations = new Dictionary<string, PeerInformation>();

            // Connect the listviews
            receivedInvitations = ReceivedInvitations;
            receivedInvitations.HorizontalAlignment = HorizontalAlignment.Center;

            PeerFinder.ConnectionRequested += ConnectionRequested;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            LoadState(e.Parameter, null);
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
            // Set the app role - should be either client or singlepair
            string displayName = await UserInformation.GetDisplayNameAsync();
            progressBar.Visibility = Visibility.Visible;
            if (!string.IsNullOrEmpty(displayName))
            {
                PeerFinder.DisplayName = displayName;
            }

            //Checks whether the PC supports Wi-Fi Direct, NFC or both.  
            //Depending on the hardware supported, different messages are displayed to the user.
            if (((PeerFinder.SupportedDiscoveryTypes & PeerDiscoveryTypes.Triggered) == PeerDiscoveryTypes.Triggered) && 
                ((PeerFinder.SupportedDiscoveryTypes & PeerDiscoveryTypes.Browse) == PeerDiscoveryTypes.Browse))
            {
                PeerFinder.Role = PeerRole.Client;
                PeerFinder.TriggeredConnectionStateChanged += TriggeredConnectionStateChanged;
                PeerFinder.AllowBluetooth = false;
                PeerFinder.AllowInfrastructure = false;
                PeerFinder.AllowWiFiDirect = true;
                PeerFinder.Start();
            }
            else if ((PeerFinder.SupportedDiscoveryTypes & PeerDiscoveryTypes.Browse) == PeerDiscoveryTypes.Browse)
            {
                PeerFinder.Role = PeerRole.Client;
                PeerFinder.Start();
            }
            else
            {
                proximityStatus.Text = "Neither Wi-Fi Direct nor NFC are supported :( Try on another PC";
            }
        }

        private void InvitationSelected(object sender, SelectionChangedEventArgs e)
        {
            IList<object> addedItems = (IList<object>)e.AddedItems;

            foreach (object addedItem in addedItems)
            {
                selectedInvitation = addedItem.ToString();            
            }

            try
            {
                // connect to the one item in the selectedInvitations list
                if (selectedInvitation != null)
                {
                    WaitForHost(invitations[selectedInvitation]);
                }
                else
                {
                    acceptInvitation.Text = "Please select an invitation to continue";
                }
            }
            catch (System.Exception err)
            {
                if (err.GetType() == typeof(System.ObjectDisposedException))
                {
                    acceptInvitation.Text = "Peer has closed the socket";
                }
                else
                {
                    acceptInvitation.Text = "Failed to connect";
                }
                acceptInvitation.Visibility = Visibility.Collapsed;
            }  
        }

        private void AcceptInvitationClick(object sender, RoutedEventArgs e)
        {
            try
            {
                // connect to the one item in the selectedInvitations list
                if (selectedInvitation != null)
                {
                    WaitForHost(invitations[selectedInvitation]);
                }
                else
                {
                    AcceptInvitation = "Please select an invitation to continue";
                }
            }
            catch (Exception err)
            {
                if (err is ObjectDisposedException)
                {
                    acceptInvitation.Text = "Peer has closed the socket";
                }
                else
                {
                    acceptInvitation.Text = "Failed to connect";
                }
            }
        }

        private async void ConnectionRequested(object sender, ConnectionRequestedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {            
                // handles case where you have no existing invitation
                List<string> existingInvitations = new List<string>();
                existingInvitations.Add(e.PeerInformation.DisplayName);

                // put the display name and peer information in a dictionary
                invitations[e.PeerInformation.DisplayName] = e.PeerInformation;

                receivedInvitations.SelectionMode = ListViewSelectionMode.Single;
                receivedInvitations.ItemsSource = existingInvitations;

                ReceivedInvitationsHeader.Text = "STOMP invitations :-)";
                progressBar.Visibility = Visibility.Collapsed;
                directions.Visibility = Visibility.Visible;
            });
        }

        //Used to display state of the connection (i.e. connecting, connected, disconnected) when two PCs are tapped        
        private async void TriggeredConnectionStateChanged(object sender, TriggeredConnectionStateChangedEventArgs e)
        {
            if (e.State == TriggeredConnectState.PeerFound)
            {
                await messageDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    progressBar.Visibility = Visibility.Collapsed;
                    ReceivedInvitationsHeader.Visibility = Visibility.Collapsed;
                    receivedInvitations.Visibility = Visibility.Collapsed;
                    backButton.Visibility = Visibility.Collapsed;
                    
                    tapProgressBar.Visibility = Visibility.Visible;
                    tapConnectStatus.Text = "Connecting to Host...";
                });
            }
            else if (e.State == TriggeredConnectState.Completed)
            {
                try
                {
                    await messageDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        StreamSocket socket = e.Socket;
                        tapConnectStatus.Text = "Connected!";
                        tapProgressBar.Visibility = Visibility.Collapsed;
                        WaitForHost(socket);
                    });
                }
                catch (Exception)
                {
                    tapConnectStatus.Text = "Cannot connect";
                    tapProgressBar.Visibility = Visibility.Collapsed;
                    PeerFinder.Stop();
                    backButton.Visibility = Visibility.Visible;
                }
            }
            else if (e.State == TriggeredConnectState.Failed)
            {
                await messageDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    tapConnectStatus.Text = "Cannot connect";
                    PeerFinder.Stop();
                    backButton.Visibility = Visibility.Visible;
                    tapProgressBar.Visibility = Visibility.Collapsed;
                });
            }
            
        }

        private async void WaitForHost(StreamSocket connectSocket)
        {
            if (connectSocket != null)
            {
                try
                {
                    PeerFinder.TriggeredConnectionStateChanged -= TriggeredConnectionStateChanged;
                    WaitingForHostParameters parameters = new WaitingForHostParameters()
                    {
                        Socket = connectSocket,
                        ConnectionType = Connection.TapAndConnect,
                        Peer = null,
                    };

                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        Frame.Navigate(typeof(WaitingForHost), parameters);
                    });

                }
                catch (Exception err)
                {
                    AcceptInvitation += err.ToString();
                }
            }
        }  
          
        private void WaitForHost(PeerInformation peer)
        {
            if (peer != null)
            {
                try
                {
                    WaitingForHostParameters parameters = new WaitingForHostParameters()
                    {
                        ConnectionType = Connection.BrowseAndConnect,
                        Peer = peer,
                        Socket = null,
                    };
                    PeerFinder.TriggeredConnectionStateChanged -= TriggeredConnectionStateChanged;
                    ReceivedInvitations.SelectionChanged -= InvitationSelected;
                    ReceivedInvitations.SelectedItem = null;
                    Frame.Navigate(typeof(WaitingForHost), parameters);                 
                }
                catch (Exception err)
                {
                    acceptInvitation.FontSize = Constants.FontSizeNormal;
                    acceptInvitation.LineHeight = Constants.FontSizeNormal;
                    AcceptInvitation += err.ToString();
                }
            }
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            PeerFinder.Stop();
            if (Frame != null && Frame.CanGoBack) Frame.GoBack();
        }
    }
}
