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
// UpdateCost.xaml.cpp
// Implementation of the UpdateCost class
//

#include "pch.h"
#include "UpdateCost.xaml.h"

using namespace SDKSample::ProvisioningAgent;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Platform;
using namespace Windows::Networking;
using namespace Windows::Networking::Connectivity;
using namespace Windows::Networking::NetworkOperators;


UpdateCost::UpdateCost()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void UpdateCost::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
    util = ref new Util();

    mediaType->SelectionChanged += ref new SelectionChangedEventHandler(this, &UpdateCost::MediaType_SelectionChanged);
    mediaType->SelectedItem = mediaType_Wlan;

    networkCostCategory->SelectionChanged += ref new SelectionChangedEventHandler(this, &UpdateCost::NetworkCostCategory_SelectionChanged);
    networkCostCategory->SelectedItem = cost_unknown;

}

void UpdateCost::MediaType_SelectionChanged(Object^ sender, SelectionChangedEventArgs^ e)
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

void UpdateCost::NetworkCostCategory_SelectionChanged(Object^ sender, SelectionChangedEventArgs^ e)
{
    if (networkCostCategory->SelectedIndex == 0)
    {
        networkCostType = NetworkCostType::Unknown;
    }
    else if (networkCostCategory->SelectedIndex == 1)
    {
        networkCostType = NetworkCostType::Unrestricted;
    }
    else if (networkCostCategory->SelectedIndex == 2)
    {
        networkCostType = NetworkCostType::Fixed;
    }
    else if (networkCostCategory->SelectedIndex == 3)
    {
        networkCostType = NetworkCostType::Variable;
    }
}


void UpdateCost::UpdateCost_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if (profileNameText->Text == L"")
    {
        rootPage->NotifyUser("Profile name cannot be empty", NotifyType::ErrorMessage);
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

        UpdateCostButton->IsEnabled = false;

        // For the sake of simplicity, assume we want to use the first account.
        // Refer to the MobileBroadbandAccount API's for how to select a specific account ID.
        auto networkAccountId = networkAccIds->GetAt(0);

        // Create provisioning agent for specified network account ID
        auto provisioningAgent = Windows::Networking::NetworkOperators::ProvisioningAgent::CreateFromNetworkAccountId(networkAccountId);

        // Retrieve associated provisioned profile
        auto provisionedProfile = provisioningAgent->GetProvisionedProfile(profileMediaType, profileName);

        // Set the new cost
        provisionedProfile->UpdateCost(networkCostType);

        wchar_t buf[5000];
        HRESULT hr = StringCchPrintf(buf, ARRAYSIZE(buf), L"Profile %s has been updated with the cost type %d", profileName->Data(), networkCostType);
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

    UpdateCostButton->IsEnabled = true;

}
