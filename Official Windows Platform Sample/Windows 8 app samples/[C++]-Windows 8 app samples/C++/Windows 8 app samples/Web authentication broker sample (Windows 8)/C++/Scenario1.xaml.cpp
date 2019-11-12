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
// Scenario1.xaml.cpp
// Implementation of the Scenario1 class
//

#include "pch.h"
#include "Scenario1.xaml.h"

using namespace SDKSample::WebAuthentication;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

using namespace Concurrency;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Security::Authentication::Web;

Scenario1::Scenario1()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario1::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void SDKSample::WebAuthentication::Scenario1::Launch_Click(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    FacebookDebugArea->Text = "";
    FacebookReturnedToken->Text = "";

    String^ facebookURL = "https://www.facebook.com/dialog/oauth?client_id=";

    auto clientID = FacebookClientID->Text;
    if (clientID == nullptr || clientID->IsEmpty())
    {
        FacebookDebugArea->Text = "Enter a ClientID";
        return;
    }

    facebookURL += clientID + "&redirect_uri=https%3A%2F%2Fwww.facebook.com%2Fconnect%2Flogin_success.html&scope=read_stream&display=popup&response_type=token";

    try
    {
        auto facebookOutput = FacebookDebugArea;
        auto facebookToken = FacebookReturnedToken;

        auto startURI = ref new Uri(facebookURL);
        auto endURI = ref new Uri("https://www.facebook.com/connect/login_success.html");
        FacebookDebugArea->Text += "Navigating to: " + facebookURL + "\n";
        
        create_task(WebAuthenticationBroker::AuthenticateAsync(WebAuthenticationOptions::None, startURI, endURI)).then([this](WebAuthenticationResult^ result)
        {
            String^ statusString;
            switch (result->ResponseStatus)
            {
            case WebAuthenticationStatus::ErrorHttp:
                statusString = "ErrorHttp: " + result->ResponseErrorDetail;
                break;
            case WebAuthenticationStatus::Success:
                statusString = "Success";				
                FacebookReturnedToken->Text += result->ResponseData;
                break;
            case WebAuthenticationStatus::UserCancel:
                statusString = "UserCancel";
                break;
            }

            FacebookDebugArea->Text += "Status returned by WebAuth broker: " + statusString;						

         });
    }
    catch (Exception^ ex)
    {
        FacebookDebugArea->Text += "Error launching WebAuth " + ex->Message;
        return;
    }
}
