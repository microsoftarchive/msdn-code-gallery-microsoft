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
// GetSIPUri.xaml.cpp
// Implementation of the GetSIPUri class
//

#include "pch.h"
#include "GetSIPUri.xaml.h"
#include <ppltasks.h>

using namespace Windows::System::UserProfile;
using namespace Windows::Foundation;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Platform;
using namespace SDKSample::UserDomainName;
using namespace concurrency;

GetSIPUri::GetSIPUri()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void GetSIPUri::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}


void SDKSample::UserDomainName::GetSIPUri::GetUri_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // The user or domain administrator may disable access to personal information by apps
    if (UserInformation::NameAccessAllowed)
    {
        UriResultText->Text = "Getting sip uri.";
        create_task(UserInformation::GetSessionInitiationProtocolUriAsync()).then([this](Uri^ sipUri)
        {
            // a null uri may be returned in any of the following circumstances:
            // The information cannot be retrieved from the directory
            // The calling user is not a member of a domain
            // The user or administrator has disabled the privacy setting
            UriResultText->Text = sipUri ? sipUri->RawUri : "No SIP Uri returned for the current user.";
        });
    }
    else
    {
        UriResultText->Text = "Access to user information is disabled by the user or administrator";
    }
}
