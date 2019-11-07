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
using Windows.Storage.Pickers;
using Windows.UI.ViewManagement;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.Foundation;

namespace BasicMediaPlayback
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario4 : SDKTemplate.Common.LayoutAwarePage
    {
        /// <summary>
        /// Timer used to update the media position Slider.
        /// </summary>
        private DispatcherTimer _timer;
        /// <summary>
        /// Tracks whether the user is currently interacting with the media position Slider.
        /// </summary>
        private bool _sliderPressed = false;

        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario4()
        {
            this.InitializeComponent();
            TimelineSlider.AddHandler(Slider.PointerPressedEvent, new PointerEventHandler(TimelineSlider_PointerPressed), true);
            TimelineSlider.AddHandler(Slider.PointerReleasedEvent, new PointerEventHandler(TimelineSlider_PointerReleased), true);
            
            _timer = new DispatcherTimer();
            SetFullWindowMode(false);
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        /// <summary>
        /// Invoked when this page will no longer be displayed in a Frame.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Set source to null to ensure it stops playing to a Play To device if applicable.
            Scenario4MediaElement.Source = null;
        }

        /// <summary>
        /// Click handler for the "Select a media file" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker fileOpenPicker = new FileOpenPicker();

            // Filter to include a sample subset of file types
            fileOpenPicker.FileTypeFilter.Add(".wmv");
            fileOpenPicker.FileTypeFilter.Add(".mp4");
            fileOpenPicker.FileTypeFilter.Add(".mp3");
            fileOpenPicker.FileTypeFilter.Add(".wma");
            fileOpenPicker.SuggestedStartLocation = PickerLocationId.VideosLibrary;

            // Prompt user to select a file            
            StorageFile file = await fileOpenPicker.PickSingleFileAsync();

            // Ensure a file was selected
            if (file != null)
            {
                // Open the selected file and set it as the MediaElement's source
                IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);
                Scenario4MediaElement.SetSource(stream, file.ContentType);
            }
        }

        /// <summary>
        /// Click handler for the "Play" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            Scenario4MediaElement.PlaybackRate = 1.0;

            Scenario4MediaElement.Play();
            SetupTimer();
        }

        /// <summary>
        /// Click handler for the "Pause" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            Scenario4MediaElement.Pause();
        }

        /// <summary>
        /// Click handler for the "Stop" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            Scenario4MediaElement.Stop();
        }

        /// <summary>
        /// Click handler for the "Rewind" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RewindButton_Click(object sender, RoutedEventArgs e)
        {
            // Play media in reverse at 1x speed
            Scenario4MediaElement.PlaybackRate = -1.0;
        }

        /// <summary>
        /// Click handler for the "Forward" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            // Fast forward media at 2x speed
            Scenario4MediaElement.PlaybackRate = 2.0;
        }

        /// <summary>
        /// Click handler for the "Zoom" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZoomButton_Click(object sender, RoutedEventArgs e)
        {
            if (Scenario4MediaElement.Stretch == Stretch.Uniform)
            {
                // Stretch to fill, which will zoom in and clip content to remove letterboxing if the media file's aspect ratio does not match the MediaElement's aspect ratio
                Scenario4MediaElement.Stretch = Stretch.UniformToFill;
            }
            else
            {
                // Stretch uniformly, which will result in letterboxing if the media file's aspect ratio does not match the MediaElement's aspect ratio
                Scenario4MediaElement.Stretch = Stretch.Uniform;
            }
        }

        /// <summary>
        /// Click handler for the "Full Window" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FullWindowButton_Click(object sender, RoutedEventArgs e)
        {
            SetFullWindowMode(!Scenario4MediaElement.IsFullWindow);
        }

        /// <summary>
        /// Double tap handler for the MediaElement.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MediaElement_DoubleTapped(object sender, Windows.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            SetFullWindowMode(!Scenario4MediaElement.IsFullWindow);
        }

        /// <summary>
        /// Toggles full-window mode and updates the position of transport controls
        /// </summary>
        /// <param name="isFullWindow"></param>
        private void SetFullWindowMode(bool isFullWindow)
        {
            Scenario4MediaElement.IsFullWindow = isFullWindow;

            if (isFullWindow)
            {
                // When displaying in full-window mode, center transport controls at the bottom of the window

                // Since the Popup is in the Output Grid, the offset must account for its parent
                var rootFrame = Window.Current.Content as Frame;
                var outputGridOffset = Output.TransformToVisual(rootFrame).TransformPoint(new Point(0, 0));

                TransportControlsPopup.HorizontalOffset = (rootFrame.ActualWidth - TransportControlsPopup.ActualWidth) / 2 - outputGridOffset.X;
                TransportControlsPopup.VerticalOffset = rootFrame.ActualHeight - TransportControlsPopup.ActualHeight - outputGridOffset.Y;
            }
            else
            {
                // When displaying in embedded mode, center transport controls at the bottom of the MediaElement
                TransportControlsPopup.HorizontalOffset = (Scenario4MediaElement.Width - TransportControlsPopup.ActualWidth) / 2;
                TransportControlsPopup.VerticalOffset = Scenario4MediaElement.Height - TransportControlsPopup.ActualHeight;
            }
        }

        /// <summary>
        /// MediaOpened event handler for the MediaElement.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            // Set slider maximum to the media's duration in seconds
            TimelineSlider.Maximum = Math.Round(Scenario4MediaElement.NaturalDuration.TimeSpan.TotalSeconds, MidpointRounding.AwayFromZero);
            TimelineSlider.StepFrequency = CalculateSliderFrequency(Scenario4MediaElement.NaturalDuration.TimeSpan);

            SetupTimer();
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            _timer.Tick -= Timer_Tick;

            TimelineSlider.Value = 0.0;
        }

        #region Timeline Slider interaction

        void TimelineSlider_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _sliderPressed = true;
        }

        void TimelineSlider_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (_sliderPressed)
            { 
                Scenario4MediaElement.Position = TimeSpan.FromSeconds(TimelineSlider.Value);
                _sliderPressed = false;
            }
        }

        private double CalculateSliderFrequency(TimeSpan timevalue)
        {
            // Calculate the slider step frequency based on the timespan length
            double stepFrequency = 1;

            if (timevalue.TotalHours >= 1)
            {
                stepFrequency = 60;
            }
            else if (timevalue.TotalMinutes > 30)
            {
                stepFrequency = 30;
            }
            else if (timevalue.TotalMinutes > 10)
            {
                stepFrequency = 10;
            }
            else
            {
                stepFrequency = Math.Round(timevalue.TotalSeconds / 100, MidpointRounding.AwayFromZero);
            }

            return stepFrequency;
        }

        private void SetupTimer()
        {
            if (!_timer.IsEnabled)
            {
                _timer.Interval = TimeSpan.FromSeconds(TimelineSlider.StepFrequency);
                _timer.Tick += Timer_Tick;
                _timer.Start();
            }
        }

        private void Timer_Tick(object sender, object e)
        {
            // Don't update the Slider's position while the user is interacting with it
            if (!_sliderPressed)
            {
                TimelineSlider.Value = Scenario4MediaElement.Position.TotalSeconds;
            }
        }

        #endregion

    }
}
