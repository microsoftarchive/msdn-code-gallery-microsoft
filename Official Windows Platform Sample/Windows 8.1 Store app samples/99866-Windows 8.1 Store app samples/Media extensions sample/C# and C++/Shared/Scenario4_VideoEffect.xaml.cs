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
using Windows.Foundation.Collections;

namespace MediaExtensions
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class VideoEffect : Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public VideoEffect()
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
        }

        /// <summary>
        /// Called when a page is no longer the active page in a frame. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Video.MediaFailed -= new ExceptionRoutedEventHandler(rootPage.VideoOnError);
        }

        /// <summary>
        /// This is the click handler for the 'OpenGrayscale' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenGrayscale_Click(object sender, RoutedEventArgs e)
        {
            Video.RemoveAllEffects();
            Video.AddVideoEffect("GrayscaleTransform.GrayscaleEffect", true, null);

            rootPage.PickSingleFileAndSet(new string[] { ".mp4", ".wmv", ".avi" }, Video);
        }

        /// <summary>
        /// This is the click handler for the 'OpenFisheye' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenFisheye_Click(object sender, RoutedEventArgs e)
        {
            OpenVideoWithPolarEffect("Fisheye");
        }

        /// <summary>
        /// This is the click handler for the 'OpenPinch' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenPinch_Click(object sender, RoutedEventArgs e)
        {
            OpenVideoWithPolarEffect("Pinch");
        }

        /// <summary>
        /// This is the click handler for the 'OpenWarp' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenWarp_Click(object sender, RoutedEventArgs e)
        {
            OpenVideoWithPolarEffect("Warp");
        }

        /// <summary>
        /// This is the click handler for the 'OpenInvert' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenInvert_Click(object sender, RoutedEventArgs e)
        {
            Video.RemoveAllEffects();
            Video.AddVideoEffect("InvertTransform.InvertEffect", true, null);

            rootPage.PickSingleFileAndSet(new string[] { ".mp4", ".wmv", ".avi" }, Video);
        }

        /// <summary>
        /// This is the click handler for the 'Stop' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            Video.Source = null;
        }

        private void OpenVideoWithPolarEffect(string effectName)
        {
            Video.RemoveAllEffects();
            PropertySet configuration = new PropertySet();
            configuration.Add("effect", effectName);
            Video.AddVideoEffect("PolarTransform.PolarEffect", true, configuration);

            rootPage.PickSingleFileAndSet(new string[] { ".mp4", ".wmv", ".avi" }, Video);
        }
    }
}
