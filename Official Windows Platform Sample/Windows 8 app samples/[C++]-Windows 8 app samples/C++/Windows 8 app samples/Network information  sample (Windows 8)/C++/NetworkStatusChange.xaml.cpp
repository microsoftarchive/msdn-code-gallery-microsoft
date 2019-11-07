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
// NetworkStatusChange.xaml.cpp
// Implementation of the NetworkStatusChange class
//

#include "pch.h"
#include "NetworkStatusChange.xaml.h"

using namespace SDKSample::NetworkInformationApi;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::System;
using namespace Windows::Foundation;
using namespace Platform;
using namespace Windows::Networking;
using namespace Windows::Networking::Connectivity;

NetworkStatusChange::NetworkStatusChange()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void NetworkStatusChange::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;

    //create handler for network status changed event
    networkStatusCallback = ref new NetworkStatusChangedEventHandler(this, &NetworkStatusChange::OnNetworkStatusChange, CallbackContext::Same);
}

// Get Connection Cost based suggestions
String^ NetworkStatusChange::GetCostBasedSuggestions(ConnectionCost^ connectionCost)
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
String^ NetworkStatusChange::GetConnectionCostInfo(ConnectionCost^ connectionCost)
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
        strRet += L"Roaming    : Yes\n";
    }
    else
    {
        strRet += L"Roaming    : No\n";
    }

    if (connectionCost->OverDataLimit)
    {
        strRet += L"OverDataLimit    : Yes\n";
    }
    else
    {
        strRet += L"OverDataLimit    : No\n";
    }

    if (connectionCost->ApproachingDataLimit)
    {
        strRet += L"ApproachingDataLimit    : Yes\n";
    }
    else
    {
        strRet += L"ApproachingDataLimit    : No\n";
    }

    //Get cost based suggestions
    strRet += GetCostBasedSuggestions(connectionCost);
    return strRet;
}

// Get Dataplan Status Information
String^ NetworkStatusChange::GetDataPlanStatusInfo(DataPlanStatus^ dataPlan)
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
String^ NetworkStatusChange::GetNetworkSecuritySettingsInfo(NetworkSecuritySettings^ netSecuritySettings)
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


// Get Connection Profile Information
String^ NetworkStatusChange::GetConnectionProfile(ConnectionProfile^ connectionProfile)
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

    //Get Connection Cost information
    ConnectionCost^ connectionCost = connectionProfile->GetConnectionCost();
    strProfileInfo += GetConnectionCostInfo(connectionCost);

    //Get Dataplan Status information
    DataPlanStatus^ dataPlan = connectionProfile->GetDataPlanStatus();
    strProfileInfo += GetDataPlanStatusInfo(dataPlan);

    //Get Network Security Settings
    NetworkSecuritySettings^ netSecuritySettings = connectionProfile->NetworkSecuritySettings;
    strProfileInfo += GetNetworkSecuritySettingsInfo(netSecuritySettings);

    return strProfileInfo;
}

// Register for Network Status Change notifications
void NetworkStatusChange::NetworkStatusChange_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    try
    {
        // register for network status change notifications
        if (!registeredNetworkStatusNotif)
        {
            cookie = NetworkInformation::NetworkStatusChanged += networkStatusCallback;
            registeredNetworkStatusNotif = true;
        }
        if (internetProfileInfo == "")
        {
            rootPage->NotifyUser(L"No network status change. ", NotifyType::StatusMessage);
        }
        else
        {
            rootPage->NotifyUser(internetProfileInfo, NotifyType::StatusMessage);
        }
    }

    catch (Exception^ ex)
    {
        rootPage->NotifyUser("An unexpected exception occured: " + ex->Message, NotifyType::ErrorMessage);
    }
}

// Event handler for Network Status Change event
void NetworkStatusChange::OnNetworkStatusChange(Object^ sender)
{
    try
    {
        //network status changed
        internetProfileInfo = "Network Status Changed: \n";

        // Display new Interent Connection profile information
        ConnectionProfile^ internetConnectionProfile = NetworkInformation::GetInternetConnectionProfile();
        if (internetConnectionProfile == nullptr)
        {
            rootPage->NotifyUser(L"Not connected to Internet\n", NotifyType::StatusMessage);
        }
        else
        {
            String^ connectionProfileInfo = L"Profile Name : " + internetConnectionProfile->ProfileName + L"\n" + GetConnectionProfile(internetConnectionProfile);
            rootPage->NotifyUser(connectionProfileInfo, NotifyType::StatusMessage);
        }
        internetProfileInfo = "";
    }

    catch (Exception^ ex)
    {
        rootPage->NotifyUser("An unexpected exception occured: " + ex->Message, NotifyType::ErrorMessage);
    }
}

// Unregister Network Status Change notifications
void NetworkStatusChange::UnRegisterForNetworkStatusChangeNotif ()
{
    try
    {
        NetworkInformation::NetworkStatusChanged -= cookie;
        internetProfileInfo = "";
        registeredNetworkStatusNotif = false;
    }
    catch (Exception^ ex)
    {
        rootPage->NotifyUser("An unexpected exception occured: " + ex->Message, NotifyType::ErrorMessage);
    }
}

void NetworkStatusChange::OnNavigatedFrom(NavigationEventArgs^ e)
{
    //on scenario change, unregister from Network Status Change notifications if registerd
    if (registeredNetworkStatusNotif)
    {
        UnRegisterForNetworkStatusChangeNotif ();
        registeredNetworkStatusNotif = false;
    }
}

