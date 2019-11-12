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
    public sealed partial class SignVerify : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public SignVerify()
        {
            this.InitializeComponent();
            AlgorithmNames.SelectedIndex = 0;
            KeySizes.SelectedIndex = 0;
            bAsymmetric.IsChecked = true;
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
            SignVerifyText.Text = "";

            String cookie = "Some Data to sign";
            IBuffer Data = CryptographicBuffer.ConvertStringToBinary(cookie, BinaryStringEncoding.Utf16BE);

            CryptographicKey key = null;
            if ((bool)bHmac.IsChecked)
                key = GenerateHMACKey();
            else
                key = GenerateAsymmetricKey();

            if (key != null)
            {
                // Sign the data by using the generated key.
                IBuffer Signature = CryptographicEngine.Sign(key, Data);
                SignVerifyText.Text += "    Data was successfully signed.\n";

                // Verify the signature by using the public key.
                if (!CryptographicEngine.VerifySignature(key, Data, Signature))
                {
                    SignVerifyText.Text += "Signature verification failed!";
                    return;
                }
                SignVerifyText.Text += "    Signature was successfully verified.\n";
            }
        }

        private void AsymmetricImportExport(CryptographicKey keyPair)
        {
            String algName = AlgorithmNames.SelectionBoxItem.ToString();
            AsymmetricKeyAlgorithmProvider Algorithm = AsymmetricKeyAlgorithmProvider.OpenAlgorithm(algName);

            // Export the public key.
            IBuffer blobOfPublicKey = keyPair.ExportPublicKey();
            IBuffer blobOfKeyPair = keyPair.Export();
            SignVerifyText.Text += "    Key pair was successfully exported.\n";

            // Import the public key.
            CryptographicKey keyPublic = Algorithm.ImportPublicKey(blobOfPublicKey);

            // Check the key size.
            if (keyPublic.KeySize != keyPair.KeySize)
            {
                SignVerifyText.Text += "ImportPublicKey failed!  The imported key's size did not match the original's!";
                return;
            }
            SignVerifyText.Text += "    Public key was successfully imported.\n";

            // Import the key pair.
            keyPair = Algorithm.ImportKeyPair(blobOfKeyPair);

            // Check the key size.
            if (keyPublic.KeySize != keyPair.KeySize)
            {
                SignVerifyText.Text += "ImportKeyPair failed!  The imported key's size did not match the original's!";
                return;
            }
            SignVerifyText.Text += "    Key pair was successfully imported.\n";

        }

        private CryptographicKey GenerateHMACKey()
        {
            String algName = AlgorithmNames.SelectionBoxItem.ToString();

            // Created a MacAlgorithmProvider object for the algorithm specified on input.
            MacAlgorithmProvider algorithm = MacAlgorithmProvider.OpenAlgorithm(algName);

            SignVerifyText.Text += "*** Sample Hmac Algorithm: " + algorithm.AlgorithmName + "\n";

            // Create a key.
            IBuffer keymaterial = CryptographicBuffer.GenerateRandom(algorithm.MacLength);
            return algorithm.CreateKey(keymaterial);
        }

        private CryptographicKey GenerateAsymmetricKey()
        {
            String algName = AlgorithmNames.SelectionBoxItem.ToString();
            UInt32 keySize = UInt32.Parse(KeySizes.SelectionBoxItem.ToString());

            CryptographicKey keyPair;
            // Create an AsymmetricKeyAlgorithmProvider object for the algorithm specified on input.
            AsymmetricKeyAlgorithmProvider Algorithm = AsymmetricKeyAlgorithmProvider.OpenAlgorithm(algName);

            SignVerifyText.Text += "*** Sample Signature Algorithm\n";
            SignVerifyText.Text += "    Algorithm Name: " + Algorithm.AlgorithmName + "\n";
            SignVerifyText.Text += "    Key Size: " + keySize + "\n";

            // Generate a key pair.
            try
            {
                keyPair = Algorithm.CreateKeyPair(keySize);
            }
            catch (ArgumentException ex)
            {
                SignVerifyText.Text += ex.Message + "\n";
                SignVerifyText.Text += "An invalid key size was specified for the given algorithm.";
                return null;
            }
            return keyPair;
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

            KeySizes.IsEnabled = false;
        }

        private void bAsymmetric_Checked(object sender, RoutedEventArgs e)
        {
            KeySizes.IsEnabled = true;

            AlgorithmNames.Items.Clear();
            AlgorithmNames.Items.Add(AsymmetricAlgorithmNames.EcdsaP256Sha256);
            AlgorithmNames.Items.Add(AsymmetricAlgorithmNames.EcdsaP384Sha384);
            AlgorithmNames.Items.Add(AsymmetricAlgorithmNames.EcdsaP521Sha512);
            AlgorithmNames.Items.Add(AsymmetricAlgorithmNames.RsaSignPkcs1Sha1);
            AlgorithmNames.Items.Add(AsymmetricAlgorithmNames.RsaSignPkcs1Sha256);
            AlgorithmNames.Items.Add(AsymmetricAlgorithmNames.RsaSignPkcs1Sha384);
            AlgorithmNames.Items.Add(AsymmetricAlgorithmNames.RsaSignPkcs1Sha512);
            AlgorithmNames.Items.Add(AsymmetricAlgorithmNames.RsaSignPssSha1);
            AlgorithmNames.Items.Add(AsymmetricAlgorithmNames.RsaSignPssSha256);
            AlgorithmNames.Items.Add(AsymmetricAlgorithmNames.RsaSignPssSha384);
            AlgorithmNames.Items.Add(AsymmetricAlgorithmNames.RsaSignPssSha512);
            AlgorithmNames.Items.Add(AsymmetricAlgorithmNames.DsaSha1);
            AlgorithmNames.SelectedIndex = 0;
        }
    }
}
