//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using GesturesApp.Controls;
using System;
using System.Collections.Generic;

namespace GesturesApp.Pages
{
    public sealed partial class SwipePage : GesturePageBase
    {
        private List<string> _items;
        private Windows.UI.Xaml.Controls.Button _clearSelectionButton;

        private Windows.UI.Xaml.Controls.AppBar _appBar;

        public SwipePage() :
            base( 
                "Swipe",
                "Swipe to select",
                "Slide a short distance perpendicular to the scrolling direction to select an item. For example, if a page scrolls left and right, slide the object up or down. This selects the item and brings up the relevant app commands.",
                "Similar to when you use a mouse and right-click an item.",
                "Assets/swipe.png")
        {
            this.InitializeComponent();

            // Configure the grid view that displays the tiles that can be swiped for selection
            this._items = new List<string>();
            for (int i = 1; i < 11; i++)
            {
                this._items.Add("../Assets/swipe-" + i + ".png");
            }
            this.itemsViewSource.Source = this._items;

            // Configure the app bar items for this page
            // GesturePageBase.Selected uses this._nonContextualItems to populate the global app bar when the page is selected.

            // Links button
            this._links["Doc: Guidelines for swipe to select/cross-slide"] = new Uri("http://msdn.microsoft.com/en-us/library/windows/apps/hh465299.aspx");
            this._links["Doc: Guidelines for visual feedback"] = new Uri("http://msdn.microsoft.com/en-us/library/windows/apps/hh465342.aspx");
            this._links["API: CrossSliding event"] = new Uri("http://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.input.gesturerecognizer.crosssliding.aspx");
            this._links["API: CrossSlidingEventArgs"] = new Uri("http://msdn.microsoft.com/en-US/library/windows/apps/windows.ui.input.crossslidingeventargs");
            this._nonContextualItems.Add(GesturePageBase.CreateLinksAppBarButton(this._links));

            // Store a reference to the globally shared AppBar. We will need it to automatically open the AppBar when something is selected in the grid view.
            this._appBar = (Windows.UI.Xaml.Controls.AppBar)GesturesApp.Pages.SemanticZoomPage.Current.FindName("globalAppBar");

            // Clear selection button
            this._clearSelectionButton = new Windows.UI.Xaml.Controls.Button
            {
                Style = App.Current.Resources["ClearSelectionAppBarButtonStyle"] as Windows.UI.Xaml.Style
            };
            this._clearSelectionButton.Click += (object sender, Windows.UI.Xaml.RoutedEventArgs e) =>
            {
                this.tilesGridView.SelectedItems.Clear();
            };
        }

        private void OnSelectionChanged(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            // Open the app bar if something is selected, close it otherwise
            if (this.tilesGridView.SelectedItems.Count > 0)
            {
                // Add button for clearing the selection but make sure it is added only once
                if (!this._contextualItemsPanel.Children.Contains(this._clearSelectionButton))
                {
                    this._contextualItemsPanel.Children.Add(this._clearSelectionButton);
                }

                this._appBar.IsOpen = true;
            }
            else
            {
                this._contextualItemsPanel.Children.Remove(this._clearSelectionButton);
                this._appBar.IsOpen = false;
            }
        }

        protected override void OnUnselected()
        {
            base.OnUnselected();

            // Clear selection
            this.tilesGridView.SelectedItems.Clear();
        }
    }
}
