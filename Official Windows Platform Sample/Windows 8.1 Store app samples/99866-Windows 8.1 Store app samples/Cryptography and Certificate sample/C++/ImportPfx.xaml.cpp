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
// ImportPfx.xaml.cpp
// Implementation of the ImportPfx class
//

#include "pch.h"
#include "ImportPfx.xaml.h"

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


ImportPfx::ImportPfx()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void ImportPfx::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void ImportPfx::RunSample_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    OutputTextBlock->Text = L"Importing PFX certificate ...";
       
    String^ friendlyName = L"test pfx certificate";
    pfxPassword = PfxPasswordBox->Password;

    if (UserStoreCheckBox->IsChecked->Value)
    {
        // target store is User's Certificate Store
        // call User Certificate Enrollment function importPfxData to install the certificate
        UserCertificateEnrollmentManager^ mgr = CertificateEnrollmentManager::UserCertificateEnrollmentManager;
            
        create_task(mgr->ImportPfxDataAsync(pfxCertificate,
            pfxPassword,
            ExportOption::NotExportable,
            KeyProtectionLevel::NoConsent,
            InstallOptions::None,
            friendlyName))
            .then([this](task<void> t)
            {
                OutputTextBlock->Text += "\nCertificate installation succeeded. The certificate is in the User's certificate store";
            })
            .then([this](task<void> t)
            {
                try
                {
                    t.get();
                }
                catch(Platform::Exception^ e)
                {
                    OutputTextBlock->Text += L"\nFailed to install certificate. Error: " + e->Message;
                }
            });
    }
    else
    {
        // target store is App's certificate store
        // call Certificate Enrollment function importPFXData to install the certificate
        create_task(CertificateEnrollmentManager::ImportPfxDataAsync(pfxCertificate,
            pfxPassword,
            ExportOption::NotExportable,
            KeyProtectionLevel::NoConsent,
            InstallOptions::None,
            friendlyName))
            .then([this](task<void> t)
            {
                OutputTextBlock->Text += "\nCertificate installation succeeded. The certificate is in the App's certificate store";
            })
            .then([this](task<void> t)
            {
                try
                {
                    t.get();
                }
                catch(Platform::Exception^ e)
                {
                    OutputTextBlock->Text += L"\nFailed to install certificate. Error: " + e->Message;
                }
            });
    }        
}

void ImportPfx::Browse_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // create FileOpen picker with filter .pfx
    FileOpenPicker^ filePicker = ref new FileOpenPicker();

    filePicker->FileTypeFilter->Append(".pfx");
    filePicker->SuggestedStartLocation = PickerLocationId::DocumentsLibrary;

    rootPage = MainPage::Current;
    TextBlock^ outputTextBlock =  safe_cast<TextBlock^>(rootPage->FindName(L"OutputTextBlock"));

    try
    {
        create_task(filePicker->PickSingleFileAsync()).then([this](StorageFile^ file)
        {
            if (file)
            {
                // file was picked and is available for read
                // try to read the file content
                create_task(FileIO::ReadBufferAsync(file)).then([this, file](task<IBuffer^> task)
                {
                    IBuffer^ buffer = task.get();

                    pfxCertificate = CryptographicBuffer::EncodeToBase64String(buffer);
                });
                // convert to Base64 for using with ImportPfx
                
                
                // update UI elements
                TextBox^ PfxFileName =  safe_cast<TextBox^>(rootPage->FindName(L"PfxFileName"));
                StackPanel^ spPassword =  safe_cast<StackPanel^>(rootPage->FindName(L"PasswordPanel"));
                StackPanel^ spImport =  safe_cast<StackPanel^>(rootPage->FindName(L"ImportPanel"));
                    
                PfxFileName->Text = file->Path;
                spPassword->Visibility = Windows::UI::Xaml::Visibility::Visible;
                spImport->Visibility = Windows::UI::Xaml::Visibility::Visible;
            }
        });

    }
    catch (Exception^ ex)
    {
        outputTextBlock->Text += "\nPFX file selection failed with error: " + ex->ToString();
    }
}