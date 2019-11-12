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
    public sealed partial class Scenario6 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        // Initialize a nonce value.
        static byte[] NonceBytes = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public Scenario6()
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
        /// This utility function returns a nonce value for authenticated encryption modes.
        /// </summary>
        /// <returns></returns>
        static IBuffer GetNonce()
        {
            // NOTE: 
            // 
            // The security best practises require that the Encrypt operation
            // not be called more than once with the same nonce for the same key.
            // 
            // Nonce can be predictable, but must be unique per secure session.

            int carry = 1;

            for (int i = 0; i < NonceBytes.Length && carry == 1; i++)
            {
                if (NonceBytes[i] == 255)
                {
                    NonceBytes[i] = 0;
                }
                else
                {
                    NonceBytes[i]++;
                    carry = 0;
                }
            }

            return CryptographicBuffer.CreateFromByteArray(NonceBytes);
        }

        /// <summary>
        /// This is the click handler for the 'RunSample' button.  It is responsible for executing the sample code.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RunSample_Click(object sender, RoutedEventArgs e)
        {
            String algName = AlgorithmNames.SelectionBoxItem.ToString();
            UInt32 keySize = UInt32.Parse(KeySizes.SelectionBoxItem.ToString());
            Scenario6Text.Text = "";

            IBuffer Decrypted;
            IBuffer Data;
            IBuffer Nonce;
            String Cookie = "Some Cookie to Encrypt";

            // Data to encrypt.
            Data = CryptographicBuffer.ConvertStringToBinary(Cookie, BinaryStringEncoding.Utf16LE);

            // Created a SymmetricKeyAlgorithmProvider object for the algorithm specified on input.
            SymmetricKeyAlgorithmProvider Algorithm = SymmetricKeyAlgorithmProvider.OpenAlgorithm(algName);

            Scenario6Text.Text += "*** Sample Authenticated Encryption\n";
            Scenario6Text.Text += "    Algorithm Name: " + Algorithm.AlgorithmName + "\n";
            Scenario6Text.Text += "    Key Size: " + keySize + "\n";
            Scenario6Text.Text += "    Block length: " + Algorithm.BlockLength + "\n";

            // Generate a random key.
            IBuffer keymaterial = CryptographicBuffer.GenerateRandom((keySize + 7) / 8);
            CryptographicKey key = Algorithm.CreateSymmetricKey(keymaterial);


            // Microsoft GCM implementation requires a 12 byte Nonce.
            // Microsoft CCM implementation requires a 7-13 byte Nonce.
            Nonce = GetNonce();

            // Encrypt and create an authenticated tag on the encrypted data.
            EncryptedAndAuthenticatedData Encrypted = CryptographicEngine.EncryptAndAuthenticate(key, Data, Nonce, null);

            Scenario6Text.Text += "    Plain text: " + Data.Length + " bytes\n";
            Scenario6Text.Text += "    Encrypted: " + Encrypted.EncryptedData.Length + " bytes\n";
            Scenario6Text.Text += "    AuthTag: " + Encrypted.AuthenticationTag.Length + " bytes\n";

            // Create another instance of the key from the same material.
            CryptographicKey key2 = Algorithm.CreateSymmetricKey(keymaterial);

            if (key.KeySize != key2.KeySize)
            {
                Scenario6Text.Text += "CreateSymmetricKey failed!  The imported key's size did not match the original's!";
                return;
            }

            // Decrypt and verify the authenticated tag.
            Decrypted = CryptographicEngine.DecryptAndAuthenticate(key2, Encrypted.EncryptedData, Nonce, Encrypted.AuthenticationTag, null);

            if (!CryptographicBuffer.Compare(Decrypted, Data))
            {
                Scenario6Text.Text += "Decrypted does not match original!";
                return;
            }
        }
    }
}
