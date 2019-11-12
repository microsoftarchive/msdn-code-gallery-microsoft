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
// ViewCert.xaml.cpp
// Implementation of the ViewCert class
//

#include "pch.h"
#include "ViewCert.xaml.h"

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

ViewCert::ViewCert()
{
    InitializeComponent();
    EnumerateCertificateList();
    EnumerateVerifyList();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void ViewCert::OnNavigatedTo(NavigationEventArgs^ e)
{
    rootPage = MainPage::Current;
}

void ViewCert::EnumerateVerifyList()
{
    VerifyCert->Items->Clear();
    VerifyCert->Items->Append("Verify Certificate");
    VerifyCert->Items->Append("Sign/Verify using certificate key");
    VerifyCert->Items->Append("Sign/Verify using CMS based format");
    VerifyCert->Items->Append("Get certificate and show details");
    VerifyCert->SelectedIndex = 0;
}

void ViewCert::EnumerateCertificateList()
{
    CertificateList->Visibility = Windows::UI::Xaml::Visibility::Visible;
    CertificateList->Items->Clear();
    
    create_task(CertificateStores::FindAllAsync()).then([this](Windows::Foundation::Collections::IVectorView<Windows::Security::Cryptography::Certificates::Certificate ^> ^ vecCertList)
    {
        certList = vecCertList;
        CertificateList->Items->Clear();

        for (uint32 i = 0; i < certList->Size; i++)
        {
            Certificate^ cert = certList->GetAt(i);
            CertificateList->Items->Append(cert->Subject);
        }        
    
        if (CertificateList->Items->Size == 0)
        {
            VerifyCert->IsEnabled = false;
            RunSample->IsEnabled = false;
            CertificateList->Items->Append("No certificates found");
        }
        else
        {
            VerifyCert->IsEnabled = true;
            RunSample->IsEnabled = true;
        }
        CertificateList->SelectedIndex = 0;
    });
}



/// <summary>
/// This is the click handler for the 'RunSample' button.  It is responsible for executing the sample code.
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
void ViewCert::RunSample_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Certificate^ selectedCertificate = nullptr;
    Platform::String^ verifyselection = VerifyCert->SelectionBoxItem->ToString();

    //get the selected certificate
    if (CertificateList->SelectedIndex >= 0 && safe_cast<unsigned int>(CertificateList->SelectedIndex) < certList->Size)
    {
        selectedCertificate = certList->GetAt(CertificateList->SelectedIndex);
    }

    if (selectedCertificate == nullptr)
    {
        ViewCertText->Text = "Please select a certificate first.";
        return;
    }

    // a certificate was selected, do the desired operation
    if (verifyselection->Equals("Verify Certificate"))
    {
        //Build the chain
        create_task(selectedCertificate->BuildChainAsync(nullptr, nullptr)).then([this](CertificateChain^ chain)
        {
            //Validate the chain
            ChainValidationResult result = chain->Validate();

            ViewCertText->Text += "\n Verification Result :" + result.ToString();
        });
               
    }
    else if (verifyselection->Equals("Sign/Verify using certificate key"))
    {
        // get private key
        create_task(PersistedKeyProvider::OpenKeyPairFromCertificateAsync(selectedCertificate, HashAlgorithmNames::Sha1, CryptographicPadding::RsaPkcs1V15))
            .then([this](CryptographicKey^ keyPair)
        {
            String^ cookie = "Some Data to sign";
            IBuffer^ Data = CryptographicBuffer::ConvertStringToBinary(cookie, BinaryStringEncoding::Utf16BE);

            try
            {
                //sign the data by using the key
                IBuffer^ Signed = CryptographicEngine::Sign(keyPair, Data);
                bool bresult = CryptographicEngine::VerifySignature(keyPair, Data, Signed);

                if (bresult == true)
                {
                    ViewCertText->Text += "\n Verification Result : Successfully signed and verified signature";
                }
                else
                {
                    ViewCertText->Text += "\n Verification Result : Verify Signature Failed";
                }
            }
            catch (Exception^ ex)
            {
                ViewCertText->Text += "\n Verification Failed. Exception Occurred :" + ex->Message;
            }
        });        
    }
    else if(verifyselection->Equals("Get certificate and show details"))
    {
        DisplayCertificate(selectedCertificate);
    }   
    else if (verifyselection->Equals("Sign/Verify using CMS based format"))
    {
        InMemoryRandomAccessStream^ originalData = ref new InMemoryRandomAccessStream();

        //Populate the new memory stream
        create_task(originalData->WriteAsync(CryptographicBuffer::GenerateRandom(10000))).then([this, originalData, selectedCertificate](size_t c)
        {
            if (c <= 0)
            {
                ViewCertText->Text += "\nCouldn't write to random stream\n";
                return;
            }

            create_task(originalData->FlushAsync()).then([this, originalData, selectedCertificate](bool res)
            {
                if (!res)
                {
                    ViewCertText->Text += "\nCouldn't flush to random stream\n";
                    return;
                }

                IInputStream^ pdfInputstream = originalData->GetInputStreamAt(0);
        
                CmsSignerInfo^ signer = ref new CmsSignerInfo();
                signer->Certificate = selectedCertificate;
                signer->HashAlgorithmName = HashAlgorithmNames::Sha1;

                Windows::Foundation::Collections::IVector<CmsSignerInfo^>^ signers = ref new Platform::Collections::Vector<CmsSignerInfo^>();
                signers->Append(signer);

                create_task(CmsDetachedSignature::GenerateSignatureAsync(pdfInputstream, signers, nullptr)).then([this, originalData, pdfInputstream](IBuffer^ signature)
                {
                    if (signature)
                    {
                        CmsDetachedSignature^ cmsSignedData = ref new CmsDetachedSignature(signature);
                        IInputStream^ pdfInputstream = originalData->GetInputStreamAt(0);

                        create_task(cmsSignedData->VerifySignatureAsync(pdfInputstream)).then([this, originalData, signature](SignatureValidationResult validationResult)
                        {
                            if (SignatureValidationResult::Success == validationResult)
                            {
                                ViewCertText->Text +=  "\n Verification Result : Successfully signed and verified Signature";
                            }
                            else
                            {
                                ViewCertText->Text += "\n Verification Result : Verify Signature using CMS based format Failed";
                            }
                        });
                    }
                    else 
                    {
                        ViewCertText->Text +=  "\n Failed to Sign";
                    }
                });
            });
        });
    }         
}

/// <summary>
/// This function displays the selected Certificate details
/// </summary>
void ViewCert::DisplayCertificate(Certificate^ selectedcertificate)
{
    try
    {
        Windows::Globalization::DateTimeFormatting::DateTimeFormatter dateformatter("longdate");
        Windows::Globalization::DateTimeFormatting::DateTimeFormatter timeformatter("longtime");

        //this function displays the certificate details
        ViewCertText->Text = " ";
        ViewCertText->Text += "\n Certificate Selected :";
        ViewCertText->Text += "\n";
        ViewCertText->Text += "\n Subject : " + selectedcertificate->Subject;
        ViewCertText->Text += "\n Issuer  : " + selectedcertificate->Issuer;
        ViewCertText->Text += "\n Friendly Name : " + selectedcertificate->FriendlyName;
        ViewCertText->Text += "\n Thumbprint : " + CryptographicBuffer::EncodeToHexString(CryptographicBuffer::CreateFromByteArray(selectedcertificate->GetHashValue()));
        ViewCertText->Text += "\n Serial Number : " + CryptographicBuffer::EncodeToHexString(CryptographicBuffer::CreateFromByteArray(selectedcertificate->SerialNumber));
        ViewCertText->Text += "\n ValidFrom : " + dateformatter.Format(selectedcertificate->ValidFrom) + " : " + timeformatter.Format(selectedcertificate->ValidFrom);
        ViewCertText->Text += "\n ValidTo : " + dateformatter.Format(selectedcertificate->ValidTo)  + " : " + timeformatter.Format(selectedcertificate->ValidTo);;
    }
    catch (Exception^ ex)
    {
        ViewCertText->Text += "Excepted while displaying Certificate Information\nException: " + ex->Message;
    }
}

void ViewCert::CertificateList_SelectionChanged(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    ViewCertText->Text = "";
}

void ViewCert::VerifyCert_SelectionChaged(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    ViewCertText->Text  = "";
}
