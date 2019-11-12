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
    public sealed partial class Create : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Create()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Creates a playlist with the audio picked by the user in the FilePicker
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async private void PickAudioButton_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = MainPage.CreateFilePicker(MainPage.audioExtensions);
            IReadOnlyList<StorageFile> files = await picker.PickMultipleFilesAsync();

            if (files != null && files.Count > 0)
            {
                MainPage.playlist = new Playlist();

                foreach (StorageFile file in files)
                {
                    MainPage.playlist.Files.Add(file);
                }

                StorageFile savedFile = await MainPage.playlist.SaveAsAsync(KnownFolders.MusicLibrary,
                                                                            "Sample",
                                                                            NameCollisionOption.ReplaceExisting,
                                                                            PlaylistFormat.WindowsMedia);

                this.OutputStatus.Text = savedFile.Name + " was created and saved with " + MainPage.playlist.Files.Count + " files.";

                // Reset playlist so subsequent SaveAsync calls don't fail
                MainPage.playlist = null;
            }
            else
            {
                this.OutputStatus.Text = "No files picked.";
            }
        }
    }
}
