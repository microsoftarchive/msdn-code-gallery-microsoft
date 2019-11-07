//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media;
using SDKTemplate;
using System;
using Windows.Storage.Pickers;
using Windows.UI.ViewManagement;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Media;

namespace BasicMediaPlayback
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario2 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        SystemMediaTransportControls systemMediaControls = null;

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
            if (rootPage.SystemMediaTransportControlsInitialized)
            {
                // PlayTo handling requires either no support for SystemMediaTransportControls or
                // support for Play and Pause similar to background audio.  If the SystemMediaTransportControls
                // have been initialized in another scenario than we also need to hook them up here or PlayTo
                // will fail.
                systemMediaControls = SystemMediaTransportControls.GetForCurrentView();
                systemMediaControls.ButtonPressed += SystemMediaControls_ButtonPressed;
                systemMediaControls.IsPlayEnabled = true;
                systemMediaControls.IsPauseEnabled = true;
                systemMediaControls.IsStopEnabled = true;

                Scenario2MediaElement.CurrentStateChanged += MediaElement_CurrentStateChanged;
            }
        }

        /// <summary>
        /// Invoked when this page is about to be removed from a Frame.
        /// </summary>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (systemMediaControls != null)
            {
                systemMediaControls.ButtonPressed -= SystemMediaControls_ButtonPressed;
                systemMediaControls.IsPlayEnabled = false;
                systemMediaControls.IsPauseEnabled = false;
                systemMediaControls.IsStopEnabled = false;
                systemMediaControls.PlaybackStatus = MediaPlaybackStatus.Closed;
                systemMediaControls.DisplayUpdater.ClearAll();
                systemMediaControls.DisplayUpdater.Update();
                systemMediaControls = null;
            }

            // Set source to null to ensure it stops playing to a Play To device if applicable.
            Scenario2MediaElement.Source = null;
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
                if (systemMediaControls != null)
                {
                    MediaPlaybackType mediaPlaybackType;
                    if ((file.FileType == ".mp4") || (file.FileType == ".wmv"))
                    {
                        mediaPlaybackType = MediaPlaybackType.Video;
                    }
                    else
                    {
                        mediaPlaybackType = MediaPlaybackType.Music;
                    }

                    // Inform the system transport controls of the media information
                    if (!(await systemMediaControls.DisplayUpdater.CopyFromFileAsync(mediaPlaybackType, file)))
                    {
                        //  Problem extracting metadata- just clear everything
                        systemMediaControls.DisplayUpdater.ClearAll();
                    }
                    systemMediaControls.DisplayUpdater.Update();
                }

                // Open the selected file and set it as the MediaElement's source
                IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);
                Scenario2MediaElement.SetSource(stream, file.ContentType);
            }
        }

        /// <summary>
        /// Handler for the system transport controls button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SystemMediaControls_ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs e)
        {
            switch (e.Button)
            {
                case SystemMediaTransportControlsButton.Play:
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        Scenario2MediaElement.Play();
                    });
                    break;

                case SystemMediaTransportControlsButton.Pause:
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        Scenario2MediaElement.Pause();
                    });
                    break;

                case SystemMediaTransportControlsButton.Stop:
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        Scenario2MediaElement.Stop();
                    });
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Handler for the media element state change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MediaElement_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            SystemMediaTransportControls systemMediaControls = SystemMediaTransportControls.GetForCurrentView();
            switch (Scenario2MediaElement.CurrentState)
            {
                default:
                case MediaElementState.Closed:
                    systemMediaControls.PlaybackStatus = MediaPlaybackStatus.Closed;
                    break;

                case MediaElementState.Opening:
                    systemMediaControls.PlaybackStatus = MediaPlaybackStatus.Changing;
                    break;

                case MediaElementState.Buffering:
                case MediaElementState.Playing:
                    systemMediaControls.PlaybackStatus = MediaPlaybackStatus.Playing;
                    break;

                case MediaElementState.Paused:
                    systemMediaControls.PlaybackStatus = MediaPlaybackStatus.Paused;
                    break;

                case MediaElementState.Stopped:
                    systemMediaControls.PlaybackStatus = MediaPlaybackStatus.Stopped;
                    break;
            }
        }
    }
}
