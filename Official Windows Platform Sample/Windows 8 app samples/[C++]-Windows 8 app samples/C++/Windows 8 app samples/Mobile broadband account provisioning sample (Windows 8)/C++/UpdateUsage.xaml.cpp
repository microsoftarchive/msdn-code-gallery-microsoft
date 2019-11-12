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
// UpdateUsage.xaml.cpp
// Implementation of the UpdateUsage class
//

#include "pch.h"
#include "UpdateUsage.xaml.h"

using namespace SDKSample::ProvisioningAgent;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::System;
using namespace Windows::Foundation;
using namespace Platform;
using namespace Windows::Networking;
using namespace Windows::Networking::Connectivity;
using namespace Windows::Networking::NetworkOperators;

UpdateUsage::UpdateUsage()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void UpdateUsage::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
    util = ref new Util();

    mediaType->SelectionChanged += ref new SelectionChangedEventHandler(this, &UpdateUsage::MediaType_SelectionChanged);
    mediaType->SelectedItem = mediaType_Wlan;

}

void UpdateUsage::MediaType_SelectionChanged(Object^ sender, SelectionChangedEventArgs^ e)
{
    if (mediaType->SelectedIndex == 0)
    {
        profileMediaType = ProfileMediaType::Wlan;
    }
    else if (mediaType->SelectedIndex == 1)
    {
        profileMediaType = ProfileMediaType::Wwan;
    }
}

void UpdateUsage::UpdateUsage_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if (profileNameText->Text == L"")
    {
        rootPage->NotifyUser("Profile name cannot be empty", NotifyType::ErrorMessage);
        return;
    }

    int usageInMegabytesValue;
    if (1 != swscanf_s( usageInMegabytesText->Text->Data(), L"%I32u", &usageInMegabytesValue))
    {
        rootPage->NotifyUser("Usage in megabytes should be a valid number", NotifyType::ErrorMessage);
        return;
    }

    auto profileName = profileNameText->Text;

    try
    {
        // Get the network account ID.
        auto networkAccIds = MobileBroadbandAccount::AvailableNetworkAccountIds;
        if (networkAccIds->Size == 0)
        {
            rootPage->NotifyUser("No network account ID found", NotifyType::ErrorMessage);
            return;
        }

        UpdateUsageButton->IsEnabled = false;

        // For the sake of simplicity, assume we want to use the first account.
        // Refer to the MobileBroadbandAccount API's for how to select a specific account ID.
        auto networkAccountId = networkAccIds->GetAt(0);

        // Create provisioning agent for specified network account ID
        auto provisioningAgent = Windows::Networking::NetworkOperators::ProvisioningAgent::CreateFromNetworkAccountId(networkAccountId);

        // Retrieve associated provisioned profile
        auto provisionedProfile = provisioningAgent->GetProvisionedProfile(profileMediaType, profileName);

        ProfileUsage profileUsage;
        profileUsage.UsageInMegabytes = usageInMegabytesValue;

        //Get Current time as FILETIME
        FILETIME fileTime;
        GetSystemTimeAsFileTime(&fileTime);

        ULARGE_INTEGER uiCurrTime;
        uiCurrTime.LowPart = fileTime.dwLowDateTime;
        uiCurrTime.HighPart = fileTime.dwHighDateTime;

        DateTime dtCurrentTime;
        dtCurrentTime.UniversalTime = uiCurrTime.QuadPart;
        profileUsage.LastSyncTime = dtCurrentTime;

        // Update usage
        provisionedProfile->UpdateUsage(profileUsage);

        wchar_t buf[5000];
        HRESULT hr = StringCchPrintf(buf, ARRAYSIZE(buf), L"Usage of %dMB has been set for the profile %s", usageInMegabytesValue, profileName->Data());
        if (FAILED(hr))
        {
            throw Exception::CreateException(hr);
        }
        String^ str = ref new String(buf);
        rootPage->NotifyUser(str, NotifyType::StatusMessage);
    }
    catch (Exception^ ex)
    {
        rootPage->NotifyUser(util->PrintExceptionCode(ex), NotifyType::ErrorMessage);
    }

    UpdateUsageButton->IsEnabled = true;
}
