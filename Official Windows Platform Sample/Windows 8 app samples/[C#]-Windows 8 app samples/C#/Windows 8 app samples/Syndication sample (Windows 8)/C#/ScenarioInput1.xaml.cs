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
using Windows.Web.Syndication;
using Windows.Web;

namespace Syndication
{
    public sealed partial class ScenarioInput1 : Page
    {
        // A pointer back to the main page which is used to gain access to the input and output frames and their content.
        MainPage rootPage = null;
        TextBox outputField = null;
        TextBlock feedTitleField = null;
        TextBlock itemTitleField = null;
        HyperlinkButton linkField = null;
        ListBox extensionsListBox = null;
        WebView contentWebView = null;

        SyndicationFeed currentFeed = null;
        int currentItemIndex;

        public ScenarioInput1()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Get a pointer to our main page.
            rootPage = e.Parameter as MainPage;

            // We want to be notified when the OutputFrame is loaded so we can get to the content.
            rootPage.OutputFrameLoaded += new System.EventHandler(rootPage_OutputFrameLoaded);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            rootPage.OutputFrameLoaded -= new System.EventHandler(rootPage_OutputFrameLoaded);
        }

        void rootPage_OutputFrameLoaded(object sender, object e)
        {
            // At this point, we know that the Output Frame has been loaded and we can go ahead
            // and reference elements in the page contained in the Output Frame.

            // Get a pointer to the content within the OutputFrame.
            Page outputFrame = (Page)rootPage.OutputFrame.Content;

            // Make the WebView fills the output frame.
            var contentRoot = rootPage.FindName("ContentRoot") as Grid;
            if (contentRoot != null && contentRoot.Children.Count >= 2)
            {
                var scrollViewer = contentRoot.Children[1] as ScrollViewer;
                if (scrollViewer != null)
                {
                    var grid = scrollViewer.Content as Grid;
                    if (grid != null)
                    {
                        grid.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch;
                    }
                }
            }
            outputFrame.Frame.MaxWidth = Double.PositiveInfinity;

            outputField = outputFrame.FindName("OutputField") as TextBox;
            feedTitleField = outputFrame.FindName("FeedTitleField") as TextBlock;
            itemTitleField = outputFrame.FindName("ItemTitleField") as TextBlock;
            linkField = outputFrame.FindName("LinkField") as HyperlinkButton;
            extensionsListBox = outputFrame.FindName("ExtensionsField") as ListBox;
            contentWebView = outputFrame.FindName("ContentWebView") as WebView;
        }

        private async void GetFeed_Click(object sender, RoutedEventArgs e)
        {
            outputField.Text = "";

            // By default 'FeedUri' is disabled and URI validation is not required. When enabling the text box
            // validating the URI is required since it was received from an untrusted source (user input).
            // The URI is validated by calling Uri.TryCreate() that will return 'false' for strings that are not valid URIs.
            // Note that when enabling the text box users may provide URIs to machines on the intrAnet that require
            // the "Home or Work Networking" capability.
            Uri uri;
            if (!Uri.TryCreate(FeedUri.Text.Trim(), UriKind.Absolute, out uri))
            {
                rootPage.NotifyUser("Error: Invalid URI.", NotifyType.ErrorMessage);
                return;
            }

            SyndicationClient client = new SyndicationClient();
            client.BypassCacheOnRetrieve = true;

            // Although most HTTP servers do not require User-Agent header, others will reject the request or return
            // a different response if this header is missing. Use SetRequestHeader() to add custom headers.
            client.SetRequestHeader("User-Agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");

            rootPage.NotifyUser("Downloading feed...", NotifyType.StatusMessage);
            outputField.Text = "Downloading feed: " + uri.ToString() + "\r\n";

            try
            {
                currentFeed = await client.RetrieveFeedAsync(uri);
                rootPage.NotifyUser("Feed download complete.", NotifyType.StatusMessage);

                DisplayFeed();
            }
            catch (Exception ex)
            {
                SyndicationErrorStatus status = SyndicationError.GetStatus(ex.HResult);
                if (status == SyndicationErrorStatus.InvalidXml)
                {
                    outputField.Text += "An invalid XML exception was thrown. " +
                        "Please make sure to use a URI that points to a RSS or Atom feed.";
                }

                if (status == SyndicationErrorStatus.Unknown)
                {
                    WebErrorStatus webError = WebError.GetStatus(ex.HResult);

                    if (webError == WebErrorStatus.Unknown)
                    {
                        // Neither a syndication nor a web error. Rethrow.
                        throw;
                    }
                }

                rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage);
            }
        }

        private void DisplayFeed()
        {
            ISyndicationText title = currentFeed.Title;
            feedTitleField.Text = title != null ? title.Text : "(no title)";

            currentItemIndex = 0;
            if (currentFeed.Items.Count > 0)
            {
                DisplayCurrentItem();
            }

            // List the items.
            outputField.Text += "Items: " + currentFeed.Items.Count + "\r\n";
        }

        private void PreviousItem_Click(object sender, RoutedEventArgs e)
        {
            if (currentFeed != null && currentItemIndex > 0)
            {
                currentItemIndex--;
                DisplayCurrentItem();
            }
        }

        private void NextItem_Click(object sender, RoutedEventArgs e)
        {
            if (currentFeed != null && currentItemIndex < (currentFeed.Items.Count - 1))
            {
                currentItemIndex++;
                DisplayCurrentItem();
            }
        }

        private void DisplayCurrentItem()
        {
            SyndicationItem item = currentFeed.Items[currentItemIndex];

            // Display item number.
            IndexField.Text = String.Format("{0} of {1}", currentItemIndex + 1, currentFeed.Items.Count);

            // Display title.
            itemTitleField.Text = item.Title != null ? item.Title.Text : "(no title)";

            // Display the main link.
            string link = string.Empty;
            if (item.Links.Count > 0)
            {
                link = item.Links[0].Uri.AbsoluteUri;
            }
            linkField.Content = link;

            // Display item extensions.
            extensionsListBox.ItemsSource = item.ElementExtensions;

            // Display the body as HTML.
            string content = "(no content)";
            if (item.Content != null)
            {
                content = item.Content.Text;
            }
            else if (item.Summary != null)
            {
                content = item.Summary.Text;
            }
            contentWebView.NavigateToString(content);
        }
    }
}
