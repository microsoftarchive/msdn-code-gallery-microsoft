// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

using SDKTemplateCS;
using System;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Web;
using Windows.Web.AtomPub;
using Windows.Web.Syndication;

namespace AtomPub
{
    public sealed partial class ScenarioInput2 : Page
    {
        // A pointer back to the main page which is used to gain access to the input and output frames and their content.
        MainPage rootPage = null;

        // Controls from the output frame.
        TextBox outputField;
        TextBox titleField;
        TextBox contentField;

        public ScenarioInput2()
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

            // Get a pointer to the content within the OutputFrame
            Page outputFrame = (Page)rootPage.OutputFrame.Content;

            outputField = outputFrame.FindName("OutputField") as TextBox;
            titleField = outputFrame.FindName("TitleField") as TextBox;
            contentField = outputFrame.FindName("ContentField") as TextBox;
        }

        // Submit an item.
        private async void SubmitItem_Click(object sender, RoutedEventArgs e)
        {
            outputField.Text = "";

            // Since the value of 'ServiceAddressField' was provided by the user it is untrusted input. We'll validate
            // it by using Uri.TryCreate().
            // Note that this app has both "Internet (Client)" and "Home and Work Networking" capabilities set, since
            // the user may provide URIs for servers located on the intErnet or intrAnet. If apps only communicate
            // with servers on the intErnet, only the "Internet (Client)" capability should be set. Similarly if an
            // app is only intended to communicate on the intrAnet, only the "Home and Work Networking" capability should be set.
            Uri serviceUri;
            if (!Uri.TryCreate(ServiceAddressField.Text.Trim() + CommonData.ServiceDocUri, UriKind.Absolute, out serviceUri))
            {
                rootPage.NotifyUser("Invalid URI.", NotifyType.ErrorMessage);
                return;
            }

            // The title cannot be an empty string or a string with white spaces only, since it is used also
            // as the resource description (Slug header).
            if (String.IsNullOrWhiteSpace(titleField.Text))
            {
                rootPage.NotifyUser("Post title cannot be blank.", NotifyType.ErrorMessage);
                return;
            }

            rootPage.NotifyUser("Performing operation...", NotifyType.StatusMessage);
            outputField.Text = "Fetching Service document: " + serviceUri + "\r\n";

            try
            {
                // The result here is usually the same as:
                // Uri resourceUri = new Uri(ServiceAddressField.Text.Trim() + Common.EditUri);
                Uri resourceUri = await FindEditUri(serviceUri);

                if (resourceUri == null)
                {
                    rootPage.NotifyUser("URI not found in service document.", NotifyType.ErrorMessage);
                    return;
                }

                outputField.Text += "Uploading Post: " + resourceUri + "\r\n";

                SyndicationItem item = new SyndicationItem();
                item.Title = new SyndicationText(titleField.Text, SyndicationTextType.Text);
                item.Content = new SyndicationContent(contentField.Text, SyndicationTextType.Html);

                SyndicationItem result = await CommonData.GetClient().CreateResourceAsync(resourceUri, item.Title.Text, item);

                outputField.Text += "Posted at " + result.ItemUri + "\r\n";
                rootPage.NotifyUser("New post created.", NotifyType.StatusMessage);
            }
            catch (Exception ex)
            {
                if (!CommonData.HandleException(ex, outputField, rootPage))
                {
                    throw;
                }
            }
        }

        // Read the service document to find the URI we're supposed to use when uploading content.
        private async Task<Uri> FindEditUri(Uri serviceUri)
        {
            ServiceDocument serviceDocument = await CommonData.GetClient().RetrieveServiceDocumentAsync(serviceUri);

            foreach (Workspace workspace in serviceDocument.Workspaces)
            {
                foreach (ResourceCollection collection in workspace.Collections)
                {
                    if (string.Join(";", collection.Accepts) == "application/atom+xml;type=entry")
                    {
                        return collection.Uri;
                    }
                }
            }

            return null;
        }

        private void UserNameField_TextChanged(object sender, TextChangedEventArgs e)
        {
            CommonData.Save(this);
        }

        private void PasswordField_PasswordChanged(object sender, RoutedEventArgs e)
        {
            CommonData.Save(this);
        }
    }
}
