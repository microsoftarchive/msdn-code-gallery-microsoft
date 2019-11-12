//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using Windows.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Net.Http;
using Expression.Blend.SampleData.SampleDataSource;
using SDKTemplate;
using Windows.Data.Json;
using Windows.Security.Authentication.OnlineId;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;


namespace MicrosoftAccount
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario01_DelegationToken : Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;
        OnlineIdAuthenticator _authenticator;
        Uri _unknownUserUri = new Uri("ms-appx:///Images/user.png");
        string _accessToken;
        static string URI_API_LIVE = "https://apis.live.net/v5.0/";
        static string URI_PICTURE = URI_API_LIVE + "me/picture?access_token=";
        static string URI_CONTACTS = URI_API_LIVE + "me/contacts?access_token=";
        static string URI_USER_INFO = URI_API_LIVE + "me?access_token=";

        public Boolean NeedsToGetTicket
        {
            get;
            private set;
        }

        public Boolean CanSignOut
        {
            get { return _authenticator.CanSignOut; }
        }

        public string AccessToken
        {
            get
            {
                return _accessToken;
            }

            private set
            {
                if (_accessToken != value)
                {
                    _accessToken = value;

                    if (value != null)
                    {
                        UserPicture.Source = new BitmapImage(new Uri(Scenario01_DelegationToken.URI_PICTURE + value));

                        GetUserInfo(value);
                        GetUserContacts(value);
                    }
                    else
                    {
                        cvs1.Source = null;
                        UserName.Text = "";
                        UserPicture.Source = new BitmapImage(_unknownUserUri);
                    }
                }
            }
        }

        public CredentialPromptType PromptType
        {
            get;
            private set;
        }

        public Scenario01_DelegationToken()
        {
            this.InitializeComponent();

            _authenticator = new OnlineIdAuthenticator();

            PromptType = CredentialPromptType.PromptIfNeeded;
            SignInOptions.SelectedItem = PromptIfNeeded;
            AccessToken = null;
            NeedsToGetTicket = true;
            SignOutButton.Visibility = CanSignOut ? Visibility.Visible : Visibility.Collapsed;
        }

        private void DebugPrint(String trace)
        {
            DebugArea.Text = trace;
        }

        private void SignInOptions_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (SignInOptions.SelectedItem == PromptIfNeeded)
            {
                PromptType = CredentialPromptType.PromptIfNeeded;
            }
            else if (SignInOptions.SelectedItem == DontPrompt)
            {
                PromptType = CredentialPromptType.DoNotPrompt;
            }
            else if (SignInOptions.SelectedItem == RetypeCredentials)
            {
                PromptType = CredentialPromptType.RetypeCredentials;
            }

            SignInButton.Visibility = Visibility.Visible;
        }

        private async void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            var targetArray = new List<OnlineIdServiceTicketRequest>();
            targetArray.Add(new OnlineIdServiceTicketRequest("wl.basic wl.contacts_photos wl.calendars", "DELEGATION"));

            AccessToken = null;
            NeedsToGetTicket = true;

            DebugPrint("Signing in...");

            try
            {
                var result = await _authenticator.AuthenticateUserAsync(targetArray, PromptType);
                if (result.Tickets[0].Value != string.Empty)
                {
                    DebugPrint("Signed in.");

                    NeedsToGetTicket = false;
                    AccessToken = result.Tickets[0].Value;
                }
                else
                {
                    // errors are to be handled here.
                    DebugPrint("Unable to get the ticket. Error: " + result.Tickets[0].ErrorCode.ToString());
                }
            }
            catch (System.OperationCanceledException)
            {
                // errors are to be handled here.
                DebugPrint("Operation canceled.");

            }
            catch (System.Exception ex)
            {
                // errors are to be handled here.
                DebugPrint("Unable to get the ticket. Exception: " + ex.Message);
            }

            SignInButton.Visibility = NeedsToGetTicket ? Visibility.Visible : Visibility.Collapsed;
        }

        private async void SignOutButton_Click(object sender, RoutedEventArgs e)
        {
            if (CanSignOut)
            {
                DebugPrint("Signing out...");
                await _authenticator.SignOutUserAsync();

                AccessToken = null;
                DebugPrint("Signed out.");
            }

            NeedsToGetTicket = true;
            SignOutButton.Visibility = CanSignOut ? Visibility.Visible : Visibility.Collapsed;
        }

        public async void GetUserInfo(string token)
        {
            var uri = new Uri(Scenario01_DelegationToken.URI_USER_INFO + token);
            var client = new HttpClient();
            var result = await client.GetAsync(uri);
            
            string jsonUserInfo = await result.Content.ReadAsStringAsync();
            if (jsonUserInfo != null)
            {
                var json = JsonObject.Parse(jsonUserInfo);
                UserName.Text = "Welcome, " + json["name"].GetString();
            }
        }

        public async void GetUserContacts(string token)
        {
            Uri[] _localImages = 
            {
                new Uri("ms-appx:///Images/user.png"),
                new Uri("ms-appx:///SampleData/Images/60Mail01.png"),
                new Uri("ms-appx:///SampleData/Images/60Mail02.png"),
                new Uri("ms-appx:///SampleData/Images/60Mail03.png"),
                new Uri("ms-appx:///SampleData/Images/msg.png"),
            };

            var uri = new Uri(Scenario01_DelegationToken.URI_CONTACTS + token);
            var client = new HttpClient();
            var result = await client.GetAsync(uri);
            string jsonUserContacts = await result.Content.ReadAsStringAsync();
            if (jsonUserContacts != null)
            {
                var json = JsonObject.Parse(jsonUserContacts);
                var contacts = json["data"] as JsonValue;
                var storeData = new StoreData();
                int index = 0;

                foreach (JsonValue contact in contacts.GetArray())
                {
                    var obj = contact.GetObject();
                    var item = new Item();
                    item.Name = obj["name"].GetString();
                    item.Id = obj["id"].GetString();

                    if (obj["user_id"].ValueType == JsonValueType.String)
                    {
                        string userId = obj["user_id"].GetString();
                        item.SetImage(Scenario01_DelegationToken.URI_API_LIVE  + userId + "/picture?access_token=" + token);
                    }
                    else
                    {
                        item.Image = new BitmapImage(_localImages[index % _localImages.Length]);
                    }

                    storeData.Collection.Add(item);
                    index++;
                }
                cvs1.Source = storeData.GetGroupsByLetter();
            }
        }
    }
}