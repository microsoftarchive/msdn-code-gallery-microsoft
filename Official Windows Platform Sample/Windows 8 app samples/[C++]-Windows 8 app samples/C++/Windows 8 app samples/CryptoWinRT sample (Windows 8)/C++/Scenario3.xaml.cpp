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
// Scenario3.xaml.cpp
// Implementation of the Scenario3 class
//

#include "pch.h"
#include "Scenario3.xaml.h"

using namespace SDKSample::CryptoWinRT;

using namespace Platform;
using namespace Windows::Security::Cryptography;
using namespace Windows::Security::Cryptography::Core;
using namespace Windows::Storage::Streams;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario3::Scenario3()
{
    InitializeComponent();
    AlgorithmNames->SelectedIndex = 0;
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario3::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}


void SDKSample::CryptoWinRT::Scenario3::RunSample_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Scenario3Text->Text = "";

    // Create a sample message.
    String^ Message = "Some message to authenticate";
    String^ algName = AlgorithmNames->SelectionBoxItem->ToString();

    // Created a MacAlgorithmProvider object for the algorithm specified on input.
    MacAlgorithmProvider^ Algorithm = MacAlgorithmProvider::OpenAlgorithm(algName);

    Scenario3Text->Text += "*** Sample Hmac Algorithm: " + Algorithm->AlgorithmName + "\n";
    Scenario3Text->Text += "\tOriginal Message" + Message + "\n";

    // Create a key.
    IBuffer^ keymaterial = CryptographicBuffer::GenerateRandom(Algorithm->MacLength);
    CryptographicKey^ hmacKey = Algorithm->CreateKey(keymaterial);

    // Sign the message by using the key.
    IBuffer^ signature = CryptographicEngine::Sign(hmacKey,
                                                   CryptographicBuffer::ConvertStringToBinary(Message, BinaryStringEncoding::Utf8));

    Scenario3Text->Text += "\tSignature:  " + CryptographicBuffer::EncodeToHexString(signature) + "\n";

    // Verify the signature.
    hmacKey = Algorithm->CreateKey(keymaterial);

    bool IsAuthenticated = CryptographicEngine::VerifySignature(
                                    hmacKey,
                                    CryptographicBuffer::ConvertStringToBinary(Message, BinaryStringEncoding::Utf8),
                                    signature
                                    );

    if (IsAuthenticated)
    {
        Scenario3Text->Text += "Verified HMAC\n";
    }
    else
    {
        Scenario3Text->Text += "Failed to verify HMAC\n";
    }
}
