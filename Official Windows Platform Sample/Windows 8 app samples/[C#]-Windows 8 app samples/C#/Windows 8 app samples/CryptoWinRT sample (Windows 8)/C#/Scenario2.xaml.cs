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
    public sealed partial class Scenario2 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario2()
        {
            this.InitializeComponent();
            AlgorithmNames.SelectedIndex = 0;
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
        /// This is the click handler for the 'RunSample' button.  It is responsible for executing the sample code.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RunSample_Click(object sender, RoutedEventArgs e)
        {
            Scenario2Text.Text = "";
            String algName = AlgorithmNames.SelectionBoxItem.ToString();

            // Create a HashAlgorithmProvider object.
            HashAlgorithmProvider Algorithm = HashAlgorithmProvider.OpenAlgorithm(algName);
            IBuffer vector = CryptographicBuffer.DecodeFromBase64String("uiwyeroiugfyqcajkds897945234==");

            Scenario2Text.Text += "\n*** Sample Hash Algorithm: " + Algorithm.AlgorithmName + "\n";
            Scenario2Text.Text += "    Initial vector:  uiwyeroiugfyqcajkds897945234==\n";

            // Compute the hash in one call.
            IBuffer digest = Algorithm.HashData(vector);

            if (digest.Length != Algorithm.HashLength)
            {
                Scenario2Text.Text += "HashAlgorithmProvider failed to generate a hash of proper length!\n";
                return;
            }

            Scenario2Text.Text += "    Hash:  " + CryptographicBuffer.EncodeToHexString(digest) + "\n";

            // Use a reusable hash object to hash the data by using multiple calls.
            CryptographicHash reusableHash = Algorithm.CreateHash();

            reusableHash.Append(vector);

            // Note that calling GetValue resets the data that has been appended to the
            // CryptographicHash object.
            IBuffer digest2 = reusableHash.GetValueAndReset();

            if (!CryptographicBuffer.Compare(digest, digest2))
            {
                Scenario2Text.Text += "CryptographicHash failed to generate the same hash data!\n";
                return;
            }

            reusableHash.Append(vector);
            digest2 = reusableHash.GetValueAndReset();

            if (!CryptographicBuffer.Compare(digest, digest2))
            {
                Scenario2Text.Text += "Reusable CryptographicHash failed to generate the same hash data!\n";
                return;
            }
        }

    }
}
