/****************************** Module Header ******************************\
* Module Name:  MainPage.cs
* Project:      CSUnvsAppIgnoreSSLError
* Copyright (c) Microsoft Corporation.
*
* This sample demonstrates how to ignore SSL errors in universal Windows apps.
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Security.Cryptography.Certificates;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace CSUnvsAppIgnoreSSLError
{
    public sealed partial class MainPage
    {
        HttpClient m_httpClient;
        public MainPage()
        {
            this.InitializeComponent();
            m_httpClient = new HttpClient();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

        }
        private async void Footer_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton hyperlinkButton = sender as HyperlinkButton;
            if (hyperlinkButton != null)
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri(hyperlinkButton.Tag.ToString()));
            }
        }

#if WINDOWS_APP
        private void Page_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            if (e.NewSize.Width < 700.0)
            {
                VisualStateManager.GoToState(this, "MinimalLayout", true);
            }
            else if (e.NewSize.Width < e.NewSize.Height)
            {
                VisualStateManager.GoToState(this, "PortraitLayout", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "DefaultLayout", true);
            }
        }
#endif
        private async Task<string> TestCertificate(Uri theUri, string theExpectedIssuer)
        {
            // Simple GET for URI passed in
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, theUri);
            // Retry for cert error issues
            bool retryIgnoreCertErrors = false;
            // return value
            string retVal = "trying to GET";
            // Base  filter 
            HttpBaseProtocolFilter httpBaseProtocolFilter = null;

            try
            {
                HttpResponseMessage response = await m_httpClient.SendRequestAsync(request);
                // hit here if no exceptions!
                retVal = "No Cert errors";
            }
            catch (Exception ex)
            {
                retVal = ex.Message;

                // Mask the HResult and if this is error code 12045 which means there was a certificate error
                if ((ex.HResult & 65535) == 12045)
                {
                    // Get a list of the server cert errors
                    IReadOnlyList<ChainValidationResult> errors = request.TransportInformation.ServerCertificateErrors;
                    
                    // I expect that the cert is expired and it is untrusted for my scenario...
                    if ((errors != null) && (errors.Contains(ChainValidationResult.Expired)
                           && errors.Contains(ChainValidationResult.Untrusted)))
                    {
                        // Specifically validate that this came from a particular Issuer
                        if (request.TransportInformation.ServerCertificate.Issuer == theExpectedIssuer)
                        {
                            // Create a Base Protocol Filter to add certificate errors I want to ignore...
                            httpBaseProtocolFilter = new HttpBaseProtocolFilter();
                            // I purposefully have an expired cert to show setting multiple Ignorable Errors
                            httpBaseProtocolFilter.IgnorableServerCertificateErrors.Add(ChainValidationResult.Expired);
                            // Untrused because this is a self signed cert that is not installed
                            httpBaseProtocolFilter.IgnorableServerCertificateErrors.Add(ChainValidationResult.Untrusted);
                            // OK to retry since I expected these errors from this host!
                            retryIgnoreCertErrors = true;
                        }
                    }
                }
            }

            try
            {
                // Retry with a temporary HttpClient and ignore some very specific errors!
                if (retryIgnoreCertErrors)
                {
                    // Create a Client to use just for this request and ignore some cert errors.
                    HttpClient aTempClient = new HttpClient(httpBaseProtocolFilter);
                    // Try to execute the request (should not fail now for those two errors)
                    HttpRequestMessage aTempReq = new HttpRequestMessage(HttpMethod.Get, theUri);
                    HttpResponseMessage aResp2 = await aTempClient.SendRequestAsync(aTempReq);
                    retVal = "No Cert errors";
                }
            }
            catch (Exception ex)
            {
                // some other exception occurred
                retVal = ex.Message;
            }
            return retVal;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Uri targetUri = new Uri(txtURI.Text);

            txtResult.Text = await TestCertificate(targetUri, targetUri.Host);

        }
    }
}
