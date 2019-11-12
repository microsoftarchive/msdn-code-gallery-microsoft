using System;
using Windows.Networking.Proximity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CritterStomp
{
    public sealed partial class MainPage : Page
    {
        public string Invitation
        {
            get { return invitationText.Text; }
            set { invitationText.Text = value; }
        }

        public MainPage()
        {
            InitializeComponent();
            PeerFinder.Stop();

            SizeChanged += OnWindowSizeChanged;
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

        private void StompAloneClick(object sender, RoutedEventArgs e)
        {
            GamePageParameters parameters = new GamePageParameters()
            {
                GameMode = Constants.SinglePlayer,
                Role = PeerFinder.Role,
                ConnectedPeers = null,
                PlayerCount = 1
            };

            Frame.Navigate(typeof(GamePage), parameters);
        }

        private void StompWithFriendsClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(JoinOrHost));            
        }
    }
}
