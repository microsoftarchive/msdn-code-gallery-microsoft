// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

using SDKTemplateCS;
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

namespace AtomPub
{
    public sealed partial class ScenarioOutput4 : Page
    {
        // A pointer back to the main page which is used to gain access to the input and output frames and their content.
        MainPage rootPage = null;

        public ScenarioOutput4()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Get a pointer to our main page.
            rootPage = e.Parameter as MainPage;

            // We want to be notified with the OutputFrame is loaded so we can get to the content.
            rootPage.InputFrameLoaded += new System.EventHandler(rootPage_InputFrameLoaded);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            rootPage.InputFrameLoaded -= new System.EventHandler(rootPage_InputFrameLoaded);
        }

        void rootPage_InputFrameLoaded(object sender, object e)
        {
            // At this point, we know that the Input Frame has been loaded and we can go ahead
            // and reference elements in the page contained in the Input Frame.

            // Get a pointer to the content within the IntputFrame.
            Page inputFrame = (Page)rootPage.InputFrame.Content;

            // ex: flipView1 = inputFrame.FindName("FlipView1") as FlipView;
        }
    }
}
