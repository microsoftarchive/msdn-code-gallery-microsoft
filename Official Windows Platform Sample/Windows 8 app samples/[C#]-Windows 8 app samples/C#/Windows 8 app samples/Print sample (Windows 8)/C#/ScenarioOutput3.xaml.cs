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

namespace PrintSample
{
    public sealed partial class ScenarioOutput3 : Page
    {
        // A pointer back to the main page which is used to gain access to the input and output frames and their content
        MainPage rootPage = null;

        public ScenarioOutput3()
        {
            InitializeComponent();

            #region ViewState and Resolution change code for THIS page
            Loaded += new RoutedEventHandler(ScenarioOutput3_Loaded);

            // Hook the ViewState and Resolution changed events.  This is only necessary if you need to modify your
            // content to fit well in the various view states and/or orientations.
            Window.Current.SizeChanged += new WindowSizeChangedEventHandler(ScenarioOutput3_SizeChanged);
            DisplayProperties.LogicalDpiChanged += new DisplayPropertiesEventHandler(DisplayProperties_LogicalDpiChanged);
            #endregion
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Get a pointer to our main page.
            rootPage = e.Parameter as MainPage;
        }

        #region ViewState and Resolution change code for THIS page
        void ScenarioOutput3_Loaded(object sender, RoutedEventArgs e)
        {
            CheckResolutionAndViewState();
        }

        // You may or may not need to handle resolution and view state changes in your specific scenario page content.
        // It will simply depend on your content.  In the case of this specific example, we need to adjust the content 
        // to fit well when the application is in portrait or when snapped.

        void DisplayProperties_LogicalDpiChanged(object sender)
        {
            CheckResolutionAndViewState();
        }

        void CheckResolutionAndViewState()
        {
            VisualStateManager.GoToState(this, ApplicationView.Value.ToString() + DisplayProperties.ResolutionScale.ToString(), false);
        }

        void ScenarioOutput3_SizeChanged(Object sender, Windows.UI.Core.WindowSizeChangedEventArgs args)
                {
                         CheckResolutionAndViewState();
        }
        #endregion

    }
}
