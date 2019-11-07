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
using Windows.UI.Xaml.Media.Imaging;
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
    public sealed partial class Scenario2 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;
        PlayToManager playToManager = null;
        CoreDispatcher dispatcher = null;
        DispatcherTimer playlistTimer = null;

        public Scenario2()
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

            playlistTimer = new DispatcherTimer();
            playlistTimer.Interval = new TimeSpan(0, 0, 10);
            playlistTimer.Tick += playlistTimer_Tick;

            playToManager = PlayToManager.GetForCurrentView();
            playToManager.SourceRequested += playToManager_SourceRequested;
            playListPlayNext();
        }

        void playlistTimer_Tick(object sender, object e)
        {
            if (Playlist != null)
            {
                playListPlayNext();
            }
        }

        void playToManager_SourceRequested(PlayToManager sender, PlayToSourceRequestedEventArgs args)
        {
            var deferral = args.SourceRequest.GetDeferral();
            var handler = dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                args.SourceRequest.SetSource(ImageSource.PlayToSource);
                deferral.Complete();
            });

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            playToManager.SourceRequested -= playToManager_SourceRequested;
        }

        private void playSlideshow_Click(object sender, RoutedEventArgs e)
        {
            if (Playlist != null)
            {
                rootPage.NotifyUser("Playlist playing", NotifyType.StatusMessage);
                playlistTimer.Start();
            }
        }

        private void pauseSlideshow_Click(object sender, RoutedEventArgs e)
        {
            if (Playlist != null)
            {
                rootPage.NotifyUser("Playlist paused", NotifyType.StatusMessage);
                playlistTimer.Stop();
            }
        }

        private void previousItem_Click(object sender, RoutedEventArgs e)
        {
            if (Playlist != null)
            {
                Playlist.SelectedIndex = (Playlist.SelectedIndex - 1 >= 0) ? Playlist.SelectedIndex - 1 : Playlist.Items.Count - 1;
                rootPage.NotifyUser("Previous item selected in the list", NotifyType.StatusMessage);
            }
        }

        private void nextItem_Click(object sender, RoutedEventArgs e)
        {
            if (Playlist != null)
            {
                playListPlayNext();
                rootPage.NotifyUser("Next item selected in the list", NotifyType.StatusMessage);
            }
        }

        private void playListPlayNext()
        {
            if (Playlist != null)
            {
                Playlist.SelectedIndex = (Playlist.SelectedIndex + 1 < Playlist.Items.Count) ? Playlist.SelectedIndex + 1 : 0;
            }
        }

        private void Playlist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Playlist != null)
            {
                ImageSource.Source = new BitmapImage(new Uri("ms-appx:///Assets/" + ((ListBoxItem)Playlist.SelectedItem).Content.ToString()));
                rootPage.NotifyUser(((ListBoxItem)Playlist.SelectedItem).Content.ToString() + " selected from the list", NotifyType.StatusMessage);
            }
        }

    }
}
