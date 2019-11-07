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
// Hashing.xaml.cpp
// Implementation of the Hashing class
//

#include "pch.h"
#include "Hashing.xaml.h"

using namespace std;
using namespace SDKSample;

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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

Hashing::Hashing()
{
    InitializeComponent();
    bHash->IsChecked = true;
    AlgorithmNames->SelectedIndex = 0;
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Hashing::OnNavigatedTo(NavigationEventArgs^ e)
{
    (void) e;    // Unused parameter
}

/// <summary>
/// This is the click handler for the 'RunSample' button.  It is responsible for executing the sample code.
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
void Hashing::RunSample_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    rootPage = MainPage::Current;
    ComboBox^ algoNames = safe_cast<ComboBox^>(rootPage->FindName(L"AlgorithmNames"));
    TextBlock^ hashingText = safe_cast<TextBlock^>(rootPage->FindName(L"HashingText"));
    RadioButton^ bHmac = safe_cast<RadioButton^>(rootPage->FindName(L"bHmac"));

    IBuffer^ vector = CryptographicBuffer::DecodeFromBase64String("uiwyeroiugfyqcajkds897945234==");
    hashingText->Text = "";

    // Use a reusable hash object to hash the data by using multiple calls.
    CryptographicHash^ reusableHash;

    if (bHmac->IsChecked->Value)
    {
        reusableHash = CreateHmacCryptographicHash();
    }
    else
    {
        reusableHash = CreateHashCryptographicHash();
    }

    if (reusableHash == nullptr)
        return;

    reusableHash->Append(vector);

    // Note that calling GetValue resets the data that has been appended to the
    // CryptographicHash object.
    IBuffer^ digest2 = reusableHash->GetValueAndReset();

    if (!CryptographicBuffer::Compare(digest, digest2))
    {
        hashingText->Text += "CryptographicHash failed to generate the same hash data!\n";
        return;
    }

    reusableHash->Append(vector);
    digest2 = reusableHash->GetValueAndReset();

    if (!CryptographicBuffer::Compare(digest, digest2))
    {
        hashingText->Text += "Reusable CryptographicHash failed to generate the same hash data!\n";
        return;
    }
}

CryptographicHash^ Hashing::CreateHashCryptographicHash()
{
    rootPage = MainPage::Current;
    ComboBox^ algoNames = safe_cast<ComboBox^>(rootPage->FindName(L"AlgorithmNames"));
    TextBlock^ hashingText = safe_cast<TextBlock^>(rootPage->FindName(L"HashingText"));

    String^ algName = algoNames->SelectionBoxItem->ToString();

    // Create a HashAlgorithmProvider object.
    HashAlgorithmProvider^ Algorithm = HashAlgorithmProvider::OpenAlgorithm(algName);
    IBuffer^ vector = CryptographicBuffer::DecodeFromBase64String("uiwyeroiugfyqcajkds897945234==");

    hashingText->Text += "\n*** Sample Hash Algorithm: " + Algorithm->AlgorithmName + "\n";
    hashingText->Text += "    Initial vector:  uiwyeroiugfyqcajkds897945234==\n";

    // Compute the hash in one call.
    digest = Algorithm->HashData(vector);

    if (digest->Length != Algorithm->HashLength)
    {
        hashingText->Text += "HashAlgorithmProvider failed to generate a hash of proper length!\n";
        return nullptr;
    }

    hashingText->Text += "    Hash:  " + CryptographicBuffer::EncodeToHexString(digest) + "\n";

    return Algorithm->CreateHash();
}

CryptographicHash^ Hashing::CreateHmacCryptographicHash()
{
    rootPage = MainPage::Current;
    ComboBox^ algoNames = safe_cast<ComboBox^>(rootPage->FindName(L"AlgorithmNames"));
    TextBlock^ hashingText = safe_cast<TextBlock^>(rootPage->FindName(L"HashingText"));
    
    String^ algName = algoNames->SelectionBoxItem->ToString();

    // Create a HashAlgorithmProvider object.
    MacAlgorithmProvider^ Algorithm = MacAlgorithmProvider::OpenAlgorithm(algName);
    IBuffer^ vector = CryptographicBuffer::DecodeFromBase64String("uiwyeroiugfyqcajkds897945234==");

    hashingText->Text += "\n*** Sample Hash Algorithm: " + Algorithm->AlgorithmName + "\n";
    hashingText->Text += "    Initial vector:  uiwyeroiugfyqcajkds897945234==\n";

    IBuffer^ hmacKeyMaterial = CryptographicBuffer::GenerateRandom(Algorithm->MacLength);

    // Compute the hash in one call.
    CryptographicHash^ hash = Algorithm->CreateHash(hmacKeyMaterial);
    hash->Append(vector);
    digest = hash->GetValueAndReset();

    hashingText->Text += "    Hash:  " + CryptographicBuffer::EncodeToHexString(digest) + "\n";

    return Algorithm->CreateHash(hmacKeyMaterial);
}

void Hashing::bHash_Checked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    AlgorithmNames->Items->Clear();
    AlgorithmNames->Items->Append(HashAlgorithmNames::Md5);
    AlgorithmNames->Items->Append(HashAlgorithmNames::Sha1);
    AlgorithmNames->Items->Append(HashAlgorithmNames::Sha256);
    AlgorithmNames->Items->Append(HashAlgorithmNames::Sha384);
    AlgorithmNames->Items->Append(HashAlgorithmNames::Sha512);
    AlgorithmNames->SelectedIndex = 0;
}

void Hashing::bHmac_Checked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    AlgorithmNames->Items->Clear();
    AlgorithmNames->Items->Append(MacAlgorithmNames::AesCmac);
    AlgorithmNames->Items->Append(MacAlgorithmNames::HmacMd5);
    AlgorithmNames->Items->Append(MacAlgorithmNames::HmacSha1);
    AlgorithmNames->Items->Append(MacAlgorithmNames::HmacSha256);
    AlgorithmNames->Items->Append(MacAlgorithmNames::HmacSha384);
    AlgorithmNames->Items->Append(MacAlgorithmNames::HmacSha512);
    AlgorithmNames->SelectedIndex = 0;
}
