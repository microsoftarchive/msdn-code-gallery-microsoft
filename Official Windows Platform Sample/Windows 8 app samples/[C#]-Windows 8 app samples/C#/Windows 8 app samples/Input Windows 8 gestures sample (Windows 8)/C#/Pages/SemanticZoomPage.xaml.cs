//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using GesturesApp.Common;
using GesturesApp.Controls;
using System;
using System.Collections.Generic;

namespace GesturesApp.Pages
{
    /// <summary>
    /// Main page of this sample app, uses <see cref="Windows.UI.Xaml.Controls.SemanticZoom"/>
    /// to allow for quick navigation of the app's content.
    /// The zoomed out view is a <see cref="Windows.UI.Xaml.Controls.GridView"/> and the zoomed
    /// in view is a custom-made <see cref="GesturesApp.Controls.SemanticFlipView"/>.
    /// </summary>
    /// <remarks>
    /// Note that <c>SemanticZoomPage</c> is different than all the other pages defined in this
    /// namespace in that it does not derive from <see cref="GesturePageBase"/>. This is because
    /// this page is used to navigate through all the other pages, which is the purpose of the
    /// semantic zoom gesture.
    /// </remarks>
    /// <seealso cref="SemanticZoomPage.xaml"/>
    public sealed partial class SemanticZoomPage : LayoutAwarePage
    {
        public static SemanticZoomPage Current;

        /// <summary>
        /// Extension of <see cref="GesturePageBase"/> that describes the semantic zoom gesture.
        /// </summary>
        class GesturePage : GesturePageBase
        {
            public GesturePage() :
                base( 
                    "SemanticZoom",
                    "Pinch to semantic zoom",
                    "Semantic zoom is a new concept in our touch language. It’s designed to make it fast and fluid to jump within a list of content. Pinch two or more fingers on the screen  to change to an overview view.  Then tap or stretch the region or item you want to see more details for. For example, in People, pinch your contact list to see alphabet tiles (A, B, etc.), and then tap the desired letter to get back to the individual contact level (J for Jan).",
                    "Similar to when you use a mouse and click the “-” button, usually found in the lower-right corner.",
                    "Assets/pinch_sezo.png")
            {
                // Configure the app bar items for this page
                // GesturePageBase.Selected uses this._nonContextualItems to populate the global app bar when the page is selected.

                // Links button
                this._links["Doc: Guidelines for semantic zoom"] = new Uri("http://msdn.microsoft.com/en-us/library/windows/apps/hh465319.aspx");
                this._nonContextualItems.Add(GesturePageBase.CreateLinksAppBarButton(this._links));

                // Create the play area for this page
                var grid = new Windows.UI.Xaml.Controls.Grid();

                var row = new Windows.UI.Xaml.Controls.RowDefinition();
                row.Height = new Windows.UI.Xaml.GridLength(1, Windows.UI.Xaml.GridUnitType.Star);
                grid.RowDefinitions.Add(row);

                row = new Windows.UI.Xaml.Controls.RowDefinition();
                row.Height = new Windows.UI.Xaml.GridLength(1, Windows.UI.Xaml.GridUnitType.Auto);
                grid.RowDefinitions.Add(row);

                var col = new Windows.UI.Xaml.Controls.ColumnDefinition();
                col.Width = new Windows.UI.Xaml.GridLength(2, Windows.UI.Xaml.GridUnitType.Star);
                grid.ColumnDefinitions.Add(col);

                col = new Windows.UI.Xaml.Controls.ColumnDefinition();
                col.Width = new Windows.UI.Xaml.GridLength(1, Windows.UI.Xaml.GridUnitType.Star);
                grid.ColumnDefinitions.Add(col);

                var image = new Windows.UI.Xaml.Controls.Image
                {
                    Margin = new Windows.UI.Xaml.Thickness(30, 30, 30, 30),
                    Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/semantic_zoom.png")),
                    Stretch = Windows.UI.Xaml.Media.Stretch.Uniform
                };
                grid.Children.Add(image);
                Windows.UI.Xaml.Controls.Grid.SetColumn(image, 0);
                Windows.UI.Xaml.Controls.Grid.SetRow(image, 0);

                var textBlock = new Windows.UI.Xaml.Controls.TextBlock
                {
                    Style = App.Current.Resources["AppSubtitleTextStyle"] as Windows.UI.Xaml.Style,
                    Text = "Pinch anywhere in this app to zoom out",
                    HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center,
                    VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center,
                    TextWrapping = Windows.UI.Xaml.TextWrapping.Wrap,
                    TextAlignment = Windows.UI.Xaml.TextAlignment.Center
                };
                grid.Children.Add(textBlock);
                Windows.UI.Xaml.Controls.Grid.SetColumn(textBlock, 1);
                Windows.UI.Xaml.Controls.Grid.SetRow(textBlock, 0);

                this.Content = grid;
            }
        }

        // Data source for the semantic zoom
        private List<IGesturePageInfo> _pages;

        // General links
        private Dictionary<string, Uri> _links;
        private Windows.UI.Xaml.Controls.Button _linksButton;

        public SemanticZoomPage()
        {
            this.InitializeComponent();

            SemanticZoomPage.Current = this;

            // Links button
            this._links = new Dictionary<string, Uri>();
            this._links["Doc: Touch Interaction Design"] = new Uri("http://msdn.microsoft.com/en-us/library/windows/apps/hh465415.aspx");
            this._links["Doc: Guidelines for panning"] = new Uri("http://msdn.microsoft.com/en-us/library/windows/apps/hh465310.aspx");
            this._links["API: Windows.UI.Input namespace"] = new Uri("http://msdn.microsoft.com/en-us/library/windows/apps/br212145.aspx");
            this._links["API: GestureRecognizer class"] = new Uri("http://msdn.microsoft.com/en-us/library/windows/apps/br241937.aspx");
            this._links["API: ManipulationStarted event"] = new Uri("http://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.input.gesturerecognizer.manipulationstarted.aspx");
            this._linksButton = GesturePageBase.CreateLinksAppBarButton(this._links);

            // Create the data source for the semantic zoom.
            // This is the actual content of this app: there is one item (page) for each gesture.
            this._pages = new List<IGesturePageInfo>
            {
                new WelcomePage().AppPageInfo,
                new AppEdgyPage().AppPageInfo,
                new SysEdgyPage().AppPageInfo,
                new TapPage().AppPageInfo,
                new PressAndHoldPage().AppPageInfo,
                new SwipePage().AppPageInfo,
                new ObjectZoomPage().AppPageInfo,
                new RotatePage().AppPageInfo,
                new SemanticZoomPage.GesturePage().AppPageInfo
            };
            this.gesturesViewSource.Source = this._pages;
        }

        // SelectionChanged event handler for the SemanticFlipView
        private void OnSelectionChanged(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            // Close the app bar
            this.globalAppBar.IsOpen = false;

            // Let the page know it is has been unselected
            foreach (var item in e.RemovedItems)
            {
                GesturePageBase gesturePage = (item as IGesturePageInfo).PlayArea;
                gesturePage.Selected = false;
            }

            // Let the page know it has been selected
            foreach (var item in e.AddedItems)
            {
                GesturePageBase gesturePage = (item as IGesturePageInfo).PlayArea;
                gesturePage.Selected = true;
            }
        }

        // ViewChangeStarted event handler for the SemanticZoom
        private void OnViewChangeStarted(object sender, Windows.UI.Xaml.Controls.SemanticZoomViewChangedEventArgs e)
        {
            // Close the app bar
            this.globalAppBar.IsOpen = false;

            if (!e.IsSourceZoomedInView)
            {
                // Unsnap the app (if necessary) before going into zoomed in view
                if (Windows.UI.ViewManagement.ApplicationView.Value == Windows.UI.ViewManagement.ApplicationViewState.Snapped)
                {
                    if (!Windows.UI.ViewManagement.ApplicationView.TryUnsnap())
                    {
                        // Could not unsnap, go back to zoomed out view
                        this.semanticZoom.IsZoomedInViewActive = false;
                    }
                }
            }
            else
            {
                // Reset the flip view's selected item so that when we go back to zoomed in view
                // SelectionChanged event is triggered for sure
                this.zoomedInFlipView.SelectedIndex = -1;
            }
        }

        // ViewChangeCompleted event handler for the SemanticZoom
        private void OnViewChangeCompleted(object sender, Windows.UI.Xaml.Controls.SemanticZoomViewChangedEventArgs e)
        {
            if (e.IsSourceZoomedInView)
            {
                // Going to zoomed out view

                this.nonContextualItemsPanel.Children.Add(this._linksButton);
            }
            else
            {
                // Going to zoomed in view

                this.nonContextualItemsPanel.Children.Remove(this._linksButton);
            }
        }
    }
}

