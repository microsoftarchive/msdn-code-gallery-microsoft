//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the Microsoft Public License.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WindowsBlogReader
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            FeedDataSource feedDataSource = (FeedDataSource)App.Current.Resources["feedDataSource"];

            if (feedDataSource != null)
            {
                if (feedDataSource.Feeds.Count == 0)
                {
                    await feedDataSource.GetFeedsAsync();
                }
                this.DataContext = (feedDataSource.Feeds).First();
            }
        }

        private void ItemListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If there's a selected item (in AddedItems)
            // show it in the WebView.
            if (e.AddedItems.Count > 0)
            {
                FeedItem feedItem = e.AddedItems[0] as FeedItem;
                if (feedItem != null)
                {
                    // Navigate the WebView to the blog post content HTML string.
                    ContentView.NavigateToString(feedItem.Content);
                }
            }
            else
            {
                // If the item was de-selected, clear the WebView.
                ContentView.NavigateToString("");
            }
        }
    }
}
