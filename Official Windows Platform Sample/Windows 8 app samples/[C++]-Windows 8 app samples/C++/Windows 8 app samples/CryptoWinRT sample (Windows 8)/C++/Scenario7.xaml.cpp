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
// Scenario7.xaml.cpp
// Implementation of the Scenario7 class
//

#include "pch.h"
#include "Scenario7.xaml.h"

using namespace SDKSample::CryptoWinRT;

using namespace Platform;
using namespace Windows::Security::Cryptography;
using namespace Windows::Security::Cryptography::Core;
using namespace Windows::Storage::Streams;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario7::Scenario7()
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
void Scenario7::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void SDKSample::CryptoWinRT::Scenario7::RunSample_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Scenario7Text->Text = "";

    IBuffer^ Data;
    String^ algName = AlgorithmNames->SelectionBoxItem->ToString();
    unsigned int KeySize;

    IBuffer^ Encrypted;
    IBuffer^ Decrypted;
    IBuffer^ blobOfPublicKey;
    IBuffer^ blobOfKeyPair;

    switch (AlgorithmNames->SelectedIndex)
    {
    case 0:
        Data = CryptographicBuffer::ConvertStringToBinary("Some cookie to encrypt", BinaryStringEncoding::Utf16LE);
        break;

    // OAEP Padding depends on key size, message length and hash block length
    // 
    // The maximum plaintext length is KeyLength - 2*HashBlock - 2
    //
    // OEAP padding supports an optional label with the length is restricted by plaintext/key/hash sizes.
    // Here we just use a small label.
    case 1:
        Data = CryptographicBuffer::GenerateRandom(1024 / 8 - 2 * 20 - 2);
        break;
    case 2:
        Data = CryptographicBuffer::GenerateRandom(1024 / 8 - 2 * (256 / 8) - 2);
        break;
    case 3:
        Data = CryptographicBuffer::GenerateRandom(2048 / 8 - 2 * (384 / 8) - 2);
        break;
    case 4:
        Data = CryptographicBuffer::GenerateRandom(2048 / 8 - 2 * (512 / 8) - 2);
        break;
    default:
        Scenario7Text->Text += "An invalid Algorithm was selected";
        return;
    }

    switch (KeySizes->SelectedIndex)
    {
    case 0:
        KeySize = 512;
        break;
    case 1:
        KeySize = 1024;
        break;
    case 2:
        KeySize = 2048;
        break;
    case 3:
        KeySize = 4096;
        break;
    default:
        Scenario7Text->Text += "An invalid size was selected";
        return;
    }

    // Crate an AsymmetricKeyAlgorithmProvider object for the algorithm specified on input.
    AsymmetricKeyAlgorithmProvider^ Algorithm = AsymmetricKeyAlgorithmProvider::OpenAlgorithm(algName);

    Scenario7Text->Text += "\n*** Sample Encryption Algorithm\n";
    Scenario7Text->Text += "    Algorithm Name: " + Algorithm->AlgorithmName + "\n";
    Scenario7Text->Text += "    KeySize = " + KeySize + "\n";

    CryptographicKey^ keyPair;
    try
    {
        // Generate a random key.
        keyPair = Algorithm->CreateKeyPair(KeySize);

        // Encrypt the data.
        Encrypted = CryptographicEngine::Encrypt(keyPair, Data, nullptr);
    }
    catch (InvalidArgumentException^ e)
    {
        Scenario7Text->Text += "Exception: " + e->Message + "\n";
        Scenario7Text->Text += "The given keysize is not supported for the given algorithm\n";
        return;
    }

    Scenario7Text->Text += "    Plain text: " + Data->Length + " bytes\n";
    Scenario7Text->Text += "    Encrypted: " + Encrypted->Length + " bytes\n";

    // Export the public key.
    blobOfPublicKey = keyPair->ExportPublicKey();
    blobOfKeyPair = keyPair->Export();

    // Import the public key.
    CryptographicKey^ keyPublic = Algorithm->ImportPublicKey(blobOfPublicKey);
    if (keyPublic->KeySize != keyPair->KeySize)
    {
        Scenario7Text->Text += "ImportPublicKey failed!  The imported key's size did not match the original's!\n";
        return;
    }

    // Import the key pair.
    keyPair = Algorithm->ImportKeyPair(blobOfKeyPair);

    // Check the key size of the imported key.
    if (keyPublic->KeySize != keyPair->KeySize)
    {
        Scenario7Text->Text += "ImportKeyPair failed!  The imported key's size did not match the original's!\n";
        return;
    }

    // Decrypt the data.
    Decrypted = CryptographicEngine::Decrypt(keyPair, Encrypted, nullptr);

    if (!CryptographicBuffer::Compare(Decrypted, Data))
    {
        Scenario7Text->Text += "Decrypted data does not match original!\n";
        return;
    }
}
