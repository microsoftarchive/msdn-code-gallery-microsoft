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
    public sealed partial class Hashing : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;
        IBuffer digest;

        public Hashing()
        {
            this.InitializeComponent();
            bHash.IsChecked = true;
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
            IBuffer vector = CryptographicBuffer.DecodeFromBase64String("uiwyeroiugfyqcajkds897945234==");
            HashingText.Text = "";

            // Use a reusable hash object to hash the data by using multiple calls.
            CryptographicHash reusableHash;
            if (bHmac.IsChecked.Value)
            {
                reusableHash = CreateHmacCryptographicHash();
            }
            else
            {
                reusableHash = CreateHashCryptographicHash();
            }

            if (reusableHash == null)
                return;

            reusableHash.Append(vector);

            // Note that calling GetValue resets the data that has been appended to the
            // CryptographicHash object.
            IBuffer digest2 = reusableHash.GetValueAndReset();

            if (!CryptographicBuffer.Compare(digest, digest2))
            {
                HashingText.Text += "CryptographicHash failed to generate the same hash data!\n";
                return;
            }

            reusableHash.Append(vector);
            digest2 = reusableHash.GetValueAndReset();

            if (!CryptographicBuffer.Compare(digest, digest2))
            {
                HashingText.Text += "Reusable CryptographicHash failed to generate the same hash data!\n";
                return;
            }
        }

        private CryptographicHash CreateHashCryptographicHash()
        {
            String algName = AlgorithmNames.SelectionBoxItem.ToString();

            // Create a HashAlgorithmProvider object.
            HashAlgorithmProvider Algorithm = HashAlgorithmProvider.OpenAlgorithm(algName);
            IBuffer vector = CryptographicBuffer.DecodeFromBase64String("uiwyeroiugfyqcajkds897945234==");

            HashingText.Text += "\n*** Sample Hash Algorithm: " + Algorithm.AlgorithmName + "\n";
            HashingText.Text += "    Initial vector:  uiwyeroiugfyqcajkds897945234==\n";

            // Compute the hash in one call.
            digest = Algorithm.HashData(vector);

            if (digest.Length != Algorithm.HashLength)
            {
                HashingText.Text += "HashAlgorithmProvider failed to generate a hash of proper length!\n";
                return null;
            }

            HashingText.Text += "    Hash:  " + CryptographicBuffer.EncodeToHexString(digest) + "\n";

            return Algorithm.CreateHash();
        }

        private CryptographicHash CreateHmacCryptographicHash()
        {
            String algName = AlgorithmNames.SelectionBoxItem.ToString();

            // Create a HashAlgorithmProvider object.
            MacAlgorithmProvider Algorithm = MacAlgorithmProvider.OpenAlgorithm(algName);
            IBuffer vector = CryptographicBuffer.DecodeFromBase64String("uiwyeroiugfyqcajkds897945234==");

            HashingText.Text += "\n*** Sample Hash Algorithm: " + Algorithm.AlgorithmName + "\n";
            HashingText.Text += "    Initial vector:  uiwyeroiugfyqcajkds897945234==\n";

            IBuffer hmacKeyMaterial = CryptographicBuffer.GenerateRandom(Algorithm.MacLength);

            // Compute the hash in one call.
            CryptographicHash hash = Algorithm.CreateHash(hmacKeyMaterial);
            hash.Append(vector);
            digest = hash.GetValueAndReset();

            HashingText.Text += "    Hash:  " + CryptographicBuffer.EncodeToHexString(digest) + "\n";

            return Algorithm.CreateHash(hmacKeyMaterial);
        }

        private void bHash_Checked(object sender, RoutedEventArgs e)
        {
            AlgorithmNames.Items.Clear();
            AlgorithmNames.Items.Add(HashAlgorithmNames.Md5);
            AlgorithmNames.Items.Add(HashAlgorithmNames.Sha1);
            AlgorithmNames.Items.Add(HashAlgorithmNames.Sha256);
            AlgorithmNames.Items.Add(HashAlgorithmNames.Sha384);
            AlgorithmNames.Items.Add(HashAlgorithmNames.Sha512);
            AlgorithmNames.SelectedIndex = 0;
        }

        private void bHmac_Checked(object sender, RoutedEventArgs e)
        {
            AlgorithmNames.Items.Clear();
            AlgorithmNames.Items.Add(MacAlgorithmNames.AesCmac);
            AlgorithmNames.Items.Add(MacAlgorithmNames.HmacMd5);
            AlgorithmNames.Items.Add(MacAlgorithmNames.HmacSha1);
            AlgorithmNames.Items.Add(MacAlgorithmNames.HmacSha256);
            AlgorithmNames.Items.Add(MacAlgorithmNames.HmacSha384);
            AlgorithmNames.Items.Add(MacAlgorithmNames.HmacSha512);
            AlgorithmNames.SelectedIndex = 0;
        }

    }
}
