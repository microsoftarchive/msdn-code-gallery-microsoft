// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using System.Linq;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Security.Credentials.UI;

namespace CredentialPickerCS
{
    public sealed partial class ScenarioInput3 : Page
    {
        // A pointer back to the main page which is used to gain access to the input and output frames and their content.
        MainPage rootPage = null;

        public ScenarioInput3()
        {
            InitializeComponent();
        }

        #region Template-Related Code - Do not remove
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Get a pointer to our main page
            rootPage = e.Parameter as MainPage;

            // We want to be notified with the OutputFrame is loaded so we can get to the content.
            rootPage.OutputFrameLoaded += new System.EventHandler(rootPage_OutputFrameLoaded);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            rootPage.OutputFrameLoaded -= new System.EventHandler(rootPage_OutputFrameLoaded);
        }
        #endregion

        #region Use this code if you need access to elements in the output frame - otherwise delete
        void rootPage_OutputFrameLoaded(object sender, object e)
        {
            // At this point, we know that the Output Frame has been loaded and we can go ahead
            // and reference elements in the page contained in the Output Frame.

            // Get a pointer to the content within the OutputFrame.
            Page outputFrame = (Page)rootPage.OutputFrame.Content;

            // Go find the elements that we need for this scenario.
            // ex: flipView1 = outputFrame.FindName("FlipView1") as FlipView;

        }

        void SetError(string err)
        {
            rootPage.NotifyUser(err, NotifyType.ErrorMessage);
        }

        void SetPasswordExplainVisibility(bool isShown)
        {
            Page outputFrame = (Page)rootPage.OutputFrame.Content;
            TextBlock text1 = outputFrame.FindName("PasswordExplain1") as TextBlock;
            TextBlock text2 = outputFrame.FindName("PasswordExplain2") as TextBlock;
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
            Page outputFrame = (Page)rootPage.OutputFrame.Content;
            TextBox status = outputFrame.FindName("Status") as TextBox;
            status.Text = String.Format("OK (Returned Error Code: {0})",res.ErrorCode);
            TextBox domain = outputFrame.FindName("Domain") as TextBox;
            domain.Text = res.CredentialDomainName;
            TextBox username = outputFrame.FindName("Username") as TextBox;
            username.Text = res.CredentialUserName;
            TextBox password = outputFrame.FindName("Password") as TextBox;
            password.Text = res.CredentialPassword;
            TextBox credsaved = outputFrame.FindName("CredentialSaved") as TextBox;
            credsaved.Text = (res.CredentialSaved ? "true" : "false");
            TextBox checkboxState = outputFrame.FindName("CheckboxState") as TextBox;
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
        #endregion

       
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

            var credPickerResults = await CredentialPicker.PickAsync(credPickerOptions);
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
