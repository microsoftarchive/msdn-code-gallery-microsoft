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
using Windows.Security.Cryptography.Certificates;
using System.Net;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Windows.Data.Xml.Dom;

namespace CertificateEnrollment
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario_Enroll : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;
        string certificateRequest;

        public Scenario_Enroll()
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
        /// Create a certificate request
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CreateRequest_Click(object sender, RoutedEventArgs e)
        {
            TextBlock outputTextBlock = rootPage.FindName("OutputTextBlock") as TextBlock;
            outputTextBlock.Text = "Creating certificate request...";

            try
            {
                //call the default constructor of CertificateRequestProperties
                CertificateRequestProperties reqProp = new CertificateRequestProperties();
                reqProp.Subject = "Toby";
                reqProp.FriendlyName = "Toby's Cert";

                //call Certificate Enrollment function createRequest to create a certificate request
                certificateRequest = await CertificateEnrollmentManager.CreateRequestAsync(reqProp);
                outputTextBlock.Text += "\nRequest created, content:\n" + certificateRequest;
            }
            catch (Exception ex)
            {
                outputTextBlock.Text += "\n\nCertificate request creation failed with error: " + ex.Message;
            }
        }

        /// <summary>
        /// Submit request to a server then install the returned certificate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void InstallCertifiate_Click(object sender, RoutedEventArgs e)
        {
            TextBlock outputTextBlock = rootPage.FindName("OutputTextBlock") as TextBlock;
            string response = "";            

            if (String.IsNullOrEmpty(certificateRequest))
            {
                outputTextBlock.Text += "\nYou need to create a certificate request first";
                return;
            }

            // To submit request, a valid url need to be provided. the url needs to point to a server which can take certificate request and issue certs
            // Add code here to initialize url
            string url = "";
            
            if (String.IsNullOrEmpty(url))
            {
                outputTextBlock.Text = "\nTo submit a request, please update the code provide a valid URL.";
                return;
            }

            try
            {
                outputTextBlock.Text = "Submitting request to server...";
                response = await SubmitCertificateRequestAndGetResponse(certificateRequest, url);

                if (String.IsNullOrEmpty(response))
                {
                    outputTextBlock.Text += "\nSubmit request succeeded but the returned response is empty.";
                }

                outputTextBlock.Text += "\nResponse received, content: \n" + response;

                // install
                outputTextBlock.Text += "\nInstalling certificate ...";
                await CertificateEnrollmentManager.InstallCertificateAsync(response, InstallOptions.None);
                outputTextBlock.Text += "\nThe certificate response is installed sucessfully.\n";
            }
            catch (Exception ex)
            {
                outputTextBlock.Text += "\n\nCertificate Installation failed with error: " + ex.Message + "\n";
            }
        }

        /// <summary>
        /// Submit a certificate request to a Certificate Services
        /// The request is encapsulated in an XML and sent to server over http, the XML format is defined by the server.
        /// </summary>
        /// <param name="certificateRequest"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        private async static Task<string> SubmitCertificateRequestAndGetResponse(string certificateRequest, string url)
        {
            WebRequest webRequest = WebRequest.Create(url);
            webRequest.ContentType = "text/xml;charset=utf-8";
            webRequest.Method = "POST";

            // Depends on the server implementaion, the XML format could be different
            string body = "<SubmitRequest xmlns=\"http://tempuri.org/\"><strRequest>" + certificateRequest + "</strRequest></SubmitRequest>";
            byte[] writeBuffer = Encoding.UTF8.GetBytes(body);
            using (Stream stream = await webRequest.GetRequestStreamAsync())
            {
                stream.Write(writeBuffer, 0, writeBuffer.Length);
            }

            // response
            var respAsyncResult = await webRequest.GetResponseAsync();
            string rawServerResponse = null;

            using (WebResponse response = await webRequest.GetResponseAsync())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        rawServerResponse = reader.ReadToEnd(); // read response data
                    }
                }
            }

            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(rawServerResponse);
            var nodeList = xDoc.GetElementsByTagName("SubmitRequestResult");
            if (nodeList.Length != 1)
            {
                throw new XmlException("The certificate response is not in expected XML format.");
            }
            return nodeList[0].InnerText;
        }

    }
}
