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
    public sealed partial class Scenario1 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario1()
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
        /// This is the click handler for the 'RunSample' button.  It is responsible for executing the sample code.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RunSample_Click(object sender, RoutedEventArgs e)
        {
            IBuffer buffer;
            Scenario1Text.Text = "";

            // Initialize example data.
            byte[] ByteArray = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            String base64String = "uiwyeroiugfyqcajkds897945234==";
            String hexString = "30313233";
            String inputString = "Input string";

            // Generate random bytes.
            buffer = CryptographicBuffer.GenerateRandom(32);
            Scenario1Text.Text += "GenerateRandom\n";
            Scenario1Text.Text += "  Buffer: " + CryptographicBuffer.EncodeToHexString(buffer) + "\n\n";

            // Convert from a byte array.
            buffer = CryptographicBuffer.CreateFromByteArray(ByteArray);
            Scenario1Text.Text += "CreateFromByteArray\n";
            Scenario1Text.Text += "  Buffer: " + CryptographicBuffer.EncodeToHexString(buffer) + "\n\n";

            // Decode a Base64 encoded string to binary.
            buffer = CryptographicBuffer.DecodeFromBase64String(base64String);
            Scenario1Text.Text += "DecodeFromBase64String\n";
            Scenario1Text.Text += "  Base64 String: " + base64String + "\n";
            Scenario1Text.Text += "  Buffer:        " + CryptographicBuffer.EncodeToHexString(buffer) + "\n\n";

            // Decode a hexadecimal string to binary.
            buffer = CryptographicBuffer.DecodeFromHexString(hexString);
            Scenario1Text.Text += "DecodeFromHexString\n";
            Scenario1Text.Text += "  Hex String: " + hexString + "\n";
            Scenario1Text.Text += "  Buffer:     " + CryptographicBuffer.EncodeToHexString(buffer) + "\n\n";

            // Convert a string to UTF16BE binary data.
            buffer = CryptographicBuffer.ConvertStringToBinary(inputString, BinaryStringEncoding.Utf16BE);
            Scenario1Text.Text += "ConvertStringToBinary (Utf16BE)\n";
            Scenario1Text.Text += "  String: " + inputString + "\n";
            Scenario1Text.Text += "  Buffer: " + CryptographicBuffer.EncodeToHexString(buffer) + "\n\n";

            // Convert a string to UTF16LE binary data.
            buffer = CryptographicBuffer.ConvertStringToBinary(inputString, BinaryStringEncoding.Utf16LE);
            Scenario1Text.Text += "ConvertStringToBinary (Utf16LE)\n";
            Scenario1Text.Text += "  String: " + inputString + "\n";
            Scenario1Text.Text += "  Buffer: " + CryptographicBuffer.EncodeToHexString(buffer) + "\n\n";

            // Convert a string to UTF8 binary data.
            buffer = CryptographicBuffer.ConvertStringToBinary(inputString, BinaryStringEncoding.Utf8);
            Scenario1Text.Text += "ConvertStringToBinary (Utf8)\n";
            Scenario1Text.Text += "  String: " + inputString + "\n";
            Scenario1Text.Text += "  Buffer: " + CryptographicBuffer.EncodeToHexString(buffer) + "\n\n";

            // Decode from a Base64 encoded string.
            buffer = CryptographicBuffer.DecodeFromBase64String(base64String);
            Scenario1Text.Text += "DecodeFromBase64String \n";
            Scenario1Text.Text += "  String: " + base64String + "\n";
            Scenario1Text.Text += "  Buffer (Hex): " + CryptographicBuffer.EncodeToHexString(buffer) + "\n\n";
            Scenario1Text.Text += "  Buffer (Base64): " + CryptographicBuffer.EncodeToBase64String(buffer) + "\n\n";

            // Decode from a hexadecimal encoded string.
            buffer = CryptographicBuffer.DecodeFromHexString(hexString);
            Scenario1Text.Text += "DecodeFromHexString \n";
            Scenario1Text.Text += "  String: " + hexString + "\n";
            Scenario1Text.Text += "  Buffer: " + CryptographicBuffer.EncodeToHexString(buffer) + "\n\n";
        }
    }
}
