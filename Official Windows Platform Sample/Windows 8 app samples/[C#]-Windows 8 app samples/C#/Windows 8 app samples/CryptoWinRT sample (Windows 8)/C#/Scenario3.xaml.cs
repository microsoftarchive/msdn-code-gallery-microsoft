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
    public sealed partial class Scenario3 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario3()
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
            String algName = AlgorithmNames.SelectionBoxItem.ToString();
            Scenario3Text.Text = "";

            // Create a sample message.
            String Message = "Some message to authenticate";

            // Created a MacAlgorithmProvider object for the algorithm specified on input.
            MacAlgorithmProvider Algorithm = MacAlgorithmProvider.OpenAlgorithm(algName);

            Scenario3Text.Text += "*** Sample Hmac Algorithm: " + Algorithm.AlgorithmName + "\n";

            // Create a key.
            IBuffer keymaterial = CryptographicBuffer.GenerateRandom(Algorithm.MacLength);
            CryptographicKey hmacKey = Algorithm.CreateKey(keymaterial);

            // Sign the message by using the key.
            IBuffer signature = Windows.Security.Cryptography.Core.CryptographicEngine.Sign(
                                            hmacKey,
                                            CryptographicBuffer.ConvertStringToBinary(Message, BinaryStringEncoding.Utf8)
                                            );

            Scenario3Text.Text += "    Signature:  " + CryptographicBuffer.EncodeToHexString(signature) + "\n";

            // Verify the signature.
            hmacKey = Algorithm.CreateKey(keymaterial);

            bool IsAuthenticated = Windows.Security.Cryptography.Core.CryptographicEngine.VerifySignature(
                                            hmacKey,
                                            CryptographicBuffer.ConvertStringToBinary(Message, BinaryStringEncoding.Utf8),
                                            signature
                                            );

            if (!IsAuthenticated)
            {
                Scenario3Text.Text += "HashAlgorithmProvider failed to generate a hash of proper length!\n";
                return;
            }
        }

    }
}
