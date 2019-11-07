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
    public sealed partial class ScenarioInput4 : Page
    {
        // A pointer back to the main page which is used to gain access to the input and output frames and their content.
        MainPage rootPage = null;

        // Helps iterating through feeds.
        SyndicationItemIterator feedIterator = new SyndicationItemIterator();

        // Controls from the output frame.
        TextBox outputField;
        TextBlock indexField;
        TextBox titleField;
        TextBox contentField;

        public ScenarioInput4()
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

            outputField = outputFrame.FindName("OutputField") as TextBox;
            titleField = outputFrame.FindName("TitleBox") as TextBox;
            contentField = outputFrame.FindName("ContentBox") as TextBox;
            indexField = FindName("IndexField") as TextBlock;
        }

        // Download a feed.
        private async void GetFeed_Click(object sender, RoutedEventArgs e)
        {
            outputField.Text = "";

            // If we retrieve the feed via the Edit uri then we will be logged in and be
            // able to modify/delete the resource.

            // Since the value of 'ServiceAddressField' was provided by the user it is untrusted input. We'll validate
            // it by using Uri.TryCreate().
            // Note that this app has both "Internet (Client)" and "Home and Work Networking" capabilities set, since
            // the user may provide URIs for servers located on the intErnet or intrAnet. If apps only communicate
            // with servers on the intErnet, only the "Internet (Client)" capability should be set. Similarly if an
            // app is only intended to communicate on the intrAnet, only the "Home and Work Networking" capability should be set.
            Uri resourceUri;
            if (!Uri.TryCreate(ServiceAddressField.Text.Trim() + CommonData.EditUri, UriKind.Absolute, out resourceUri))
            {
                rootPage.NotifyUser("Invalid URI.", NotifyType.ErrorMessage);
                return;
            }

            rootPage.NotifyUser("Fetching feed...", NotifyType.StatusMessage);
            outputField.Text = "Requested resource: " + resourceUri + "\r\n";

            try
            {
                feedIterator.AttachFeed(await CommonData.GetClient().RetrieveFeedAsync(resourceUri));

                if (feedIterator.HasElements())
                {
                    outputField.Text += "Got feed\r\n";
                    outputField.Text += "Title: " + feedIterator.GetTitle() + "\r\n";
                    outputField.Text += "EditUri: " + feedIterator.GetEditUri() + "\r\n";

                    DisplayItem();
                }
                else
                {
                    outputField.Text += "Got empty feed\r\n";
                }

                rootPage.NotifyUser("Fetching feed completed.", NotifyType.StatusMessage);
            }
            catch (Exception ex)
            {
                if (!CommonData.HandleException(ex, outputField, rootPage))
                {
                    throw;
                }
            }
        }

        // Update the current item.
        private async void UpdateItem_Click(object sender, RoutedEventArgs e)
        {
            if (!feedIterator.HasElements())
            {
                outputField.Text += "No item currently displayed, please download a feed first.\r\n";
                return;
            }

            rootPage.NotifyUser("Updating item...", NotifyType.StatusMessage);
            outputField.Text += "Item location: " + feedIterator.GetEditUri() + "\r\n";

            try
            {
                // Update the item
                SyndicationItem updatedItem = new SyndicationItem();
                updatedItem.Title = new SyndicationText(titleField.Text, SyndicationTextType.Text);
                updatedItem.Content = new SyndicationContent(contentField.Text, SyndicationTextType.Html);

                await CommonData.GetClient().UpdateResourceAsync(feedIterator.GetEditUri(), updatedItem);

                rootPage.NotifyUser("Updating item completed.", NotifyType.StatusMessage);
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
            contentField.Text = feedIterator.GetContent();
        }
    }
}
