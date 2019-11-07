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
using Windows.Security.Credentials;
using System.Linq;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.Storage;

namespace PasswordVault
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

            try
            {
                // SettingsPane is the existing  Win8 API for the Settings Contract
                SettingsPane.GetForCurrentView().CommandsRequested += MainPage_CommandsRequested;
                //Adding AccountControl callback
                AccountsSettingsPane.GetForCurrentView().AccountCommandsRequested += MainPage_AccountCommandsRequested;
            }
            catch (Exception Error) // No stored credentials, so none to delete
            {
                DebugPrint(Error.Message);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            AccountsSettingsPane.GetForCurrentView().AccountCommandsRequested -= MainPage_AccountCommandsRequested; 
        }

        void MainPage_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            try
            {
                // Callback invoked to request the app for setting commands when the settings flyout is about to be displayed
                //Add Custom commands
                UICommandInvokedHandler handler = new UICommandInvokedHandler(bla);

                SettingsCommand generalCommand = new SettingsCommand(
                    "generalSettings", "General", handler);
                args.Request.ApplicationCommands.Add(generalCommand);


                SettingsCommand helpCommand = new SettingsCommand("helpPage", "Help", handler);
                args.Request.ApplicationCommands.Add(helpCommand);
                args.Request.ApplicationCommands.Add(SettingsCommand.AccountsCommand); //This will show the “Accounts” command when the “Settings” pane is displayed
            }
            catch (Exception Error) // No stored credentials, so none to delete
            {
                DebugPrint(Error.Message);
            }

        }

        private void bla(IUICommand command)
        {
            DebugPrint("Specific cmd is not implemented, add yours");
            throw new NotImplementedException();
        }

        void MainPage_AccountCommandsRequested(AccountsSettingsPane sender, AccountsSettingsPaneCommandsRequestedEventArgs args)
        {
         
            // Callback invoked to request the app for accounts when the accounts flyout is about to be displayed
            // Get the Deferral object and do some async operation if needed           
            var Deferral = args.GetDeferral();

            // do some async operation 
            //Uri uri = new Uri("ms-appx:///Assets/Smalllogo.png");
            //StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(uri);

  
            // Add CredLocker Credentials     
            CredentialCommandCredentialDeletedHandler credDeletedHandler = new CredentialCommandCredentialDeletedHandler(CredentialDeletedHandler);

            Windows.Security.Credentials.PasswordVault vault = new Windows.Security.Credentials.PasswordVault();
            IReadOnlyList<PasswordCredential> creds = vault.RetrieveAll();

            if (creds.Count == 0)
                args.HeaderText = "There is not credential saved by the sample app, please go to Scenario 1 and add some credential, then try again.";
            else
                args.HeaderText = "Here are the credentials saved by sample app in Scenario 1.";

            foreach (PasswordCredential c in creds)
            {
                try
                {
                    CredentialCommand credCommand1 = new CredentialCommand(c, credDeletedHandler);
                    // Deleted is invoked after the system deletes the credential
                    args.CredentialCommands.Add(credCommand1);
                }
                catch (Exception Error) // Stored credential was deleted
                {
                    DebugPrint(Error.ToString());
                }
            }

            try
            {
                // Add Global commands     
                Object commandID = 1;
                UICommandInvokedHandler appCmdInvokedHandler = new UICommandInvokedHandler(CommandInvokedHandler);

                // SettingsCommand is an existing WinRT class used in the SettingsPane
                SettingsCommand command = new SettingsCommand(
                                                    commandID,
                                                    "App Specific Command Label...",
                                                    appCmdInvokedHandler);
                args.Commands.Add(command);
                // Add more commands here
            }
            catch (Exception Error) // No stored credentials, so none to delete
            {
                DebugPrint(Error.Message);
            }

            // Complete the Deferral()
            Deferral.Complete();
        }


        void CredentialDeletedHandler(CredentialCommand sender)
        {
            try
            {
                // callback invoked when the user requests deletion of the credential in the accounts flyout
                // System will invoke this callback after successfully deleting the credential from the vault
                // do post delete work 
                // show accounts settings pane to reflect the new state
                AccountsSettingsPane.Show();
                DebugPrint("Your credential is removed from PasswordVault");
            }
            catch (Exception Error) // No stored credentials, so none to delete
            {
                DebugPrint(Error.Message);
            }
        }


        void CommandInvokedHandler(IUICommand command) // IUICommand is an existing interface
        {
            // callback invoked when the user selects one of the app provided commands shown as links below the accounts
            // do some work based on command id
            if (command.Id.Equals(1))
            {
                DebugPrint("Please implement customized commands");
            }

        }


        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>

        private void DebugPrint(String Trace)
        {
            TextBox ErrorMessage = rootPage.FindName("ErrorMessage") as TextBox;
            ErrorMessage.Text += Trace + "\r\n";
        }

        private void Manage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AccountsSettingsPane.Show();
            }
            catch (Exception Error) // No stored credentials, so none to delete
            {
                DebugPrint(Error.Message);
            }
        }


   

    }
}
