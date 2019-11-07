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
    public sealed partial class Scenario4 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario4()
        {
            this.InitializeComponent();
            AlgorithmNames.SelectedIndex = 0;
            KeySizes.SelectedIndex = 0;
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
            String algName = AlgorithmNames.SelectionBoxItem.ToString();
            IBuffer Secret = CryptographicBuffer.ConvertStringToBinary("Master key to derive from", BinaryStringEncoding.Utf8);
            UInt32 TargetSize = UInt32.Parse(KeySizes.SelectionBoxItem.ToString());
            Scenario4Text.Text = "";
            KeyDerivationParameters Params;

            if (algName.Contains("PBKDF2"))
            {
                // Password based key derivation function (PBKDF2).
                Params = KeyDerivationParameters.BuildForPbkdf2(
                                CryptographicBuffer.GenerateRandom(16),  // Salt
                                10000                       // PBKDF2 Iteration Count
                                );
            }
            else if (algName.Contains("SP800_108"))
            {
                // SP800_108_CTR_HMAC key derivation function.
                Params = KeyDerivationParameters.BuildForSP800108(
                                 CryptographicBuffer.ConvertStringToBinary("Label", BinaryStringEncoding.Utf8),  // Label
                                 CryptographicBuffer.DecodeFromHexString("303132333435363738")                   // Context
                                 );
            }
            else if (algName.Contains("SP800_56A"))
            {
                Params = KeyDerivationParameters.BuildForSP80056a(
                    CryptographicBuffer.ConvertStringToBinary("AlgorithmId", BinaryStringEncoding.Utf8),
                    CryptographicBuffer.ConvertStringToBinary("VParty", BinaryStringEncoding.Utf8),
                    CryptographicBuffer.ConvertStringToBinary("UParty", BinaryStringEncoding.Utf8),
                    CryptographicBuffer.ConvertStringToBinary("SubPubInfo", BinaryStringEncoding.Utf8),
                    CryptographicBuffer.ConvertStringToBinary("SubPrivInfo", BinaryStringEncoding.Utf8)
                    );
            }
            else
            {
                Scenario4Text.Text += "    An invalid algorithm was specified.\n";
                return;
            }

            // Create a KeyDerivationAlgorithmProvider object for the algorithm specified on input.
            KeyDerivationAlgorithmProvider Algorithm = KeyDerivationAlgorithmProvider.OpenAlgorithm(algName);

            Scenario4Text.Text += "*** Sample Kdf Algorithm: " + Algorithm.AlgorithmName + "\n";
            Scenario4Text.Text += "    Secrect Size: " + Secret.Length + "\n";
            Scenario4Text.Text += "    Target Size: " + TargetSize + "\n";

            // Create a key.
            CryptographicKey key = Algorithm.CreateKey(Secret);

            // Derive a key from the created key.
            IBuffer derived = CryptographicEngine.DeriveKeyMaterial(key, Params, TargetSize);
            Scenario4Text.Text += "    Derived  " + derived.Length + " bytes\n";
            Scenario4Text.Text += "    Derived: " + CryptographicBuffer.EncodeToHexString(derived) + "\n";
        }
    }
}
