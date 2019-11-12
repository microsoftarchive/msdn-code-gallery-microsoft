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
    public sealed partial class Scenario7 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario7()
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
            UInt32 KeySize = UInt32.Parse(KeySizes.SelectionBoxItem.ToString());
            Scenario7Text.Text = "";

            IBuffer Data;
            String cookie = "Some cookie to encrypt";

            switch (AlgorithmNames.SelectedIndex)
            {
                case 0:
                    Data = CryptographicBuffer.ConvertStringToBinary(cookie, BinaryStringEncoding.Utf16LE);
                    break;

                // OAEP Padding depends on key size, message length and hash block length
                // 
                // The maximum plaintext length is KeyLength - 2*HashBlock - 2
                //
                // OEAP padding supports an optional label with the length is restricted by plaintext/key/hash sizes.
                // Here we just use a small label.
                case 1:
                    Data = CryptographicBuffer.GenerateRandom(1024 / 8 - 2 * 20 - 2);
                    break;
                case 2:
                    Data = CryptographicBuffer.GenerateRandom(1024 / 8 - 2 * (256 / 8) - 2);
                    break;
                case 3:
                    Data = CryptographicBuffer.GenerateRandom(2048 / 8 - 2 * (384 / 8) - 2);
                    break;
                case 4:
                    Data = CryptographicBuffer.GenerateRandom(2048 / 8 - 2 * (512 / 8) - 2);
                    break;
                default:
                    Scenario7Text.Text += "An invalid algorithm was selected";
                    return;
            }

            IBuffer Encrypted;
            IBuffer Decrypted;
            IBuffer blobOfPublicKey;
            IBuffer blobOfKeyPair;

            // Crate an AsymmetricKeyAlgorithmProvider object for the algorithm specified on input.
            AsymmetricKeyAlgorithmProvider Algorithm = AsymmetricKeyAlgorithmProvider.OpenAlgorithm(algName);

            Scenario7Text.Text += "*** Sample Encryption Algorithm\n";
            Scenario7Text.Text += "    Algorithm Name: " + Algorithm.AlgorithmName + "\n";
            Scenario7Text.Text += "    Key Size: " + KeySize + "\n";

            // Generate a random key.
            CryptographicKey keyPair = Algorithm.CreateKeyPair(KeySize);

            // Encrypt the data.
            try
            {
                Encrypted = CryptographicEngine.Encrypt(keyPair, Data, null);
            }
            catch (ArgumentException ex)
            {
                Scenario7Text.Text += ex.Message + "\n";
                Scenario7Text.Text += "An invalid key size was selected for the given algorithm.\n";
                return;
            }

            Scenario7Text.Text += "    Plain text: " + Data.Length + " bytes\n";
            Scenario7Text.Text += "    Encrypted: " + Encrypted.Length + " bytes\n";

            // Export the public key.
            blobOfPublicKey = keyPair.ExportPublicKey();
            blobOfKeyPair = keyPair.Export();

            // Import the public key.
            CryptographicKey keyPublic = Algorithm.ImportPublicKey(blobOfPublicKey);
            if (keyPublic.KeySize != keyPair.KeySize)
            {
                Scenario7Text.Text += "ImportPublicKey failed!  The imported key's size did not match the original's!";
                return;
            }

            // Import the key pair.
            keyPair = Algorithm.ImportKeyPair(blobOfKeyPair);

            // Check the key size of the imported key.
            if (keyPublic.KeySize != keyPair.KeySize)
            {
                Scenario7Text.Text += "ImportKeyPair failed!  The imported key's size did not match the original's!";
                return;
            }

            // Decrypt the data.
            Decrypted = CryptographicEngine.Decrypt(keyPair, Encrypted, null);

            if (!CryptographicBuffer.Compare(Decrypted, Data))
            {
                Scenario7Text.Text += "Decrypted data does not match original!";
                return;
            }
        }
    }
}
