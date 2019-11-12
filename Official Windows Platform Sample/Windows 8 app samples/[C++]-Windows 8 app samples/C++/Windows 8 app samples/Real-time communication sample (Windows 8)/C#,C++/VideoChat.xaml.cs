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
using Windows.UI.Core;
using Windows.Media.MediaProperties;
using SimpleCommunication.Common;
using Microsoft.Samples.SimpleCommunication;

namespace SimpleCommunication
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class VideoChat : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        CaptureDevice device = null;
        bool? roleIsActive = null;
        int isTerminator = 0;
        bool activated = false;

        public VideoChat()
        {
            this.InitializeComponent();

            rootPage.EnsureMediaExtensionManager();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            var cameraFound = await CaptureDevice.CheckForRecordingDeviceAsync();

            if (cameraFound)
            {
                device = new CaptureDevice();
                await InitializeAsync();
                device.IncomingConnectionArrived += device_IncomingConnectionArrived;
                device.CaptureFailed += device_CaptureFailed;
                RemoteVideo.MediaFailed += RemoteVideo_MediaFailed;
            }
            else
            {
                rootPage.NotifyUser("A machine with a camera and a microphone is required to run this sample.", NotifyType.ErrorMessage);
            }
        }

        protected async override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            if (activated)
            {
                RemoteVideo.Stop();
                RemoteVideo.Source = null;
            }

            await device.CleanUpAsync();
            device = null;
        }

        private async Task InitializeAsync(CancellationToken cancel = default(CancellationToken))
        {
            rootPage.NotifyUser("Initializing...", NotifyType.StatusMessage);

            try
            {
                await device.InitializeAsync();
                await StartRecordToCustomSink();

                HostName.IsEnabled = Call.IsEnabled = true;
                EndCall.IsEnabled = false;
                RemoteVideo.Source = null;
                RemoteVideo.Height = (RemoteVideo.Parent as Grid).ActualHeight;

                // Each client starts out as passive
                roleIsActive = false;
                Interlocked.Exchange(ref isTerminator, 0);

                rootPage.NotifyUser("Tap 'Call' button to start call", NotifyType.StatusMessage);
            }
            catch (Exception)
            {
                rootPage.NotifyUser("Initialization error. Restart the sample to try again.", NotifyType.ErrorMessage);
            }

        }

        async void RemoteVideo_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            if (Interlocked.CompareExchange(ref isTerminator, 1, 0) == 0)
            {
                await EndCallAsync();
            }
        }

        async void device_IncomingConnectionArrived(object sender, IncomingConnectionEventArgs e)
        {
            e.Accept();

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, (() =>
            {
                activated = true;
                CallPoster.Visibility = Visibility.Collapsed;
                RemoteVideo.Visibility = Visibility.Visible;

                var remoteAddress = e.RemoteUrl;
                EndCall.IsEnabled = true;
                Interlocked.Exchange(ref isTerminator, 0);

                if (!((bool)roleIsActive))
                {
                    // Passive client
                    RemoteVideo.Source = new Uri(remoteAddress);
                    HostName.IsEnabled = Call.IsEnabled = false;
                }

                remoteAddress = remoteAddress.Replace("stsp://", "");
                rootPage.NotifyUser("Connected. Remote machine address: " + remoteAddress, NotifyType.StatusMessage);
            }));
        }

        async void device_CaptureFailed(object sender, Windows.Media.Capture.MediaCaptureFailedEventArgs e)
        {
            if (Interlocked.CompareExchange(ref isTerminator, 1, 0) == 0)
            {
                await EndCallAsync();
            }
        }

        private async Task StartRecordToCustomSink()
        {
            MediaEncodingProfile mep = MediaEncodingProfile.CreateMp4(VideoEncodingQuality.Qvga);

            mep.Video.FrameRate.Numerator = 15;
            mep.Video.FrameRate.Denominator = 1;
            mep.Container = null;

            await device.StartRecordingAsync(mep);
        }

        /// <summary>
        /// This is the click handler for the 'Default' button.  You would replace this with your own handler
        /// if you have a button or buttons on this page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Call_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            var address = HostName.Text;

            if (b != null && !String.IsNullOrEmpty(address))
            {
                roleIsActive = true;
                RemoteVideo.Source = new Uri("stsp://" + address);
                HostName.IsEnabled = Call.IsEnabled = false;
                rootPage.NotifyUser("Initiating connection... Please wait.", NotifyType.StatusMessage);
            }
        }

        /// <summary>
        /// This is the click handler for the 'Other' button.  You would replace this with your own handler
        /// if you have a button or buttons on this page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void EndCall_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b != null && Interlocked.CompareExchange(ref isTerminator, 1, 0) == 0)
            {
                await EndCallAsync();
            }
        }

        private async Task EndCallAsync()
        {
            await device.CleanUpAsync();

            // end the call session
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, (() =>
            {
                RemoteVideo.Visibility = Visibility.Collapsed;
                CallPoster.Visibility = Visibility.Visible;
                RemoteVideo.Stop();
                RemoteVideo.Source = null;
            }));

            // Start waiting for a new call
            await InitializeAsync();
        }
    }
}
