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
using Expression.Blend.SampleData.SampleDataSource;

namespace ListViewInteraction
{
    public sealed partial class ScenarioOutput3 : Page
    {
        // A pointer back to the main page which is used to gain access to the input and output frames and their content
        MainPage rootPage = null;
        CommentData commentData = null;

        public ScenarioOutput3()
        {
            InitializeComponent();

            commentData = new CommentData();
            ReviewsListView.ItemsSource = commentData.Collection;

            #region ViewState and Resolution change code for THIS page - Remove unless you need it
            Loaded += new RoutedEventHandler(ScenarioOutput3_Loaded);
            Unloaded += new RoutedEventHandler(ScenarioOutput3_Unloaded);
            #endregion
        }

        #region ViewState and Resolution change code for THIS page - Remove unless you need it
        void ScenarioOutput3_Loaded(object sender, RoutedEventArgs e)
        {
            // Hook the ViewState and Resolution changed events.  This is only necessary if you need to modify your
            // content to fit well in the various view states and/or orientations.
            Window.Current.SizeChanged += new WindowSizeChangedEventHandler(ScenarioOutput3_SizeChanged);
            DisplayProperties.LogicalDpiChanged += new DisplayPropertiesEventHandler(DisplayProperties_LogicalDpiChanged);

            CheckResolutionAndViewState();
        }

        void ScenarioOutput3_Unloaded(object sender, RoutedEventArgs e)
        {
            Window.Current.SizeChanged -= new WindowSizeChangedEventHandler(ScenarioOutput3_SizeChanged);
            DisplayProperties.LogicalDpiChanged -= new DisplayPropertiesEventHandler(DisplayProperties_LogicalDpiChanged);
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

        #region Template-Related Code - Do not remove
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
        #endregion

        #region Use this code if you need access to elements in the input frame - otherwise delete
        void rootPage_InputFrameLoaded(object sender, object e)
        {
            // At this point, we know that the Input Frame has been loaded and we can go ahead
            // and reference elements in the page contained in the Input Frame.

            // Get a pointer to the content within the IntputFrame.
            Page inputFrame = (Page)rootPage.InputFrame.Content;

            // Go find the elements that we need for this scenario.
            // ex: flipView1 = inputFrame.FindName("FlipView1") as FlipView;
        }
        #endregion
    }
}
