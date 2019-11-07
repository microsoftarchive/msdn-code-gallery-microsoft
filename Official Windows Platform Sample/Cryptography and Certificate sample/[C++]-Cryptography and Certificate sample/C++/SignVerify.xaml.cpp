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
// SignVerify.xaml.cpp
// Implementation of the SignVerify class
//

#include "pch.h"
#include "SignVerify.xaml.h"

using namespace std;
using namespace SDKSample::CryptographyAndCertificate;
using namespace Platform;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::ApplicationModel::Resources;
using namespace Windows::Security::Cryptography;
using namespace Windows::Security::Cryptography::Certificates;
using namespace Concurrency;
using namespace Windows::Storage::Pickers;
using namespace Windows::Storage;
using namespace Windows::Storage::Streams;
using namespace Windows::Security::Cryptography::Core;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

SignVerify::SignVerify()
{
    InitializeComponent();
    AlgorithmNames->SelectedIndex = 0;
    KeySizes->SelectedIndex = 0;
    bAsymmetric->IsChecked = true;
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void SignVerify::OnNavigatedTo(NavigationEventArgs^ e)
{
    rootPage = MainPage::Current;
}

/// <summary>
/// This is the click handler for the 'RunSample' button.  It is responsible for executing the sample code.
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
void SignVerify::RunSample_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    rootPage = MainPage::Current;
    TextBlock^ signVerifyText = safe_cast<TextBlock^>(rootPage->FindName(L"SignVerifyText"));

    SignVerifyText->Text = "";

    String^ cookie = "Some Data to sign";
    IBuffer^ Data = CryptographicBuffer::ConvertStringToBinary(cookie, BinaryStringEncoding::Utf16BE);

    CryptographicKey^ key = nullptr;
    if (bHmac->IsChecked->Value)
        key = GenerateHMACKey();
    else
        key = GenerateAsymmetricKey();

    if (key != nullptr)
    {
        // Sign the data by using the generated key.
        IBuffer^ Signature = CryptographicEngine::Sign(key, Data);
        signVerifyText->Text += "    Data was successfully signed.\n";

        // Verify the signature by using the public key.
        if (!CryptographicEngine::VerifySignature(key, Data, Signature))
        {
            signVerifyText->Text += "Signature verification failed!";
            return;
        }
        signVerifyText->Text += "    Signature was successfully verified.\n";
    }
}

void SignVerify::AsymmetricImportExport(CryptographicKey^ keyPair)
{
    rootPage = MainPage::Current;
    ComboBox^ algoNames = safe_cast<ComboBox^>(rootPage->FindName(L"AlgorithmNames"));
    TextBlock^ signVerifyText = safe_cast<TextBlock^>(rootPage->FindName(L"SignVerifyText"));


    String^ algName = algoNames->SelectedItem->ToString();
    AsymmetricKeyAlgorithmProvider^ Algorithm = AsymmetricKeyAlgorithmProvider::OpenAlgorithm(algName);

    // Export the public key.
    IBuffer^ blobOfPublicKey = keyPair->ExportPublicKey();
    IBuffer^ blobOfKeyPair = keyPair->Export();
    signVerifyText->Text += "    Key pair was successfully exported.\n";

    // Import the public key.
    CryptographicKey^ keyPublic = Algorithm->ImportPublicKey(blobOfPublicKey);

    // Check the key size.
    if (keyPublic->KeySize != keyPair->KeySize)
    {
        signVerifyText->Text += "ImportPublicKey failed!  The imported key's size did not match the original's!";
        return;
    }
    signVerifyText->Text += "    Public key was successfully imported.\n";

    // Import the key pair.
    keyPair = Algorithm->ImportKeyPair(blobOfKeyPair);

    // Check the key size.
    if (keyPublic->KeySize != keyPair->KeySize)
    {
        signVerifyText->Text += "ImportKeyPair failed!  The imported key's size did not match the original's!";
        return;
    }
    signVerifyText->Text += "    Key pair was successfully imported.\n";

}

CryptographicKey^ SignVerify::GenerateHMACKey()
{
    rootPage = MainPage::Current;
    ComboBox^ algoNames = safe_cast<ComboBox^>(rootPage->FindName(L"AlgorithmNames"));
    TextBlock^ signVerifyText = safe_cast<TextBlock^>(rootPage->FindName(L"SignVerifyText"));

    Platform::String^ algName = ref new Platform::String(algoNames->SelectedItem->ToString()->Data());

    // Created a MacAlgorithmProvider object for the algorithm specified on input.
    MacAlgorithmProvider^ algorithm = MacAlgorithmProvider::OpenAlgorithm(algName);

    signVerifyText->Text += "*** Sample Hmac Algorithm: " + algorithm->AlgorithmName + "\n";

    // Create a key.
    IBuffer^ keymaterial = CryptographicBuffer::GenerateRandom(algorithm->MacLength);
    return algorithm->CreateKey(keymaterial);
}

CryptographicKey^ SignVerify::GenerateAsymmetricKey()
{
    String^ algName = AlgorithmNames->SelectedItem->ToString();
    uint32 keySize = _wtoi(KeySizes->SelectedItem->ToString()->Data());

    CryptographicKey^ keyPair;
    // Create an AsymmetricKeyAlgorithmProvider object for the algorithm specified on input.
    AsymmetricKeyAlgorithmProvider^ Algorithm = AsymmetricKeyAlgorithmProvider::OpenAlgorithm(algName);

    SignVerifyText->Text += "*** Sample Signature Algorithm\n";
    SignVerifyText->Text += "    Algorithm Name: " + Algorithm->AlgorithmName + "\n";
    SignVerifyText->Text += "    Key Size: " + keySize + "\n";

    // Generate a key pair.
    try
    {
        keyPair = Algorithm->CreateKeyPair(keySize);
    }
    catch (Exception^ ex)
    {
        SignVerifyText->Text += ex->Message + "\n";
        SignVerifyText->Text += "An invalid key size was specified for the given algorithm.";
        return nullptr;
    }
    return keyPair;
}

void SignVerify::bHmac_Checked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    AlgorithmNames->Items->Clear();
    AlgorithmNames->Items->Append(MacAlgorithmNames::AesCmac);
    AlgorithmNames->Items->Append(MacAlgorithmNames::HmacMd5);
    AlgorithmNames->Items->Append(MacAlgorithmNames::HmacSha1);
    AlgorithmNames->Items->Append(MacAlgorithmNames::HmacSha256);
    AlgorithmNames->Items->Append(MacAlgorithmNames::HmacSha384);
    AlgorithmNames->Items->Append(MacAlgorithmNames::HmacSha512);
    AlgorithmNames->SelectedIndex = 0;

    KeySizes->SelectedIndex = -1;
    KeySizes->IsEnabled = false;
}

void SignVerify::bAsymmetric_Checked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    AlgorithmNames->Items->Clear();
    AlgorithmNames->Items->Append(AsymmetricAlgorithmNames::EcdsaP256Sha256);
    AlgorithmNames->Items->Append(AsymmetricAlgorithmNames::EcdsaP384Sha384);
    AlgorithmNames->Items->Append(AsymmetricAlgorithmNames::EcdsaP521Sha512);
    AlgorithmNames->Items->Append(AsymmetricAlgorithmNames::RsaSignPkcs1Sha1);
    AlgorithmNames->Items->Append(AsymmetricAlgorithmNames::RsaSignPkcs1Sha256);
    AlgorithmNames->Items->Append(AsymmetricAlgorithmNames::RsaSignPkcs1Sha384);
    AlgorithmNames->Items->Append(AsymmetricAlgorithmNames::RsaSignPkcs1Sha512);
    AlgorithmNames->Items->Append(AsymmetricAlgorithmNames::RsaSignPssSha1);
    AlgorithmNames->Items->Append(AsymmetricAlgorithmNames::RsaSignPssSha256);
    AlgorithmNames->Items->Append(AsymmetricAlgorithmNames::RsaSignPssSha384);
    AlgorithmNames->Items->Append(AsymmetricAlgorithmNames::RsaSignPssSha512);
    AlgorithmNames->Items->Append(AsymmetricAlgorithmNames::DsaSha1);
    AlgorithmNames->SelectedIndex = 0;

    KeySizes->Items->Clear();
    KeySizes->Items->Append("256");
    KeySizes->Items->Append("384");
    KeySizes->Items->Append("521");
    KeySizes->Items->Append("1024");
    KeySizes->Items->Append("2048");
    KeySizes->Items->Append("3072");
    KeySizes->Items->Append("4096");
    KeySizes->SelectedIndex = 0;
    KeySizes->IsEnabled = true;
}

