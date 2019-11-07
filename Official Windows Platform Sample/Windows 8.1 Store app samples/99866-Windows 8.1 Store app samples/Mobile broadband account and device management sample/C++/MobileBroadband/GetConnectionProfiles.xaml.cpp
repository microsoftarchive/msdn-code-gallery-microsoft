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
// GetConnectionProfiles.xaml.cpp
// Implementation of the GetConnectionProfiles class
//

#include "pch.h"
#include "GetConnectionProfiles.xaml.h"

using namespace SDKSample::MobileBroadband;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Networking::NetworkOperators;
using namespace Windows::Networking::Connectivity;
using namespace Platform;

GetConnectionProfiles::GetConnectionProfiles()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void GetConnectionProfiles::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;

    PrepareScenario();
}

// Get Connection Cost based suggestions
String^ GetConnectionProfiles::GetCostBasedSuggestions(ConnectionCost^ connectionCost)
{
    String^ strRet = "";

    strRet = L"Cost Based Suggestions: \n";
    if (connectionCost->Roaming)
    {
        strRet += L"Connection is out of MNO's network, using the connection may result in additional charge. Application can implement High Cost behavior in this scenario\n";
    }
    else if (connectionCost->NetworkCostType == NetworkCostType::Variable)
    {
        strRet += L"Connection cost is variable, and the connection is charged based on usage, so application can implement the Conservative behavior\n";
    }
    else if (connectionCost->NetworkCostType == NetworkCostType::Fixed)
    {
        if (connectionCost->OverDataLimit || connectionCost->ApproachingDataLimit)
        {
            strRet += L"Connection has exceeded the usage cap limit or is approaching the datalimit, and the application can implement High Cost behavior in this scenario\n";
        }
        else
        {
            strRet += L"Application can implemement the Conservative behavior\n";
        }
    }
    else
    {
        strRet += L"Application can implement the Standard behavior\n";
    }

    return strRet;
}

// Get Connection Cost information
String^ GetConnectionProfiles::GetConnectionCostInfo(ConnectionCost^ connectionCost)
{
    String^ strRet = "";

    if (connectionCost == nullptr)
    {
        return strRet;
    }

    strRet = L"Connection Cost Information: \n";
    strRet += L"====================\n";
    switch(connectionCost->NetworkCostType)
    {
    case NetworkCostType::Unrestricted:
        strRet += L"Cost: Unrestricted\n";
        break;
    case NetworkCostType::Fixed:
        strRet += L"Cost: Fixed\n";
        break;
    case NetworkCostType::Variable:
        strRet += L"Cost: Variable\n";
        break;
    case NetworkCostType::Unknown:
        strRet += L"Cost: Unknown\n";
        break;
    default:
        strRet += L"Cost: Error\n";
        break;
    }

    if (connectionCost->Roaming)
    {
        strRet += L"Roaming: Yes\n";
    }
    else
    {
        strRet += L"Roaming: No\n";
    }

    if (connectionCost->OverDataLimit)
    {
        strRet += L"OverDataLimit: Yes\n";
    }
    else
    {
        strRet += L"OverDataLimit: No\n";
    }

    if (connectionCost->ApproachingDataLimit)
    {
        strRet += L"ApproachingDataLimit: Yes\n";
    }
    else
    {
        strRet += L"ApproachingDataLimit: No\n";
    }

    // Get cost based suggestions
    strRet += GetCostBasedSuggestions(connectionCost);
    return strRet;
}

// Get Dataplan Status Information
String^ GetConnectionProfiles::GetDataPlanStatusInfo(DataPlanStatus^ dataPlan)
{
    String ^strRet = "";

    if (dataPlan == nullptr)
    {
        return strRet;
    }

    strRet = L"Dataplan Status Information: \n";
    strRet += L"====================\n";

    if (dataPlan->DataPlanUsage != nullptr)
    {
        auto lastSyncTime = dataPlan->DataPlanUsage->LastSyncTime;
        ULARGE_INTEGER lst;
        lst.QuadPart = lastSyncTime.UniversalTime;
        FILETIME ftLastSyncTime;
        SYSTEMTIME stLastSyncTime;

        //get FILETIME from lastSyncTime, convert FILETIME to SYSTEMTIME and display
        ftLastSyncTime.dwLowDateTime = lst.LowPart;
        ftLastSyncTime.dwHighDateTime = lst.HighPart;
        FileTimeToSystemTime(&ftLastSyncTime, &stLastSyncTime);
        strRet += L"Usage In Megabytes : " + dataPlan->DataPlanUsage->MegabytesUsed.ToString() + "\n";
        strRet += L"LastSyncTime: " + stLastSyncTime.wMonth.ToString() + "/" + stLastSyncTime.wDay.ToString() + "/" + stLastSyncTime.wYear.ToString() + " " + stLastSyncTime.wHour.ToString() + ":" + stLastSyncTime.wMinute.ToString() + ":"+ stLastSyncTime.wSecond.ToString() + "\n";
    }
    else
    {
        strRet += L"DataPlan Usage : Not Defined\n";
    }

    if (dataPlan->InboundBitsPerSecond != nullptr)
    {
        auto inboundBandwidth = dataPlan->InboundBitsPerSecond->Value;
        strRet += L"InboundBitsPerSecond : " + inboundBandwidth.ToString() + "\n";
    }
    else
    {
        strRet += L"InboundBitsPerSecond : Not Defined\n";
    }

    if (dataPlan->OutboundBitsPerSecond != nullptr)
    {
        auto outboundBandwidth = dataPlan->OutboundBitsPerSecond->Value;
        strRet += L"OutboundBitsPerSecond : " + outboundBandwidth.ToString() + "\n";
    }
    else
    {
        strRet += L"OutboundBitsPerSecond : Not Defined\n";
    }

    if (dataPlan->DataLimitInMegabytes != nullptr)
    {
        auto dataLimit = dataPlan->DataLimitInMegabytes->Value;
        strRet += L"DataLimitInMegabytes : " + dataLimit.ToString() + "\n";
    }
    else
    {
        strRet += L"DataLimitInMegabytes : Not Defined\n";
    }

    if (dataPlan->NextBillingCycle != nullptr)
    {
        auto nextBillingCycle = dataPlan->NextBillingCycle->Value;
        ULARGE_INTEGER nbc;
        nbc.QuadPart = nextBillingCycle.UniversalTime;
        FILETIME ftNextBillingCycle;
        SYSTEMTIME stNextBillingCycle;

        //get FILETIME from NextBillingCycle, convert to FILETIME to SYSTEMTIME and display
        ftNextBillingCycle.dwLowDateTime = nbc.LowPart;
        ftNextBillingCycle.dwHighDateTime = nbc.HighPart;
        FileTimeToSystemTime(&ftNextBillingCycle, &stNextBillingCycle);
        strRet += L"NextBillingCycle: " + stNextBillingCycle.wMonth.ToString() + "/" + stNextBillingCycle.wDay.ToString() + "/" + stNextBillingCycle.wYear.ToString() + " " + stNextBillingCycle.wHour.ToString() + ":" + stNextBillingCycle.wMinute.ToString() + ":" + stNextBillingCycle.wSecond.ToString() + "\n";
    }
    else
    {
        strRet += L"NextBillingCycle : Not Defined\n";
    }

    if (dataPlan->MaxTransferSizeInMegabytes != nullptr)
    {
        auto transferSize = dataPlan->MaxTransferSizeInMegabytes->Value;
        strRet += L"MaxTransferSizeInMegabytes : " + transferSize.ToString() + "\n";
    }
    else
    {
        strRet += L"MaxTransferSizeInMegabytes : Not Defined\n";
    }

    return strRet;
}


// Get Network Security Settings information
String^ GetConnectionProfiles::GetNetworkSecuritySettingsInfo(NetworkSecuritySettings^ netSecuritySettings)
{
    String^ networkSecurity = "";
    networkSecurity += L"Network Security Settings: \n";
    networkSecurity += L"====================\n";

    if (netSecuritySettings == nullptr)
    {
        networkSecurity += L"Network Security Settings not available\n";
        return networkSecurity;
    }

    //NetworkAuthenticationType
    switch (netSecuritySettings->NetworkAuthenticationType)
    {
    case NetworkAuthenticationType::None:
        networkSecurity += L"NetworkAuthenticationType: None";
        break;
    case NetworkAuthenticationType::Unknown:
        networkSecurity += L"NetworkAuthenticationType: Unknown";
        break;
    case NetworkAuthenticationType::Open80211:
        networkSecurity += L"NetworkAuthenticationType: Open80211";
        break;
    case NetworkAuthenticationType::SharedKey80211:
        networkSecurity += L"NetworkAuthenticationType: SharedKey80211";
        break;
    case NetworkAuthenticationType::Wpa:
        networkSecurity += L"NetworkAuthenticationType: Wpa";
        break;
    case NetworkAuthenticationType::WpaPsk:
        networkSecurity += L"NetworkAuthenticationType: WpaPsk";
        break;
    case NetworkAuthenticationType::WpaNone:
        networkSecurity += L"NetworkAuthenticationType: WpaNone";
        break;
    case NetworkAuthenticationType::Rsna:
        networkSecurity += L"NetworkAuthenticationType: Rsna";
        break;
    case NetworkAuthenticationType::RsnaPsk:
        networkSecurity += L"NetworkAuthenticationType: RsnaPsk";
        break;
    default:
        networkSecurity += L"NetworkAuthenticationType: Error";
        break;
    }
    networkSecurity += L"\n";

    //NetworkEncryptionType
    switch (netSecuritySettings->NetworkEncryptionType)
    {
    case NetworkEncryptionType::None:
        networkSecurity += L"NetworkEncryptionType: None";
        break;
    case NetworkEncryptionType::Unknown:
        networkSecurity += L"NetworkEncryptionType: Unknown";
        break;
    case NetworkEncryptionType::Wep:
        networkSecurity += L"NetworkEncryptionType: Wep";
        break;
    case NetworkEncryptionType::Wep40:
        networkSecurity += L"NetworkEncryptionType: Wep40";
        break;
    case NetworkEncryptionType::Wep104:
        networkSecurity += L"NetworkEncryptionType: Wep104";
        break;
    case NetworkEncryptionType::Tkip:
        networkSecurity += L"NetworkEncryptionType: Tkip";
        break;
    case NetworkEncryptionType::Ccmp:
        networkSecurity += L"NetworkEncryptionType: Ccmp";
        break;
    case NetworkEncryptionType::WpaUseGroup:
        networkSecurity += L"NetworkEncryptionType: WpaUseGroup";
        break;
    case NetworkEncryptionType::RsnUseGroup:
        networkSecurity += L"NetworkEncryptionType: RsnUseGroup";
        break;
    default:
        networkSecurity += L"NetworkEncryptionType: Error";
        break;
    }
    networkSecurity += L"\n";
    return networkSecurity;
}

// Get Wireless LAN Connection Profile Details Information
String^ GetConnectionProfiles::GetWlanConnectionProfileDetailsInfo(Windows::Networking::Connectivity::WlanConnectionProfileDetails^ wlanConnectionProfileDetails)
{
    String^ wlanDetails = "";
    wlanDetails += L"Wireless LAN Connection Profile Details:\n";
    wlanDetails += L"====================\n";

    if (wlanConnectionProfileDetails == nullptr)
    {
        wlanDetails += L"Wireless LAN connection profile details unavailable.\n";
        return wlanDetails;
    }

    wlanDetails += L"Connected SSID: " + wlanConnectionProfileDetails->GetConnectedSsid() + "\n";
    return wlanDetails;
}

// Get Wireless WAN Connection Profile Details Information
String^ GetConnectionProfiles::GetWwanConnectionProfileDetailsInfo(Windows::Networking::Connectivity::WwanConnectionProfileDetails^ wwanConnectionProfileDetails)
{
    String^ wwanDetails = "";
    wwanDetails += L"Wireless WAN Connection Profile Details:\n";
    wwanDetails += L"====================\n";

    if (wwanConnectionProfileDetails == nullptr)
    {
        wwanDetails += L"Wireless WAN connection profile details unavailable.\n";
        return wwanDetails;
    }

    wwanDetails += L"Home Provider ID: " + wwanConnectionProfileDetails->HomeProviderId + "\n";
    wwanDetails += L"Access Point Name: " + wwanConnectionProfileDetails->AccessPointName + "\n";
    wwanDetails += L"Network Registration State: " + wwanConnectionProfileDetails->GetNetworkRegistrationState().ToString() + "\n";
    wwanDetails += L"Current Data Class: " + wwanConnectionProfileDetails->GetCurrentDataClass().ToString() + "\n";

    return wwanDetails;
}

// Get Connection Profile Information
String^ GetConnectionProfiles::GetConnectionProfile(ConnectionProfile^ connectionProfile)
{
    String^ strProfileInfo = "";

    switch (connectionProfile->GetNetworkConnectivityLevel())
    {
    case NetworkConnectivityLevel::None:
        strProfileInfo += "Connectivity Level : None\n";
        break;
    case NetworkConnectivityLevel::LocalAccess:
        strProfileInfo += "Connectivity Level : Local Access\n";
        break;
    case NetworkConnectivityLevel::ConstrainedInternetAccess:
        strProfileInfo += "Connectivity Level : Constrained Internet Access\n";
        break;
    case NetworkConnectivityLevel::InternetAccess:
        strProfileInfo += "Connectivity Level : Internet Access\n";
        break;
    }

    switch (connectionProfile->GetDomainConnectivityLevel())
    {
    case DomainConnectivityLevel::None:
        strProfileInfo += "Domain Connectivity Level : None\n";
        break;
    case DomainConnectivityLevel::Unauthenticated:
        strProfileInfo += "Domain Connectivity Level : Unauthenticated\n";
        break;
    case DomainConnectivityLevel::Authenticated:
        strProfileInfo += "Domain Connectivity Level : Authenticated\n";
        break;
    }

    // Get Connection Cost information
    ConnectionCost^ connectionCost = connectionProfile->GetConnectionCost();
    strProfileInfo += GetConnectionCostInfo(connectionCost);

    // Get Dataplan Status information
    DataPlanStatus^ dataPlan = connectionProfile->GetDataPlanStatus();
    strProfileInfo += GetDataPlanStatusInfo(dataPlan);

    // Get Network Security Settings
    NetworkSecuritySettings^ netSecuritySettings = connectionProfile->NetworkSecuritySettings;
    strProfileInfo += GetNetworkSecuritySettingsInfo(netSecuritySettings);

    // Get Wlan Connection Profile Details if this is a Wlan ConnectionProfile
    if (connectionProfile->IsWlanConnectionProfile)
    {
        WlanConnectionProfileDetails^ wlanConnectionProfileDetails = connectionProfile->WlanConnectionProfileDetails;
        strProfileInfo += GetWlanConnectionProfileDetailsInfo(wlanConnectionProfileDetails);
    }

    // Get Wwan Connection Profile Details if this is a Wwan ConnectionProfile
    if (connectionProfile->IsWwanConnectionProfile)
    {
        WwanConnectionProfileDetails^ wwanConnectionProfileDetails = connectionProfile->WwanConnectionProfileDetails;
        strProfileInfo += GetWwanConnectionProfileDetailsInfo(wwanConnectionProfileDetails);
    }

    //Get the Service Provider GUID
    if (connectionProfile->ServiceProviderGuid != nullptr)
    {
        strProfileInfo += L"====================\n";
        strProfileInfo += L"Service Provider GUID: " + connectionProfile->ServiceProviderGuid + "\n";
    }

    //Get the number of signal bars
    if (connectionProfile->GetSignalBars() != nullptr)
    {
        strProfileInfo += L"====================\n";
        strProfileInfo += L"Signal Bars: " + connectionProfile->GetSignalBars() + "\n";
    }

    return strProfileInfo;
}

void GetConnectionProfiles::PrintConnectionProfiles(IVectorView<ConnectionProfile^>^ connectionProfiles)
{
    try
    {
        connectionProfileInfo = "";
        rootPage->NotifyUser("", NotifyType::StatusMessage);

        std::for_each(begin(connectionProfiles), end(connectionProfiles), [this](ConnectionProfile^ connectionProfile)
        {
            if (connectionProfile != nullptr)
            {
                // Display Connection Profile information for each of the profiles
                connectionProfileInfo += L"Profile Name : " + connectionProfile->ProfileName + L"\n" + GetConnectionProfile(connectionProfile);
                connectionProfileInfo += L"--------------------------------------------------------------------\n";
            }
        });
        if (connectionProfiles->Size == 0)
        {
            connectionProfileInfo += L"No Connection Profiles found.\n";
        }

        rootPage->NotifyUser(connectionProfileInfo, NotifyType::StatusMessage);
    }

    catch (Exception^ ex)
    {
        rootPage->NotifyUser("An unexpected exception occurred: " + ex->Message, NotifyType::ErrorMessage);
    }
}

void SDKSample::MobileBroadband::GetConnectionProfiles::GetConnectionProfilesButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ b = safe_cast<Button^>(sender);
    if (b != nullptr)
    {
        try
        {
            // For the sake of simplicity, we only get the ConnectionProfiles from the first NetworkAccountId
            auto mobileBroadbandAccount = MobileBroadbandAccount::CreateFromNetworkAccountId(deviceAccountId->GetAt(0));
            PrintConnectionProfiles(mobileBroadbandAccount->GetConnectionProfiles());
        }
        catch (Platform::Exception^ ex)
        {
            rootPage->NotifyUser("Error:" + ex->Message, NotifyType::ErrorMessage);
        }
    }
}


void SDKSample::MobileBroadband::GetConnectionProfiles::PrepareScenario()
{
    rootPage->NotifyUser("", NotifyType::StatusMessage);

    try
    {
        deviceAccountId = MobileBroadbandAccount::AvailableNetworkAccountIds;

        if (deviceAccountId->Size != 0)
        {
            GetConnectionProfilesButton->Content = "Get Connection Profiles";
            GetConnectionProfilesButton->IsEnabled = true;
        }
        else
        {
            GetConnectionProfilesButton->Content = "No available accounts detected";
            GetConnectionProfilesButton->IsEnabled = false;
        }
    }
    catch (Platform::Exception^ ex)
    {
        rootPage->NotifyUser("Error:" + ex->Message, NotifyType::ErrorMessage);
    }
}