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
using Windows.Security.Credentials.UI;

namespace CredentialPicker
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario2 : SDKTemplate.Common.LayoutAwarePage
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

                void SetError(string err)
        {
            rootPage.NotifyUser(err, NotifyType.ErrorMessage);
        }

        void SetResult(CredentialPickerResults res)
        {
            TextBox status = rootPage.FindName("Status") as TextBox;
            if (res.ErrorCode == 0)
            {
                status.Text = String.Format("OK");
            }
            else
            {
                status.Text = String.Format("Error returned from CredPicker API: {0}", res.ErrorCode);
            }
            TextBox domain = rootPage.FindName("Domain") as TextBox;
            domain.Text = res.CredentialDomainName;
            TextBox username = rootPage.FindName("Username") as TextBox;
            username.Text = res.CredentialUserName;
            TextBox password = rootPage.FindName("Password") as TextBox;
            password.Text = res.CredentialPassword;
            TextBox credsaved = rootPage.FindName("CredentialSaved") as TextBox;
            credsaved.Text = (res.CredentialSaved ? "true" : "false");
            TextBox checkboxState = rootPage.FindName("CheckboxState") as TextBox;
            switch (res.CredentialSaveOption)
            {
                case CredentialSaveOption.Hidden:
                    checkboxState.Text = "Hidden";
                    break;
                case CredentialSaveOption.Selected:
                    checkboxState.Text = "Selected";
                    break;
                case CredentialSaveOption.Unselected:
                    checkboxState.Text = "Unselected";
                    break;
            }
        }


        #region Click Handlers
        private async void Launch_Click(object sender, RoutedEventArgs e)
        {
            if ((Target.Text != "") && (Message.Text != "") && (Caption.Text != ""))
            {
                var credPickerResults = await Windows.Security.Credentials.UI.CredentialPicker.PickAsync(Target.Text, Message.Text, Caption.Text);
                SetResult(credPickerResults);
            }
        }

        #endregion

    }
}
