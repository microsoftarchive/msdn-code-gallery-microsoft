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
// Enroll.xaml.cpp
// Implementation of the Enroll class
//

#include "pch.h"
#include "Enroll.xaml.h"

using namespace std;
using namespace SDKSample::CryptographyAndCertificate;
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

Enroll::Enroll()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Enroll::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void Enroll::RunSample_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Platform::String^ url = "";
    
    OutputTextBlock->Text = "";

    if (UrlTextBox->Text != nullptr || UrlTextBox->Text != "")
    {
        try
        {
            Windows::Foundation::Uri^ uri = ref new Windows::Foundation::Uri(UrlTextBox->Text);
            url = UrlTextBox->Text;
        }
        catch(Platform::Exception^ e)
        {
            OutputTextBlock->Text = "A valid URL is not provided, so request will be created but will not be submitted.";
        }

        if (url == "")
            return;
    }
    else
    {
        OutputTextBlock->Text = "A valid URL is not provided, so request will be created but will not be submitted.";
    }


    if (UserStoreCheckBox->IsChecked->Value)
        RunSampleUserEnroll(url);
    else
        RunSampleAppEnroll(url);
    
}

Platform::String^ Enroll::FormatHttpRequest(Platform::String^ encodedRequest)
{
    if (encodedRequest == "")
        return "";

    ////Depends on the server implementaion, the XML format could be different
    return L"<SubmitRequest xmlns=\"http://tempuri.org/\"><request>" + encodedRequest + "</request>"
                    + "<attributes xmlns:a=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\"><a:KeyValueOfstringstring><a:Key>CertificateTemplate</a:Key><a:Value>WebServer</a:Value></a:KeyValueOfstringstring></attributes>"
                    + "</SubmitRequest>";
}

Platform::String^ Enroll::GetCertFromXmlResponse(Platform::String^ xmlResponse)
{
    XmlDocument^ xDoc = ref new XmlDocument();
        
    xDoc->LoadXml(xmlResponse);
    auto nodeList = xDoc->GetElementsByTagName("SubmitRequestResult");
    if (nodeList->Length != 1)
    {
        return L"Error: The certificate response is not in expected XML format";;
    }

    String^ certResponse = nodeList->GetAt(0)->InnerText;
    if (certResponse->IsEmpty())
    {
        return L"Error: Submit request succeeded but the returned response is empty";
    }
    else
    {
        return certResponse;
    }

}

void Enroll::RunSampleUserEnroll(Platform::String^ url)
{
    //call the default constructor of CertificateRequestProperties
    CertificateRequestProperties^ reqProp = ref new CertificateRequestProperties();
    reqProp->Subject = L"Toby";
    reqProp->FriendlyName = L"Toby's Cert";

    OutputTextBlock->Text += L"\nCreating certificate request...";

    // have to use User's Certificate Store
    // call User Certificate Enrollment function createRequest to create a certificate request
    UserCertificateEnrollmentManager^ mgr = CertificateEnrollmentManager::UserCertificateEnrollmentManager;

    create_task(mgr->CreateRequestAsync(reqProp))
        .then([this](String^ req)
        {
            OutputTextBlock->Text += L"\nRequest created for User's certificate store, content:\n" + req;
            certRequest = FormatHttpRequest(req);
        })
        .then([this, url] (void)
        {
            if (certRequest == "" || url == nullptr || url == "")
                return;

            OutputTextBlock->Text += L"\nSubmitting request to Enrollment URL ...";

            Uri^ uri = ref new Uri(url);    
            Web::HttpRequest request;

            request.PostAsync(uri, L"text/xml;charset=utf-8", certRequest->Data())
                .then([this](task<std::wstring> response)
                {
                    try
                    {
                        String^ xmlResponse = ref new String(response.get().c_str());
                        String^ certResponse = GetCertFromXmlResponse(xmlResponse);

                        if (wcsstr(certResponse->Data(), L"Error:"))
                        {
                            //there was an error while getting response from enrollment server
                            OutputTextBlock->Text += certResponse;
                            return;
                        }

                        OutputTextBlock->Text += "\nResponse received, content: \n" + certResponse;
                        OutputTextBlock->Text += L"\nInstalling certificate ...";

                        UserCertificateEnrollmentManager^ mgr = CertificateEnrollmentManager::UserCertificateEnrollmentManager;

                        create_task(mgr->InstallCertificateAsync(certResponse, InstallOptions::None)).then([this](task<void> t)
                        {
                            OutputTextBlock->Text += L"\nThe certificate response is installed sucessfully in User's certificate store";
                        })
                        .then( [this] (task<void> t)
                        {
                            try
                            {
                                t.get();
                            }
                            catch(Platform::InvalidArgumentException^ e)
                            {
                                OutputTextBlock->Text += L"\nFailed to install certificate. Error: " + e->Message;
                            }
                        });
                    }
                    catch(Exception^ ex)
                    {
                        OutputTextBlock->Text += ex->Message;
                    }
                }, task_continuation_context::use_current());
        })
        .then( [this] (task<void> t)
        {
            try
            {
                t.get();
            }
            catch(Platform::InvalidArgumentException^ e)
            {
                OutputTextBlock->Text += L"\n\nCertificate request creation for Users's certificate store failed with error: " + e->Message;
            }
        });
}

void Enroll::RunSampleAppEnroll(Platform::String^ url)
{ 
    //call the default constructor of CertificateRequestProperties
    CertificateRequestProperties^ reqProp = ref new CertificateRequestProperties();
    reqProp->Subject = L"Toby";
    reqProp->FriendlyName = L"Toby's Cert";

    OutputTextBlock->Text += L"\nCreating certificate request...";

    // have to use App's Certificate Store
    // call Certificate Enrollment function createRequest to create a certificate request
    create_task(CertificateEnrollmentManager::CreateRequestAsync(reqProp))
        .then([this](String^ req)
        {
            OutputTextBlock->Text += L"\nRequest created for App's certificate store, content:\n" + req;
            certRequest = FormatHttpRequest(req);
        })
        .then([this, url] (void)
        {
            if (certRequest == "" || url == nullptr || url == "")
                return;

            OutputTextBlock->Text += L"\nSubmitting request to Enrollment URL ...";

            Uri^ uri = ref new Uri(url);    
            Web::HttpRequest request;

            request.PostAsync(uri, L"text/xml;charset=utf-8", certRequest->Data())
                .then([this](task<std::wstring> response)
                {
                    try
                    {
                        String^ xmlResponse = ref new String(response.get().c_str());
                        String^ certResponse = GetCertFromXmlResponse(xmlResponse);

                        if (wcsstr(certResponse->Data(), L"Error:"))
                        {
                            //there was an error while getting response from enrollment server
                            OutputTextBlock->Text += certResponse;
                            return;
                        }

                        OutputTextBlock->Text += "\nResponse received, content: \n" + certResponse;

                        OutputTextBlock->Text += L"\nInstalling certificate ...";

                        create_task(CertificateEnrollmentManager::InstallCertificateAsync(certResponse, InstallOptions::None)).then([this](task<void> t)
                        {
                            OutputTextBlock->Text += L"\nThe certificate response is installed sucessfully in App's certificate store";
                        })
                        .then( [this] (task<void> t)
                        {
                            try
                            {
                                t.get();
                            }
                            catch(Platform::InvalidArgumentException^ e)
                            {
                                OutputTextBlock->Text += L"\nFailed to install certificate. Error: " + e->Message;
                            }
                        });
                    }
                    catch(Exception^ ex)
                    {
                        OutputTextBlock->Text += ex->Message;
                    }
                }, task_continuation_context::use_current());
        })
        .then( [this] (task<void> t)
        {
            try
            {
                t.get();
            }
            catch(Platform::InvalidArgumentException^ e)
            {
                OutputTextBlock->Text += L"\n\nCertificate request creation for App's certificate store failed with error: " + e->Message;
            }
        });
}
