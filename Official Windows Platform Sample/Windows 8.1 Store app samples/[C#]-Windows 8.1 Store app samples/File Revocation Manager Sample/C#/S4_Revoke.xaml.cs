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
using Windows.Security.EnterpriseData;

namespace FileRevocation
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class S4_Revoke : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage RootPage = MainPage.Current;

        public S4_Revoke()
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

        /// <summary>
        /// Revoke the enterprise id that user entered
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Revoke_Click(object sender, RoutedEventArgs e)
        {
            if ("" == InputTextBox.Text)
            {
                RootPage.NotifyUser("Please enter an Enterpise ID that you want to use.", NotifyType.ErrorMessage);
                return;
            }

            try
            {
                FileRevocationManager.Revoke(InputTextBox.Text);
                RootPage.NotifyUser("The Enterprise ID " + InputTextBox.Text + " was revoked. The files protected by it will not be accessible anymore.", NotifyType.StatusMessage);
            }

            //
            // NOTE: Generally you should not rely on exception handling
            // to validate an Enterprise ID string. In real-world
            // applications, the domain name of the enterprise might be
            // parsed out of an email address or a URL, and may even be
            // entered by a user. Your app-specific code to extract the
            // Enterprise ID should validate the Enterprise ID string is an
            // internationalized domain name before passing it to
            // Revoke.
            //

            catch (ArgumentException)
            {
                RootPage.NotifyUser("Given Enterprise ID string is invalid.\n" +
                                    "Please try again using a properly formatted Internationalized Domain Name as the Enterprise ID string.",
                                    NotifyType.ErrorMessage);
            }
        }
    }
}
