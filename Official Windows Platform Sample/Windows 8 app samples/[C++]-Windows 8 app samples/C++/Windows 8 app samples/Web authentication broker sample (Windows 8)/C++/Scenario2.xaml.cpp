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

using namespace SDKSample::WebAuthentication;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

using namespace Concurrency;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Security::Authentication::Web;

Scenario2::Scenario2()
{
    InitializeComponent();
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

void SDKSample::WebAuthentication::Scenario2::Launch_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    GoogleDebugArea->Text = "";
    String^ googleURL = "https://accounts.google.com/o/oauth2/auth?";
    auto clientID = GoogleClientID->Text;
    if (clientID == nullptr || clientID->IsEmpty())
    {
        GoogleDebugArea->Text = "Enter a ClientID for Google";
        return;
    }

    googleURL += "client_id=" + clientID + "&redirect_uri=urn:ietf:wg:oauth:2.0:oob&response_type=code&scope=http%3A%2F%2Fpicasaweb.google.com%2Fdata";

    try
    {        
        auto startURI = ref new Uri(googleURL);
        auto endURI = ref new Uri("https://accounts.google.com/o/oauth2/approval?");
        GoogleDebugArea->Text += "Navigating to: " + googleURL + "\n";

        create_task(WebAuthenticationBroker::AuthenticateAsync(WebAuthenticationOptions::UseTitle, startURI, endURI)).then([this](WebAuthenticationResult^ result)
        {
            String^ statusString;
            switch (result->ResponseStatus)
            {
            case WebAuthenticationStatus::ErrorHttp:
                statusString = "ErrorHttp: " + result->ResponseErrorDetail;
                break;
            case WebAuthenticationStatus::Success:
                statusString = "Success";
				GoogleReturnedToken->Text += result->ResponseData;
                break;
            case WebAuthenticationStatus::UserCancel:
                statusString = "UserCancel";
                break;
            }

            GoogleDebugArea->Text += "Status returned by WebAuth broker: " + statusString;			
        });
    }
    catch (Exception^ ex)
    {
        GoogleDebugArea->Text += "Error launching WebAuth " + ex->Message;
        return;
    }
}
