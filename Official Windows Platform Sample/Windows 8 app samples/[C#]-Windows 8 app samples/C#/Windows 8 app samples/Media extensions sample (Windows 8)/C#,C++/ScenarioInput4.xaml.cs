// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using System.Linq;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SDKTemplateCS;

namespace MediaExtensionsCS
{
    public sealed partial class ScenarioInput4 : Page
    {
        // A pointer back to the main page which is used to gain access to the input and output frames and their content.
        MainPage rootPage = null;
        MediaElement outputVideo;

        public ScenarioInput4()
        {
            InitializeComponent();
        }

        #region Template-Related Code - Do not remove
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Get a pointer to our main page
            rootPage = e.Parameter as MainPage;

            // We want to be notified with the OutputFrame is loaded so we can get to the content.
            rootPage.OutputFrameLoaded += new System.EventHandler(rootPage_OutputFrameLoaded);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            rootPage.OutputFrameLoaded -= new System.EventHandler(rootPage_OutputFrameLoaded);
        }

        #endregion

        #region Event handlers
        void rootPage_OutputFrameLoaded(object sender, object e)
        {
            // At this point, we know that the Output Frame has been loaded and we can go ahead
            // and reference elements in the page contained in the Output Frame.

            // Get a pointer to the content within the OutputFrame.
            Page outputFrame = (Page)rootPage.OutputFrame.Content;

            // Go find the elements that we need for this scenario.
            outputVideo = outputFrame.FindName("Video") as MediaElement;
        }

        /// <summary>
        /// This is the click handler for the 'OpenGrayscale' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenGrayscale_Click(object sender, RoutedEventArgs e)
        {
            outputVideo.RemoveAllEffects();
            outputVideo.AddVideoEffect("GrayscaleTransform.GrayscaleEffect", true, null);

            rootPage.PickSingleFileAndSet(new string[] { ".mp4", ".wmv", ".avi" }, outputVideo);
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
            outputVideo.RemoveAllEffects();
            outputVideo.AddVideoEffect("InvertTransform.InvertEffect", true, null);

            rootPage.PickSingleFileAndSet(new string[] { ".mp4", ".wmv", ".avi" }, outputVideo);
        }

        /// <summary>
        /// This is the click handler for the 'Stop' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            outputVideo.Source = null;
        }
        #endregion

        private void OpenVideoWithPolarEffect(string effectName)
        {
            outputVideo.RemoveAllEffects();
            PropertySet configuration = new PropertySet();
            configuration.Add("effect", effectName);
            outputVideo.AddVideoEffect("PolarTransform.PolarEffect", true, configuration);

            rootPage.PickSingleFileAndSet(new string[] { ".mp4", ".wmv", ".avi" }, outputVideo);
        }
    }
}
