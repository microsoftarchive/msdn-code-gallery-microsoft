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

namespace MediaExtensions
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class VideoStabilization : Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public VideoStabilization()
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
            Video.MediaFailed += new ExceptionRoutedEventHandler(rootPage.VideoOnError);
            VideoStabilized.MediaFailed += new ExceptionRoutedEventHandler(rootPage.VideoOnError);
        }

        /// <summary>
        /// Called when a page is no longer the active page in a frame. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Video.MediaFailed -= new ExceptionRoutedEventHandler(rootPage.VideoOnError);
            VideoStabilized.MediaFailed -= new ExceptionRoutedEventHandler(rootPage.VideoOnError);
        }

        /// <summary>
        /// This is the click handler for the 'Open' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            VideoStabilized.RemoveAllEffects();
            try
            {
                VideoStabilized.AddVideoEffect(Windows.Media.VideoEffects.VideoStabilization, true, null);
            }
            catch (Exception exc)
            {
                // HRESULT_FROM_WIN32(ERROR_NOT_SUPPORTED)
                if ((uint)exc.HResult == 0x80070032)
                {
                    MainPage.Current.NotifyUser("Video Stabilization not supported.", NotifyType.ErrorMessage);
                }
                else
                {
                    MainPage.Current.NotifyUser("Video Stabilization error.", NotifyType.ErrorMessage);
                }
                return;
            }
            rootPage.PickSingleFileAndSet(new string[] { ".mp4", ".wmv", ".avi" }, Video, VideoStabilized);
        }

        /// <summary>
        /// This is the click handler for the 'Stop' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            Video.Source = null;
            VideoStabilized.Source = null;
        }
    }
}
