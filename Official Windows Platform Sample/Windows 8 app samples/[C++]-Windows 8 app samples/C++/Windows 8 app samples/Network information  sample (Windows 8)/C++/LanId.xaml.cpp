//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

// LanId.xaml.cpp
// Implementation of the LanId class

#include "pch.h"
#include "LanId.xaml.h"

using namespace SDKSample::NetworkInformationApi;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

#define MAX_GUID_STRING_SIZE 39

LanId::LanId()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void LanId::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

// Display Lan Identifier Data
void LanId::DisplayLanIdentifierData(LanIdentifier^ lanIdentifier)
{
    try
    {
        if (lanIdentifier == nullptr)
        {
            lanIdInfo += L"Lan Identifier not defined";
            return;
        }

        // Display InfrastructureId
        if (lanIdentifier->InfrastructureId != nullptr)
        {
            lanIdInfo += L"Infrastructure Type: " + lanIdentifier->InfrastructureId->Type.ToString() + "\n";
            lanIdInfo += L"InfraStructure Value : ";
            auto lanIdVals = lanIdentifier->InfrastructureId->Value;
            if (lanIdVals->Size != 0)
            {
                std::for_each(begin(lanIdVals), end(lanIdVals), [this](int lanIdVal)
                {
                    lanIdInfo += lanIdVal.ToString() + " ";
                });
            }
        }

        // Display PortId
        if (lanIdentifier->PortId != nullptr)
        {
            lanIdInfo += L"\nPortId Type: " + lanIdentifier->PortId->Type.ToString() + "\n";
            lanIdInfo += L"PortId Value : ";
            auto portIdVals = lanIdentifier->PortId->Value;
            if (portIdVals->Size != 0)
            {
                std::for_each(begin(portIdVals), end(portIdVals), [this](int portIdVal)
                {
                    lanIdInfo += portIdVal.ToString() + " ";
                });
            }
        }

        // Display NetworkAdapter Id
        WCHAR networkAdapterId[MAX_GUID_STRING_SIZE];
        if (StringFromGUID2(lanIdentifier->NetworkAdapterId, networkAdapterId, ARRAYSIZE(networkAdapterId)))
        {
            String^ strAdaptertId = ref new String(networkAdapterId);
            lanIdInfo += L"\nNetwork Adapter Id: " + strAdaptertId + L"\n";
        }
    }
    catch (Exception^ ex)
    {
        rootPage->NotifyUser("An unexpected exception occured: " + ex->Message, NotifyType::ErrorMessage);
    }
}

// Display Lan Identifiers
void SDKSample::NetworkInformationApi::LanId::LanId_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    try
    {
        lanIdInfo = "";
        auto lanIdentifiers = NetworkInformation::GetLanIdentifiers();
        if (lanIdentifiers->Size != 0)
        {
            lanIdInfo += L"Number of Lan Identifiers retrieved : " + lanIdentifiers->Size.ToString() + "\n";
            lanIdInfo += L"===========================================\n";

            std::for_each(begin(lanIdentifiers), end(lanIdentifiers), [this](LanIdentifier^ lanIdentifier)
            {
                // Display Lan Identifiers information
                DisplayLanIdentifierData(lanIdentifier);
                lanIdInfo += "---------------------------------------------------\n";
            });

            rootPage->NotifyUser(lanIdInfo, NotifyType::StatusMessage);
        }
        else
        {
            rootPage->NotifyUser(L"No Lan Identifier Data found", NotifyType::StatusMessage);
        }
    }
    catch (Exception^ ex)
    {
        rootPage->NotifyUser("An unexpected exception occured: " + ex->Message, NotifyType::ErrorMessage);
    }
}
