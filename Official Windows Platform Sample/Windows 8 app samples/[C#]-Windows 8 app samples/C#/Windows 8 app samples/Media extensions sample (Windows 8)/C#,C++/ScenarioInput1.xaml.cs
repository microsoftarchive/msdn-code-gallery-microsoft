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
    public sealed partial class ScenarioInput1 : Page
    {
        private readonly Guid MFVideoFormat_MPG1 = Guid.Parse("3147504d-0000-0010-8000-00aa00389b71");

        // A pointer back to the main page which is used to gain access to the input and output frames and their content.
        MainPage rootPage = null;
        MediaElement outputVideo;

        public ScenarioInput1()
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

            rootPage.ExtensionManager.RegisterByteStreamHandler("MPEG1Source.MPEG1ByteStreamHandler", ".mpg", "video/mpeg");
            rootPage.ExtensionManager.RegisterVideoDecoder("MPEG1Decoder.MPEG1Decoder", MFVideoFormat_MPG1, Guid.Empty);
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
        /// This is the click handler for the 'Open' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            rootPage.PickSingleFileAndSet(new string[] { ".mpg" }, outputVideo);
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
    }
}
