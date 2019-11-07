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
using System.Linq;
using System.Net.Http;
using System.Xml.Linq;
using SDKTemplate;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Microsoft.Samples.Networking.HttpClientSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario3 : SDKTemplate.Common.LayoutAwarePage, IDisposable
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        HttpClient httpClient;

        public Scenario3()
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
            Helpers.CreateHttpClient(ref httpClient);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Dispose();
        }

        private async void Start_Click(object sender, RoutedEventArgs e)
        {
            Helpers.ScenarioStarted(StartButton, CancelButton);
            rootPage.NotifyUser("In progress", NotifyType.StatusMessage);
            OutputField.Text = string.Empty;

            try
            {
                // 'AddressField' is a disabled text box, so the value is considered trusted input. When enabling the
                // text box make sure to validate user input (e.g., by catching FormatException as shown in scenario 1).
                string resourceAddress = AddressField.Text;
                HttpResponseMessage response = await httpClient.GetAsync(resourceAddress);
                
                await Helpers.DisplayTextResult(response, OutputField);
                response.EnsureSuccessStatusCode();
                XElement element = XElement.Parse(await response.Content.ReadAsStringAsync());
                OutputList.ItemsSource =
                    (from c in element.Elements("item")
                     select c.Attribute("name").Value
                    );

                rootPage.NotifyUser("Completed", NotifyType.StatusMessage);
            }
            catch (HttpRequestException hre)
            {
                rootPage.NotifyUser("Error", NotifyType.ErrorMessage);
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
