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
using System.Net;
using System.IO;
using System.Xml;
using Windows.Storage;

namespace SDKTemplate
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class XmlReaderWriterScenario : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public XmlReaderWriterScenario()
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

            string filename = "manchester_us.xml";
            try
            {
                await ProcessWithReaderWriter(filename);

                // show the content of the file just created
                using (Stream s = await KnownFolders.PicturesLibrary.OpenStreamForReadAsync(filename))
                {
                    using (StreamReader sr = new StreamReader(s))
                    {
                        OutputTextBlock1.Text = sr.ReadToEnd();
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                OutputTextBlock1.Text = "Exception happend, Message:" + ex.Message;
            }
            catch (System.Net.WebException webEx)
            {
                OutputTextBlock1.Text = "Exception happend, Message:" + webEx.Message + " Have you updated the bing map key in function ProcessWithReaderWriter()?";
            }
        }


        private async Task ProcessWithReaderWriter(string filename)
        {
            // you need to acquire a Bing Maps key. See http://www.bingmapsportal.com/
            string bingMapKey = "INSERT_YOUR_BING_MAPS_KEY";
            // the following uri will returns a response with xml content
            Uri uri = new Uri(String.Format("http://dev.virtualearth.net/REST/v1/Locations?q=manchester&o=xml&key={0}", bingMapKey));
            WebRequest request = WebRequest.Create(uri);

            // if needed, specify credential here
            // request.Credentials = new NetworkCredential();

            // GetResponseAsync() returns immediately after the header is ready
            WebResponse response = await request.GetResponseAsync();
            Stream inputStream = response.GetResponseStream();

            XmlReaderSettings xrs = new XmlReaderSettings() { Async = true, CloseInput = true };
            using (XmlReader reader = XmlReader.Create(inputStream, xrs))
            {
                XmlWriterSettings xws = new XmlWriterSettings() { Async = true, Indent = true, CloseOutput = true };
                Stream outputStream = await KnownFolders.PicturesLibrary.OpenStreamForWriteAsync(filename, CreationCollisionOption.OpenIfExists);

                using (XmlWriter writer = XmlWriter.Create(outputStream, xws))
                {
                    string prefix = "";
                    string nameSpace = "";
                    await writer.WriteStartDocumentAsync();
                    await writer.WriteStartElementAsync(prefix, "Locations", nameSpace);

                    // iterate through the REST message, and find the Mancesters in US then write to file
                    while (await reader.ReadAsync())
                    {
                        // take element nodes with name "Address"
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "Address")
                        {
                            // create a XmlReader from the Address element
                            using (XmlReader subReader = reader.ReadSubtree())
                            {
                                bool isInUS = false;
                                while (await subReader.ReadAsync())
                                {
                                    // check if the CountryRegion element contains "United States"
                                    if (subReader.Name == "CountryRegion")
                                    {
                                        string value = await subReader.ReadInnerXmlAsync();
                                        if (value.Contains("United States"))
                                        {
                                            isInUS = true;
                                        }
                                    }

                                    // write the FormattedAddress node of the reader, if the address is within US
                                    if (isInUS && subReader.NodeType == XmlNodeType.Element && subReader.Name == "FormattedAddress")
                                    {
                                        await writer.WriteNodeAsync(subReader, false);
                                    }
                                }
                            }
                        }
                    }

                    await writer.WriteEndElementAsync();
                    await writer.WriteEndDocumentAsync();
                }
            }

        }
    }
}
