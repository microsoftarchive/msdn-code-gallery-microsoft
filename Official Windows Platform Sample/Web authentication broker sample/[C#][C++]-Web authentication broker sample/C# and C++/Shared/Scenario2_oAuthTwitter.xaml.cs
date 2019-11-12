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
using Windows.Web.Http;
using Windows.Web.Http.Headers;
using Windows.ApplicationModel.Activation;

namespace WebAuthentication
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
#if WINDOWS_PHONE_APP
    public sealed partial class Scenario2 : Page, IWebAuthenticationContinuable
#else
    public sealed partial class Scenario2 : Page  
#endif  
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario2()
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

        private async Task<String> SendDataAsync(String Url)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                return await httpClient.GetStringAsync(new Uri(Url));
            }
            catch (Exception Err)
            {
                rootPage.NotifyUser("Error getting data from server." + Err.Message, NotifyType.StatusMessage);
            }

            return null;
        }

        private void DebugPrint(String Trace)
        {
            TwitterDebugArea.Text += Trace + "\r\n";
        }

        private void OutputToken(String TokenUri)
        {
            TwitterReturnedToken.Text = TokenUri;
        }

#if WINDOWS_PHONE_APP
        private async void Launch_Click(object sender, RoutedEventArgs e)
#else
        private async void Launch_Click(object sender, RoutedEventArgs e)
#endif 
        {
            if (TwitterClientID.Text == "")
            {
                rootPage.NotifyUser("Please enter an Client ID.", NotifyType.StatusMessage);
            }
            else if (TwitterCallbackUrl.Text == "")
            {
                rootPage.NotifyUser("Please enter an Callback URL.", NotifyType.StatusMessage);
            }
            else if (TwitterClientSecret.Text == "")
            {
                rootPage.NotifyUser("Please enter an Client Secret.", NotifyType.StatusMessage);
            }

            try
            {
                string oauth_token = await GetTwitterRequestTokenAsync(TwitterCallbackUrl.Text, TwitterClientID.Text);
                string TwitterUrl = "https://api.twitter.com/oauth/authorize?oauth_token=" + oauth_token;
                System.Uri StartUri = new Uri(TwitterUrl);
                System.Uri EndUri = new Uri(TwitterCallbackUrl.Text);

//                DebugPrint("Navigating to: " + TwitterUrl);
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
                    await GetTwitterUserNameAsync(WebAuthenticationResult.ResponseData.ToString());
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
                await GetTwitterUserNameAsync(result.ResponseData.ToString());
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

        private async Task GetTwitterUserNameAsync(string webAuthResultResponseData)
        {
            //
            // Acquiring a access_token first
            //

            string responseData = webAuthResultResponseData.Substring(webAuthResultResponseData.IndexOf("oauth_token"));
            string request_token = null;
            string oauth_verifier = null;
            String[] keyValPairs = responseData.Split('&');

            for (int i = 0; i < keyValPairs.Length; i++)
            {
                String[] splits = keyValPairs[i].Split('=');
                switch (splits[0])
                {
                    case "oauth_token":
                        request_token = splits[1];
                        break;
                    case "oauth_verifier":
                        oauth_verifier = splits[1];
                        break;
                }
            }

            String TwitterUrl = "https://api.twitter.com/oauth/access_token";

            string timeStamp = GetTimeStamp();
            string nonce = GetNonce();

            String SigBaseStringParams = "oauth_consumer_key=" + TwitterClientID.Text;
            SigBaseStringParams += "&" + "oauth_nonce=" + nonce;
            SigBaseStringParams += "&" + "oauth_signature_method=HMAC-SHA1";
            SigBaseStringParams += "&" + "oauth_timestamp=" + timeStamp;
            SigBaseStringParams += "&" + "oauth_token=" + request_token;
            SigBaseStringParams += "&" + "oauth_version=1.0";
            String SigBaseString = "POST&";
            SigBaseString += Uri.EscapeDataString(TwitterUrl) + "&" + Uri.EscapeDataString(SigBaseStringParams);

            String Signature = GetSignature(SigBaseString, TwitterClientSecret.Text);

            HttpStringContent httpContent = new HttpStringContent("oauth_verifier=" + oauth_verifier, Windows.Storage.Streams.UnicodeEncoding.Utf8);
            httpContent.Headers.ContentType = HttpMediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");
            string authorizationHeaderParams = "oauth_consumer_key=\"" + TwitterClientID.Text + "\", oauth_nonce=\"" + nonce + "\", oauth_signature_method=\"HMAC-SHA1\", oauth_signature=\"" + Uri.EscapeDataString(Signature) + "\", oauth_timestamp=\"" + timeStamp + "\", oauth_token=\"" + Uri.EscapeDataString(request_token) + "\", oauth_version=\"1.0\"";

            HttpClient httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Authorization = new HttpCredentialsHeaderValue("OAuth", authorizationHeaderParams);
            var httpResponseMessage = await httpClient.PostAsync(new Uri(TwitterUrl), httpContent);
            string response = await httpResponseMessage.Content.ReadAsStringAsync();

            String[] Tokens = response.Split('&');
            string oauth_token_secret = null;
            string access_token = null;
            string screen_name = null;

            for (int i = 0; i < Tokens.Length; i++)
            {
                String[] splits = Tokens[i].Split('=');
                switch (splits[0])
                {
                    case "screen_name":
                        screen_name = splits[1];
                        break;
                    case "oauth_token":
                        access_token = splits[1];
                        break;
                    case "oauth_token_secret":
                        oauth_token_secret = splits[1];
                        break;
                }
            }

            
	    //you can store access_token and oauth_token_secret for further use. See Scenario5(Account Management).
            if (access_token != null)
            {
                DebugPrint("access_token = " + access_token);
            }

            if (oauth_token_secret != null)
            {
                DebugPrint("oauth_token_secret = " + oauth_token_secret);
            }
            if (screen_name != null)
            {
                rootPage.NotifyUser(screen_name + " is connected!!", NotifyType.StatusMessage);
            }
        }

        private async Task<string> GetTwitterRequestTokenAsync(string twitterCallbackUrl, string consumerKey)
        {
            //
            // Acquiring a request token
            //
            string TwitterUrl = "https://api.twitter.com/oauth/request_token";

            string nonce = GetNonce();
            string timeStamp = GetTimeStamp();
            string SigBaseStringParams = "oauth_callback=" + Uri.EscapeDataString(twitterCallbackUrl);
            SigBaseStringParams += "&" + "oauth_consumer_key=" + consumerKey;
            SigBaseStringParams += "&" + "oauth_nonce=" + nonce;
            SigBaseStringParams += "&" + "oauth_signature_method=HMAC-SHA1";
            SigBaseStringParams += "&" + "oauth_timestamp=" + timeStamp;
            SigBaseStringParams += "&" + "oauth_version=1.0";
            string SigBaseString = "GET&";
            SigBaseString += Uri.EscapeDataString(TwitterUrl) + "&" + Uri.EscapeDataString(SigBaseStringParams);
            string Signature = GetSignature(SigBaseString, TwitterClientSecret.Text);

            TwitterUrl += "?" + SigBaseStringParams + "&oauth_signature=" + Uri.EscapeDataString(Signature);
            HttpClient httpClient = new HttpClient();
            string GetResponse = await httpClient.GetStringAsync(new Uri(TwitterUrl));

//          DebugPrint("Received Data: " + GetResponse);

            string request_token = null;
            string oauth_token_secret = null;
            string[] keyValPairs = GetResponse.Split('&');

            for (int i = 0; i < keyValPairs.Length; i++)
            {
                string[] splits = keyValPairs[i].Split('=');
                switch (splits[0])
                {
                    case "oauth_token":
                        request_token = splits[1];
                        break;
                    case "oauth_token_secret":
                        oauth_token_secret = splits[1];
                        break;
                }
            }

            return request_token;
        }

        string GetNonce()
        {
            Random rand = new Random();
            int nonce = rand.Next(1000000000);
            return nonce.ToString();
        }

        string GetTimeStamp()
        {
            TimeSpan SinceEpoch = DateTime.UtcNow - new DateTime(1970, 1, 1);
            return Math.Round(SinceEpoch.TotalSeconds).ToString();
        }

        string GetSignature(string sigBaseString, string consumerSecretKey)
        {
            IBuffer KeyMaterial = CryptographicBuffer.ConvertStringToBinary(consumerSecretKey + "&", BinaryStringEncoding.Utf8);
            MacAlgorithmProvider HmacSha1Provider = MacAlgorithmProvider.OpenAlgorithm("HMAC_SHA1");
            CryptographicKey MacKey = HmacSha1Provider.CreateKey(KeyMaterial);
            IBuffer DataToBeSigned = CryptographicBuffer.ConvertStringToBinary(sigBaseString, BinaryStringEncoding.Utf8);
            IBuffer SignatureBuffer = CryptographicEngine.Sign(MacKey, DataToBeSigned);
            string Signature = CryptographicBuffer.EncodeToBase64String(SignatureBuffer);

            return Signature;
        }

    }
}
