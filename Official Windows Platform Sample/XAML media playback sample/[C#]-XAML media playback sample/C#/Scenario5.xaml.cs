//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;
using Windows.Storage.Pickers;
using Windows.UI.ViewManagement;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Collections.Generic;

namespace BasicMediaPlayback
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario5 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario5()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        /// <summary>
        /// Invoked when this page will no longer be displayed in a Frame.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Set source to null to ensure it stops playing to a Play To device if applicable.
            Scenario5MediaElement.Source = null;
        }

        /// <summary>
        /// Click handler for the "Select multiple media files" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker fileOpenPicker = new FileOpenPicker();

            // Filter to include a sample subset of file types
            fileOpenPicker.FileTypeFilter.Add(".wmv");
            fileOpenPicker.FileTypeFilter.Add(".mp4");
            fileOpenPicker.FileTypeFilter.Add(".mp3");
            fileOpenPicker.FileTypeFilter.Add(".wma");
            fileOpenPicker.SuggestedStartLocation = PickerLocationId.VideosLibrary;

            // Prompt user to select one or more files            
            IReadOnlyList<StorageFile> files = await fileOpenPicker.PickMultipleFilesAsync();

            // Add selected files to the playlist ListBox
            foreach (StorageFile file in files)
            {
                Playlist.Items.Add(file);
            }

        }

        /// <summary>
        /// Handler for the MediaEnded event of the MediaElement.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            // Advance to the next file in the playlist if any remain
            if (Playlist.SelectedIndex < Playlist.Items.Count - 1)
            {
                Playlist.SelectedIndex++;
            }
        }

        /// <summary>
        /// Selection changed handler for the playlist ListBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Playlist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StorageFile file = Playlist.SelectedItem as StorageFile;

            if (file != null)
            {
                // Open the selected file and set it as the MediaElement's source
                IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);
                Scenario5MediaElement.SetSource(stream, file.ContentType);
            }
        }

    }
}
