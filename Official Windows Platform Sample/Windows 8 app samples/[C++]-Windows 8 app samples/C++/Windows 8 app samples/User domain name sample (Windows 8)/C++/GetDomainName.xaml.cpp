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
// GetDomainName.xaml.cpp
// Implementation of the GetDomainName class
//

#include "pch.h"
#include "GetDomainName.xaml.h"
#include <ppltasks.h>

using namespace Windows::System::UserProfile;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Platform;
using namespace SDKSample::UserDomainName;
using namespace concurrency;

GetDomainName::GetDomainName()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void GetDomainName::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}


void GetDomainName::GetDNSDomain_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // The user or domain administrator may disable access to personal information by apps
    if (UserInformation::NameAccessAllowed)
    {
        DomainResultText->Text = "Getting domain name.";
        create_task(UserInformation::GetDomainNameAsync()).then([this](String^ domainName)
        {
            // an empty string may be returned in any of the following circumstances:
            // The information cannot be retrieved from the directory
            // The calling user is not a member of a domain
            // The user or administrator has disabled the privacy setting
            if (domainName->IsEmpty())
            {
                DomainResultText->Text = "No DNS domain name returned for the current user.";
            }
            else
            {
                DomainResultText->Text = domainName;
            }
        });
    }
    else
    {
        DomainResultText->Text = "Access to user information is disabled by the user or administrator";
    }
}
