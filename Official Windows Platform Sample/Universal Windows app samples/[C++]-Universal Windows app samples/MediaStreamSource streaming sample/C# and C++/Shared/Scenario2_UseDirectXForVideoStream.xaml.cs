//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

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
using Windows.Media.Core;
using Windows.Media.MediaProperties;
using SDKTemplate;

namespace MediaStreamSource
{

    public sealed partial class Scenario2_UseDirectXForVideoStream
    {
        private MainPage rootPage = MainPage.Current;
        private VideoStreamDescriptor _videoDesc = null;
        private Windows.Media.Core.MediaStreamSource _mss = null;
        private DXSurfaceGenerator.SampleGenerator _sampleGenerator = null;
        private bool _hasSetMediaSource = false;
        private bool _mediaSourceIsLoaded = false;
        private bool _playRequestPending = false;
        
        private const int c_frameRateN = 30;
        private const int c_frameRateD = 1;
        
        public Scenario2_UseDirectXForVideoStream()
        {
            this.InitializeComponent();
        }

        void InitializeMediaPlayer()
        {
            int iWidth = (int)Window.Current.Bounds.Width;
            int iHeight = (int)Window.Current.Bounds.Height;

            // Even frame size with a 16:9 ratio
            iWidth = Math.Min(iWidth, ((iHeight * 16 / 9) >> 1) * 2);
            iHeight = Math.Min(iHeight, ((iWidth * 9 / 16) >> 1) * 2);

            VideoEncodingProperties videoProperties = VideoEncodingProperties.CreateUncompressed(MediaEncodingSubtypes.Bgra8, (uint)iWidth, (uint)iHeight);
            _videoDesc = new VideoStreamDescriptor(videoProperties);
            _videoDesc.EncodingProperties.FrameRate.Numerator = c_frameRateN;
            _videoDesc.EncodingProperties.FrameRate.Denominator = c_frameRateD;
            _videoDesc.EncodingProperties.Bitrate = (uint)(c_frameRateN * c_frameRateD * iWidth * iHeight * 4);

            _mss = new Windows.Media.Core.MediaStreamSource(_videoDesc);
            TimeSpan spanBuffer = new TimeSpan(0, 0, 0, 0, 250);
            _mss.BufferTime = spanBuffer;
            _mss.Starting += _mss_Starting;
            _mss.SampleRequested += _mss_SampleRequested;

            _sampleGenerator = new DXSurfaceGenerator.SampleGenerator();

            mediaPlayer.AutoPlay = false;
            mediaPlayer.CurrentStateChanged += mediaPlayer_CurrentStateChanged;
            mediaPlayer.SetMediaStreamSource(_mss);
            _hasSetMediaSource = true;
        }

        void mediaPlayer_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            switch (mediaPlayer.CurrentState)
            {
                case MediaElementState.Paused:
                case MediaElementState.Stopped:
                    _mediaSourceIsLoaded = true;
                    if (_playRequestPending)
                    {
                        mediaPlayer.Play();
                        _playRequestPending = false;
                    }
                    break;
            }
        }

        void _mss_SampleRequested(Windows.Media.Core.MediaStreamSource sender, MediaStreamSourceSampleRequestedEventArgs args)
        {
            _sampleGenerator.GenerateSample(args.Request);
        }

        void _mss_Starting(Windows.Media.Core.MediaStreamSource sender, MediaStreamSourceStartingEventArgs args)
        {
            _sampleGenerator.Initialize(_mss, _videoDesc);
            args.Request.SetActualStartPosition(new TimeSpan(0));
        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_hasSetMediaSource)
            {
                InitializeMediaPlayer();
            }

            if (_mediaSourceIsLoaded)
            {
                mediaPlayer.Play();
                _playRequestPending = false;
            }
            else
            {
                _playRequestPending = true;
            }
            rootPage.NotifyUser("Playing the DirectX Media", NotifyType.StatusMessage);
        }

        private void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_playRequestPending)
            {
                _playRequestPending = false;
            }

            mediaPlayer.Pause();
            rootPage.NotifyUser("Pausing the DirectX Media", NotifyType.StatusMessage);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            // As we are still playing we need to stop MSS from requesting for more samples otherwise we'll crash
            if (_mss != null)
            {
                _mss.NotifyError(MediaStreamSourceErrorStatus.Other);
                mediaPlayer.Stop();
            }
        }
    }
}
