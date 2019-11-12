using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace CritterStomp
{
    // This class holds information about each critter that appears on the game canvas.
    public class Critter
    {
        private BitmapImage critterFace;
        private BitmapImage stompedFace;
        private BitmapImage blankFace;
        private Image GridDisplay;
        private DispatcherTimer lifeTimer;

        private int timeRemaining;  // Remaining time to show as either stompable or stomped.
        private CritterStatus status;

        private enum CritterStatus
        {
            Empty,          // No critter.
            Stompable,      // Critter is alive and stompable.
            Stomped         // Critter is stomped.
        }

        public Critter()
        {
        }

        public Critter(BitmapImage blankImage)
        {
            blankFace = blankImage;

            lifeTimer = new DispatcherTimer();
            lifeTimer.Interval = TimeSpan.FromMilliseconds(Constants.TimeUnit);
            lifeTimer.Tick += UpdateCritter;

            status = CritterStatus.Empty;
        }

        public void Initialize(
            Image tappedImage,
            BitmapImage critterImage,
            BitmapImage stompedImage,
            int timeToLive
            )
        {
            GridDisplay = tappedImage;
            critterFace = critterImage;
            stompedFace = stompedImage;

            tappedImage.Source = critterFace;
            status = CritterStatus.Stompable;
            timeRemaining = timeToLive;

            lifeTimer.Start();
        }

        private void UpdateCritter(object sender, object e)
        {
            if (status == CritterStatus.Stompable
                || status == CritterStatus.Stomped)
            {
                timeRemaining--;
                if (timeRemaining <= 0)
                {
                    ResetCritter();
                }
            }
        }

        public void ResetCritter()
        {
            lifeTimer.Stop();
            status = CritterStatus.Empty;
            GridDisplay.Source = blankFace;
        }

        // Should be called when a player taps a critter.
        public void Stomped()
        {
            if (status == CritterStatus.Stompable)
            {
                status = CritterStatus.Stomped;
                GridDisplay.Source = stompedFace;
                timeRemaining = Constants.TimeToShowAsStomped;
            }
        }

        public bool IsStompable
        {
            get { return status == CritterStatus.Stompable; }
        }

        public override string ToString()
        {
            return IsStompable.ToString();
        }
    }
}
