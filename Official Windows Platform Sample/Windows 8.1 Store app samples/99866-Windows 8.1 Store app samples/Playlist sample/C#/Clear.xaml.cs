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
using Windows.UI.Xaml;

namespace PlaylistSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Clear : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Clear()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Clears the playlist picked by the user in the FilePicker
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
                    MainPage.playlist.Files.Clear();

                    try
                    {
                        await MainPage.playlist.SaveAsync();
                        this.OutputStatus.Text = "Playlist cleared.";
                    }
                    catch (Exception error)
                    {
                        this.OutputStatus.Text = error.Message;
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
