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
// Scenario_Import.xaml.cpp
// Implementation of the Scenario_Import class
//

#include "pch.h"
#include "Scenario_Import.xaml.h"

using namespace SDKSample::CertificateEnrollment;
using namespace Platform;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::ApplicationModel::Resources;
using namespace Windows::Security::Cryptography::Certificates;
using namespace Concurrency;

Scenario_Import::Scenario_Import()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario_Import::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void SDKSample::CertificateEnrollment::Scenario_Import::ImportPfx_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	rootPage = MainPage::Current;
	TextBlock^ outputTextBlock =  safe_cast<TextBlock^>(rootPage->FindName(L"OutputTextBlock"));

    try
    {
        outputTextBlock->Text = L"Importing PFX certificate ...";
        
        // Load the pfx certificate from resource string.
        ResourceLoader^ rl = ref new ResourceLoader();
        String^ pfxCertificate = rl->GetString(L"Certificate");
            
        String^ password = L"sampletest";     //password to access the certificate in PFX format
        String^ friendlyName = L"test pfx certificate";


        //call Certificate Enrollment funciton importPFXData to install the certificate
        create_task(CertificateEnrollmentManager::ImportPfxDataAsync(pfxCertificate,
            password,
            ExportOption::NotExportable,
            KeyProtectionLevel::NoConsent,
            InstallOptions::None,
            friendlyName))
            .then([outputTextBlock]()
        {
            outputTextBlock->Text += "\nCertificate installation succeeded. The certificate is in the appcontainer Personal certificate store";
        });
    }
    catch (Platform::Exception^ ex)
    {
        outputTextBlock->Text += "\nCertificate installation failed with error: " + ex->ToString();
    }
}

