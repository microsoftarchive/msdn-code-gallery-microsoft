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
// Scenario5.xaml.cpp
// Implementation of the Scenario5 class
//

#include "pch.h"
#include "Scenario5.xaml.h"

using namespace SDKSample::CryptoWinRT;

using namespace Platform;
using namespace Windows::Security::Cryptography;
using namespace Windows::Security::Cryptography::Core;
using namespace Windows::Storage::Streams;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario5::Scenario5()
{
    InitializeComponent();
    AlgorithmNames->SelectedIndex = 0;
    KeySizes->SelectedIndex = 0;
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario5::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void SDKSample::CryptoWinRT::Scenario5::RunSample_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    IBuffer^ encrypted;
    IBuffer^ decrypted;
    IBuffer^ buffer;
    IBuffer^ iv = nullptr;
    String^ blockCookie = "1234567812345678"; // 16 bytes
    String^ algName = AlgorithmNames->SelectionBoxItem->ToString();
    int keySize;
    bool cbc = false;

    Scenario5Text->Text = "";

    switch (AlgorithmNames->SelectedIndex)
    {
    case 0:
    case 2:
    case 4:
    case 6:
    case 8:
    case 10:
        cbc = true;
        break;
    }

    switch (KeySizes->SelectedIndex)
    {
    case 0:
        keySize = 64;
        break;
    case 1:
        keySize = 128;
        break;
    case 2:
        keySize = 192;
        break;
    case 3:
        keySize = 256;
        break;
    case 4:
        keySize = 512;
        break;
    default:
        Scenario5Text->Text += "An invalid key size was selected";
        return;
    }

    // Open the algorithm provider for the algorithm specified on input.
    SymmetricKeyAlgorithmProvider^ Algorithm = SymmetricKeyAlgorithmProvider::OpenAlgorithm(algName);
    Scenario5Text->Text += "*** Sample Cipher Encryption\n";
    Scenario5Text->Text += "    Algorithm Name: " + Algorithm->AlgorithmName + "\n";
    Scenario5Text->Text += "    Key Size: " + keySize + "\n";
    Scenario5Text->Text += "    Block length: " + Algorithm->BlockLength + "\n";

    // Generate a symmetric key.
    IBuffer^ keymaterial = CryptographicBuffer::GenerateRandom((keySize + 7) / 8);
    CryptographicKey^ key;

    try
    {
        key = Algorithm->CreateSymmetricKey(keymaterial);
    }
    catch(InvalidArgumentException^ e)
    {
        Scenario5Text->Text += "Exception: " + e->Message + "\n";
        Scenario5Text->Text += "Invalid Algorithm / Key Size pairing.";
        return;
    }

    // CBC mode needs Initialization vector, here just random data.
    // IV property will be set on "Encrypted".
    if (cbc)
        iv = CryptographicBuffer::GenerateRandom(Algorithm->BlockLength);

    // Set the data to encrypt. 
    buffer = CryptographicBuffer::ConvertStringToBinary(blockCookie, BinaryStringEncoding::Utf8);

    // Encrypt and create an authenticated tag.
    encrypted = CryptographicEngine::Encrypt(key, buffer, iv);

    Scenario5Text->Text += "    Plain text: " + buffer->Length + " bytes, Encrypted: " + encrypted->Length + " bytes\n";

    // Create another instance of the key from the same material.
    CryptographicKey^ key2 = Algorithm->CreateSymmetricKey(keymaterial);

    if (key->KeySize != key2->KeySize)
    {
        Scenario5Text->Text += "CreateSymmetricKey failed!  The imported key's size did not match the original's!";
        return;
    }

    // Decrypt and verify the authenticated tag.
    decrypted = CryptographicEngine::Decrypt(key2, encrypted, iv);

    if (!CryptographicBuffer::Compare(decrypted, buffer))
    {
        Scenario5Text->Text += "Decrypted does not match original!";
        return;
    }
}
