// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
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
using Windows.Security.Credentials;
using SDKTemplateCS;

namespace SDKTemplateCS
{
    public abstract class InitializePasswordVault
    {
        public static void Initialize()
        {
            Task.Factory.StartNew(() => { InitializePasswordVault.LoadPasswordVault(); });
        }

        private static void LoadPasswordVault()
        {
            // any call to the password vault will load the vault
            Windows.Security.Credentials.PasswordVault vault = new Windows.Security.Credentials.PasswordVault();
            vault.RetrieveAll();
        }
    }
    public sealed partial class ScenarioInput1 : Page
    {
        // A pointer back to the main page which is used to gain access to the input and output frames and their content.
        MainPage rootPage = null;

        public ScenarioInput1()
        {
            InitializeComponent();
            // Initialize the password vault, this may take less than a second
            // An optimistic initialization at this stage improves the UI performance
            // when any other call to the password vault may be made later on
            InitializePasswordVault.Initialize();
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
        #endregion

        public void DebugPrint(String Trace)
        {
            Page outputFrame = (Page)rootPage.OutputFrame.Content;
            TextBox ErrorMessage = outputFrame.FindName("ErrorMessage") as TextBox;
            ErrorMessage.Text += Trace + "\r\n";
        }

        private void Launch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Page outputFrame = (Page)rootPage.OutputFrame.Content;
                Page inputFrame = (Page)rootPage.InputFrame.Content;
                Reset1();
                CheckBox AuthenticationFailCheck = outputFrame.FindName("AuthenticationFailCheck") as CheckBox;
                TextBox WelcomeMessage = inputFrame.FindName("WelcomeMessage") as TextBox;
                if (AuthenticationFailCheck.IsChecked == true)
                    WelcomeMessage.Text = "Blocked";
                else
                {
                    PasswordCredential cred = new PasswordCredential();
                    Windows.Security.Credentials.PasswordVault vault = new Windows.Security.Credentials.PasswordVault();
                    IReadOnlyList<PasswordCredential> Creds = vault.FindAllByResource("Scenario 1");

                    foreach (PasswordCredential c in Creds)
                    {
                        try
                        {
                            vault.Remove(c);
                        }
                        catch (Exception Error) // Stored credential was deleted
                        {
                            DebugPrint(Error.ToString());
                        }
                    }

                    WelcomeMessage.Text = "Scenario is ready, please sign in";
                }

            }
            catch (Exception Error)
            {
                //
                // Bad Parameter, Machine infor Unavailable errors are to be handled here.
                //
                DebugPrint(Error.ToString());
            }
        }

        private void ChangeUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Windows.Security.Credentials.PasswordVault vault = new Windows.Security.Credentials.PasswordVault();
                IReadOnlyList<PasswordCredential> creds = vault.FindAllByResource("Scenario 1");
                foreach (PasswordCredential c in creds)
                {
                    try
                    {
                        vault.Remove(c);
                    }
                    catch (Exception Error) // Stored credential was deleted
                    {
                        DebugPrint(Error.ToString());
                    }
                }
            }
            catch (Exception Error) // No stored credentials, so none to delete
            {
                DebugPrint(Error.ToString());
            }
            Reset1();
            Page outputFrame = (Page)rootPage.OutputFrame.Content;
            Page inputFrame = (Page)rootPage.InputFrame.Content;
            CheckBox AuthenticationFailCheck = outputFrame.FindName("AuthenticationFailCheck") as CheckBox;
            AuthenticationFailCheck.IsChecked = false;
            TextBox WelcomeMessage = inputFrame.FindName("WelcomeMessage") as TextBox;
            WelcomeMessage.Text = "User has been changed, please resign in with new credentials, choose save and launch scenario again";
        }

        private void Signin_Click(object sender, RoutedEventArgs e)
        {
            Page outputFrame = (Page)rootPage.OutputFrame.Content;
            Page inputFrame = (Page)rootPage.InputFrame.Content;
            TextBox InputUserNameValue = outputFrame.FindName("InputUserNameValue") as TextBox;
            PasswordBox InputPasswordValue = outputFrame.FindName("InputPasswordValue") as PasswordBox;
            TextBox WelcomeMessage = inputFrame.FindName("WelcomeMessage") as TextBox;
            CheckBox SaveCredCheck = outputFrame.FindName("SaveCredCheck") as CheckBox;
            if (InputUserNameValue.Text == "" || InputPasswordValue.Password == "")
            {
                TextBox ErrorMessage = outputFrame.FindName("ErrorMessage") as TextBox;
                ErrorMessage.Text = "User name and password are not allowed to be empty, Please input user name and password";
            }
            else
            {
                try
                {
                    Windows.Security.Credentials.PasswordVault vault = new Windows.Security.Credentials.PasswordVault();
                    PasswordCredential c = new PasswordCredential("Scenario 1", InputUserNameValue.Text, InputPasswordValue.Password);
                    if ((Boolean)SaveCredCheck.IsChecked)
                        vault.Add(c);

                    Reset1();
                    WelcomeMessage.Text = "Welcome to " + c.Resource + ", " + c.UserName;
                }
                catch (Exception Error) // No stored credentials, so none to delete
                {
                    DebugPrint(Error.ToString());
                }
            }
            Reset1();

            CheckBox AuthenticationFailCheck = outputFrame.FindName("AuthenticationFailCheck") as CheckBox;
            AuthenticationFailCheck.IsChecked = false;
        }

        private void Reset1()
        {

            try
            {
                Page outputFrame = (Page)rootPage.OutputFrame.Content;
                Page inputFrame = (Page)rootPage.InputFrame.Content;
                TextBox InputUserNameValue = outputFrame.FindName("InputUserNameValue") as TextBox;
                InputUserNameValue.Text = "";
                PasswordBox InputPasswordValue = outputFrame.FindName("InputPasswordValue") as PasswordBox;
                InputPasswordValue.Password= "";
                TextBox ErrorMessage = outputFrame.FindName("ErrorMessage") as TextBox;
                ErrorMessage.Text = "";
                TextBox WelcomeMessage = inputFrame.FindName("WelcomeMessage") as TextBox;
                CheckBox SaveCredCheck = outputFrame.FindName("SaveCredCheck") as CheckBox;
                SaveCredCheck.IsChecked = false;
            }
            catch (Exception Error)
            {
                //
                // Bad Parameter, Machine infor Unavailable errors are to be handled here.
                //
                DebugPrint(Error.ToString());
            }
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            Reset1();

        }                
    }
}
