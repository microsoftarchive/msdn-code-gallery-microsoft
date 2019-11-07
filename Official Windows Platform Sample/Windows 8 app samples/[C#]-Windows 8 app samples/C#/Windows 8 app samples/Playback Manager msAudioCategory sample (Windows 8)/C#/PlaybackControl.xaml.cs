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
using Windows.UI.Xaml.Media;
using Windows.UI.Core;
using Windows.Media;

namespace PlaybackManager
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlaybackControl
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        CoreWindow cw = Window.Current.CoreWindow;
        bool IsPlaying = false;

        static bool IsMediaControlRegistered = false;
        static EventHandler<object> SoundLevelChangedHandler = null;
        static EventHandler<object> PlayPauseTogglePressedHandler = null;
        static EventHandler<object> PlayPressedHandler = null;
        static EventHandler<object> PausePressedHandler = null;
        static EventHandler<object> StopPressedHandler = null;

        public PlaybackControl()
        {
            this.InitializeComponent();

            if (IsMediaControlRegistered)
            {
                // remove previous handlers
                MediaControl.SoundLevelChanged -= SoundLevelChangedHandler;
                MediaControl.PlayPauseTogglePressed -= PlayPauseTogglePressedHandler;
                MediaControl.PlayPressed -= PlayPressedHandler;
                MediaControl.PausePressed -= PausePressedHandler;
                MediaControl.StopPressed -= StopPressedHandler;
            }
            // add new handlers
            MediaControl.SoundLevelChanged += MediaControl_SoundLevelChanged;
            MediaControl.PlayPauseTogglePressed += MediaControl_PlayPauseTogglePressed;
            MediaControl.PlayPressed += MediaControl_PlayPressed;
            MediaControl.PausePressed += MediaControl_PausePressed;
            MediaControl.StopPressed += MediaControl_StopPressed;

            // save current handlers
            SoundLevelChangedHandler = MediaControl_SoundLevelChanged;
            PlayPauseTogglePressedHandler = MediaControl_PlayPauseTogglePressed;
            PlayPressedHandler = MediaControl_PlayPressed;
            PausePressedHandler = MediaControl_PausePressed;
            StopPressedHandler = MediaControl_StopPressed;

            IsMediaControlRegistered = true;

            MediaControl.IsPlaying = false;
        }

        async public void Play()
        {
            await cw.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                OutputMedia.Play();
                MediaControl.IsPlaying = true;
            });
        }

        async public void Pause()
        {
            await cw.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                OutputMedia.Pause();
                MediaControl.IsPlaying = false;
            });
        }

        async public void Stop()
        {
            await cw.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                OutputMedia.Stop();
                MediaControl.IsPlaying = false;
            });
        }

        public void SetAudioCategory(AudioCategory category)
        {
            OutputMedia.AudioCategory = category;
        }

        public void SetAudioDeviceType(AudioDeviceType devicetype)
        {
            OutputMedia.AudioDeviceType = devicetype;
        }

        public async void SelectFile()
        {
            Windows.Storage.Pickers.FileOpenPicker picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.MusicLibrary;
            picker.FileTypeFilter.Add(".mp3");
            picker.FileTypeFilter.Add(".mp4");
            picker.FileTypeFilter.Add(".m4a");
            picker.FileTypeFilter.Add(".wma");
            picker.FileTypeFilter.Add(".wav");
            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                OutputMedia.AutoPlay = false;
                await cw.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    OutputMedia.SetSource(stream, file.ContentType);
                });
            }
        }

        async void DisplayStatus(string text)
        {
            await cw.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Status.Text += text + "\n";
            });
        }

        string GetTimeStampedMessage(string eventText)
        {
            return eventText + "   " + DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString();
        }

	//If your app is playing media you feel that a user should not miss if a VOIP call comes in, you may
	//want to consider pausing playback when your app receives a SoundLevel(Low) notification.
	//A SoundLevel(Low) means your app volume has been attenuated by the system (likely for a VOIP call).

        string SoundLevelToString(SoundLevel level)
        {
            string LevelString;

            switch (level)
            {
                case SoundLevel.Muted:
                    LevelString = "Muted";
                    break;
                case SoundLevel.Low:
                    LevelString = "Low";
                    break;
                case SoundLevel.Full:
                    LevelString = "Full";
                    break;
                default:
                    LevelString = "Unknown";
                    break;
            }
            return LevelString;
        }

        async void AppMuted()
        {
            await cw.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (OutputMedia.CurrentState != MediaElementState.Paused)
                {
                    IsPlaying = true;
                    Pause();
                }
                else
                {
                    IsPlaying = false;
                }
            });
            DisplayStatus(GetTimeStampedMessage("Muted"));
        }

        async void AppUnmuted()
        {
            await cw.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (IsPlaying)
                {
                    Play();
                }
            });
            DisplayStatus(GetTimeStampedMessage("Unmuted"));
        }

        void MediaControl_SoundLevelChanged(object sender, object e)
        {
            var soundLevelState = MediaControl.SoundLevel;

            DisplayStatus(GetTimeStampedMessage("App sound level is [" + SoundLevelToString(soundLevelState) + "]"));
            if (soundLevelState == SoundLevel.Muted)
            {
                AppMuted();
            }
            else
            {
                AppUnmuted();
            }
        }

        void MediaControl_PlayPauseTogglePressed(object sender, object e)
        {
            if (MediaControl.IsPlaying)
            {
                Pause();
                DisplayStatus(GetTimeStampedMessage("Play/Pause Pressed - Pause"));
            }
            else
            {
                Play();
                DisplayStatus(GetTimeStampedMessage("Play/Pause Pressed - Play"));
            }
        }

        void MediaControl_PlayPressed(object sender, object e)
        {
            Play();
            DisplayStatus(GetTimeStampedMessage("Play Pressed"));
        }

        void MediaControl_PausePressed(object sender, object e)
        {
            Pause();
            DisplayStatus(GetTimeStampedMessage("Pause Pressed"));
        }

        void MediaControl_StopPressed(object sender, object e)
        {
            Stop();
            DisplayStatus(GetTimeStampedMessage("Stop Pressed"));
        }

        private void Button_Play_Click(object sender, RoutedEventArgs e)
        {
            Play();
        }

        private void Button_Pause_Click(object sender, RoutedEventArgs e)
        {
            Pause();
        }

        private void Button_Stop_Click(object sender, RoutedEventArgs e)
        {
            Stop();
        }
    }
}
