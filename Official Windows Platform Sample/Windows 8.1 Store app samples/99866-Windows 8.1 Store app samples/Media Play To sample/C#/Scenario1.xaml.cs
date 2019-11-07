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
using Windows.UI.Core;
using Windows.Media.PlayTo;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using SDKTemplate;
using System;

namespace PlayTo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario1 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;
        PlayToManager playToManager = null;
        CoreDispatcher dispatcher = null;

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
            dispatcher = Window.Current.CoreWindow.Dispatcher;
            playToManager = PlayToManager.GetForCurrentView();
            playToManager.SourceRequested += playToManager_SourceRequested;
        }

        void playToManager_SourceRequested(PlayToManager sender, PlayToSourceRequestedEventArgs args)
        {
            var deferral = args.SourceRequest.GetDeferral();
            var handler = dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    args.SourceRequest.SetSource(VideoSource.PlayToSource);
                    deferral.Complete();
                });
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            playToManager.SourceRequested -= playToManager_SourceRequested;
        }

        /// <summary>
        /// This is the click handler for the 'webContent' button.  You would replace this with your own handler
        /// if you have a button or buttons on this page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void webContent_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b != null)
            {
                VideoSource.Source = new Uri("http://ie.microsoft.com/testdrive/Graphics/VideoFormatSupport/big_buck_bunny_trailer_480p_high.mp4");
                rootPage.NotifyUser("You are playing a web content", NotifyType.StatusMessage);
            }
        }

        /// <summary>
        /// This is the click handler for the 'videoFile' button.  You would replace this with your own handler
        /// if you have a button or buttons on this page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async private void videoFile_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b != null)
            {           
                FileOpenPicker filePicker = new FileOpenPicker();
                filePicker.SuggestedStartLocation = PickerLocationId.VideosLibrary;
                filePicker.FileTypeFilter.Add(".mp4");
                filePicker.FileTypeFilter.Add(".wmv");
                filePicker.ViewMode = PickerViewMode.Thumbnail;

                StorageFile localVideo = await filePicker.PickSingleFileAsync();
                if (localVideo != null)
                {
                    var stream = await localVideo.OpenAsync(FileAccessMode.Read);
                    VideoSource.SetSource(stream, localVideo.ContentType);
                    rootPage.NotifyUser("You are playing a local video file", NotifyType.StatusMessage);
                }
            }
        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            VideoSource.Play();
        }

        private void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            VideoSource.Pause();
        }

        private void playToButton_Click(object sender, RoutedEventArgs e)
        {
            PlayToManager.ShowPlayToUI();
        }

    }
}
