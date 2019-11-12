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
    public sealed partial class ScenarioInput2 : Page
    {
        // A pointer back to the main page which is used to gain access to the input and output frames and their content.
        MainPage rootPage = null;
        MediaElement outputVideo;

        public ScenarioInput2()
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

            rootPage.ExtensionManager.RegisterSchemeHandler("GeometricSource.GeometricSchemeHandler", "myscheme:");
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
        /// This is the click handler for the 'Circle' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Circle_Click(object sender, RoutedEventArgs e)
        {
            outputVideo.Source = new Uri("myscheme://circle");
        }


        /// <summary>
        /// This is the click handler for the 'Square' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Square_Click(object sender, RoutedEventArgs e)
        {
            outputVideo.Source = new Uri("myscheme://square");
        }

        /// <summary>
        /// This is the click handler for the 'Triangle' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Triangle_Click(object sender, RoutedEventArgs e)
        {
            outputVideo.Source = new Uri("myscheme://triangle");
        }
        #endregion
    }
}
