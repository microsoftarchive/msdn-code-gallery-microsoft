using System;
using System.Collections.Generic;
using Windows.Networking.Proximity;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace CritterStomp
{
    public struct GamePageParameters
    {
        public int GameMode { get; set; }
        public PeerRole Role { get; set; }
        public Dictionary<ConnectedPeer, SocketReaderWriter> ConnectedPeers { get; set; }
        public int PlayerCount { get; set; }
    }

    public sealed partial class GamePage
    {
        #region Declare all class variables

        // Images for critters and stomped critters, respectively. First one for each image list
        // is an empty space (i.e. no critter); others are real critters.
        private List<BitmapImage> critterImages = new List<BitmapImage>();
        private List<BitmapImage> stompedImages = new List<BitmapImage>();
        
        private Image[,] images;
        private Dictionary<int, string> playerList;
        private Dictionary<ConnectedPeer, SocketReaderWriter> connectedPeers;

        private int gameMode;
        private int playersReady;

        private SocketReaderWriter socket;

        private DispatcherTimer gameTimer;

        private Random random;

        public int playerCount;
        public int playerID;

        private int timesTicked = 0;
        public int timesToTick = 0;
        private TextBlock timeText;

        private int timeToSpawn = 0;
        int gameScore;

        private bool windowTooSmall = false;

        #endregion

        public GamePage()
        {
            InitializeComponent();

            playerCount = 0;
            timeText = time;
            gameScore = 0;

            SizeChanged += OnWindowSizeChanged;
            windowTooSmall = false;
        }

        void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            CheckWindowSizeSupport(e.NewSize.Width, e.NewSize.Height);
        }

        void CheckWindowSizeSupport(double windowWidth, double windowHeight)
        {
            bool previousWindowTooSmall = windowTooSmall;
            windowTooSmall = (windowWidth < Constants.GridColumns * Constants.CellWidth);

            if (!previousWindowTooSmall && windowTooSmall)
            {
                // User shrinks the app window.
                if (gameMode == Constants.SinglePlayer)
                {
                    VisualStateManager.GoToState(this, "SizeNotSupportedQuitAllowedState", false);

                    // In single-player mode, stop the timer and show the button for user to quit.
                    // In multi-player mode, let the time ticks so that other players can continue.
                    gameTimer.Stop();
                }
                else
                {
                    VisualStateManager.GoToState(this, "SizeNotSupportedQuitNotAllowedState", false);
                }
            }
            else if (previousWindowTooSmall && !windowTooSmall)
            {
                // User enlarges the app window.
                VisualStateManager.GoToState(this, "SizeSupportedState", false);

                if (gameMode == Constants.SinglePlayer)
                {
                    // Resume the game.
                    gameTimer.Start();
                }
            }
        }

        private void QuitGameClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        // Update this method for multiplayer and singleplayer mode.
        private void InitImages()
        {
            // Set up critter images. First one is an empty space (i.e. no critter);
            // other images are real critters using 1-based index.
            string relativeUri = @"Assets\Critter{0}.png";
            for (int n = 0; n <= Constants.CritterCount; n++)
            {
                Uri uri = new Uri(BaseUri, string.Format(relativeUri, n));
                critterImages.Add(new BitmapImage(uri));
            }

            // Set up critter stomped images. First one is an empty space (i.e. no critter);
            // other images are real critters using 1-based index.
            string stompedUri = @"Assets\Critter{0}_Stomped.png";
            for (int n = 0; n <= Constants.CritterCount; n++)
            {
                Uri uri = new Uri(BaseUri, string.Format(stompedUri, n));
                stompedImages.Add(new BitmapImage(uri));
            }

            // Store a reference to the images represented by each cell in an array
            foreach (Image image in GameArea.Children)
            {
                int row = Grid.GetRow(image);
                int column = Grid.GetColumn(image);

                images[row, column] = image;
                image.Tag = new Critter(critterImages[0]);
            }
        }

        public void SendParams(SocketReaderWriter socket, int id)
        {
            // Should only be called if the app is a host.
            // Otherwise it has no idea which socket is being used.
            if (PeerFinder.Role == PeerRole.Host)
            {
                // Write the op code, number of players, time limit.
                socket.WriteMessage(string.Format("{0} {1} {2} {3}", Constants.OpCodeParams, playerCount, timesToTick, id));
            }
        }

        public void SetParams(int playerCount, int timesToTick, int playerID)
        {
            // Should only be called if the app is a client.
            this.playerCount = playerCount;
            this.timesToTick = timesToTick;
            this.playerID = playerID;
            InitImages();
            socket.WriteMessage(Message.Ready);
        }

        public void TrackReady()
        {
            // Another host-only function
            playersReady++;
            if (playersReady == connectedPeers.Count)
            {
                foreach (SocketReaderWriter socket in connectedPeers.Values)
                {
                    socket.WriteMessage(Message.Start);
                }
                StartGame();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            LoadState(e.Parameter, null);
        }

        private void LoadState(object navigationParameter, Dictionary<string, object> pageState)
        {
            // Unpack the navigation parameters passed from either the Main Page (singleplayer mode)
            // or the Invite Players page (singlePair, multiplayer mode).

            GamePageParameters parameters = (GamePageParameters)navigationParameter;

            if (PeerFinder.Role == PeerRole.Host)
            {
                timesToTick = Constants.PlayTime * Constants.TimeUnitsPerSecond;
            }

            gameMode = parameters.GameMode;

            if (gameMode != Constants.SinglePlayer)
            {
                // Not for single player games.

                connectedPeers = parameters.ConnectedPeers;

                // You should only have one socket if you're a client
                // Regardless, update each socket's current page to the game page
                foreach (ConnectedPeer host in connectedPeers.Keys)
                {
                    socket = connectedPeers[host];
                    socket.currentPage = this;
                    socket.UpdateCurrentPageType();
                }

                playerCount = parameters.PlayerCount;
            }

            images = new Image[Constants.GridRows, Constants.GridColumns];
            random = new Random();

            gameTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromMilliseconds(Constants.TimeUnit);
            gameTimer.Tick += UpdateTimer;

            if (gameMode == Constants.SinglePlayer)
            {
                InitImages();

                timesToTick = Constants.PlayTime * Constants.TimeUnitsPerSecond;
                StartGame();
            }
            else if (gameMode == Constants.MultiPlayer)
            {
                // See what your role is.
                if (PeerFinder.Role == PeerRole.Host)
                {
                    playerList = new Dictionary<int, string>();
                    playerList[0] = PeerFinder.DisplayName;
                    playerID = 0;

                    int counter = 1;

                    foreach (ConnectedPeer peer in connectedPeers.Keys)
                    {
                        playerList[counter] = peer.DisplayName;
                        SendParams(connectedPeers[peer], counter);
                        counter++;
                    }

                    InitImages();
                }
            }
        }

        public void StartGame()
        {
            gameTimer.Start();
            UpdateTimeText();

            // Check whether window size is large enough to run the game.
            CheckWindowSizeSupport(Window.Current.Bounds.Width, Window.Current.Bounds.Height);
        }

        private void UpdateTimeText()
        {
            int timeRemainingToPlay = timesToTick - timesTicked;
            int seconds = (timeRemainingToPlay / Constants.TimeUnitsPerSecond) % Constants.SecondsPerMinute;
            int minutes = (timeRemainingToPlay / Constants.TimeUnitsPerSecond) / Constants.SecondsPerMinute;
            TimeSpan timeLeft = new TimeSpan(0, minutes, seconds);

            timeText.Text = " " + timeLeft.ToString(@"m\:ss");

            if (PeerFinder.Role == PeerRole.Host)
            {
                foreach (SocketReaderWriter tempsocket in connectedPeers.Values)
                {
                    tempsocket.WriteMessage(string.Format("{0} {1}", Constants.OpCodeUpdateClientTime, timeLeft.ToString(@"m\:ss")));
                }
            }
        }

        public void UpdateTimer(object sender, object e)
        {
            // Check whether to end the game.
            if (gameMode == Constants.SinglePlayer || PeerFinder.Role == PeerRole.Host)
            {
                if (timesTicked < timesToTick)
                {
                    timesTicked++;
                    UpdateTimeText();
                }
                else
                {
                    if (gameMode != Constants.SinglePlayer)
                    {
                        foreach (ConnectedPeer peer in connectedPeers.Keys)
                        {
                            connectedPeers[peer].WriteMessage(Message.Done);
                        }
                    }
                    NavigateToScorePage();
                }
            }

            // Check whether to spawn a critter.
            if (gameMode == Constants.SinglePlayer)
            {
                if (timeToSpawn == 0)
                {
                    int column = random.Next(0, Constants.GridColumns);
                    int row = random.Next(0, Constants.GridRows);
                    int critterIndex = random.Next(1, Constants.CritterCount + 1);
                    int timeToLive = random.Next(Constants.TimeToLiveMin, Constants.TimeToLiveMax);

                    Critter critter = (Critter)images[row, column].Tag;
                    critter.Initialize(images[row, column], critterImages[critterIndex], stompedImages[critterIndex], timeToLive);

                    timeToSpawn = random.Next(Constants.TimeToSpawnMin, Constants.TimeToSpawnMax);
                }
                timeToSpawn--;
            }
            else if (gameMode == Constants.MultiPlayer)
            {
                if (PeerFinder.Role == PeerRole.Host)
                {
                    if (timeToSpawn == 0)
                    {
                        int column = random.Next(0, Constants.GridColumns);
                        int row = random.Next(0, Constants.GridRows);

                        // Pick the type of critter.
                        int critterIndex = random.Next(1, Constants.CritterCount + 1);

                        // Pick the critter time to live.
                        int timeToLive = random.Next(Constants.TimeToLiveMin, Constants.TimeToLiveMax);

                        foreach (SocketReaderWriter tempsocket in connectedPeers.Values)
                        {
                            tempsocket.WriteMessage(
                                string.Format("{0} {1} {2} {3} {4}", Constants.OpCodeCritterBorn, column, row, critterIndex, timeToLive)
                                );
                        }

                        Critter critter = (Critter)images[row, column].Tag;
                        critter.Initialize(images[row, column], critterImages[critterIndex], stompedImages[critterIndex], timeToLive);

                        timeToSpawn = random.Next(Constants.TimeToSpawnMin, Constants.TimeToSpawnMax);
                    }
                    timeToSpawn--;
                }
            }
        }

        public void UpdateClientTime(string timeLeft)
        {
            timeText.Text = " " + timeLeft;
        }
        
        public void SpawnCritter(int column, int row, int critterIndex, int time)
        {
            // Should only be called if the player is a client.
            if (PeerFinder.Role == PeerRole.Client)
            {
                Critter tempCritter = (Critter)images[row, column].Tag;
                tempCritter.Initialize(images[row, column], critterImages[critterIndex], stompedImages[critterIndex], time);
            }
        }

        private void StompTap(object sender, TappedRoutedEventArgs e)
        {
            // Cast the tapped image to a FrameworkElement so you can get the column and row 
            // with respect to the parent grid.
            FrameworkElement tapped = (FrameworkElement)sender;
            int row = Grid.GetRow(tapped);
            int column = Grid.GetColumn(tapped);

            Image tappedImage = images[row, column];
            Critter critter = (Critter)images[row, column].Tag;

            if (critter.IsStompable)
            {
                if (gameMode == Constants.SinglePlayer)
                {
                    critter.Stomped();
                    gameScore += Constants.ScoreIncrement;
                    scoreText.Text = gameScore.ToString();
                }
                else if (gameMode == Constants.MultiPlayer)
                {
                    string message = string.Format("{0} {1} {2} {3}", Constants.OpCodeCritterStomp, row, column, playerID);

                    if (PeerFinder.Role == PeerRole.Host)
                    {
                        foreach (ConnectedPeer peer in connectedPeers.Keys)
                        {
                            connectedPeers[peer].WriteMessage(message);
                        }
                    }
                    else if (PeerFinder.Role == PeerRole.Client)
                    {
                        socket.WriteMessage(message);
                    }

                    critter.Stomped();
                    gameScore += Constants.ScoreIncrement;
                    scoreText.Text = gameScore.ToString();
                }
            }
        }

        public async void UpdateBoard(int row, int column, int playerId)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Critter critter = (Critter)images[row, column].Tag;
                critter.ResetCritter();
            });
        }

        public void NavigateToScorePage()
        {
            try
            {
                gameTimer.Stop();

                ScorePageParameters parameters = new ScorePageParameters()
                {
                    Score = gameScore,
                    ConnectedPeers = connectedPeers,
                    GameMode = gameMode
                };

                Frame.Navigate(typeof(ScorePage), parameters);
            }
            catch (Exception err)
            {
                scoreText.Text = "Error navigating: " + err.ToString();
            }
        }


        public void NavigateToMainPage()
        {
            Frame.Navigate(typeof(MainPage));
        }

        public void SetScore(string score)
        {
            scoreText.FontSize = 12;
            Foreground = new SolidColorBrush(Colors.Red);
            scoreText.Text = score;
        }
    }
}
