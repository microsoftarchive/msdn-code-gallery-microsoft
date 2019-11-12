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
using System.Threading.Tasks;




namespace PasswordVault
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

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario1 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario1()
        {
            this.InitializeComponent();
            // Initialize the password vault, this may take less than a second
            // An optimistic initialization at this stage improves the UI performance
            // when any other call to the password vault may be made later on
            InitializePasswordVault.Initialize();

        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }


        public void DebugPrint(String Trace)
        {
            TextBox ErrorMessage = rootPage.FindName("ErrorMessage") as TextBox;
            ErrorMessage.Text = DateTime.Now.ToString("HH:mm:ss tt") + " " + Trace + "\r\n" + ErrorMessage.Text;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            TextBox InputResourceValue = rootPage.FindName("InputResourceValue") as TextBox;
            TextBox InputUserNameValue = rootPage.FindName("InputUserNameValue") as TextBox;
            PasswordBox InputPasswordValue = rootPage.FindName("InputPasswordValue") as PasswordBox;
            String result = "";
            if (InputResourceValue.Text == "" || InputUserNameValue.Text == "" || InputPasswordValue.Password == "")
            {
                DebugPrint("Inputs are missing. Resource, User name and Password are required");
            }
            else
            {
                try
                {
                    //
                    //Add a credential to PasswordVault by supplying resource, username, and password
                    //
                    Windows.Security.Credentials.PasswordVault vault = new Windows.Security.Credentials.PasswordVault();
                    PasswordCredential cred = new PasswordCredential(InputResourceValue.Text, InputUserNameValue.Text, InputPasswordValue.Password);
                    vault.Add(cred);
                    //
                    //Output credential added to debug spew
                    //
                    result = "Credential saved successfully. " + "Resource: " + cred.Resource.ToString() + " Username: " + cred.UserName.ToString() + " Password: " + cred.Password.ToString() + ".";
                    DebugPrint(result.ToString());
                }
                catch (Exception Error) // No stored credentials, so none to delete
                {
                    DebugPrint(Error.Message);
                }
            }
    
        }

        private void Read_Click(object sender, RoutedEventArgs e)
        {
            TextBox InputResourceValue = rootPage.FindName("InputResourceValue") as TextBox;
            TextBox InputUserNameValue = rootPage.FindName("InputUserNameValue") as TextBox;
            String result = "";

            try
            {
                //
                //Read a credential from PasswordVault by supplying resource or username
                //
                Windows.Security.Credentials.PasswordVault vault = new Windows.Security.Credentials.PasswordVault();
                IReadOnlyList<PasswordCredential> creds = null;
                PasswordCredential cred = null;

                //
                //If both resource and username are empty, you can use RetrieveAll() to enumerate all credentials
                //
                if (InputUserNameValue.Text == "" && InputResourceValue.Text == "")
                {
                    DebugPrint("Retrieving all credentials since resource or username are not specified.");
                    creds = vault.RetrieveAll();
                }
                //
                //If there is only resouce, you can use FindAllByResource() to enumerate by resource. 
                //Note: the password will not be returned, you need to call retrieveAll with returned resouce and username to get the credential with password
                //
                else if (InputUserNameValue.Text == "")
                {
                    DebugPrint("Retrieve credentials by resouces that you provided");
                    creds = vault.FindAllByResource(InputResourceValue.Text);      
                }
                //
                //If there is only username, you can use findbyusername() to enumerate by resource. 
                //Note: the password will not be returned, you need to call retrieveAll with returned resouce and username to get the credential with password 
                //
                else if (InputResourceValue.Text == "")
                {
                    DebugPrint("Retrieve credentials by username that you provided"); 
                    creds = vault.FindAllByUserName(InputUserNameValue.Text);
                }
                //
                //Read by explicit resource and username name, result will be a single credential if it exists. Password will be returned.
                //
                else
                    cred = vault.Retrieve(InputResourceValue.Text, InputUserNameValue.Text);

                //
                //Output credential added to debug spew
                //
                if (creds != null)
                {
                    DebugPrint("There are " + creds.Count + " credential(s) found.");
                    foreach (PasswordCredential c in creds)
                    {
                        try
                        {
                            PasswordCredential c1 = vault.Retrieve(c.Resource.ToString(), c.UserName.ToString());
                            result = "Credential read successfully. " + "Resource: " + c.Resource.ToString() + ", " + "Username: " + c.UserName.ToString()  + "Password: " + c1.Password.ToString() + ".";
                            DebugPrint(result.ToString());
                        }
                        catch (Exception Error)
                        {
                            DebugPrint(Error.Message);
                        }
                    }
                }

                else if (cred != null)
                {
                    result = "Credential read successfully. " + "Resource: " + cred.Resource + ", " + "Username: " + cred.UserName  + "Password: " + cred.Password.ToString() + ".";
                    DebugPrint(result.ToString());
                }
                else
                {
                    result = "Credential not found.";
                    DebugPrint(result.ToString());
                }

                
            }
            catch (Exception Error) // No stored credentials, so none to delete
            {
                if (Error.HResult == -2147023728)
                    DebugPrint("Credential not found.");
                else
                    DebugPrint(Error.Message);
            }
            

        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            TextBox InputResourceValue = rootPage.FindName("InputResourceValue") as TextBox;
            TextBox InputUserNameValue = rootPage.FindName("InputUserNameValue") as TextBox;
            String result = "";
            if (InputResourceValue.Text == "" || InputUserNameValue.Text == "")
            {
                DebugPrint("Inputs are missing. Resource and Username are required.");
            }
            else
            {
                try
                {
                    //
                    //this is the code to remove a credentialt from PasswordVault by supplying resource or username
                    //
                    Windows.Security.Credentials.PasswordVault vault = new Windows.Security.Credentials.PasswordVault();
                    PasswordCredential cred = vault.Retrieve(InputResourceValue.Text, InputUserNameValue.Text);
                    vault.Remove(cred);
                    result = "Credential removed successfully. Resource: " + InputResourceValue.Text + " Username: " + InputUserNameValue.Text + ".";
                    DebugPrint(result.ToString());
                }
                catch (Exception Error) // No stored credentials, so none to delete
                {
                    DebugPrint(Error.Message);
                }
            }

        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TextBox InputResourceValue = rootPage.FindName("InputResourceValue") as TextBox;
                InputResourceValue.Text = "";
                TextBox InputUserNameValue = rootPage.FindName("InputUserNameValue") as TextBox;
                InputUserNameValue.Text = "";
                PasswordBox InputPasswordValue = rootPage.FindName("InputPasswordValue") as PasswordBox;
                InputPasswordValue.Password = "";
                TextBox ErrorMessage = rootPage.FindName("ErrorMessage") as TextBox;
                ErrorMessage.Text = "";

                Windows.Security.Credentials.PasswordVault vault = new Windows.Security.Credentials.PasswordVault();
                IReadOnlyList<PasswordCredential> creds = vault.RetrieveAll();
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

                DebugPrint("Scenario has been reset. All credentials are removed.");
            }
            catch (Exception Error)
            {
                //
                // Bad Parameter, Machine infor Unavailable errors are to be handled here.
                //
                DebugPrint(Error.Message);
            }
        }                
    }
}
