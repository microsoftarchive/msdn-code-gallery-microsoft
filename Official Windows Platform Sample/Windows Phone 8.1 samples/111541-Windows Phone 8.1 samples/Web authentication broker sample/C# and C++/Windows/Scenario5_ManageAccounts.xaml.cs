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
using Windows.Security.Authentication.Web;
using Windows.Security.Cryptography.Core;
using Windows.Security.Cryptography;
using Windows.Security.Credentials;
using System.Threading.Tasks;
using Windows.UI.ApplicationSettings;
using Windows.Web.Http;
using Windows.Web.Http.Headers;
using Windows.UI.Popups;
using Windows.Storage.Streams;
using Windows.Data.Json;
using Windows.Storage;

namespace WebAuthentication
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario5 : Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()

        MainPage rootPage = MainPage.Current;
        
        string facebookUserName;
        string twitterUserName;
        WebAccount facebookAccount;
        WebAccount twitterAccount;
        WebAccountProvider facebookProvider;
        WebAccountProvider twitterProvider;

        bool isFacebookUserLoggedIn;
        bool isTwitterUserLoggedIn;

        const string FACEBOOK_ID = "Facebook.com";
        const string FACEBOOK_DISPLAY_NAME = "Facebook";
        const string TWITTER_ID = "Twitter.com";
        const string TWITTER_DISPLAY_NAME = "Twitter";
        
        const string FACEBOOK_OAUTH_TOKEN = "FACEBOOK_OAUTH_TOKEN";
        const string TWITTER_OAUTH_TOKEN = "TWITTER_OAUTH_TOKEN";
        const string TWITTER_OAUTH_TOKEN_SECRET = "TWITTER_OAUTH_TOKEN_SECRET";
        const string FACEBOOK_USER_NAME = "FACEBOOK_USER_NAME";
        const string TWITTER_USER_NAME = "TWITTER_USER_NAME";

        ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;

       
        public Scenario5()
        {
            this.InitializeComponent();
            
            InitializeWebAccountProviders();
            InitializeWebAccounts();

            SettingsPane.GetForCurrentView().CommandsRequested += CommandsRequested;
            AccountsSettingsPane.GetForCurrentView().AccountCommandsRequested += AccountCommandsRequested;
        }

        void InitializeWebAccountProviders()
        {
            facebookProvider = new WebAccountProvider(
                FACEBOOK_ID,
                FACEBOOK_DISPLAY_NAME,
                new Uri("ms-appx:///icons/Facebook.png"));

            twitterProvider = new WebAccountProvider(
                                    TWITTER_ID,
                                    TWITTER_DISPLAY_NAME,
                                    new Uri("ms-appx:///icons/Twitter.png"));
        }

        void InitializeWebAccounts()
        {
            //Initialize facebok account object if user was already logged in.
            Object facebookToken = roamingSettings.Values[FACEBOOK_OAUTH_TOKEN];
            if (facebookToken == null)
            {
                isFacebookUserLoggedIn = false;
            }
            else
            {
                Object facebookUser = roamingSettings.Values[FACEBOOK_USER_NAME];
                if (facebookUser != null)
                {
                    facebookUserName = facebookUser.ToString();
                    facebookAccount = new WebAccount(facebookProvider, facebookUserName, WebAccountState.Connected);
                    isFacebookUserLoggedIn = true;
                }
            }

            //Initialize twitter account if user was already logged in.
            Object twitterToken = roamingSettings.Values[TWITTER_OAUTH_TOKEN];
            Object twitterTokenSecret = roamingSettings.Values[TWITTER_OAUTH_TOKEN_SECRET];
            if (twitterToken == null || twitterTokenSecret == null)
            {
                isTwitterUserLoggedIn = false;
            }
            else
            {
                Object twitteruser = roamingSettings.Values[TWITTER_USER_NAME];
                if (twitteruser != null)
                {
                    twitterUserName = twitteruser.ToString();
                    twitterAccount = new WebAccount(twitterProvider, twitterUserName, WebAccountState.Connected);
                    isTwitterUserLoggedIn = true;
                }
            }
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
        /// Invoked when the navigation is about to change to a different page. You can use this function for cleanup.
        /// </summary>
        /// <param name="e">Event data describing the conditions that led to the event.</param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            AccountsSettingsPane.GetForCurrentView().AccountCommandsRequested -= AccountCommandsRequested;
        }

        #region AccountsSettings pane functions

        /// <summary>
        /// This event is generated when the user opens the settings pane. During this event, append your
        /// SettingsCommand objects to the available ApplicationCommands vector to make them available to the
        /// SettingsPange UI.
        /// </summary>
        /// <param name="settingsPane">Instance that triggered the event.</param>
        /// <param name="eventArgs">Event data describing the conditions that led to the event.</param>
        private void CommandsRequested(SettingsPane settingsPane, SettingsPaneCommandsRequestedEventArgs eventArgs)
        {
            eventArgs.Request.ApplicationCommands.Add(SettingsCommand.AccountsCommand); //This will add Accounts command in settings pane
        }

        /// <summary>
        /// This event is generated when the user clicks on Accounts command in settings pane. During this event, add your
        /// WebAccountProviderCommand, WebAccountCommand, CredentialCommand and  SettingsCommand objects to make them available to the
        /// AccountsSettingsPane UI.
        /// </summary>
        /// <param name="accountsSettingsPane">Instance that triggered the event.</param>
        /// <param name="eventArgs">Event data describing the conditions that led to the event.</param>
        private void AccountCommandsRequested(AccountsSettingsPane accountsSettingsPane, AccountsSettingsPaneCommandsRequestedEventArgs eventArgs)
        {
            var deferral = eventArgs.GetDeferral();

            //Add header text.
            eventArgs.HeaderText = "This is sample text. You can put a message here to give context to user. This section is optional.";

            //Add WebAccountProviders
            WebAccountProviderCommandInvokedHandler providerCmdHandler = new WebAccountProviderCommandInvokedHandler(WebAccountProviderInvokedHandler);
            WebAccountProviderCommand facebookProviderCommand = new WebAccountProviderCommand(facebookProvider, WebAccountProviderInvokedHandler);
            eventArgs.WebAccountProviderCommands.Add(facebookProviderCommand);
            WebAccountProviderCommand twitterProviderCommand = new WebAccountProviderCommand(twitterProvider, WebAccountProviderInvokedHandler);
            eventArgs.WebAccountProviderCommands.Add(twitterProviderCommand);

            //Add WebAccounts if available.
            WebAccountCommandInvokedHandler accountCmdHandler = new WebAccountCommandInvokedHandler(WebAccountInvokedHandler);

            if (isFacebookUserLoggedIn)
            {
                facebookAccount = new WebAccount(facebookProvider, facebookUserName, WebAccountState.Connected);
                WebAccountCommand facebookAccountCommand = new WebAccountCommand(
                facebookAccount, WebAccountInvokedHandler,
                SupportedWebAccountActions.Remove | SupportedWebAccountActions.Manage);
                eventArgs.WebAccountCommands.Add(facebookAccountCommand);
            }

            if (isTwitterUserLoggedIn)
            {
                twitterAccount = new WebAccount(twitterProvider, twitterUserName, WebAccountState.Connected);
                WebAccountCommand twitterAccountCommand = new WebAccountCommand(
                twitterAccount, WebAccountInvokedHandler,
                SupportedWebAccountActions.Remove | SupportedWebAccountActions.Manage);
                eventArgs.WebAccountCommands.Add(twitterAccountCommand);
            }

            // Add links if needed.
            Object commandID = 1;
            UICommandInvokedHandler globalLinkInvokedHandler = new UICommandInvokedHandler(GlobalLinkInvokedhandler);
            SettingsCommand command = new SettingsCommand(
                commandID,
                "More details",
                globalLinkInvokedHandler);
            eventArgs.Commands.Add(command);

            SettingsCommand command1 = new SettingsCommand(
                commandID,
                "Privacy policy",
                globalLinkInvokedHandler);
            eventArgs.Commands.Add(command1);

            deferral.Complete();

        }

        /// <summary>
        /// This is the event handler for links added to Accounts Settings pane. This method can do more work based on selected link.
        /// </summary>
        /// <param name="command">Link instance that triggered the event.</param>
        private  void GlobalLinkInvokedhandler(IUICommand command)
        {
           OutputText("Link clicked: " + command.Label);
        }

        /// <summary>
        /// This event is generated when the user clicks on action button on account details pane. This method is 
        /// responsible for handling what to do with selected action.
        /// </summary>
        /// <param name="command">Instance that triggered the event.</param>
        /// <param name="eventArgs">Event data describing the conditions that led to the event.</param>
        private void WebAccountInvokedHandler(WebAccountCommand command, WebAccountInvokedArgs eventArgs)
        {
            OutputText("Account State = " + command.WebAccount.State.ToString() + " and Selected Action = " + eventArgs.Action);
            
            if (eventArgs.Action == WebAccountAction.Remove)
            {
                //Remove user logon information since user requested to remove account.
                if (command.WebAccount.WebAccountProvider.Id.Equals(FACEBOOK_ID))
                {
                    roamingSettings.Values.Remove(FACEBOOK_USER_NAME);
                    roamingSettings.Values.Remove(FACEBOOK_OAUTH_TOKEN);
                    isFacebookUserLoggedIn = false;
                }
                else if (command.WebAccount.WebAccountProvider.Id.Equals(TWITTER_ID))
                {
                    roamingSettings.Values.Remove(TWITTER_USER_NAME);
                    roamingSettings.Values.Remove(TWITTER_OAUTH_TOKEN);
                    isTwitterUserLoggedIn = false;
                }
            }
        }

        /// <summary>
        /// This event is generated when the user clicks on Account provider tile. This method is 
        /// responsible for deciding what to do further.
        /// </summary>
        /// <param name="command">WebAccountProviderCommand instance that triggered the event.</param>
        private async void WebAccountProviderInvokedHandler(WebAccountProviderCommand command)
        {
            if(command.WebAccountProvider.Id.Equals(FACEBOOK_ID))
            {
                if (!isFacebookUserLoggedIn)
                {
                    await AuthenticateToFacebookAsync();
                }
                else
                {
                    OutputText("User is already logged in. If you support multiple accounts from the same provider then do something here to connect new user.");
                }
            }
            else if (command.WebAccountProvider.Id.Equals(TWITTER_ID))
            {
                if (!isTwitterUserLoggedIn)
                {
                    await AuthenticateToTwitterAsync();
                }
                else
                {
                    OutputText("User is already logged in. If you support multiple accounts from the same provider then do something here to connect new user.");
                }
            }
        }

        /// <summary>
        /// Event handler for Show button. This method demonstrates how to show AccountsSettings pane programatically.
        /// </summary>
        /// <param name="sender">Instance that triggered the event.</param>
        /// <param name="e">Event data describing the conditions that led to the event.</param>
        private void Show_Click(object sender, RoutedEventArgs e)
        {
            AccountsSettingsPane.Show();
        }

        #endregion


        #region Facebook authentication and related functions

        private async Task AuthenticateToFacebookAsync()
        {
            if (FacebookClientID.Text == "")
            {
                rootPage.NotifyUser("Please enter an Client ID.", NotifyType.StatusMessage);
                return;
            }
            else if (FacebookCallbackUrl.Text == "")
            {
                rootPage.NotifyUser("Please enter an Callback URL.", NotifyType.StatusMessage);
                return;
            }

            try
            {
                String FacebookURL = "https://www.facebook.com/dialog/oauth?client_id=" + Uri.EscapeDataString(FacebookClientID.Text) + "&redirect_uri=" + Uri.EscapeDataString(FacebookCallbackUrl.Text) + "&scope=read_stream&display=popup&response_type=token";

                System.Uri StartUri = new Uri(FacebookURL);
                System.Uri EndUri = new Uri(FacebookCallbackUrl.Text);

                WebAuthenticationResult WebAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(
                                                        WebAuthenticationOptions.None,
                                                        StartUri,
                                                        EndUri);
                if (WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
                {
                    OutputText(WebAuthenticationResult.ResponseData.ToString());
                    await GetFacebookUserNameAsync(WebAuthenticationResult.ResponseData.ToString());
                    isFacebookUserLoggedIn = true;
                }
                else if (WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
                {
                    OutputText("HTTP Error returned by AuthenticateAsync() : " + WebAuthenticationResult.ResponseErrorDetail.ToString());
                }
                else
                {
                    OutputText("Error returned by AuthenticateAsync() : " + WebAuthenticationResult.ResponseStatus.ToString());
                }

            }
            catch (Exception Error)
            {
                //
                // Bad Parameter, SSL/TLS Errors and Network Unavailable errors are to be handled here.
                //
                DebugPrint(Error.ToString());
            }

            
        }

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
                        access_token = splits[1];
                        break;
                    case "expires_in":
                        expires_in = splits[1];
                        break;
                }
            }

            roamingSettings.Values[FACEBOOK_OAUTH_TOKEN] = access_token; //store access token locally for further use.
            DebugPrint("access_token = " + access_token);

            //Request User info.
            HttpClient httpClient = new HttpClient();
            string response = await httpClient.GetStringAsync(new Uri("https://graph.facebook.com/me?access_token=" + access_token));
            JsonObject value = JsonValue.Parse(response).GetObject();

            facebookUserName = value.GetNamedString("name");
            roamingSettings.Values[FACEBOOK_USER_NAME] = facebookUserName; //store user name locally for further use.
            rootPage.NotifyUser(facebookUserName + " is connected!!", NotifyType.StatusMessage);

        }

        #endregion 

        #region Twitter authentication and related functions
        private async Task AuthenticateToTwitterAsync()
        {
            if (TwitterClientID.Text == "")
            {
                rootPage.NotifyUser("Please enter an Client ID.", NotifyType.StatusMessage);
                return;
            }
            else if (TwitterCallbackUrl.Text == "")
            {
                rootPage.NotifyUser("Please enter an Callback URL.", NotifyType.StatusMessage);
                return;
            }
            else if (TwitterClientSecret.Text == "")
            {
                rootPage.NotifyUser("Please enter an Client Secret.", NotifyType.StatusMessage);
                return;
            }

            try
            {
                string oauth_token = await GetTwitterRequestTokenAsync(TwitterCallbackUrl.Text, TwitterClientID.Text);
                string TwitterUrl = "https://api.twitter.com/oauth/authorize?oauth_token=" + oauth_token;
                System.Uri StartUri = new Uri(TwitterUrl);
                System.Uri EndUri = new Uri(TwitterCallbackUrl.Text);
                                
                WebAuthenticationResult WebAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(
                                                        WebAuthenticationOptions.None,
                                                        StartUri,
                                                        EndUri);
                if (WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
                {
                    OutputText(WebAuthenticationResult.ResponseData.ToString());
                    await GetTwitterUserNameAsync(WebAuthenticationResult.ResponseData.ToString());
                    isTwitterUserLoggedIn = true;
                }
                else if (WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
                {
                    OutputText("HTTP Error returned by AuthenticateAsync() : " + WebAuthenticationResult.ResponseErrorDetail.ToString());
                }
                else
                {
                    OutputText("Error returned by AuthenticateAsync() : " + WebAuthenticationResult.ResponseStatus.ToString());
                }


            }
            catch (Exception Error)
            {
                //
                // Bad Parameter, SSL/TLS Errors and Network Unavailable errors are to be handled here.
                //
                DebugPrint(Error.ToString());
            }
        }

        /// <summary>
        /// This function extracts oauth_token and oauth_verifier from the response returned from web authentication broker
        /// and uses that token to get Twitter access token. 
        /// </summary>
        /// <param name="webAuthResultResponseData">responseData returned from AuthenticateAsync result.</param>
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
            string authorizationHeaderParams = "oauth_consumer_key=\"" + TwitterClientID.Text + "\", oauth_nonce=\"" + nonce + "\", oauth_signature_method=\"HMAC-SHA1\", oauth_signature=\"" + Uri.EscapeDataString(Signature) + "\", oauth_timestamp=\"" +timeStamp + "\", oauth_token=\"" + Uri.EscapeDataString(request_token) + "\", oauth_version=\"1.0\"";

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

            if (access_token != null)
            {
                roamingSettings.Values[TWITTER_OAUTH_TOKEN] = access_token; //store access token for further use.
                DebugPrint("access_token = " + access_token);
            }

            if (oauth_token_secret != null)
            {
                roamingSettings.Values[TWITTER_OAUTH_TOKEN_SECRET] = oauth_token_secret; //store token secret for further use.
                DebugPrint("oauth_token_secret = " + oauth_token_secret);
            }

            if (screen_name != null)
            {
                twitterUserName = screen_name;
                roamingSettings.Values[TWITTER_USER_NAME] = twitterUserName; //Store user name locally for further use.
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

        #endregion 


        private void OutputText(string text)
        {
            OutPutTextArea.Text = text + "\r\n";
        }

        private void DebugPrint(String Trace)
        {
            OutPutTextArea.Text += Trace + "\r\n";
        }
    }
}
