//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using SDKTemplate;
using Windows.Media.Playlists;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml;

namespace PlaylistSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Display : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Display()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Displays the playlist picked by the user in the FilePicker
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async private void PickPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainPage.playlist = await MainPage.PickPlaylistAsync();

                if (MainPage.playlist != null)
                {
                    this.OutputStatus.Text = "Playlist content:\n";
                    if (MainPage.playlist.Files.Count == 0)
                    {
                        this.OutputStatus.Text += "(playlist is empty)";
                    }
                    else
                    {
                        foreach (StorageFile file in MainPage.playlist.Files)
                        {
                            MusicProperties properties = await file.Properties.GetMusicPropertiesAsync();
                            this.OutputStatus.Text += "\n";
                            this.OutputStatus.Text += "Title: " + properties.Title + "\n";
                            this.OutputStatus.Text += "Album: " + properties.Album + "\n";
                            this.OutputStatus.Text += "Artist: " + properties.Artist + "\n";
                        }
                    }
                }
                else
                {
                    this.OutputStatus.Text = "No playlist picked.";
                }
            }
            catch(UnauthorizedAccessException)
            {
                this.OutputStatus.Text = "Access is denied, cannot load playlist.";
            }
        }

    }
}
