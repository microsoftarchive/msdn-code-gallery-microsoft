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
    public sealed partial class Scenario5 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario5()
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
            UInt32 keySize = UInt32.Parse(KeySizes.SelectionBoxItem.ToString());
            Scenario5Text.Text = "";

            IBuffer encrypted;
            IBuffer decrypted;
            IBuffer buffer;
            IBuffer iv = null;
            String blockCookie = "1234567812345678"; // 16 bytes

            // Open the algorithm provider for the algorithm specified on input.
            SymmetricKeyAlgorithmProvider Algorithm = SymmetricKeyAlgorithmProvider.OpenAlgorithm(algName);

            Scenario5Text.Text += "\n*** Sample Cipher Encryption\n";
            Scenario5Text.Text += "    Algorithm Name: " + Algorithm.AlgorithmName + "\n";
            Scenario5Text.Text += "    Key Size: " + keySize + "\n";
            Scenario5Text.Text += "    Block length: " + Algorithm.BlockLength + "\n";

            // Generate a symmetric key.
            IBuffer keymaterial = CryptographicBuffer.GenerateRandom((keySize + 7) / 8);
            CryptographicKey key;
            try
            {
                key = Algorithm.CreateSymmetricKey(keymaterial);
            }
            catch (ArgumentException ex)
            {
                Scenario5Text.Text += ex.Message + "\n";
                Scenario5Text.Text += "An invalid key size was selected for the given algorithm.\n";
                return;
            }

            // CBC mode needs Initialization vector, here just random data.
            // IV property will be set on "Encrypted".
            if (algName.Contains("CBC"))
                iv = CryptographicBuffer.GenerateRandom(Algorithm.BlockLength);

            // Set the data to encrypt. 
            buffer = CryptographicBuffer.ConvertStringToBinary(blockCookie, BinaryStringEncoding.Utf8);

            // Encrypt and create an authenticated tag.
            encrypted = Windows.Security.Cryptography.Core.CryptographicEngine.Encrypt(key, buffer, iv);

            Scenario5Text.Text += "    Plain text: " + buffer.Length + " bytes\n";
            Scenario5Text.Text += "    Encrypted: " + encrypted.Length + " bytes\n";

            // Create another instance of the key from the same material.
            CryptographicKey key2 = Algorithm.CreateSymmetricKey(keymaterial);

            if (key.KeySize != key2.KeySize)
            {
                Scenario5Text.Text += "CreateSymmetricKey failed!  The imported key's size did not match the original's!";
                return;
            }

            // Decrypt and verify the authenticated tag.
            decrypted = Windows.Security.Cryptography.Core.CryptographicEngine.Decrypt(key2, encrypted, iv);

            if (!CryptographicBuffer.Compare(decrypted, buffer))
            {
                Scenario5Text.Text += "Decrypted does not match original!";
                return;
            }
        }
    }
}
