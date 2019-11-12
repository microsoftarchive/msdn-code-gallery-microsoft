//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

//
// Scenario1.xaml.cpp
// Implementation of the Scenario1 class
//

#include "pch.h"
#include "Scenario1.xaml.h"

using namespace SDKSample::CryptoWinRT;

using namespace Platform;
using namespace Windows::Security::Cryptography;
using namespace Windows::Storage::Streams;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario1::Scenario1()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario1::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void SDKSample::CryptoWinRT::Scenario1::RunSample_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Scenario1Text->Text = "";
    IBuffer^ buffer;

    // Initialize example data.
    Array<byte>^ ByteArray = ref new Array<byte>(12);
    for(UINT32 i = 0; i < ByteArray->Length; i++)
        ByteArray[i] = i;

    String^ base64String = "uiwyeroiugfyqcajkds897945234==";
    String^ hexString = "30313233";
    String^ inputString = "Input string";

    // Generate random bytes.
    buffer = CryptographicBuffer::GenerateRandom(32);
    Scenario1Text->Text += "GenerateRandom\n";
    Scenario1Text->Text += "  Buffer: " + CryptographicBuffer::EncodeToHexString(buffer) + "\n\n";

    // Convert from a byte array.
    buffer = CryptographicBuffer::CreateFromByteArray(ByteArray);
    Scenario1Text->Text += "CreateFromByteArray\n";
    Scenario1Text->Text += "  Buffer: " + CryptographicBuffer::EncodeToHexString(buffer) + "\n\n";

    // Decode a Base64 encoded string to binary.
    buffer = CryptographicBuffer::DecodeFromBase64String(base64String);
    Scenario1Text->Text += "DecodeFromBase64String\n";
    Scenario1Text->Text += "  Base64 String: " + base64String + "\n";
    Scenario1Text->Text += "  Buffer:        " + CryptographicBuffer::EncodeToHexString(buffer) + "\n\n";

    // Decode a hexadecimal string to binary.
    buffer = CryptographicBuffer::DecodeFromHexString(hexString);
    Scenario1Text->Text += "DecodeFromHexString\n";
    Scenario1Text->Text += "  Hex String: " + hexString + "\n";
    Scenario1Text->Text += "  Buffer:     " + CryptographicBuffer::EncodeToHexString(buffer) + "\n\n";

    // Convert a string to UTF16BE binary data.
    buffer = CryptographicBuffer::ConvertStringToBinary(inputString, BinaryStringEncoding::Utf16BE);
    Scenario1Text->Text += "ConvertStringToBinary (Utf16BE)\n";
    Scenario1Text->Text += "  String: " + inputString + "\n";
    Scenario1Text->Text += "  Buffer: " + CryptographicBuffer::EncodeToHexString(buffer) + "\n\n";

    // Convert a string to UTF16LE binary data.
    buffer = CryptographicBuffer::ConvertStringToBinary(inputString, BinaryStringEncoding::Utf16LE);
    Scenario1Text->Text += "ConvertStringToBinary (Utf16LE)\n";
    Scenario1Text->Text += "  String: " + inputString + "\n";
    Scenario1Text->Text += "  Buffer: " + CryptographicBuffer::EncodeToHexString(buffer) + "\n\n";

    // Convert a string to UTF8 binary data.
    buffer = CryptographicBuffer::ConvertStringToBinary(inputString, BinaryStringEncoding::Utf8);
    Scenario1Text->Text += "ConvertStringToBinary (Utf8)\n";
    Scenario1Text->Text += "  String: " + inputString + "\n";
    Scenario1Text->Text += "  Buffer: " + CryptographicBuffer::EncodeToHexString(buffer) + "\n\n";

    // Decode from a Base64 encoded string.
    buffer = CryptographicBuffer::DecodeFromBase64String(base64String);
    Scenario1Text->Text += "DecodeFromBase64String \n";
    Scenario1Text->Text += "  String: " + base64String + "\n";
    Scenario1Text->Text += "  Buffer (Hex): " + CryptographicBuffer::EncodeToHexString(buffer) + "\n\n";
    Scenario1Text->Text += "  Buffer (Base64): " + CryptographicBuffer::EncodeToBase64String(buffer) + "\n\n";

    // Decode from a hexadecimal encoded string.
    buffer = CryptographicBuffer::DecodeFromHexString(hexString);
    Scenario1Text->Text += "DecodeFromHexString \n";
    Scenario1Text->Text += "  String: " + hexString + "\n";
    Scenario1Text->Text += "  Buffer: " + CryptographicBuffer::EncodeToHexString(buffer) + "\n\n";
}
