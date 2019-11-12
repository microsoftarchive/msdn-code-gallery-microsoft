//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Security.Cryptography.DataProtection;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;

namespace CryptoWinRT
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario9 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario9()
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
        /// 
        /// </summary>
        /// <param name="descriptor">The descriptor string used to protect the data</param>
        public async void SampleDataProtection(String descriptor)
        {
            Scenario9Text.Text += "*** Sample Data Protection for " + descriptor + " ***\n";

            DataProtectionProvider Provider = new DataProtectionProvider(descriptor);
            Scenario9Text.Text += "    DataProtectionProvider is Ready\n";

            // Create random data for protection
            IBuffer data = CryptographicBuffer.GenerateRandom(73);
            Scenario9Text.Text += "    Original Data: " + CryptographicBuffer.EncodeToHexString(data) + "\n";

            // Protect the random data
            IBuffer protectedData = await Provider.ProtectAsync(data);
            Scenario9Text.Text += "    Protected Data: " + CryptographicBuffer.EncodeToHexString(protectedData) + "\n";

            if (CryptographicBuffer.Compare(data, protectedData))
            {
                Scenario9Text.Text += "ProtectAsync returned unprotected data";
                return;
            }

            Scenario9Text.Text += "    ProtectAsync succeeded\n";

            // Unprotect
            DataProtectionProvider Provider2 = new DataProtectionProvider();
            IBuffer unprotectedData = await Provider2.UnprotectAsync(protectedData);

            if (!CryptographicBuffer.Compare(data, unprotectedData))
            {
                Scenario9Text.Text += "UnprotectAsync returned invalid data";
                return;
            }

            Scenario9Text.Text += "    Unprotected Data: " + CryptographicBuffer.EncodeToHexString(unprotectedData) + "\n";
            Scenario9Text.Text += "*** Done!\n";
        }

        /// <summary>
        /// This is the click handler for the 'Default' button.  You would replace this with your own handler
        /// if you have a button or buttons on this page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RunSample_Click(object sender, RoutedEventArgs e)
        {
            String descriptor = tbDescriptor.Text;
            SampleDataProtection(descriptor);
        }
    }
}
