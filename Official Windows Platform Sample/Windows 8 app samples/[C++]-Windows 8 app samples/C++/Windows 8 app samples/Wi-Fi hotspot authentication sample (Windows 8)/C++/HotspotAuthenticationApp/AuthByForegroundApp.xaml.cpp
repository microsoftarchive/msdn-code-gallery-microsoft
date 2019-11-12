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
// AuthByForegroundApp.xaml.cpp
// Implementation of the AuthByForegroundApp class
//

#include "pch.h"
#include "AuthByForegroundApp.xaml.h"
#include "ScenarioCommon.h"

using namespace SDKSample::HotspotAuthenticationApp;
using namespace HotspotAuthenticationTask;
using namespace Platform;
using namespace Windows::Networking::NetworkOperators;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

AuthByForegroundApp::AuthByForegroundApp()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void AuthByForegroundApp::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;

    // Configure background task handler to trigger foregound app for authentication
    ConfigStore::AuthenticateThroughBackgroundTask = false;

    // Setup completion handler
    ScenarioCommon::Instance->RegisteredCompletionHandlerForBackgroundTask();

    // Register handler to update UI state on authentication event
    auto backgroundTaskCompletedCallback = [this]()
    {
        this->InitializeForegroundAppAuthentication();
    };
    auto handler = ref new Windows::UI::Core::DispatchedHandler(backgroundTaskCompletedCallback, Platform::CallbackContext::Any);
    ScenarioCommon::Instance->SetForegroundAuthenticationEventHandler(handler);

    // Check current authenticatino state
    InitializeForegroundAppAuthentication();
}

// This is the click handler for the 'Authenticate' button.
void AuthByForegroundApp::AuthenticateButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    try
    {
        authenticationContext->IssueCredentials(
            ConfigStore::UserName,
            ConfigStore::Password,
            ConfigStore::ExtraParameters,
            ConfigStore::MarkAsManualConnect);
        rootPage->NotifyUser("Issuing credentials succeeded", NotifyType::StatusMessage);
    }
    catch (Exception^ ex)
    {
        rootPage->NotifyUser(ex->ToString(), NotifyType::ErrorMessage);
    }
    ClearAuthenticationToken();
}

// This is the click handler for the 'Skip' button.
void AuthByForegroundApp::SkipButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    try
    {
        authenticationContext->SkipAuthentication();
        rootPage->NotifyUser("Authentication skipped", NotifyType::StatusMessage);
    }
    catch (Exception^ ex)
    {
        rootPage->NotifyUser(ex->ToString(), NotifyType::ErrorMessage);
    }
    ClearAuthenticationToken();
}

// This is the click handler for the 'Abort' button.
void AuthByForegroundApp::AbortButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    try
    {
        authenticationContext->AbortAuthentication(ConfigStore::MarkAsManualConnect);
        rootPage->NotifyUser("Authentication aborted", NotifyType::StatusMessage);
    }
    catch (Exception^ ex)
    {
        rootPage->NotifyUser(ex->ToString(), NotifyType::ErrorMessage);
    }
    ClearAuthenticationToken();
}

// Query authentication token from application storage and upate the UI.
// The token gets passed from the background task handler.
void AuthByForegroundApp::InitializeForegroundAppAuthentication()
{
    auto token = ConfigStore::AuthenticationToken;
    if (token == "")
    {
        return; // no token found
    }
    if (!HotspotAuthenticationContext::TryGetAuthenticationContext(token, &authenticationContext))
    {
        rootPage->NotifyUser("TryGetAuthenticationContext failed", NotifyType::ErrorMessage);
        return;
    }

    AuthenticateButton->IsEnabled = true;
    SkipButton->IsEnabled = true;
    AbortButton->IsEnabled = true;
}

// Clear the authentication token in the application storage and update the UI.
void AuthByForegroundApp::ClearAuthenticationToken()
{
    ConfigStore::AuthenticationToken = "";
    AuthenticateButton->IsEnabled = false;
    SkipButton->IsEnabled = false;
    AbortButton->IsEnabled = false;
}
