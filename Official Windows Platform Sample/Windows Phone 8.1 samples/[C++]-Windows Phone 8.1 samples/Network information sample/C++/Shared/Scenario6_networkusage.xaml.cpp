//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

// Scenario5_networkstatuschange.xaml.cpp
// Implementation of the ProfileLocalUsageData class

#include "pch.h"
#include "Scenario6_networkusage.xaml.h"

using namespace SDKSample::NetworkInformationApi;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::System;
using namespace Windows::Foundation;
using namespace Platform;
using namespace Windows::Networking;
using namespace Windows::Networking::Connectivity;
using namespace concurrency;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Core;

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

    internetConnectionProfile = NetworkInformation::GetInternetConnectionProfile();
    networkUsageStates = {};
    formatter = ref new Windows::Globalization::DateTimeFormatting::DateTimeFormatter("year month day hour minute second");
}

String^ ProfileLocalUsageData::PrintNetworkUsage(NetworkUsage^ networkUsage, DateTime startTime, DateTime endTime)
{
    String^ result = "Usage from " + formatter->Format(startTime) + " to " + formatter->Format(endTime) +
        "\n\tBytes sent: " + networkUsage->BytesSent +
        "\n\tBytes received: " + networkUsage->BytesReceived +
        "\n\tConnected duration: " + networkUsage->ConnectionDuration.Duration + "\n";

    return result;
}

// Returns the amount of time between each period of network usage for a given granularity
TimeSpan ProfileLocalUsageData::GranularityToTimeSpan(DataUsageGranularity granularity)
{
    TimeSpan timeSpan = TimeSpan();

    // TimeSpan::Duration is the timespan in 100 nanosecond increments
    switch (granularity)
    {
        case DataUsageGranularity::PerMinute:
            timeSpan.Duration = 60ll * 1000ll * 1000ll * 10ll;
            break;
        case DataUsageGranularity::PerHour:
            timeSpan.Duration = 60ll * 60ll * 1000ll * 1000ll * 10ll;
            break;
        case DataUsageGranularity::PerDay:
            timeSpan.Duration = 24ll * 60ll * 60ll * 1000ll * 1000ll * 10ll;
            break;
        default:
            timeSpan.Duration = 0;
            break;
    }

    return timeSpan;
}

DateTime ProfileLocalUsageData::GetDateTimeFromUi(DatePicker^ datePicker, TimePicker^ timePicker)
{
    DateTime dateTime;
    // DatePicker::Date is a DateTime that includes the current time of the day with the date.
    // We use the Calendar to set the time-related fields to the beginning of the day.
    auto calendar = ref new Windows::Globalization::Calendar();
    calendar->SetDateTime(datePicker->Date);
    calendar->Hour = 12;
    calendar->Minute = 0;
    calendar->Second = 0;
    calendar->Nanosecond = 0;
    calendar->Period = 1;

    // Set the start time to the Date from StartDatePicker and the time from StartTimePicker
    dateTime.UniversalTime = calendar->GetDateTime().UniversalTime + timePicker->Time.Duration;

    return dateTime;
}

DataUsageGranularity ProfileLocalUsageData::GetGranularityFromUi()
{
    String^ granularityString = ((ComboBoxItem^)GranularityComboBox->SelectedItem)->Content->ToString();

    if (granularityString == "Per Minute")
    {
        return DataUsageGranularity::PerMinute;
    }
    else if (granularityString == "Per Hour")
    {
        return DataUsageGranularity::PerHour;
    }
    else if (granularityString == "Per Day")
    {
        return DataUsageGranularity::PerDay;
    }
    else
    {
        return DataUsageGranularity::Total;
    }
}

TriStates ProfileLocalUsageData::GetTriStatesFromUi(ComboBox^ triStatesComboBox)
{
    String^ triStates = ((ComboBoxItem^)triStatesComboBox->SelectedItem)->Content->ToString();

    if (triStates == "Yes")
    {
        return TriStates::Yes;
    }
    else if (triStates == "No")
    {
        return TriStates::No;
    }
    else
    {
        return TriStates::DoNotCare;
    }
}

// Display Local Data Usage for Internet Connection Profile for the past 1 hour
void SDKSample::NetworkInformationApi::ProfileLocalUsageData::ProfileLocalUsageData_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    try
    {
        // Get settings from the UI
        granularity = GetGranularityFromUi();
        networkUsageStates.Shared = GetTriStatesFromUi(SharedComboBox);
        networkUsageStates.Roaming = GetTriStatesFromUi(RoamingComboBox);
        startTime = GetDateTimeFromUi(StartDatePicker, StartTimePicker);
        endTime = GetDateTimeFromUi(EndDatePicker, EndTimePicker);

        if (internetConnectionProfile == nullptr)
        {
            rootPage->NotifyUser("Not connected to Internet\n", NotifyType::StatusMessage);
        }
        else
        {
            auto getNetworkUsageTask = create_task(internetConnectionProfile->GetNetworkUsageAsync(startTime, endTime, granularity, networkUsageStates));
            getNetworkUsageTask.then([this](task<IVectorView<NetworkUsage^>^> tNetworkUsages)
            {
                try
                {
                    auto networkUsages = tNetworkUsages.get();
                    String^ outputString;

                    DateTime start = startTime;

                    for (NetworkUsage^ networkUsage : networkUsages)
                    {
                        DateTime end = start;
                        if (granularity == DataUsageGranularity::Total)
                        {
                            end = endTime;
                        }
                        else
                        {
                            end.UniversalTime += GranularityToTimeSpan(granularity).Duration;
                        }

                        outputString += PrintNetworkUsage(networkUsage, start, end);
                        start = end;
                    }

                    rootPage->Dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler([this, outputString]()
                    {
                        OutputText->Text = outputString;
                        rootPage->NotifyUser("Success", NotifyType::StatusMessage);
                    }));
                }
                catch (Exception^ ex)
                {
                    rootPage->Dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler([this, ex]()
                    {
                        rootPage->NotifyUser("An unexpected exception has occurred: " + ex->Message, NotifyType::ErrorMessage);
                    }));
                }
            });
        }
    }
    catch (Exception^ ex)
    {
        rootPage->NotifyUser("An unexpected exception occurred: " + ex->Message, NotifyType::ErrorMessage);
    }
}