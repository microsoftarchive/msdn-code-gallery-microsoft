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
// Scenario2.xaml.cpp
// Implementation of the Scenario2 class
//

#include "pch.h"
#include "Scenario2.xaml.h"

using namespace SDKSample::CryptoWinRT;

using namespace Platform;
using namespace Windows::Security::Cryptography;
using namespace Windows::Security::Cryptography::Core;
using namespace Windows::Storage::Streams;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario2::Scenario2()
{
    InitializeComponent();
    AlgorithmNames->SelectedIndex = 0;
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario2::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}


void SDKSample::CryptoWinRT::Scenario2::RunSample_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Scenario2Text->Text = "";
    String^ algName;

    algName = AlgorithmNames->SelectionBoxItem->ToString();

    // Create a HashAlgorithmProvider object.
    HashAlgorithmProvider^ Algorithm = HashAlgorithmProvider::OpenAlgorithm(algName);
    IBuffer^ vector = CryptographicBuffer::DecodeFromBase64String("uiwyeroiugfyqcajkds897945234==");

    Scenario2Text->Text += "*** Sample Hash Algorithm: " + Algorithm->AlgorithmName + "\n";
    Scenario2Text->Text += "\tInitial vector:  " + CryptographicBuffer::EncodeToBase64String(vector) + "\n";
 
    // Compute the hash in one call.
    IBuffer^ digest = Algorithm->HashData(vector);

    if (digest->Length != Algorithm->HashLength)
    {
        Scenario2Text->Text += "HashAlgorithmProvider failed to generate a hash of proper length!\n";
        return;
    }

    Scenario2Text->Text += "\tHash:  " + CryptographicBuffer::EncodeToHexString(digest) + "\n";

    // Use a reusable hash object to hash the data by using multiple calls.
    CryptographicHash^ reusableHash = Algorithm->CreateHash();

    reusableHash->Append(vector);

    // Note that calling GetValueAndReset resets the data that has been appended to the
    // CryptographicHash object.
    IBuffer^ digest2 = reusableHash->GetValueAndReset();

    if (!CryptographicBuffer::Compare(digest, digest2))
    {
        Scenario2Text->Text += "CryptographicHash failed to generate the same hash data!\n";
        return;
    }

    reusableHash->Append(vector);
    digest2 = reusableHash->GetValueAndReset();

    if (!CryptographicBuffer::Compare(digest, digest2))
    {
        Scenario2Text->Text += "Reusable CryptographicHash failed to generate the same hash data!\n";
        return;
    }
}
