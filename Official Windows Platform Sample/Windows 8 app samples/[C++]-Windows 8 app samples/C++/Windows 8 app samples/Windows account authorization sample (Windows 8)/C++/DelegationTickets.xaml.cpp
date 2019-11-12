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
// DelegationTickets.xaml.cpp
// Implementation of the DelegationTickets class
//

#include "pch.h"
#include "DelegationTickets.xaml.h"

using namespace concurrency;
using namespace Platform;
using namespace SDKSample::MicrosoftAccount;
using namespace Windows::Security::Authentication::OnlineId;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

DelegationTickets::DelegationTickets()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void DelegationTickets::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;

    authenticator = ref new OnlineIdAuthenticator();
    
    AccessTicket = "";

    SignOutButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
}

void DelegationTickets::SignInButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    ResultText->Text = "Signing in...";
    AccessTicket = "";
    SignInButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;

    // the ticket requests specifies the Target scopes and the Policy, 
    // which must be configured per application with authentication service provider.
    auto request = ref new OnlineIdServiceTicketRequest("wl.basic wl.contacts_photos wl.calendars", "DELEGATION");

    // authenticate user and get the ticket for the requested target
    create_task(authenticator->AuthenticateUserAsync(request)).then([this](task<UserIdentity^> userIdentity)
    {
        SignOutButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
        try
        {
            // .get() will throw if the operation failed
            OnlineIdServiceTicket^ ticket = userIdentity.get()->Tickets->GetAt(0);
            if (ticket->ErrorCode == S_OK)
            {
                AccessTicket = ticket->Value;
            }
            SignInButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
            ResultText->Text = "Signed in.";
        }
        catch (const task_canceled &)
        {
            SignInButton->Visibility = Windows::UI::Xaml::Visibility::Visible;
            // programmatic cancel or in the case of this API user dismiss as well.
            ResultText->Text = "Canceled by user or programmatic cancel.";
            throw; // propagate the task_canceled exception, it will be ignored by the runtime.
        }

        auto uri = ref new Uri("https://apis.live.net/v5.0/me?access_token=" + AccessTicket);
        cancellationTokenSource = cancellation_token_source();

        // Do an asynchronous GET.  We need to use use_current() with the continuations since the tasks are completed on
        // background threads and we need to run on the UI thread to update the UI.
        return httpRequest.GetAsync(uri, cancellationTokenSource.get_token());
    }).then([this](task<std::wstring> response)
    {
        SignOutButton->Visibility = CanSignOut ? Windows::UI::Xaml::Visibility::Visible : Windows::UI::Xaml::Visibility::Collapsed;
        try
        {
            // .get() will throw if the operation failed
            std::wstring httpResponse = response.get();
            ResultText->Text = ref new String(httpResponse.c_str());
        }
        catch (const task_canceled&)
        {
            ResultText->Text = "Request canceled.";
        }
        catch (Exception^ ex) 
        {
            ResultText->Text = "HTTP Exception: " +  ex->Message;
        }
        // schedule this back to the UI thread to enable access to UI elements.
    }, task_continuation_context::use_current());
}

void DelegationTickets::SignOutButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    AccessTicket = "";
    
    if (CanSignOut)
    {
        ResultText->Text = "Signing out...";
        SignOutButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;

        create_task(authenticator->SignOutUserAsync()).then([this]()
        {
            ResultText->Text = "Signed out.";

            SignInButton->Visibility = NeedsToGetTicket ? Windows::UI::Xaml::Visibility::Visible : Windows::UI::Xaml::Visibility::Collapsed;
            SignOutButton->Visibility = CanSignOut ? Windows::UI::Xaml::Visibility::Visible : Windows::UI::Xaml::Visibility::Collapsed;
        });
    }
    else
    {
        ResultText->Text = "User can't sign out from Application.";
    }
}
