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
    public sealed partial class Scenario8 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario8()
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
            Scenario8Text.Text = "";

            CryptographicKey keyPair;
            IBuffer blobOfPublicKey;
            IBuffer blobOfKeyPair;
            String cookie = "Some Data to sign";
            IBuffer Data = CryptographicBuffer.ConvertStringToBinary(cookie, BinaryStringEncoding.Utf16BE);

            // Create an AsymmetricKeyAlgorithmProvider object for the algorithm specified on input.
            AsymmetricKeyAlgorithmProvider Algorithm = AsymmetricKeyAlgorithmProvider.OpenAlgorithm(algName);

            Scenario8Text.Text += "*** Sample Signature Algorithm\n";
            Scenario8Text.Text += "    Algorithm Name: " + Algorithm.AlgorithmName + "\n";
            Scenario8Text.Text += "    Key Size: " + KeySize + "\n";

            // Generate a key pair.
            try
            {
                keyPair = Algorithm.CreateKeyPair(KeySize);
            }
            catch (ArgumentException ex)
            {
                Scenario8Text.Text += ex.Message + "\n";
                Scenario8Text.Text += "An invalid key size was specified for the given algorithm.";
                return;
            }
            // Sign the data by using the generated key.
            IBuffer Signature = CryptographicEngine.Sign(keyPair, Data);
            Scenario8Text.Text += "    Data was successfully signed.\n";

            // Export the public key.
            blobOfPublicKey = keyPair.ExportPublicKey();
            blobOfKeyPair = keyPair.Export();
            Scenario8Text.Text += "    Key pair was successfully exported.\n";

            // Import the public key.
            CryptographicKey keyPublic = Algorithm.ImportPublicKey(blobOfPublicKey);

            // Check the key size.
            if (keyPublic.KeySize != keyPair.KeySize)
            {
                Scenario8Text.Text += "ImportPublicKey failed!  The imported key's size did not match the original's!";
                return;
            }
            Scenario8Text.Text += "    Public key was successfully imported.\n";

            // Import the key pair.
            keyPair = Algorithm.ImportKeyPair(blobOfKeyPair);

            // Check the key size.
            if (keyPublic.KeySize != keyPair.KeySize)
            {
                Scenario8Text.Text += "ImportKeyPair failed!  The imported key's size did not match the original's!";
                return;
            }
            Scenario8Text.Text += "    Key pair was successfully imported.\n";

            // Verify the signature by using the public key.
            if (!CryptographicEngine.VerifySignature(keyPublic, Data, Signature))
            {
                Scenario8Text.Text += "Signature verification failed!";
                return;
            }
            Scenario8Text.Text += "    Signature was successfully verified.\n";
        }
    }
}
