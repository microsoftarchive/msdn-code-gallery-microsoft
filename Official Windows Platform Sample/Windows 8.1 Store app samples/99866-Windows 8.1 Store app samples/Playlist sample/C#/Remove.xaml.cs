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
    public sealed partial class Remove : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Remove()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Removes the last file in the playlist picked by the user in the FilePicker
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
                    if (MainPage.playlist.Files.Count > 0)
                    {
                        MainPage.playlist.Files.RemoveAt(MainPage.playlist.Files.Count - 1);

                        try
                        {
                            await MainPage.playlist.SaveAsync();
                            this.OutputStatus.Text = "The last item in the playlist was removed.";
                        }
                        catch (Exception error)
                        {
                            this.OutputStatus.Text = error.Message;
                        }
                    }
                    else
                    {
                        this.OutputStatus.Text = "No items in playlist.";
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
