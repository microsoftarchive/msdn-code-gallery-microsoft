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
using Windows.UI.Xaml.Media;

namespace BasicMediaPlayback
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario3 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario3()
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
            Scenario3MediaElement.Source = null;
        }

        /// <summary>
        /// Click handler for the "Select a media file" button.
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

            // Prompt user to select a file
            StorageFile file = await fileOpenPicker.PickSingleFileAsync();

            // Ensure a file was selected
            if (file != null)
            {
                // Open the selected file and set it as the MediaElement's source
                IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);
                Scenario3MediaElement.SetSource(stream, file.ContentType);
            }            
        }

        /// <summary>
        /// Click handler for the "Add marker" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddMarkerButton_Click(object sender, RoutedEventArgs e)
        {
            // Add a marker to the MediaElement at 2 seconds past the current time
            TimelineMarker marker = new TimelineMarker();
            marker.Text = "Marker Fired";
            marker.Time = TimeSpan.FromSeconds(Scenario3MediaElement.Position.TotalSeconds + 2);
            Scenario3MediaElement.Markers.Add(marker);
            
            Scenario3Text.Text += String.Format("Marker added at time = {0:g}\n", Math.Round(Scenario3MediaElement.Position.TotalSeconds + 2, 2));
        }

        /// <summary>
        /// Handler for the MediaElement.MarkerReached event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MediaElement_MarkerReached(object sender, TimelineMarkerRoutedEventArgs e)
        {
            // Display marker info when a marker is reached
            Scenario3Text.Text += e.Marker.Text + ": " + String.Format("{0:g}\n", Math.Round(e.Marker.Time.TotalSeconds, 2));
        }
    }
}
