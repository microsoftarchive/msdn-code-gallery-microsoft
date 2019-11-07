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
using Windows.UI.ViewManagement;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.Media;
using Windows.Storage.Streams;

namespace BasicMediaPlayback
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario1 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario1()
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
            Scenario1MediaElement.Source = null;
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
                Scenario1MediaElement.SetSource(stream, file.ContentType);
            }
        }

        /// <summary>
        /// Double tap handler for the MediaElement.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Scenario1MediaElement_DoubleTapped(object sender, Windows.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            // Toggle full-window mode when the user double taps
            Scenario1MediaElement.IsFullWindow = !Scenario1MediaElement.IsFullWindow;
        }
    }
}
