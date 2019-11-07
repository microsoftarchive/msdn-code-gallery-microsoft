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
    public partial class DirectContentUriScenario : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public DirectContentUriScenario()
        {
            this.InitializeComponent();
        }

        protected async void UpdateIfUriIsInCache(Uri uri, TextBlock cacheStatusTextBlock)
        {
            var filter = new Windows.Web.Http.Filters.HttpBaseProtocolFilter();
            filter.CacheControl.ReadBehavior = Windows.Web.Http.Filters.HttpCacheReadBehavior.OnlyFromCache;

            var httpClient = new Windows.Web.Http.HttpClient(filter);
            var request = new Windows.Web.Http.HttpRequestMessage(Windows.Web.Http.HttpMethod.Get, uri);

            try
            {
                await httpClient.SendRequestAsync(request);
                cacheStatusTextBlock.Text = "Yes";
                cacheStatusTextBlock.Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Green);
            }
            catch
            {
                cacheStatusTextBlock.Text = "No";
                cacheStatusTextBlock.Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Red); 
            }
        }

        protected void UpdateUriTable()
        {
            DirectContentUris.Children.Clear();
            UriCacheStatus.Children.Clear();

            foreach (Uri uri in Windows.Networking.BackgroundTransfer.ContentPrefetcher.ContentUris)
            {
                DirectContentUris.Children.Add(new TextBlock() { Text = uri.ToString() });

                TextBlock cacheStatusTextBlock = new TextBlock();
                UriCacheStatus.Children.Add(cacheStatusTextBlock);
                
                UpdateIfUriIsInCache(uri, cacheStatusTextBlock);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            UpdateUriTable();
        }

        private void AddDirectUri_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b != null)
            {
                String uriString = DirectContentUri.Text;
                if (uriString != "")
                {
                    Uri uri = null;

                    // Typically, a developer would add a predetermined URI to the ContentPrefetcher.ContentUris vector, removing the
                    // need for the following error checking. Because this sample takes URI input from the user, this sample validates
                    // the input string before passing the URI to the ContentPrefetcher API.
                    if (Uri.TryCreate(uriString, UriKind.RelativeOrAbsolute, out uri))
                    {
                        if (uri.IsAbsoluteUri)
                        {
                            Windows.Networking.BackgroundTransfer.ContentPrefetcher.ContentUris.Add(uri);
                            UpdateUriTable();
                            this.DirectContentUri.Text = "";
                            rootPage.NotifyUser("", NotifyType.StatusMessage);
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
        }

        private void ClearDirectUris_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b != null)
            {
                rootPage.NotifyUser("", NotifyType.StatusMessage);
                Windows.Networking.BackgroundTransfer.ContentPrefetcher.ContentUris.Clear();
                UpdateUriTable();
            }
        }
    }
}
