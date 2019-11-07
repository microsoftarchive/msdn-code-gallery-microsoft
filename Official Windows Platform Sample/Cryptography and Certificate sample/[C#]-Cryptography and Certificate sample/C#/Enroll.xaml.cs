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
    public sealed partial class Enroll : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;
        string certificateRequest;

        public Enroll()
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
        /// Submit request to a server then install the returned certificate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RunSample_Click(object sender, RoutedEventArgs e)
        {
            TextBlock outputTextBlock = rootPage.FindName("OutputTextBlock") as TextBlock;
            TextBox urlTextBox = rootPage.FindName("UrlTextBox") as TextBox;
            CheckBox userStoreCheckBox = rootPage.FindName("UserStoreCheckBox") as CheckBox;
            string response = "";
            string url = "";

            outputTextBlock.Text = "Creating certificate request...";

            // validate the URL as entered in "UrlTextBox"
            if (String.IsNullOrEmpty(urlTextBox.Text) ||
                !System.Uri.IsWellFormedUriString(urlTextBox.Text.Trim(), UriKind.Absolute))
            {
                outputTextBlock.Text = "a valid URL is not provided, so request will be created but will not be submitted.";
            }
            else
            {
                url = urlTextBox.Text.Trim();
            }

            // create certificate request
            try
            {
                // call the default constructor of CertificateRequestProperties
                CertificateRequestProperties reqProp = new CertificateRequestProperties();

                reqProp.Subject = "Toby";
                reqProp.FriendlyName = "Toby's Cert";

                if (true == userStoreCheckBox.IsChecked)
                {
                    // have to use User's Certificate Store
                    // call User Certificate Enrollment function createRequest to create a certificate request
                    certificateRequest = await CertificateEnrollmentManager.UserCertificateEnrollmentManager.CreateRequestAsync(reqProp);
                    outputTextBlock.Text += "\nRequest created, content:\n" + certificateRequest;
                }
                else
                {
                    // use App's certificate store
                    // call Certificate Enrollment function createRequest to create a certificate request
                    certificateRequest = await CertificateEnrollmentManager.CreateRequestAsync(reqProp);
                    outputTextBlock.Text += "\nRequest created, content:\n" + certificateRequest;
                }
            }
            catch (Exception ex)
            {
                outputTextBlock.Text += "\nCertificate request creation failed with error: " + ex.Message;
            }                

            // make sure request was created successfully and there is a valid URL
            if (String.IsNullOrEmpty(certificateRequest) ||
                string.IsNullOrEmpty(url))
            {
                return;
            }

            // submit request for enrollment and install the certificate
            try
            {
                outputTextBlock.Text += "\n\nSubmitting request to server...";
                response = await SubmitCertificateRequestAndGetResponse(certificateRequest, url);

                if (response.StartsWith("Error:", StringComparison.OrdinalIgnoreCase))
                {
                    // there was an error getting the response from server. Display the error
                    outputTextBlock.Text += "\n" + response;
                }
                else
                {
                    outputTextBlock.Text += "\nResponse received, content: \n" + response;

                    // install certificate
                    outputTextBlock.Text += "\nInstalling certificate ...";

                    if (true == userStoreCheckBox.IsChecked)
                    {
                        // install certificate to User's certificate store
                        // call User Certificate Enrollment InstallCertificate function to install certificate
                        await CertificateEnrollmentManager.UserCertificateEnrollmentManager.InstallCertificateAsync(response, InstallOptions.None);
                        outputTextBlock.Text += "\nThe certificate response is installed successfully to User's certificate store.\n";
                    }
                    else
                    {
                        // install certificate to App's certificate store
                        // call Certificate Enrollment InstallCertificate function to install certificate
                        await CertificateEnrollmentManager.InstallCertificateAsync(response, InstallOptions.None);
                        outputTextBlock.Text += "\nThe certificate response is installed successfully to App's certificate store.\n";
                    }
                }
            }
            catch (Exception ex)
            {
                outputTextBlock.Text += "\nCertificate Installation failed with error: " + ex.Message + "\n";
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
            string certResponse = "";
            WebRequest webRequest = WebRequest.Create(url);
            webRequest.ContentType = "text/xml;charset=utf-8";
            webRequest.Method = "POST";

            string body = "<SubmitRequest xmlns=\"http://tempuri.org/\"><request>" + certificateRequest + "</request>"
                + "<attributes xmlns:a=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\"><a:KeyValueOfstringstring><a:Key>CertificateTemplate</a:Key><a:Value>WebServer</a:Value></a:KeyValueOfstringstring></attributes>"
                    + "</SubmitRequest>";
            byte[] writeBuffer = Encoding.UTF8.GetBytes(body);
            using (Stream stream = await webRequest.GetRequestStreamAsync())
            {
                stream.Write(writeBuffer, 0, writeBuffer.Length);
            }

            // response
            try
            {
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
                certResponse = nodeList[0].InnerText;
            }
            catch (Exception ex)
            {
                certResponse = "Error: " + ex.Message + "\n";
            }

            return certResponse;
        }

    }
}
