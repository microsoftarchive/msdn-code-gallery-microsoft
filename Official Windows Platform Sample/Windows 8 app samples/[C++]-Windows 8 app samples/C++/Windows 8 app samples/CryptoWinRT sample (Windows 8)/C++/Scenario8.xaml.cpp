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
// Scenario8.xaml.cpp
// Implementation of the Scenario8 class
//

#include "pch.h"
#include "Scenario8.xaml.h"

using namespace SDKSample::CryptoWinRT;

using namespace Platform;
using namespace Windows::Security::Cryptography;
using namespace Windows::Security::Cryptography::Core;
using namespace Windows::Storage::Streams;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario8::Scenario8()
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
void Scenario8::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void SDKSample::CryptoWinRT::Scenario8::RunSample_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Scenario8Text->Text = "";
    String^ algName = AlgorithmNames->SelectionBoxItem->ToString();
    unsigned int KeySize;
    IBuffer^ blobOfPublicKey;
    IBuffer^ blobOfKeyPair;
    String^  cookie = "Some Data to sign";
    IBuffer^ Data = CryptographicBuffer::ConvertStringToBinary(cookie, BinaryStringEncoding::Utf16BE);

    switch (KeySizes->SelectedIndex)
    {
    case 0:
        KeySize = 256;
        break;
    case 1:
        KeySize = 384;
        break;
    case 2:
        KeySize = 521;
        break;
    case 3:
        KeySize = 1024;
        break;
    case 4:
        KeySize = 2048;
        break;
    case 5:
        KeySize = 3072;
        break;
    case 6:
        KeySize = 4096;
        break;
    default:
        Scenario8Text->Text += "An invalid size was selected";
        return;
    }

    // Create an AsymmetricKeyAlgorithmProvider object for the algorithm specified on input.
    AsymmetricKeyAlgorithmProvider^ Algorithm = AsymmetricKeyAlgorithmProvider::OpenAlgorithm(algName);

    Scenario8Text->Text += "\n*** Sample Signature Algorithm\n";
    Scenario8Text->Text += "    Algorithm Name: " + Algorithm->AlgorithmName + "\n";
    Scenario8Text->Text += "    Key Size: " + KeySize + "\n";

    CryptographicKey^ keyPair;
    IBuffer^ Signature;

    try
    {
        // Generate a key pair.
        keyPair = Algorithm->CreateKeyPair(KeySize);

        // Sign the data by using the generated key.
        Signature = CryptographicEngine::Sign(keyPair, Data);
    }
    catch (InvalidArgumentException^ e)
    {
        Scenario8Text->Text += "Exception: " + e->Message + "\n";
        Scenario8Text->Text += "An invalid Algorithm / Key Size pair was selected.\n";
        return;
    }

    // Export the public key.
    blobOfPublicKey = keyPair->ExportPublicKey();
    blobOfKeyPair = keyPair->Export();

    // Import the public key.
    CryptographicKey^ keyPublic = Algorithm->ImportPublicKey(blobOfPublicKey);

    // Check the key size.
    if (keyPublic->KeySize != keyPair->KeySize)
    {
        Scenario8Text->Text += "ImportPublicKey failed!  The imported key's size did not match the original's!\n";
        return;
    }
    Scenario8Text->Text += "    The public key was successfully imported.\n";

    // Import the key pair.
    keyPair = Algorithm->ImportKeyPair(blobOfKeyPair);

    // Check the key size.
    if (keyPublic->KeySize != keyPair->KeySize)
    {
        Scenario8Text->Text += "ImportKeyPair failed!  The imported key's size did not match the original's!\n";
        return;
    }
    Scenario8Text->Text += "    The key pair was successfully imported.\n";

    // Verify the signature by using the public key.
    if (!CryptographicEngine::VerifySignature(keyPublic, Data, Signature))
    {
        Scenario8Text->Text += "Signature verification failed!";
        return;
    }
}
