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
using System.Collections.Generic;
using SDKTemplate;
using Windows.Media.Playlists;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;

namespace PlaylistSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Add : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Add()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Loads a playlist picked by the user in the FilePicker
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
                    this.OutputStatus.Text = "Playlist loaded.";
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

        /// <summary>
        /// Adds a file to the end of the playlist loaded in PickPlaylistButton_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async private void PickAudioButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainPage.playlist != null)
            {
                FileOpenPicker picker = MainPage.CreateFilePicker(MainPage.audioExtensions);
                IReadOnlyList<StorageFile> files = await picker.PickMultipleFilesAsync();

                if (files != null && files.Count > 0)
                {
                    foreach (StorageFile file in files)
                    {
                        MainPage.playlist.Files.Add(file);
                    }

                    try
                    {
                        await MainPage.playlist.SaveAsync();
                        this.OutputStatus.Text = files.Count + " files added to playlist.";
                    }
                    catch (Exception error)
                    {
                        this.OutputStatus.Text = error.Message;
                    }
                }
                else
                {
                    this.OutputStatus.Text = "No files picked.";
                }
            }
            else
            {
                this.OutputStatus.Text = "Pick playlist first.";
            }
        }

    }
}
