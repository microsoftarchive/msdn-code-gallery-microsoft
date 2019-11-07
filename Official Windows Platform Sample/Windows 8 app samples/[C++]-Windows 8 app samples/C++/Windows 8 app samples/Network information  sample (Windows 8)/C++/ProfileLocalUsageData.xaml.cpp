//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

// ProfileLocalUsageData.xaml.cpp
// Implementation of the ProfileLocalUsageData class

#include "pch.h"
#include "ProfileLocalUsageData.xaml.h"

using namespace SDKSample::NetworkInformationApi;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::System;
using namespace Windows::Foundation;
using namespace Platform;
using namespace Windows::Networking;
using namespace Windows::Networking::Connectivity;

ProfileLocalUsageData::ProfileLocalUsageData()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void ProfileLocalUsageData::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

// Display Local Data Usage for Internet Connection Profile for the past 1 hour
void SDKSample::NetworkInformationApi::ProfileLocalUsageData::ProfileLocalUsageData_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    try
    {
        FILETIME ftCurrTime;

        //Get Current time as FILETIME in UTC
        GetSystemTimeAsFileTime(&ftCurrTime);

        //Get the DateTime object with current time
        DateTime dtCurrentTime;
        dtCurrentTime.UniversalTime = (((ULONGLONG) ftCurrTime.dwHighDateTime) << 32) + ftCurrTime.dwLowDateTime;

        //Set Start time to one hour behind current time
        DateTime dtStartTime;
        dtStartTime.UniversalTime = dtCurrentTime.UniversalTime - (3600 * 10000000ULL);;

        ConnectionProfile^ internetConnectionProfile = NetworkInformation::GetInternetConnectionProfile();

        if (internetConnectionProfile == nullptr)
        {
            rootPage->NotifyUser(L"Not connected to Internet\n", NotifyType::StatusMessage);
        }
        else
        {
            //Get Local Data usage for the Internet Connection Profile for the past 1 hour
            DataUsage^ localUsage = internetConnectionProfile->GetLocalUsage(dtStartTime, dtCurrentTime);

            String^ localUsageInfo = L"Local Data Usage:\n";
            localUsageInfo += L" Bytes Sent : " + localUsage->BytesSent.ToString() + "\n";
            localUsageInfo += L" Bytes Received : " + localUsage->BytesReceived.ToString() + "\n";
            rootPage->NotifyUser(localUsageInfo, NotifyType::StatusMessage);
        }
    }
    catch (Exception^ ex)
    {
        rootPage->NotifyUser("An unexpected exception occured: " + ex->Message, NotifyType::ErrorMessage);
    }
}
