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
// KeyDerivation.xaml.cpp
// Implementation of the KeyDerivation class
//

#include "pch.h"
#include "KeyDerivation.xaml.h"

using namespace std;
using namespace SDKSample::CryptographyAndCertificate;
using namespace Windows::Security::Cryptography::Certificates;
using namespace Concurrency;
using namespace Platform;
using namespace Microsoft::WRL::Details;
using namespace Microsoft::WRL;
using namespace Windows::Storage::Streams;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Security::Cryptography;
using namespace Windows::Security::Cryptography::Core;


KeyDerivation::KeyDerivation()
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
void KeyDerivation::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

/// <summary>
/// This is the click handler for the 'RunSample' button.  It is responsible for executing the sample code.
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
void KeyDerivation::RunSample_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    rootPage = MainPage::Current;
    Windows::UI::Xaml::Controls::ComboBox^ algoNames = safe_cast<Windows::UI::Xaml::Controls::ComboBox^>(rootPage->FindName(L"AlgorithmNames"));
    //ComboBox^ keySizes = safe_cast<ComboBox^>(rootPage->FindName(L"KeySizes"));
    TextBlock^ keyDerivationText =safe_cast<TextBlock^>(rootPage->FindName(L"KeyDerivationText"));

    Platform::String^ algName = algoNames->SelectionBoxItem->ToString();
    IBuffer^ Secret = CryptographicBuffer::ConvertStringToBinary("Master key to derive from", BinaryStringEncoding::Utf8);
    uint32 TargetSize = _wtoi(KeySizes->SelectionBoxItem->ToString()->Data());
    keyDerivationText->Text = "";
    KeyDerivationParameters^ Params;

    if (wcsstr(algName->Data(), L"PBKDF2"))
    {
        // Password based key derivation function (PBKDF2).
        Params = KeyDerivationParameters::BuildForPbkdf2(
                        CryptographicBuffer::GenerateRandom(16),  // Salt
                        10000                       // PBKDF2 Iteration Count
                        );
    }
    else if (wcsstr(algName->Data(), L"SP800_108"))
    {
        // SP800_108_CTR_HMAC key derivation function.
        Params = KeyDerivationParameters::BuildForSP800108(
                            CryptographicBuffer::ConvertStringToBinary("Label", BinaryStringEncoding::Utf8),  // Label
                            CryptographicBuffer::DecodeFromHexString("303132333435363738")                   // Context
                            );
    }
    else if (wcsstr(algName->Data(), L"SP800_56A"))
    {
        Params = KeyDerivationParameters::BuildForSP80056a(
            CryptographicBuffer::ConvertStringToBinary("AlgorithmId", BinaryStringEncoding::Utf8),
            CryptographicBuffer::ConvertStringToBinary("VParty", BinaryStringEncoding::Utf8),
            CryptographicBuffer::ConvertStringToBinary("UParty", BinaryStringEncoding::Utf8),
            CryptographicBuffer::ConvertStringToBinary("SubPubInfo", BinaryStringEncoding::Utf8),
            CryptographicBuffer::ConvertStringToBinary("SubPrivInfo", BinaryStringEncoding::Utf8)
            );
    }
    else
    {
        keyDerivationText->Text += "    An invalid algorithm was specified.\n";
        return;
    }

    // Create a KeyDerivationAlgorithmProvider object for the algorithm specified on input.
    KeyDerivationAlgorithmProvider^ Algorithm = KeyDerivationAlgorithmProvider::OpenAlgorithm(algName);

    keyDerivationText->Text += "*** Sample Kdf Algorithm: " + Algorithm->AlgorithmName + "\n";
    keyDerivationText->Text += "    Secrect Size: " + Secret->Length + "\n";
    keyDerivationText->Text += "    Target Size: " + TargetSize + "\n";

    // Create a key.
    CryptographicKey^ key = Algorithm->CreateKey(Secret);

    // Derive a key from the created key.
    IBuffer^ derived = CryptographicEngine::DeriveKeyMaterial(key, Params, TargetSize);
    keyDerivationText->Text += "    Derived  " + derived->Length + " bytes\n";
    keyDerivationText->Text += "    Derived: " + CryptographicBuffer::EncodeToHexString(derived) + "\n";
}

