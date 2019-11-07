//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using SDKTemplate;

namespace Microsoft.Samples.Networking.HttpClientSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario1 : SDKTemplate.Common.LayoutAwarePage, IDisposable
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        HttpClient httpClient;

        public Scenario1()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {    
            // In this scenario we just create an HttpClient instance with default settings. I.e. no base address
            // and no custom handlers. For examples on how to specify a base address and use custom handlers see
            // other scenarios.
            httpClient = new HttpClient();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Dispose();
        }

        private async void Start_Click(object sender, RoutedEventArgs e)
        {
            OutputField.Text = string.Empty;

            // The value of 'AddressField' is set by the user and is therefore untrusted input. If we can't create a
            // valid, absolute URI, we'll notify the user about the incorrect input.
            // Note that this app has both "Internet (Client)" and "Home and Work Networking" capabilities set,
            // since the user may provide URIs for servers located on the intErnet or intrAnet. If apps only
            // communicate with servers on the intErnet, only the "Internet (Client)" capability should be set.
            // Similarly if an app is only intended to communicate on the intrAnet, only the "Home and Work
            // Networking" capability should be set.
            Uri resourceUri;
            if (!Uri.TryCreate(AddressField.Text.Trim(), UriKind.Absolute, out resourceUri))
            {
                rootPage.NotifyUser("Invalid URI.", NotifyType.ErrorMessage);
                return;
            }

            Helpers.ScenarioStarted(StartButton, CancelButton);
            rootPage.NotifyUser("In progress", NotifyType.StatusMessage);

            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(resourceUri);

                await Helpers.DisplayTextResult(response, OutputField);

                rootPage.NotifyUser("Completed", NotifyType.StatusMessage);
            }
            catch (HttpRequestException hre)
            {
                rootPage.NotifyUser(hre.Message, NotifyType.ErrorMessage);
                OutputField.Text = hre.ToString();
            }
            catch (TaskCanceledException)
            {
                rootPage.NotifyUser("Request canceled.", NotifyType.ErrorMessage);
            }
            finally
            {
                Helpers.ScenarioCompleted(StartButton, CancelButton);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            httpClient.CancelPendingRequests();
        }

        public void Dispose()
        {
            if (httpClient != null)
            {
                httpClient.Dispose();
                httpClient = null;
            }
        }
    }
}
