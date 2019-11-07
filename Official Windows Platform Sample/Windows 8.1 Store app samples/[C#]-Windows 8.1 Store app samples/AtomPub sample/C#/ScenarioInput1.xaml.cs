// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

using SDKTemplateCS;
using System;
using System.Net;
using Windows.Security.Credentials;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Web;
using Windows.Web.AtomPub;
using Windows.Web.Syndication;

namespace AtomPub
{
    public sealed partial class ScenarioInput1 : Page
    {
        // A pointer back to the main page which is used to gain access to the input and output frames and their content.
        MainPage rootPage = null;

        // Helps iterating through feeds.
        SyndicationItemIterator feedIterator = new SyndicationItemIterator();

        // Controls from the output frame.
        TextBox outputField;
        TextBlock indexField;
        TextBlock titleField;
        WebView contentWebView;

        public ScenarioInput1()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Get a pointer to our main page
            rootPage = e.Parameter as MainPage;

            // We want to be notified with the OutputFrame is loaded so we can get to the content.
            rootPage.OutputFrameLoaded += new System.EventHandler(rootPage_OutputFrameLoaded);

            CommonData.Restore(this);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            rootPage.OutputFrameLoaded -= new System.EventHandler(rootPage_OutputFrameLoaded);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            CommonData.Save(this);
        }

        void rootPage_OutputFrameLoaded(object sender, object e)
        {
            // At this point, we know that the Output Frame has been loaded and we can go ahead
            // and reference elements in the page contained in the Output Frame.

            // Get a pointer to the content within the OutputFrame.
            Page outputFrame = (Page)rootPage.OutputFrame.Content;

            indexField = FindName("IndexField") as TextBlock;
            outputField = outputFrame.FindName("OutputField") as TextBox;
            titleField = outputFrame.FindName("TitleField") as TextBlock;
            contentWebView = outputFrame.FindName("ContentWebView") as WebView;
        }

        private async void GetFeed_Click(object sender, RoutedEventArgs e)
        {
            outputField.Text = "";

            // Note that this feed is public by default and will not require authentication.
            // We will only get back a limited use feed, without information about editing.

            // Since the value of 'ServiceAddressField' was provided by the user it is untrusted input. We'll validate
            // it by using Uri.TryCreate().
            // Note that this app has both "Internet (Client)" and "Home and Work Networking" capabilities set, since
            // the user may provide URIs for servers located on the intErnet or intrAnet. If apps only communicate
            // with servers on the intErnet, only the "Internet (Client)" capability should be set. Similarly if an
            // app is only intended to communicate on the intrAnet, only the "Home and Work Networking" capability should be set.
            Uri resourceUri;
            if (!Uri.TryCreate(ServiceAddressField.Text.Trim() + CommonData.FeedUri, UriKind.Absolute, out resourceUri))
            {
                rootPage.NotifyUser("Invalid URI.", NotifyType.ErrorMessage);
                return;
            }

            rootPage.NotifyUser("Fetching resource...", NotifyType.StatusMessage);
            outputField.Text = "Requested resource: " + resourceUri + "\r\n";

            try
            {
                feedIterator.AttachFeed(await CommonData.GetClient().RetrieveFeedAsync(resourceUri));

                rootPage.NotifyUser("Fetching resource completed.", NotifyType.StatusMessage);
                DisplayItem();
            }
            catch (Exception ex)
            {
                if (!CommonData.HandleException(ex, outputField, rootPage))
                {
                    throw;
                }
            }
        }

        private void PreviousItem_Click(object sender, RoutedEventArgs e)
        {
            feedIterator.MovePrevious();
            DisplayItem();
        }

        private void NextItem_Click(object sender, RoutedEventArgs e)
        {
            feedIterator.MoveNext();
            DisplayItem();
        }

        private void UserNameField_TextChanged(object sender, TextChangedEventArgs e)
        {
            CommonData.Save(this);
        }

        private void PasswordField_PasswordChanged(object sender, RoutedEventArgs e)
        {
            CommonData.Save(this);
        }

        private void DisplayItem()
        {
            indexField.Text = feedIterator.GetIndexDescription();
            titleField.Text = feedIterator.GetTitle();
            contentWebView.NavigateToString(feedIterator.GetContent());
        }
    }
}
