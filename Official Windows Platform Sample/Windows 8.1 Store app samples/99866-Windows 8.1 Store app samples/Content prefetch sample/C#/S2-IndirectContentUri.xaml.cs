//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;

namespace SDKTemplate
{
    public sealed partial class IndirectContentUriScenario : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public IndirectContentUriScenario()
        {
            this.InitializeComponent();
            UpdateIndirectUriBlock();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void SetIndirectContentUri_Click(object sender, RoutedEventArgs e)
        {
            rootPage.NotifyUser("", NotifyType.StatusMessage);

            String uriString = IndirectContentUriBox.Text;
            if (uriString != "")
            {
                Uri uri = null;

                // Typically, a developer would simply set the ContentPrefetcher.IndirectContentUri to the URI of a service that
                // she or he owns, removing the need for the following error checking. Because this sample takes URI input from
                // the user, this sample validates the input string before passing the URI to the ContentPrefetcher API.
                if (Uri.TryCreate(uriString, UriKind.RelativeOrAbsolute, out uri))
                {
                    if (uri.IsAbsoluteUri)
                    {
                        Windows.Networking.BackgroundTransfer.ContentPrefetcher.IndirectContentUri = uri;
                        UpdateIndirectUriBlock();
                    }
                    else
                    {
                        rootPage.NotifyUser("A URI must be an absolute URI", NotifyType.ErrorMessage);
                    }
                }
                else
                {
                    rootPage.NotifyUser("A URI must be provided that has the form scheme://address", NotifyType.ErrorMessage);
                }
            }
        }

        private void UpdateIndirectUriBlock()
        {
            rootPage.NotifyUser("", NotifyType.StatusMessage);

            Uri uri = Windows.Networking.BackgroundTransfer.ContentPrefetcher.IndirectContentUri;
            if (uri != null)
            {
                IndirectContentUriBlock.Text = "The indirect content URI is " + uri.ToString();
            }
            else
            {
                IndirectContentUriBlock.Text = "There is no indirect content URI set";
            }
        }

        private void ClearIndirectContentUri_Click(object sender, RoutedEventArgs e)
        {
            rootPage.NotifyUser("", NotifyType.StatusMessage);

            Windows.Networking.BackgroundTransfer.ContentPrefetcher.IndirectContentUri = null;
            UpdateIndirectUriBlock();
        }

    }
}
