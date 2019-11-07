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
using SDKTemplate;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Linq;

namespace SDKTemplate
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class XLinqScenario : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public XLinqScenario()
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
        }

        /// <summary>
        /// This is the click handler for the 'DoSomething' button.  You would replace this with your own handler
        /// if you have a button or buttons on this page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DoSomething_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b != null)
            {
                rootPage.NotifyUser("You clicked the " + b.Content + " button", NotifyType.StatusMessage);
            }

            try
            {
                string cities = await ProcessWithXLinq();
                OutputTextBlock1.Text = cities;
            }
            catch (System.Xml.XmlException ex)
            {
                OutputTextBlock1.Text = "Exception happened, Message:" + ex.Message;
            }
            catch (System.Net.Http.HttpRequestException netEx)
            {
                OutputTextBlock1.Text = "Exception happend, Message:" + netEx.Message + " Have you updated the bing map key in function ProcessWithXLinq()?";
            }
        }

        private async Task<string> ProcessWithXLinq()
        {
            // you need to acquire a Bing Maps key. See http://www.bingmapsportal.com/
            string bingMapKey = "INSERT_YOUR_BING_MAPS_KEY";    
            // the following uri will returns a response with xml content
            Uri uri = new Uri(String.Format("http://dev.virtualearth.net/REST/v1/Locations?q=manchester&o=xml&key={0}", bingMapKey));

            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            // ReadAsStreamAsync() returns when the whole message is downloaded
            Stream stream = await response.Content.ReadAsStreamAsync(); 

            XDocument xdoc = XDocument.Load(stream);
            XNamespace xns = "http://schemas.microsoft.com/search/local/ws/rest/v1";
            var addresses = from node in xdoc.Descendants(xns + "Address")                               // query node named "Address"
                            where node.Element(xns + "CountryRegion").Value.Contains("United States")    // where CountryRegion contains "United States"
                            select node.Element(xns + "FormattedAddress").Value;                         // select the FormattedAddress node's value

            StringBuilder stringBuilder = new StringBuilder("Manchester in US: ");
            foreach (string name in addresses)
                stringBuilder.Append(name + "; ");

            return stringBuilder.ToString();
        }

    }
}
