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
// Scenario6.xaml.cpp
// Implementation of the Scenario6 class
//

#include "pch.h"
#include "Scenario6.xaml.h"

using namespace SDKSample::CryptoWinRT;

using namespace Platform;
using namespace Windows::Security::Cryptography;
using namespace Windows::Security::Cryptography::Core;
using namespace Windows::Storage::Streams;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario6::Scenario6()
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
void Scenario6::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

static BYTE NonceBytes[] = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

IBuffer^ SDKSample::CryptoWinRT::Scenario6::GetNonce()
{
    // NOTE: 
    // 
    // The security best practises require that the Encrypt operation
    // not be called more than once with the same nonce for the same key.
    // 
    // Nonce can be predictable, but must be unique per secure session.

    unsigned int NonceBytesLength = sizeof(NonceBytes);
    int carry = 1;

    for (unsigned int i = 0; i < NonceBytesLength && carry == 1; i++)
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

    TestVectorNonce = ref new Array<byte>(NonceBytesLength);
    for(UINT32 i = 0; i < TestVectorNonce->Length;i++)
    {
        TestVectorNonce[i]= NonceBytes[i];
    }
    
    IBuffer ^ toReturn = CryptographicBuffer::CreateFromByteArray(TestVectorNonce);
    TestVectorNonce = nullptr;

    return toReturn;
}

void SDKSample::CryptoWinRT::Scenario6::RunSample_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    String^ algName = AlgorithmNames->SelectionBoxItem->ToString();
    int keySize;

    Scenario6Text->Text = "";

    switch (KeySizes->SelectedIndex)
    {
    case 0:
        keySize = 128;
        break;
    case 1:
        keySize = 256;
        break;
    default:
        Scenario6Text->Text += "An invalid size was selected";
        return;
    }

    IBuffer^ Decrypted;
    IBuffer^ Data;
    IBuffer^ Nonce;
    String^ Cookie = "Some Cookie to Encrypt";

    // Data to encrypt.
    Data = CryptographicBuffer::ConvertStringToBinary(Cookie, BinaryStringEncoding::Utf16LE);

    // Created a SymmetricKeyAlgorithmProvider object for the algorithm specified on input.
    SymmetricKeyAlgorithmProvider^ Algorithm = SymmetricKeyAlgorithmProvider::OpenAlgorithm(algName);

    Scenario6Text->Text += "\n*** Sample Authenticated Encryption\n";
    Scenario6Text->Text += "    Algorithm Name: " + Algorithm->AlgorithmName + "\n";
    Scenario6Text->Text += "    Key Size: " + keySize + "\n";
    Scenario6Text->Text += "    Block length: " + Algorithm->BlockLength + "\n";

    // Generate a random key.
    IBuffer^ keymaterial = CryptographicBuffer::GenerateRandom((keySize + 7) / 8);
    CryptographicKey^ key = Algorithm->CreateSymmetricKey(keymaterial);


    // Microsoft GCM implementation requires a 12 byte Nonce.
    // Microsoft CCM implementation requires a 7-13 byte Nonce.
    Nonce = GetNonce();

    // Encrypt and create an authenticated tag on the encrypted data.
    EncryptedAndAuthenticatedData^ Encrypted = CryptographicEngine::EncryptAndAuthenticate(key, Data, Nonce, nullptr);

    Scenario6Text->Text += "    Plain text: " + Data->Length + " bytes\n";
    Scenario6Text->Text += "    Encrypted: " + Encrypted->EncryptedData->Length + " bytes\n";
    Scenario6Text->Text += "    AuthTag: " + Encrypted->AuthenticationTag->Length + " bytes\n";

    // Create another instance of the key from the same material.
    CryptographicKey^ key2 = Algorithm->CreateSymmetricKey(keymaterial);

    if (key->KeySize != key2->KeySize)
    {
        Scenario6Text->Text += "CreateSymmetricKey failed!  The imported key's size did not match the original's!\n";
        return;
    }

    // Decrypt and verify the authenticated tag.
    Decrypted = CryptographicEngine::DecryptAndAuthenticate(key2, Encrypted->EncryptedData, Nonce, Encrypted->AuthenticationTag, nullptr);

    if (!CryptographicBuffer::Compare(Decrypted, Data))
    {
        Scenario6Text->Text += "Decrypted does not match original!\n";
        return;
    }
}
