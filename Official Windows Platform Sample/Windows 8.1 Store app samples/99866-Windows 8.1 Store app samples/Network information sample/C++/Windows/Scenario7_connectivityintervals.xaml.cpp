//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

// Scenario7_connectivityintervals.xaml.xaml.cpp
// Implementation of the ProfileConnectivityIntervals class

#include "pch.h"
#include "Scenario7_connectivityintervals.xaml.h"

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

ProfileConnectivityIntervals::ProfileConnectivityIntervals()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void ProfileConnectivityIntervals::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;

    internetConnectionProfile = NetworkInformation::GetInternetConnectionProfile();
    networkUsageStates = {};
    formatter = ref new Windows::Globalization::DateTimeFormatting::DateTimeFormatter("year month day hour minute second");
}

String^ ProfileConnectivityIntervals::PrintConnectivityIntervals(IVectorView<ConnectivityInterval^>^ connectivityIntervals)
{
    String^ result = "";

    if (connectivityIntervals == nullptr)
    {
        return "The Start Time cannot be later than the End Time, or in the future";
    }

    // Loop through the connectivity intervals
    for (ConnectivityInterval^ connectivityInterval : connectivityIntervals)
    {
        result += PrintConnectivityInterval(connectivityInterval);
    }

    return result;
}

String^ ProfileConnectivityIntervals::PrintConnectivityInterval(ConnectivityInterval^ connectivityInterval)
{
    DateTime endTime = connectivityInterval->StartTime;
    endTime.UniversalTime += connectivityInterval->ConnectionDuration.Duration;

    String^ result = "Connectivity Interval from " + formatter->Format(connectivityInterval->StartTime) + " to " + formatter->Format(endTime) + "\n";
    return result;
}

DateTime ProfileConnectivityIntervals::GetDateTimeFromUi(DatePicker^ datePicker, TimePicker^ timePicker)
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

TriStates ProfileConnectivityIntervals::GetTriStatesFromUi(ComboBox^ triStatesComboBox)
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
void SDKSample::NetworkInformationApi::ProfileConnectivityIntervals::ProfileConnectivityIntervalsButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    try
    {
        // Get settings from the UI
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
            // Get the list of ConnectivityIntervals asynchronously
            auto getConnectivityIntervalsTask = create_task(internetConnectionProfile->GetConnectivityIntervalsAsync(startTime, endTime, networkUsageStates));
            getConnectivityIntervalsTask.then([this](task<IVectorView<ConnectivityInterval^>^> tConnectivityIntervals)
            {
                try
                {
                    auto connectivityIntervals = tConnectivityIntervals.get();
                    String^ outputString;

                    outputString += PrintConnectivityIntervals(connectivityIntervals);

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
            }, task_continuation_context::use_arbitrary()); // Ensure the continuation is run in an arbitrary thread, rather than the UI thread
        }
    }
    catch (Exception^ ex)
    {
        rootPage->NotifyUser("An unexpected exception occurred: " + ex->Message, NotifyType::ErrorMessage);
    }
}