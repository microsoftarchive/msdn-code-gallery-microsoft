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
using Windows.Web.Http.Filters;

namespace WebAuthentication
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario6 : Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;
        HttpClient autoPickerHttpClient = null;
        private AuthFilters.SwitchableAuthFilter autoPicker;

        public Scenario6()
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

        HttpClient GetAutoPickerHttpClient(string clientId)
        {
            if (autoPickerHttpClient == null)
            {
                var bpf = new HttpBaseProtocolFilter();
                autoPicker = new AuthFilters.SwitchableAuthFilter(bpf);
                //You can add multiple fiters (twitter, google etc) if you are connecting to more than one service.
                autoPicker.AddOAuth2Filter(MakeFacebook(clientId, bpf));
                autoPickerHttpClient = new HttpClient(autoPicker);
            }
            return autoPickerHttpClient;
        }

        private async void Launch_Click(object sender, RoutedEventArgs e)
        {
            if (FacebookClientID.Text == "")
            {
                rootPage.NotifyUser("Please enter an Client ID.", NotifyType.StatusMessage);
                return;
            }
         
            var uri = new Uri("https://graph.facebook.com/me");
            HttpClient httpClient = GetAutoPickerHttpClient(FacebookClientID.Text);
            
            DebugPrint("Getting data from facebook....");
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            try
            {
                var response = await httpClient.SendRequestAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    string userInfo = await response.Content.ReadAsStringAsync();
                    DebugPrint(userInfo);
                }
                else
                {
                    string str = "";
                    if (response.Content != null) 
                        str = await response.Content.ReadAsStringAsync();
                    DebugPrint("ERROR: " + response.StatusCode + " " + response.ReasonPhrase + "\r\n" + str);
                }
            }
            catch (Exception ex)
            {
                DebugPrint("EXCEPTION: " + ex.Message);
            }
        }


        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            if (autoPicker != null)
            {
                autoPicker.ClearAll();
            }
        }

        
        public static AuthFilters.OAuth2Filter MakeFacebook(string clientId, IHttpFilter innerFilter)
        {
            var f = new AuthFilters.OAuth2Filter(innerFilter);
            var config = new AuthFilters.AuthConfigurationData();
            config.ClientId = clientId;

            config.TechnicalName = "facebook.com";
            config.ApiUriPrefix = "https://graph.facebook.com/";
            config.SampleUri = "https://graph.facebook.com/me";
            config.RedirectUri = "https://www.facebook.com/connect/login_success.html";
            config.ClientSecret = "";
            config.Scope = "read_stream";
            config.Display = "popup";
            config.State = "";
            config.AdditionalParameterName = "";
            config.AdditionalParameterValue = "";
            config.ResponseType = ""; // blank==default "token". null doesn't marshall. 
            config.AccessTokenLocation = ""; // blank=default "query";
            config.AccessTokenQueryParameterName = ""; // blank=default "access_token";
            config.AuthorizationUri = "https://www.facebook.com/dialog/oauth";
            config.AuthorizationCodeToTokenUri = "";
            f.AuthConfiguration = config;

            return f;
        }

        private void DebugPrint(String Trace)
        {
            FacebookDebugArea.Text += Trace + "\r\n";
        }

        private void OutputToken(String TokenUri)
        {
            FacebookDebugArea.Text = TokenUri;
        }

        
    }
}
