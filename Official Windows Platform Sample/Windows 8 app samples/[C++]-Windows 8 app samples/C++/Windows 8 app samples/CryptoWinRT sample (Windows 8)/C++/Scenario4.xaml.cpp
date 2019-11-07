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
// Scenario4.xaml.cpp
// Implementation of the Scenario4 class
//

#include "pch.h"
#include "Scenario4.xaml.h"

using namespace SDKSample::CryptoWinRT;

using namespace Platform;
using namespace Windows::Security::Cryptography;
using namespace Windows::Security::Cryptography::Core;
using namespace Windows::Storage::Streams;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario4::Scenario4()
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
void Scenario4::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void SDKSample::CryptoWinRT::Scenario4::RunSample_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Scenario4Text->Text = "";

    int TargetSize = 0;
    String^ algName = AlgorithmNames->SelectionBoxItem->ToString();
    IBuffer^ Secret = CryptographicBuffer::ConvertStringToBinary("Master key to derive from", BinaryStringEncoding::Utf8);
    KeyDerivationParameters^ Params;

    switch (AlgorithmNames->SelectedIndex)
    {
    case 0:
    case 1:
    case 2:
        Params = KeyDerivationParameters::BuildForPbkdf2(
                                        CryptographicBuffer::GenerateRandom(16),  // Salt
                                        10000                       // PBKDF2 Iteration Count
                                        );
        break;
    case 3:
    case 4:
    case 5:
        Params = KeyDerivationParameters::BuildForSP800108(
                             CryptographicBuffer::ConvertStringToBinary("Label", BinaryStringEncoding::Utf8),  // Label
                             CryptographicBuffer::DecodeFromHexString("303132333435363738")                   // Context
                             );
        break;
    case 6:
    case 7:
        Params = KeyDerivationParameters::BuildForSP80056a(
                        CryptographicBuffer::ConvertStringToBinary("AlgorithmId", BinaryStringEncoding::Utf8),
                        CryptographicBuffer::ConvertStringToBinary("VParty", BinaryStringEncoding::Utf8),
                        CryptographicBuffer::ConvertStringToBinary("UParty", BinaryStringEncoding::Utf8),
                        CryptographicBuffer::ConvertStringToBinary("SubPubInfo", BinaryStringEncoding::Utf8),
                        CryptographicBuffer::ConvertStringToBinary("SubPrivInfo", BinaryStringEncoding::Utf8)
                        );
        break;
    default:
        Scenario4Text->Text += "An invalid Algorithm was selected";
        return;
    }
    KeyDerivationAlgorithmProvider^ Algorithm = KeyDerivationAlgorithmProvider::OpenAlgorithm(algName);
    Scenario4Text->Text += "Sample Kdf Algorithm: " + Algorithm->AlgorithmName + "\n";

    switch (KeySizes->SelectedIndex)
    {
    case 0:
        TargetSize = 64;
        break;
    case 1:
        TargetSize = 256;
        break;
    default:
        Scenario4Text->Text += "An invalid size was selected";
        return;
    }
    Scenario4Text->Text += "    Target Size: " + TargetSize + "\n";
    Scenario4Text->Text += "    Secret Size: " + Secret->Length + "\n";

    // Create a key.
    CryptographicKey^ key = Algorithm->CreateKey(Secret);
    IBuffer^ derived;

    // Derive a key from the created key.
    try
    {
        derived = CryptographicEngine::DeriveKeyMaterial(key, Params, TargetSize);
    }
    catch(Exception^ e)
    {
        Scenario4Text->Text += e->Message + "\n";
        return;
    }
    Scenario4Text->Text += "    Derived  " + derived->Length + " bytes\n";
    Scenario4Text->Text += "    Derived: " + CryptographicBuffer::EncodeToHexString(derived) + "\n";
}
