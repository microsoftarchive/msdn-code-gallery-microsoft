using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.Networking.Proximity;
using Windows.Networking.Sockets;


namespace CritterStomp
{
    /// <summary>
    /// Connection type: browse and connect, or, tap and connect.
    /// </summary>
    public enum Connection
    {
        BrowseAndConnect = 0,
        TapAndConnect
    }

    /// <summary>
    /// Browse connections use a PeerInformation object, while Tap connections use a ConnectedPeer object
    /// </summary>    
    public struct WaitingForHostParameters
    {
        public Connection ConnectionType { get; set; }
        public PeerInformation Peer { get; set; }
        public StreamSocket Socket { get; set; }
    }
    
    /// <summary>
    /// This is an intermediary page for the client, before the Host starts a game.  An app could also do this with a system UI, as opposed to an app page.
    /// </summary>
    public sealed partial class WaitingForHost
    {
        private Dictionary<ConnectedPeer, SocketReaderWriter> connectedPeers;
        private SocketReaderWriter socket;
        
        public WaitingForHost()
        {
            this.InitializeComponent();
            connectedPeers = new Dictionary<ConnectedPeer, SocketReaderWriter>();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            LoadState(e.Parameter, null);
        }
        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        private async void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // Users wait (i.e. watch progress) on this page until the connection has succeeded/failed.
            backButton.Visibility = Visibility.Collapsed;
            if (navigationParameter != null)
            {
                WaitingForHostParameters parameters = (WaitingForHostParameters)navigationParameter;
                if (parameters.Peer != null)
                {
                    pageTitle.Text = "Connecting to " + parameters.Peer.DisplayName.ToString() + "...";
                    try
                    {
                        StreamSocket socket = await PeerFinder.ConnectAsync(parameters.Peer);
                        pageTitle.Text = "Connected! Waiting for Host...";
                        this.socket = new SocketReaderWriter(socket, this);
                        ConnectedPeer tempPeer = new ConnectedPeer(parameters.Peer.DisplayName);
                        connectedPeers[tempPeer] = this.socket;

                        PeerFinder.Stop();
                        StartReading();
                    }
                    catch (Exception)
                    {
                        pageTitle.Text = "Cannot connect to " + parameters.Peer.DisplayName;
                        PeerFinder.Stop();
                    }
                    progressBar.Visibility = Visibility.Collapsed;
                    backButton.Visibility = Visibility.Visible;
                }
                else if (parameters.Socket != null)
                {
                    try
                    {
                        this.socket = new SocketReaderWriter(parameters.Socket, this);
                        pageTitle.Text = "Waiting for Host...";
                        socket.WriteMessage(string.Format("{0} {1}", Constants.OpCodeSendDisplayName, PeerFinder.DisplayName));

                        PeerFinder.Stop();
                        StartReading();
                    }
                    catch (Exception)
                    {
                        pageTitle.Text = "Cannot connect";
                        PeerFinder.Stop();
                    }
                    backButton.Visibility = Visibility.Visible;
                    progressBar.Visibility = Visibility.Collapsed;
                }
            }
        }

        public void addConnectedPeer(string displayName)
        {
            pageTitle.Text = "Connected! Waiting for Host";
            ConnectedPeer tempPeer = new ConnectedPeer(displayName);
            connectedPeers[tempPeer] = this.socket;
        }

        public void NavigateToGame()
        {
            GamePageParameters parameters = new GamePageParameters();
            parameters.GameMode = Constants.MultiPlayer;
            parameters.Role = PeerFinder.Role;
            parameters.ConnectedPeers = connectedPeers;
            parameters.PlayerCount = connectedPeers.Count;

            Frame.Navigate(typeof(GamePage), parameters);
        }

        private void StartReading()
        {
            socket.ReadMessage();
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            PeerFinder.Stop();
            if (Frame != null && Frame.CanGoBack) Frame.GoBack();
        }

    }
}
