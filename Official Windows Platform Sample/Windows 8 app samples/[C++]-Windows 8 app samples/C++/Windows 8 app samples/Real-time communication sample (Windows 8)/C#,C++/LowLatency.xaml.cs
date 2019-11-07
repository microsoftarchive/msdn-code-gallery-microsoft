//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using SimpleCommunication.Common;
using Microsoft.Samples.SimpleCommunication;

namespace SimpleCommunication
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LowLatency : SDKTemplate.Common.LayoutAwarePage
    {
        private enum State
        {
            Initializing,
            Waiting,
            Previewing,
            Streaming,
            End
        }

        private enum LatencyMode
        {
            NormalLatency,
            LowLatency
        }

        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        CaptureDevice device = null;
        State? currentState = null;
        State? previousState = null;
        VideoEncodingProperties previewEncodingProperties = null;
        LatencyMode? mode = null;
        LatencyMode defaultMode = LatencyMode.NormalLatency;

        public LowLatency()
        {
            this.InitializeComponent();

            rootPage.EnsureMediaExtensionManager();

            mode = LatencyMode.NormalLatency;
            LatencyModeToggle.IsOn = (mode == LatencyMode.LowLatency);
            rootPage.NotifyUser("Initializing...Please wait.", NotifyType.StatusMessage);
        }

        /// <summary>
        /// The media element visible in the local host client area
        /// </summary>
        private MediaElement LocalhostVideo
        {
            get
            {
                return (mode == LatencyMode.NormalLatency) ? DefaultMediaElement : LowLatencyMediaElement;
            }
        }

        /// <summary>
        /// Initializes the scenario
        /// </summary>
        /// <returns></returns>
        private async Task InitializeAsync(CancellationToken cancel = default(CancellationToken))
        {
            var streamFilteringCriteria = new
            {
                //AspectRatio = 1.333333333333333,
                HorizontalResolution = (uint)480,
                SubType = "YUY2"
            };
            currentState = State.Initializing;
            device = new CaptureDevice();

            CameraPreview.Visibility = Visibility.Collapsed;
            PreviewPoster.Visibility = Visibility.Visible;
            Preview.Content = "Start Preview";
            LoopbackClient.IsEnabled = false;

            LatencyModeToggle.IsOn = (defaultMode == LatencyMode.LowLatency);
            LatencyModeToggle.IsEnabled = false;

            await device.InitializeAsync();
            var setting = await device.SelectPreferredCameraStreamSettingAsync(MediaStreamType.VideoPreview, ((x) =>
            {
                var previewStreamEncodingProperty = x as Windows.Media.MediaProperties.VideoEncodingProperties;

                return (previewStreamEncodingProperty.Width >= streamFilteringCriteria.HorizontalResolution &&
                    previewStreamEncodingProperty.Subtype == streamFilteringCriteria.SubType);
            }));

            previewEncodingProperties = setting as VideoEncodingProperties;

            PreviewSetupCompleted();
        }

        private void PreviewSetupCompleted()
        {
            device.CaptureFailed += device_CaptureFailed;
            device.IncomingConnectionArrived += device_IncomingConnectionArrived;
            previousState = currentState;
            Preview.IsEnabled = true;
            rootPage.NotifyUser("Done. Tap or click 'Start Preview' button to start webcam preview", NotifyType.StatusMessage);

            currentState = State.Waiting;
        }


        void device_IncomingConnectionArrived(object sender, IncomingConnectionEventArgs e)
        {
            e.Accept();
        }

        void device_CaptureFailed(object sender, MediaCaptureFailedEventArgs e)
        {
            rootPage.NotifyUser(e.Message, NotifyType.ErrorMessage);
        }

        async void Window_VisibilityChanged(object sender, Windows.UI.Core.VisibilityChangedEventArgs e)
        {
            if (!e.Visible)
            {
                try
                {
                    await CleanupAsync();
                }
                catch (Exception)
                {
                }
            }
            else
            {
                try
                {
                    await InitializeAsync();
                }
                catch (Exception err)
                {
                    rootPage.NotifyUser("Camera initialization error: " + err.Message + "\n Try restarting the sample.", NotifyType.ErrorMessage);
                }
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            var cameraEnumTask = CaptureDevice.CheckForRecordingDeviceAsync();

            var cameraFound = await cameraEnumTask;

            if (cameraFound)
            {
                try
                {
                    await InitializeAsync();

                    Window.Current.VisibilityChanged += Window_VisibilityChanged;
                }
                catch (Exception err)
                {
                    rootPage.NotifyUser("Camera initialization error: " + err.Message + "\n Try restarting the sample.", NotifyType.ErrorMessage);
                }
            }
            else
            {
                rootPage.NotifyUser("A machine with a camera and a microphone is required to run this sample.", NotifyType.ErrorMessage);
            }
        }

        protected async override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            Window.Current.VisibilityChanged -= Window_VisibilityChanged;
            try
            {
                await CleanupAsync();
            }
            catch (Exception)
            {
            }
            base.OnNavigatingFrom(e);
        }

        /// <summary>
        /// This is the click handler for the 'Preview' button.  It starts or stops 
        /// camera preview and streaming
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Preview_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b != null && device != null)
            {
                previousState = currentState;
                switch (currentState)
                {
                    case State.Waiting:
                        await Waiting_PreviewButtonClicked();
                        break;
                    case State.Previewing:
                        await Previewing_PreviewButtonClicked();
                        break;
                    case State.Streaming:
                        await Streaming_PreviewButtonClicked();
                        break;
                    default:
                        break;
                }
            }
        }

        private async Task Previewing_PreviewButtonClicked()
        {
            Task stopPreviewTask = device.CaptureSource.StopPreviewAsync().AsTask();

            CameraPreview.Visibility = Visibility.Collapsed;
            PreviewPoster.Visibility = Visibility.Visible;
            Preview.Content = "Start Preview";
            LoopbackClient.IsEnabled = false;

            await stopPreviewTask;
            currentState = State.Waiting;
        }

        private async Task Waiting_PreviewButtonClicked()
        {
            rootPage.NotifyUser("", NotifyType.StatusMessage);
            CameraPreview.Source = device.CaptureSource;
            await device.CaptureSource.StartPreviewAsync();

            PreviewPoster.Visibility = Visibility.Collapsed;
            CameraPreview.Visibility = Visibility.Visible;
            Preview.Content = "Stop Preview";
            LoopbackClient.IsEnabled = true;

            currentState = State.Previewing;
        }

        private async Task Streaming_PreviewButtonClicked()
        {
            LocalhostVideo.Visibility = Visibility.Collapsed;
            LocalHostPoster.Visibility = Visibility.Visible;

            LocalhostVideo.Stop();
            await device.StopRecordingAsync();
            LocalhostVideo.Source = null;
            await Previewing_PreviewButtonClicked();

            mode = defaultMode;
            LatencyModeToggle.IsOn = (defaultMode == LatencyMode.LowLatency);
            LatencyModeToggle.IsEnabled = false;
        }

        /// <summary>
        /// This is the click handler for the 'LoopbackClient' button.  It displays captured video
        /// recieved over the localhost network interface using a custom network media sink and media
        /// sources via a custom scheme
        /// </summary>
        /// <param name="sender">The LoopbackClient button</param>
        /// <param name="e">The button click routed event arguments</param>
        private async void LoopbackClient_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b != null)
            {
                previousState = currentState;
                await StartRecordingToCustomSink();

                if (currentState == State.Previewing)
                {
                    LocalhostVideo.Source = new Uri("stsp://localhost");
                    LatencyModeToggle.IsEnabled = true;
                    LoopbackClient.IsEnabled = false;
                    LocalHostPoster.Visibility = Visibility.Collapsed;
                    LocalhostVideo.Visibility = Visibility.Visible;

                    currentState = State.Streaming;
                }
            }
        }

        private async Task StartRecordingToCustomSink()
        {
            // Use the MP4 preset to an obtain H.264 video encoding profile
            var mep = MediaEncodingProfile.CreateMp4(VideoEncodingQuality.Vga);
            mep.Audio = null;
            mep.Container = null;
            if (previewEncodingProperties != null)
            {
                mep.Video.Width = previewEncodingProperties.Width;
                mep.Video.Height = previewEncodingProperties.Height;
            }

            await device.StartRecordingAsync(mep);
        }

        private async void LatencyModeToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (currentState == State.Streaming)
            {
                var foregroundVideo = LocalhostVideo;
                foregroundVideo.Stop();

                await device.StopRecordingAsync();
                mode = (mode == LatencyMode.NormalLatency) ? LatencyMode.LowLatency : LatencyMode.NormalLatency;
                await StartRecordingToCustomSink();

                foregroundVideo.Source = null;
                LocalhostVideo.Source = new Uri("stsp://localhost");
                foregroundVideo.Visibility = Visibility.Collapsed;
                LocalhostVideo.Visibility = Visibility.Visible;
            }
        }

        private async Task CleanupAsync()
        {
            if (currentState == State.Previewing || currentState == State.Streaming)
            {
                await device.CaptureSource.StopPreviewAsync();

                if (currentState == State.Streaming)
                {
                    LocalhostVideo.Stop();

                    LocalhostVideo.Visibility = Visibility.Collapsed;
                    LocalHostPoster.Visibility = Visibility.Visible;
                }
            }


            await device.CleanUpAsync();
            LocalhostVideo.Source = null;
        }
    }
}
