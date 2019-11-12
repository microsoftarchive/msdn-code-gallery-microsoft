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
    public sealed partial class Scenario3 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario3()
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

        void SetPasswordExplainVisibility(bool isShown)
        {
            TextBlock text1 = rootPage.FindName("PasswordExplain1") as TextBlock;
            TextBlock text2 = rootPage.FindName("PasswordExplain2") as TextBlock;
            if (isShown)
            {
                text1.Visibility = Windows.UI.Xaml.Visibility.Visible;
                text2.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                text1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                text2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        void SetResult(CredentialPickerResults res)
        {
            TextBox status = rootPage.FindName("Status") as TextBox;
            status.Text = String.Format("OK (Returned Error Code: {0})",res.ErrorCode);
            TextBox domain = rootPage.FindName("Domain") as TextBox;
            domain.Text = res.CredentialDomainName;
            TextBox username = rootPage.FindName("Username") as TextBox;
            username.Text = res.CredentialUserName;
            TextBox password = rootPage.FindName("Password") as TextBox;
            password.Text = res.CredentialPassword;
            TextBox credsaved = rootPage.FindName("CredentialSaved") as TextBox;
            credsaved.Text = (res.CredentialSaved ? "true" : "false");
            TextBox checkboxState = rootPage.FindName("CheckboxState2") as TextBox;
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

       
        private async void Launch_Click(object sender, RoutedEventArgs e)
        {
            if ((Target.Text == "") || (Message.Text == "") || (Caption.Text == ""))
            {
                return;
            }

            CredentialPickerOptions credPickerOptions = new CredentialPickerOptions();
            credPickerOptions.Message = Message.Text;
            credPickerOptions.Caption = Caption.Text;
            credPickerOptions.TargetName = Target.Text;
            credPickerOptions.AlwaysDisplayDialog = (AlwaysShowDialog.IsChecked == true);
            if (Protocol.SelectedItem == null)
            {
                //default protocol, if no selection
                credPickerOptions.AuthenticationProtocol = AuthenticationProtocol.Negotiate;
            }
            else
            {
                string selectedProtocol = ((ComboBoxItem)Protocol.SelectedItem).Content.ToString();
                if (selectedProtocol.Equals("Kerberos", StringComparison.CurrentCultureIgnoreCase))
                {
                    credPickerOptions.AuthenticationProtocol = AuthenticationProtocol.Kerberos;
                }
                else if (selectedProtocol.Equals("Negotiate", StringComparison.CurrentCultureIgnoreCase))
                {
                    credPickerOptions.AuthenticationProtocol = AuthenticationProtocol.Negotiate;
                }
                else if (selectedProtocol.Equals("NTLM", StringComparison.CurrentCultureIgnoreCase))
                {
                    credPickerOptions.AuthenticationProtocol = AuthenticationProtocol.Ntlm;
                }
                else if (selectedProtocol.Equals("CredSsp", StringComparison.CurrentCultureIgnoreCase))
                {
                    credPickerOptions.AuthenticationProtocol = AuthenticationProtocol.CredSsp;
                }
                else if (selectedProtocol.Equals("Basic", StringComparison.CurrentCultureIgnoreCase))
                {
                    credPickerOptions.AuthenticationProtocol = AuthenticationProtocol.Basic;
                }
                else if (selectedProtocol.Equals("Digest", StringComparison.CurrentCultureIgnoreCase))
                {
                    credPickerOptions.AuthenticationProtocol = AuthenticationProtocol.Digest;
                }
                else if (selectedProtocol.Equals("Custom", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (CustomProtocol.Text != null && CustomProtocol.Text != String.Empty)
                    {
                        credPickerOptions.AuthenticationProtocol = AuthenticationProtocol.Custom;
                        credPickerOptions.CustomAuthenticationProtocol = CustomProtocol.Text;
                    }
                    else
                    {
                        rootPage.NotifyUser("Please enter a custom protocol", NotifyType.ErrorMessage);
                    }
                }
            }

            if (CheckboxState.SelectedItem != null)
            {
                string checkboxState = ((ComboBoxItem)CheckboxState.SelectedItem).Content.ToString();
                if (checkboxState.Equals("Hidden",StringComparison.CurrentCultureIgnoreCase))
                {
                    credPickerOptions.CredentialSaveOption = CredentialSaveOption.Hidden;
                }
                else if (checkboxState.Equals("Selected", StringComparison.CurrentCultureIgnoreCase))
                {
                    credPickerOptions.CredentialSaveOption = CredentialSaveOption.Selected;
                }
                else if (checkboxState.Equals("Unselected", StringComparison.CurrentCultureIgnoreCase))
                {
                    credPickerOptions.CredentialSaveOption = CredentialSaveOption.Unselected;
                }
            }

            var credPickerResults = await Windows.Security.Credentials.UI.CredentialPicker.PickAsync(credPickerOptions);
            SetResult(credPickerResults);

        }

        private void Protocol_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox box = sender as ComboBox;
            if (Protocol == null || Protocol.SelectedItem == null)
            {
                //return if this was triggered before all components are initialized
                return;
            }
            
            string selectedProtocol = ((ComboBoxItem)Protocol.SelectedItem).Content.ToString();
            
            if (selectedProtocol.Equals("Custom", StringComparison.CurrentCultureIgnoreCase))
            {
                CustomProtcolStackPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                CustomProtcolStackPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                //Basic and Digest return plaintext passwords
                if (selectedProtocol.Equals("Basic",StringComparison.CurrentCultureIgnoreCase))
                {
                    SetPasswordExplainVisibility(false);
                }
                else if (selectedProtocol.Equals("Digest",StringComparison.CurrentCultureIgnoreCase))
                {
                    SetPasswordExplainVisibility(false);
                }
                else
                {
                    SetPasswordExplainVisibility(true);
                }
            }
        }


    }
}
