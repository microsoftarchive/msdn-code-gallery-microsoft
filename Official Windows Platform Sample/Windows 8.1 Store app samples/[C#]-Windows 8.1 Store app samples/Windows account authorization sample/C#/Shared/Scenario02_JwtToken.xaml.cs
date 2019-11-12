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
using System.Collections.Generic;
using SDKTemplate;
using Windows.Security.Authentication.OnlineId;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MicrosoftAccount
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario02_JwtToken : Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;
        OnlineIdAuthenticator _authenticator;

        private string _Token = string.Empty;
        public string Token
        {
            get
            {
                return this._Token;
            }

            set
            {
                if (this._Token != value)
                {
                    this._Token = value;

                    if (this._Token != null && this._Token != string.Empty)
                    {
                        GetUserInfo();
                    }
                }
            }
        }

        // returns true if there is no ticket obtained
        public Boolean NeedsToGetTicket
        {
            get;
            private set;
        }

        // returns true if Signed in and not a Connected Account
        public Boolean CanSignOut
        {
            get { return _authenticator.CanSignOut; }
        }

        public CredentialPromptType PromptType
        {
            get;
            private set;
        }

        public Scenario02_JwtToken()
        {
            this.InitializeComponent();

            _authenticator = new OnlineIdAuthenticator();

            PromptType = CredentialPromptType.PromptIfNeeded;
            SignInOptions.SelectedItem = PromptIfNeeded;

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
            targetArray.Add(new OnlineIdServiceTicketRequest("jsonwebtokensample.com", "JWT"));

            SignInButton.Visibility = Visibility.Collapsed;

            try
            {
                var result = await _authenticator.AuthenticateUserAsync(targetArray, PromptType);

                if (result.Tickets[0].Value != string.Empty)
                {
                    DebugPrint("Signed in.");
                    NeedsToGetTicket = false;

                    Token = result.Tickets[0].Value;
                }
                else
                {
                    // errors are to be handled here.
                    DebugPrint("Unable to get the ticket. Error: " + result.Tickets[0].ErrorCode.ToString());
                }
            }
            catch (System.Exception ex)
            {
                // errors are to be handled here.
                DebugPrint("Unable to get the ticket. Exception: " + ex.Message);
            }

            NeedsToGetTicket = Token == null || Token.Length == 0;
            SignInButton.Visibility = NeedsToGetTicket ? Visibility.Visible : Visibility.Collapsed;
        }

        private async void SignOutButton_Click(object sender, RoutedEventArgs e)
        {
            if (CanSignOut)
            {
                DebugPrint("Signin out...");
                await _authenticator.SignOutUserAsync();

                Token = null;

                DebugPrint("Signed out.");
            }

            NeedsToGetTicket = (Token == null || Token.Length == 0);

            // Connected Account can't sign out from applications
            SignOutButton.Visibility = CanSignOut ? Visibility.Visible : Visibility.Collapsed;
        }

        private void SendTicketButton_Click(object sender, RoutedEventArgs e)
        {
            GetUserInfo();
        }

        private void GetUserInfo()
        {
            DebugArea.Text = "Signed In!\nTicket succesfully returned from server.\n\n" + this._Token;
        }
    }
}