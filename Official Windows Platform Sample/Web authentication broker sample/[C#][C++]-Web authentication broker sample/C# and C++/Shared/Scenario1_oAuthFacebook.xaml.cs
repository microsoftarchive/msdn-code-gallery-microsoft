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
using System.Linq;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.Security.Authentication.Web;
using Windows.Security.Cryptography.Core;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Web.Http;
using Windows.ApplicationModel.Activation;

namespace WebAuthentication
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
#if WINDOWS_PHONE_APP
    public sealed partial class Scenario1 : Page, IWebAuthenticationContinuable
#else
    public sealed partial class Scenario1 : Page
#endif
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

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
        }

        private void DebugPrint(String Trace)
        {
            FacebookDebugArea.Text += Trace + "\r\n";
        }

        private void OutputToken(String TokenUri)
        {
            FacebookReturnedToken.Text = TokenUri;
        }

#if WINDOWS_PHONE_APP
        private void Launch_Click(object sender, RoutedEventArgs e)
#else
        private async void Launch_Click(object sender, RoutedEventArgs e)
#endif 
        {
            if (FacebookClientID.Text == "")
            {
                rootPage.NotifyUser("Please enter an Client ID.", NotifyType.StatusMessage);
            }
            else if (FacebookCallbackUrl.Text == "")
            {
                rootPage.NotifyUser("Please enter an Callback URL.", NotifyType.StatusMessage);
            }

            try
            {
                String FacebookURL = "https://www.facebook.com/dialog/oauth?client_id=" + Uri.EscapeDataString(FacebookClientID.Text) + "&redirect_uri=" + Uri.EscapeDataString(FacebookCallbackUrl.Text) + "&scope=read_stream&display=popup&response_type=token";

                System.Uri StartUri = new Uri(FacebookURL);
                System.Uri EndUri = new Uri(FacebookCallbackUrl.Text);

                DebugPrint("Navigating to: " + FacebookURL);
#if WINDOWS_PHONE_APP
                WebAuthenticationBroker.AuthenticateAndContinue(StartUri, EndUri, null, WebAuthenticationOptions.None);
#else
                WebAuthenticationResult WebAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(
                                                        WebAuthenticationOptions.None,
                                                        StartUri,
                                                        EndUri);
                if (WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
                {
                    OutputToken(WebAuthenticationResult.ResponseData.ToString());
                    await GetFacebookUserNameAsync(WebAuthenticationResult.ResponseData.ToString());
                }
                else if (WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
                {
                    OutputToken("HTTP Error returned by AuthenticateAsync() : " + WebAuthenticationResult.ResponseErrorDetail.ToString());
                }
                else
                {
                    OutputToken("Error returned by AuthenticateAsync() : " + WebAuthenticationResult.ResponseStatus.ToString());
                }
#endif
            }
            catch (Exception Error)
            {
                //
                // Bad Parameter, SSL/TLS Errors and Network Unavailable errors are to be handled here.
                //
                DebugPrint(Error.ToString());
            }
        }

#if WINDOWS_PHONE_APP
        public async void ContinueWebAuthentication(WebAuthenticationBrokerContinuationEventArgs args)
        {
            WebAuthenticationResult result = args.WebAuthenticationResult;


            if (result.ResponseStatus == WebAuthenticationStatus.Success)
            {
                OutputToken(result.ResponseData.ToString());
                await GetFacebookUserNameAsync(result.ResponseData.ToString());
            }
            else if (result.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
            {
                OutputToken("HTTP Error returned by AuthenticateAsync() : " + result.ResponseErrorDetail.ToString());
            }
            else
            {
                OutputToken("Error returned by AuthenticateAsync() : " + result.ResponseStatus.ToString());
            }
        }
#endif


        /// <summary>
        /// This function extracts access_token from the response returned from web authentication broker
        /// and uses that token to get user information using facebook graph api. 
        /// </summary>
        /// <param name="webAuthResultResponseData">responseData returned from AuthenticateAsync result.</param>
        private async Task GetFacebookUserNameAsync(string webAuthResultResponseData)
        {
            //Get Access Token first
            string responseData = webAuthResultResponseData.Substring(webAuthResultResponseData.IndexOf("access_token"));
            String[] keyValPairs = responseData.Split('&');
            string access_token = null;
            string expires_in = null;
            for (int i = 0; i < keyValPairs.Length; i++)
            {
                String[] splits = keyValPairs[i].Split('=');
                switch (splits[0])
                {
                    case "access_token":
                        access_token = splits[1]; //you may want to store access_token for further use. Look at Scenario5 (Account Management).
                        break;
                    case "expires_in":
                        expires_in = splits[1];
                        break;
                }
            }

            DebugPrint("access_token = " + access_token);
            //Request User info.
            HttpClient httpClient = new HttpClient();
            string response = await httpClient.GetStringAsync(new Uri("https://graph.facebook.com/me?access_token=" + access_token));
            JsonObject value = JsonValue.Parse(response).GetObject();
            string facebookUserName = value.GetNamedString("name");

            rootPage.NotifyUser(facebookUserName + " is connected!!", NotifyType.StatusMessage);
        }
    }
}
