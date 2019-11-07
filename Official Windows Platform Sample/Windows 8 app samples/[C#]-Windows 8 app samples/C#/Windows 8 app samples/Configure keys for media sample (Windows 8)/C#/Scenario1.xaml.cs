//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using SDKTemplate;
using System;
using Windows.Media;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;

namespace template
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario1 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()

        MainPage rootPage = MainPage.Current;

        int currentSongIndex = 0;
        int playlistCount = 0;
        bool previousRegistered = false;
        bool nextRegistered = false;
        bool wasPlaying = false;
        List<song> playlist = new List<song>();


        public class song
        {
            public Windows.Storage.StorageFile File;
            public string Artist;
            public string Track;
            //public Windows.Storage.FileProperties.StorageItemThumbnail AlbumArt;

            public song(Windows.Storage.StorageFile file)
            {
                File = file;
            }

            public async Task getMusicPropertiesAsync()
            {
                var properties = await this.File.Properties.GetMusicPropertiesAsync();
                //Windows.Storage.FileProperties.StorageItemThumbnail thumbnail = null;

                
                Artist = properties.Artist;
                Track = properties.Title;
            }
        }

        public Scenario1()
        {

            this.InitializeComponent();

            DefaultOption.Click += DefaultOption_Click;
                         
            MediaControl.PlayPauseTogglePressed += MediaControl_PlayPauseTogglePressed;
            MediaControl.PlayPressed += MediaControl_PlayPressed;
            MediaControl.PausePressed += MediaControl_PausePressed;
            MediaControl.StopPressed += MediaControl_StopPressed;

            MediaControl.FastForwardPressed += MediaControl_FastForwardPressed;
            MediaControl.RewindPressed += MediaControl_RewindPressed;
            MediaControl.RecordPressed += MediaControl_RecordPressed;
            MediaControl.ChannelDownPressed += MediaControl_ChannelDownPressed;
            MediaControl.ChannelUpPressed += MediaControl_ChannelUpPressed;

            PlayButton.Click += new RoutedEventHandler(MediaControl_PlayPauseTogglePressed);
            mediaElement.CurrentStateChanged += mediaElement_CurrentStateChanged;
            mediaElement.MediaOpened += mediaElement_MediaOpened;
            mediaElement.MediaEnded += mediaElement_MediaEnded;

            Scenario1Text.Text = "Events Registered";
        }

        async void DefaultOption_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.List;
            openPicker.SuggestedStartLocation = PickerLocationId.MusicLibrary;
            openPicker.FileTypeFilter.Add(".mp3");
            openPicker.FileTypeFilter.Add(".m4a");
            openPicker.FileTypeFilter.Add(".wma");
            IReadOnlyList<StorageFile> files = await openPicker.PickMultipleFilesAsync();
            if (files.Count > 0)
            {
                await createPlaylist(files);
                await SetCurrentPlayingAsync(currentSongIndex);
            }            
        }

        async void mediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (currentSongIndex < playlistCount - 1)
            {
                currentSongIndex++;

                await SetCurrentPlayingAsync(currentSongIndex);
                if (wasPlaying)
                {
                    mediaElement.Play();
                }
            }
        }

        void mediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (wasPlaying)
            {
                mediaElement.Play();
            }
        }



        void mediaElement_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            if (mediaElement.CurrentState == MediaElementState.Playing)
            {
                MediaControl.IsPlaying = true;
                PlayButton.Content = "Pause";
            }
            else
            {
                MediaControl.IsPlaying = false;
                PlayButton.Content = "Play";
            }
        }

        async Task SetMediaElementSourceAsync(song Song)
        {
            var stream = await Song.File.OpenAsync(Windows.Storage.FileAccessMode.Read);
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { mediaElement.SetSource(stream, Song.File.ContentType); });


            
            MediaControl.ArtistName     = Song.Artist;
            MediaControl.TrackName      = Song.Track;

        }


        async Task createPlaylist(IReadOnlyList<StorageFile> files)
        {

            playlistCount = files.Count;
            if (previousRegistered)
            {
                MediaControl.PreviousTrackPressed -= MediaControl_PreviousTrackPressed;
                previousRegistered = false;                
            }
            if (nextRegistered)
            {
                MediaControl.NextTrackPressed -= MediaControl_NextTrackPressed;
                nextRegistered = false;
            }
            currentSongIndex    = 0;
            playlist.Clear();

            if (files.Count > 0)
            {
                // Application now has read/write access to the picked file(s) 
                if (files.Count > 1)
                {
                    MediaControl.NextTrackPressed   += MediaControl_NextTrackPressed;
                    nextRegistered                  = true;
                }

                // Create the playlist
                foreach (StorageFile file in files)
                {
                    song newSong = new song(file);
                    await newSong.getMusicPropertiesAsync();
                    playlist.Add(newSong);
                }


            }
            
        }

        async Task SetCurrentPlayingAsync(int playlistIndex)
        {
            string errorMessage = null;
            wasPlaying = MediaControl.IsPlaying;
            Windows.Storage.Streams.IRandomAccessStream stream = null;

            try
            {
                stream = await playlist[playlistIndex].File.OpenAsync(Windows.Storage.FileAccessMode.Read);
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { mediaElement.SetSource(stream, playlist[playlistIndex].File.ContentType); });
            }
            catch (Exception e)
            {
                errorMessage = e.Message;                
            }

            if (errorMessage != null)
            {
                await dispatchMessage("Error" + errorMessage);
            }

              
            MediaControl.ArtistName = playlist[playlistIndex].Artist;
            MediaControl.TrackName = playlist[playlistIndex].Track;
 

        }

        async void MediaControl_ChannelUpPressed(object sender, object e)
        {
            await dispatchMessage("Channel Up Pressed");
        }

        async void MediaControl_ChannelDownPressed(object sender, object e)
        {
            await dispatchMessage("Channel Down Pressed");
        }

        async void MediaControl_RecordPressed(object sender, object e)
        {
            await dispatchMessage("Record Pressed");
        }

        async void MediaControl_RewindPressed(object sender, object e)
        {
            await dispatchMessage("Rewind Pressed");
        }

        async void MediaControl_FastForwardPressed(object sender, object e)
        {
            await dispatchMessage("Fast Forward Pressed");
        }

        async void MediaControl_PreviousTrackPressed(object sender, object e)
        {
            await dispatchMessage("Previous Track Pressed");
            if (currentSongIndex > 0) 
            {
                if (currentSongIndex == (playlistCount-1))
                {
                    if (!nextRegistered)
                    {
                        MediaControl.NextTrackPressed   += MediaControl_NextTrackPressed;
                        nextRegistered                  = true;
                    }
                }
                currentSongIndex--;

                if (currentSongIndex == 0)
                {
                    MediaControl.PreviousTrackPressed -= MediaControl_PreviousTrackPressed;
                    previousRegistered = false;
                }

                await SetCurrentPlayingAsync(currentSongIndex);
            }
            
        }

        async void MediaControl_NextTrackPressed(object sender, object e)
        {
            await dispatchMessage("Next Track Pressed");
            
            if (currentSongIndex < (playlistCount-1)) 
            {
                currentSongIndex++;
                await SetCurrentPlayingAsync(currentSongIndex);
                if (currentSongIndex > 0)
                {
                    if (!previousRegistered)
                    {
                        MediaControl.PreviousTrackPressed   += MediaControl_PreviousTrackPressed;
                        previousRegistered                  = true;
                    }

                }
                if (currentSongIndex == (playlistCount-1))
                {
                    if (nextRegistered)
                    {
                        MediaControl.NextTrackPressed   -= MediaControl_NextTrackPressed;
                        nextRegistered                  = false;
                    }
                }
            }
            
        }

        async void MediaControl_StopPressed(object sender, object e)
        {
            
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                mediaElement.Stop();                
            });

            await dispatchMessage("Stop Pressed");
        }

        async void MediaControl_PausePressed(object sender, object e)
        {
            await dispatchMessage("Pause Pressed");
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                    mediaElement.Pause();
            });
        }

        async void MediaControl_PlayPressed(object sender, object e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                mediaElement.Play();
            });
            await dispatchMessage("Play Pressed");
        }

        async void MediaControl_PlayPauseTogglePressed(object sender, object e)
        {
            if (MediaControl.IsPlaying)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    mediaElement.Pause();
                });
                await dispatchMessage("Play/Pause Pressed - Pause");
            }
            else
            {
              
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    mediaElement.Play();
                });
                await dispatchMessage("Play/Pause Pressed - Play");
            }

        }

        async Task dispatchMessage(string message)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Scenario1Text.Text = getTimeStampedMessage(message);
            });
        }

        string getTimeStampedMessage(string eventText)
        {
            return eventText + "   " + DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString();
        }        

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
    }
}
