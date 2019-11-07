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
// Scenario_Enroll.xaml.cpp
// Implementation of the Scenario_Enroll class
//

#include "pch.h"
#include "Scenario_Enroll.xaml.h"

using namespace std;
using namespace SDKSample::CertificateEnrollment;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Security::Cryptography::Certificates;
using namespace Windows::Foundation;
using namespace Concurrency;
using namespace Platform;
using namespace Microsoft::WRL::Details;
using namespace Microsoft::WRL;
using namespace Windows::Storage::Streams;
using namespace Windows::Data::Xml::Dom;


Scenario_Enroll::Scenario_Enroll()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario_Enroll::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void SDKSample::CertificateEnrollment::Scenario_Enroll::CreateRequest_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    rootPage = MainPage::Current;
    TextBlock^ outputTextBlock =  safe_cast<TextBlock^>(rootPage->FindName(L"OutputTextBlock"));
        
    outputTextBlock->Text = L"Creating certificate request...";

    //call the default constructor of CertificateRequestProperties
    CertificateRequestProperties^ reqProp = ref new CertificateRequestProperties();
    reqProp->Subject = L"Toby";
    reqProp->FriendlyName = L"Toby's Cert";

    try{
        //call Certificate Enrollment function createRequest to create a certificate request
        create_task(CertificateEnrollmentManager::CreateRequestAsync(reqProp))
            .then([this, outputTextBlock](String^ req) {
            this->certificateRequest = req;
            outputTextBlock->Text += L"\nRequest created, content:\n" + certificateRequest;
        })
        .then([this, outputTextBlock] (task<void> t)
        {
            try
            {
                t.get();
            }
            catch(Platform::Exception^ e)
            {
                outputTextBlock->Text += L"\n\nCertificate request creation failed with error: " + e->Message;
            }
        }); 
    }
    catch(Platform::Exception^ e)
    {
        outputTextBlock->Text += L"\n\nCertificate request creation failed with error: " + e->Message;
    }
}

void SDKSample::CertificateEnrollment::Scenario_Enroll::InstallCertifiate_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    String^ response = "";

    rootPage = MainPage::Current;
    TextBlock^ outputTextBlock =  safe_cast<TextBlock^>(rootPage->FindName(L"OutputTextBlock"));
        
            
    if (certificateRequest->IsEmpty())
    {
        outputTextBlock -> Text = L"\nYou need to create a certificate request first";
        return;
    }

    // To submit request, a valid url need to be provided. the url needs to point to a server which can take certificate request and issue certs
    String^ url = "";
    // Add code here to initialize url

    if (url->IsEmpty())
    {
        outputTextBlock->Text = L"\nTo submit a request, please update the code provide a valid URL.";
        return;
    }
    outputTextBlock->Text = L"Submitting request to server...";

    try
    {     
        SubmitCertificateRequestAndGetResponse(this->certificateRequest, url);                
    }
    catch(...)
    {
        outputTextBlock->Text += "\n\nCertificate Installation failed with error:";
    }
}

void Scenario_Enroll::SubmitCertificateRequestAndGetResponse(String^ certificateRequest, String^ url)
{
    rootPage = MainPage::Current;
    TextBlock^ outputTextBlock =  safe_cast<TextBlock^>(rootPage->FindName(L"OutputTextBlock"));

    ////Depends on the server implementaion, the XML format could be different
    String^ body = L"<SubmitRequest xmlns=\"http://tempuri.org/\"><strRequest>" + certificateRequest + L"</strRequest></SubmitRequest>";
    
    Uri^ uri = ref new Uri(url);    
    httpRequest.PostAsync(uri, L"text/xml;charset=utf-8", body->Data()).then([this](task<std::wstring> response)
    {
        TextBlock^ outputTextBlock =  safe_cast<TextBlock^>(rootPage->FindName(L"OutputTextBlock"));
        String^ xml = ref new String(response.get().c_str());
        
        if (200 != httpRequest.GetStatusCode())
        {
            outputTextBlock -> Text += L"\nSubmit request finished, but the returned status code indicated an error.\n\n";
            outputTextBlock -> Text += xml;
            return;
        }

        XmlDocument^ xDoc = ref new XmlDocument();
        
        xDoc->LoadXml(xml);
        auto nodeList = xDoc->GetElementsByTagName("SubmitRequestResult");
        if (nodeList->Length != 1)
        {
            outputTextBlock -> Text = L"The certificate response is not in expected XML format";
            return;
        }

        String^ certResponse = nodeList->GetAt(0)->InnerText;
        if (certResponse->IsEmpty())
        {
            outputTextBlock->Text += L"\nSubmit request succeeded but the returned response is empty.";
        }
            
        outputTextBlock->Text += L"\nResponse received, content: \n" + certResponse;

        // install
        outputTextBlock->Text += L"\nInstalling certificate ...";
        create_task(CertificateEnrollmentManager::InstallCertificateAsync(certResponse, InstallOptions::None))
        .then([outputTextBlock]()
        {
            outputTextBlock->Text += L"\nThe certificate response is installed sucessfully.\n";
        })
        .then([this, outputTextBlock] (task<void> t)
        {
            try
            {
                t.get();
            }
            catch(Platform::Exception^ ex)
            {
                outputTextBlock->Text += "\n\nCertificate Installation failed with error: " + ex->Message + "\n";
            }
        }); 

    }, task_continuation_context::use_current());
}
