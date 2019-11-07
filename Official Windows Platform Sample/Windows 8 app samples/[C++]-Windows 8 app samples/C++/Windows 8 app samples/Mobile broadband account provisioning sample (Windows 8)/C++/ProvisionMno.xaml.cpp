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
// ProvisionMno.xaml.cpp
// Implementation of the ProvisionMno class
//

#include "pch.h"
#include "ProvisionMno.xaml.h"

using namespace SDKSample::ProvisioningAgent;

using namespace concurrency;
using namespace Platform;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Networking::NetworkOperators;

ProvisionMno::ProvisionMno()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void ProvisionMno::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
    util = ref new Util();
}

void ProvisionMno::ProvisionMno_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if (provXmlText->Text == L"")
    {
        rootPage->NotifyUser("Provisioning XML cannot be empty", NotifyType::ErrorMessage);
        return;
    }

    auto provisioningXML = provXmlText->Text;

    // Get the network account ID.
    auto networkAccIds = MobileBroadbandAccount::AvailableNetworkAccountIds;
    if (networkAccIds->Size == 0)
    {
        rootPage->NotifyUser("No network account ID found", NotifyType::ErrorMessage);
        return;
    }

    ProvisionMnoButton->IsEnabled = false;

    // For the sake of simplicity, assume we want to use the first account.
    // Refer to the MobileBroadbandAccount API's for how to select a specific account ID.
    auto networkAccountId = networkAccIds->GetAt(0);

    // Create provisioning agent for specified network account ID
    auto provisioningAgent = Windows::Networking::NetworkOperators::ProvisioningAgent::CreateFromNetworkAccountId(networkAccountId);

    // Provision using XML
    task<ProvisionFromXmlDocumentResults^> getProvisioningResult(provisioningAgent->ProvisionFromXmlDocumentAsync(provisioningXML));
    getProvisioningResult.then([this](task<ProvisionFromXmlDocumentResults^> resultTask)
    {
        try
        {
            // Get will throw an exception if the task failed with an error.
            auto result = resultTask.get();

            if (result->AllElementsProvisioned)
            {
                // Provisioning is done successfully
                rootPage->NotifyUser("Device was successfully configured", NotifyType::StatusMessage);
            }
            else
            {
                // Error has occured during provisioning
                // And hence displaying result XML containing
                // errors
                rootPage->NotifyUser(util->ParseResultXML(result->ProvisionResultsXml), NotifyType::StatusMessage);
            }
        }
        catch (Platform::Exception^ ex)
        {
            // Handle errors
            rootPage->NotifyUser(util->PrintExceptionCode(ex), NotifyType::ErrorMessage);
        }
    });
    ProvisionMnoButton->IsEnabled = true;
}
