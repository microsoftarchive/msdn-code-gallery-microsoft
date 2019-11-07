using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Networking.Proximity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;


namespace CritterStomp
{
    public struct ScorePageParameters
    {
        public int GameMode { get; set; }
        public int Score { get; set; }
        public Dictionary<ConnectedPeer, SocketReaderWriter> ConnectedPeers { get; set; }
    }

    public sealed partial class ScorePage
    {
        private SocketReaderWriter socket;

        private int gameScore;
        private int gameMode;
        private string singlePairRole;
        private Dictionary<string, string> playerScores;
        private Dictionary<ConnectedPeer, SocketReaderWriter> connectedPeers;
        private int numPlayers;
        private int closedSockets;

        public string InfoText
        {
            get { return infoText.Text; }
            set { infoText.Text = value; }
        }

        public ScorePage()
        {
            InitializeComponent();
            singlePairRole = "";
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
        private void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            ScorePageParameters parameters = (ScorePageParameters)navigationParameter;

            gameScore = parameters.Score;
            gameMode = parameters.GameMode;

            if (gameMode == Constants.SinglePlayer)
            {
                finalScoreDisplay.Text = "You scored " + parameters.Score.ToString();
                progressBar.Visibility = Visibility.Collapsed;
            }
            else
            {
                // Each player starts with an empty list of scores 
                // Hosts populate their lists as they receive scores
                // Clients populate their lists when the host sends them scores
                playerScores = new Dictionary<string, string>();

                connectedPeers = parameters.ConnectedPeers;

                foreach (ConnectedPeer peer in connectedPeers.Keys)
                {
                    try
                    {
                        // Set each socket's current page to ScorePage.
                        connectedPeers[peer].currentPage = this;
                        connectedPeers[peer].UpdateCurrentPageType();

                        if (gameMode != Constants.SinglePlayer)
                        {
                            if (PeerFinder.Role == PeerRole.Client)
                            {
                                socket = connectedPeers[peer];
                            }
                            else if (PeerFinder.Role == PeerRole.Host)
                            {
                                numPlayers = connectedPeers.Count + 1;
                                closedSockets = 0;
                                playerScores[PeerFinder.DisplayName] = gameScore.ToString();
                                connectedPeers[peer].WriteMessage(Message.Navigated);
                            }
                        }
                    }
                    catch (Exception err)
                    {
                        infoText.Text += "\nError loading scores: \n" + err.ToString();
                    }
                }
            }
        }


        public void CompileScores(string clientName, string score)
        {
            try
            {
                if (PeerFinder.Role == PeerRole.Host)
                {
                    playerScores[clientName] = score;
                }

                if (playerScores.Count == numPlayers)
                {
                    DisplayScores();
                }
            }
            catch (Exception err)
            {
                infoText.Text = "\nError compiling scores: \n" + err.ToString();
            }
        }

        public void SendScores()
        {
            try
            {
                if (PeerFinder.Role == PeerRole.Client || singlePairRole == "client")
                {
                    socket.WriteMessage(string.Format("{0} {1} {2}", Constants.OpCodeUpdateScore, PeerFinder.DisplayName, gameScore));
                }
            }
            catch (Exception err)
            {
                infoText.Text += "\nError sending score to host: \n" + err.ToString();
            }
        }

        public void SendScores(string clientName, string score)
        {
            try
            {
                foreach (ConnectedPeer peer in connectedPeers.Keys)
                {
                    connectedPeers[peer].WriteMessage(string.Format("{0} {1} {2}", Constants.OpCodeSendScore, clientName, score));
                }
            }
            catch (Exception err)
            {
                infoText.Text = "\nError sending scores: \n" + err.ToString();
            }
        }

        public void ReceiveScores(string clientName, string score)
        {
            try
            {
                if (PeerFinder.Role == PeerRole.Client)
                {
                    playerScores[clientName] = score;
                }

                if (playerScores.Count == numPlayers)
                {
                    DisplayScores();
                }
            }
            catch (Exception err)
            {
                infoText.Text = "\nError receiving scores: \n" + err.ToString();
            }
        }

        public void UpdateNumPlayers(int numOfPlayers)
        {
            numPlayers = numOfPlayers;
        }

        private void FindWinner()
        {
            string winnerName = "";
            int winnerScore = 0;
            bool tie = false;

            foreach (string clientName in playerScores.Keys)
            {
                if (int.Parse(playerScores[clientName]) > winnerScore)
                {
                    winnerName = clientName;
                    winnerScore = int.Parse(playerScores[clientName]);
                    tie = false;
                }
                else if (int.Parse(playerScores[clientName]) == winnerScore)
                {
                    tie = true;
                }
            }

            progressBar.Visibility = Visibility.Collapsed;
            if (!tie)
            {
                if (PeerFinder.DisplayName.Equals(winnerName))
                {
                    finalScoreDisplay.Text = "You won!";
                }
                else
                {
                    finalScoreDisplay.Text = "Try again :(";
                }
            }
            else
            {
                finalScoreDisplay.Text = "It's a tie!";
            }
        }


        private void DisplayScores()
        {
            try
            {
                if (PeerFinder.Role == PeerRole.Host || singlePairRole == "host")
                {
                    foreach (ConnectedPeer peer in connectedPeers.Keys)
                    {
                        connectedPeers[peer].WriteMessage(string.Format("{0} {1}", Constants.OpCodeNumPlayers, numPlayers));
                    }
                    foreach (string clientName in playerScores.Keys)
                    {
                        SendScores(clientName, playerScores[clientName]);
                    }
                    foreach (ConnectedPeer peer in connectedPeers.Keys)
                    {
                        connectedPeers[peer].WriteMessage(string.Format("{0}", Message.Complete));
                    }
                }

                List<string> tempScoreList = new List<string>();

                foreach (string clientName in playerScores.Keys)
                {
                    tempScoreList.Add(clientName + " - " + playerScores[clientName]);
                }

                tempScoreList.Sort();
                FindWinner();
                scoreListView.ItemsSource = tempScoreList;
            }
            catch (Exception err)
            {
                infoText.Text += "\nError displaying scores: \n " + err.ToString();
            }
        }

        private void BackToMenuClick(object sender, RoutedEventArgs e)
        {
            connectedPeers = null;
            PeerFinder.Stop();
            Frame.Navigate(typeof(MainPage));
        }

        public void CloseSocketHost()
        {
            // Host waits for all clients to send 'Complete' message back, then closes all its own sockets
            if (PeerFinder.Role == PeerRole.Host && closedSockets == numPlayers - 1)
            {
                foreach (ConnectedPeer peer in connectedPeers.Keys)
                {
                    connectedPeers[peer].Dispose();
                }
                connectedPeers = null;
            }
            else
            {
                closedSockets++;
            }
        }

        public void CloseSocket()
        {
            // Host, client, and singlepair all close their sockets.
            if (gameMode == Constants.MultiPlayer)
            {
                if (PeerFinder.Role == PeerRole.Client)
                {
                    socket.WriteMessage(string.Format("{0}", Message.Closing));
                    socket.Dispose();
                    connectedPeers = null;
                    PeerFinder.Stop();
                }
            }
        }
    }
}
