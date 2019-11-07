using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


using Windows.Media.Capture;
using Windows.Storage;
using System.Windows;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace ScreenRecorderSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    enum RecordingStatus
    {
        stopped,
        recording,
        failed,
        sucessful
    } ;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {



        MediaCapture _mediaCapture = null;
        RecordingStatus _recordingStatus = RecordingStatus.stopped;

        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

  

        private void StartRecordButton_Click_1(object sender, RoutedEventArgs e)
        {
            StartRecording();
        }
        private async void StartRecording()
        {

            try
            {
                // Get instance of the ScreenCapture object
                var screenCapture = Windows.Media.Capture.ScreenCapture.GetForCurrentView();

                // Set the MediaCaptureInitializationSettings to use the ScreenCapture as the
                // audio and video source.
                var mcis = new Windows.Media.Capture.MediaCaptureInitializationSettings();
                mcis.VideoSource = screenCapture.VideoSource;
                mcis.AudioSource = screenCapture.AudioSource;
                mcis.StreamingCaptureMode = Windows.Media.Capture.StreamingCaptureMode.AudioAndVideo;

                // Initialize the MediaCapture with the initialization settings.
                _mediaCapture = new MediaCapture();
                await _mediaCapture.InitializeAsync(mcis);

                // Set the MediaCapture to a variable in App.xaml.cs to handle suspension.
                (App.Current as App).MediaCapture = _mediaCapture;

                // Hook up events for the Failed, RecordingLimitationExceeded, and SourceSuspensionChanged events
                _mediaCapture.Failed += new Windows.Media.Capture.MediaCaptureFailedEventHandler(RecordingFailed);
                _mediaCapture.RecordLimitationExceeded += 
                    new Windows.Media.Capture.RecordLimitationExceededEventHandler(RecordingReachedLimit);
                screenCapture.SourceSuspensionChanged += 
                    new Windows.Foundation.TypedEventHandler<ScreenCapture, SourceSuspensionChangedEventArgs>(SourceSuspensionChanged);

                // Create a file to record to.                 
                var videoFile = await ApplicationData.Current.LocalFolder.CreateFileAsync("recording.mp4", CreationCollisionOption.ReplaceExisting);

                // Create an encoding profile to use.                  
                var profile = Windows.Media.MediaProperties.MediaEncodingProfile.CreateMp4(Windows.Media.MediaProperties.VideoEncodingQuality.HD1080p);

                // Start recording
                var start_action = _mediaCapture.StartRecordToStorageFileAsync(profile, videoFile);
                start_action.Completed += CompletedStart;

                // Set a tracking variable for recording state in App.xaml.cs
                (App.Current as App).IsRecording = true;

            }
            catch (Exception ex)
            {
                NotifyUser("StartRecord Exception: " + ex.Message, NotifyType.ErrorMessage);
            }
        }
        private void StopRecordButton_Click_1(object sender, RoutedEventArgs e)
        {
            StopRecording();
        }
        private void StopRecording()
        {
 
            try
            {
                //Stop Screen Recorder                  
                var stop_action = _mediaCapture.StopRecordAsync();
                stop_action.Completed += CompletedStop;

                // Set a tracking variable for recording state in App.xaml.cs
                (App.Current as App).IsRecording = false;

            }
            catch (Exception ex)
            {
                NotifyUser("StopRecord Exception: " + ex.Message, NotifyType.ErrorMessage);
            }
        }
        public void RecordingFailed(Windows.Media.Capture.MediaCapture currentCaptureObject, MediaCaptureFailedEventArgs currentFailure)
        {
            _recordingStatus = RecordingStatus.failed;
            NotifyUser("RecordFailed Event raised. ", NotifyType.ErrorMessage);
        }
        

        public void RecordingReachedLimit(Windows.Media.Capture.MediaCapture currentCaptureObject)
        {
            _recordingStatus = RecordingStatus.failed;
            NotifyUser("RecordLimitationExceeded event raised.", NotifyType.ErrorMessage);
        }
        private void SourceSuspensionChanged(ScreenCapture sender, SourceSuspensionChangedEventArgs args)
        {
            NotifyUser("SourceSuspensionChanged Event. Args: IsAudioSuspended:" + 
                args.IsAudioSuspended.ToString() + 
                " IsVideoSuspended:" + 
                args.IsVideoSuspended.ToString(),
                NotifyType.ErrorMessage);
        }
        
        public void CompletedStart(IAsyncAction asyncInfo, AsyncStatus asyncStatus)
        {
            if (asyncStatus == AsyncStatus.Completed)
            {
                _recordingStatus = RecordingStatus.recording;
            }
        }
        public async void CompletedStop(IAsyncAction asyncInfo, AsyncStatus asyncStatus)
        {
            if (asyncStatus == AsyncStatus.Completed)
            {
                if (_recordingStatus == RecordingStatus.recording)
                {
                    _recordingStatus = RecordingStatus.sucessful;

                    var file = await ApplicationData.Current.LocalFolder.GetFileAsync("recording.mp4");
                    var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);

                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        mediaElement.SetSource(stream, "");
                        mediaElement.Play();
                    });

                    _recordingStatus = RecordingStatus.stopped;
                }

                if (_recordingStatus == RecordingStatus.failed)
                {
                    // Recording has failed somewhere. Set the recording status to stopped.
                    _recordingStatus = RecordingStatus.stopped;
                }
            }
        }
        public void Render()
        {
            if (_recordingStatus == RecordingStatus.recording)
            {
                /* render and udpate contents that needs to be recorded */
            }
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Always dispose of the MediaCapture device before your app is suspended.
            _mediaCapture.Dispose();
        }
        private void Grid_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            
            var position = e.GetCurrentPoint(_canvas).Position;
            position.Y = (position.Y < 0) ? 0 : position.Y;

            _circle.SetValue(Canvas.LeftProperty, position.X);
            _circle.SetValue(Canvas.TopProperty, position.Y);
            
        }
        public void NotifyUser(string strMessage, NotifyType type)
        {
            if (StatusBlock != null)
            {
                switch (type)
                {
                    case NotifyType.StatusMessage:
                        StatusBorder.Background = new SolidColorBrush(Windows.UI.Colors.Green);
                        break;
                    case NotifyType.ErrorMessage:
                        StatusBorder.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                        break;
                }
                StatusBlock.Text = strMessage;

                // Collapse the StatusBlock if it has no text to conserve real estate.
                if (StatusBlock.Text != String.Empty)
                {
                    StatusBorder.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
                else
                {
                    StatusBorder.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
            }
        }
        public enum NotifyType
        {
            StatusMessage,
            ErrorMessage
        };
    }
}
