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
// TimeZoneSupport.xaml.cpp
// Implementation of the TimeZoneSupport class
//

#include "pch.h"
#include "Scenario6_TimeZoneSupport.xaml.h"

using namespace SDKSample::DateTimeFormatting;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Globalization;
using namespace Windows::Globalization::DateTimeFormatting;

TimeZoneSupport::TimeZoneSupport()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void TimeZoneSupport::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void SDKSample::DateTimeFormatting::TimeZoneSupport::Display_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // This scenario illustrates TimeZone support in DateTimeFormatter class
    
    // Displayed TimeZones (other than local timezone)
    auto timeZones = ref new Platform::Collections::Vector<String^>();
    timeZones->Append("UTC");
    timeZones->Append("America/New_York");
    timeZones->Append("Asia/Kolkata");
    
    // Keep the results here
    String^ results = "";
    
    // Create formatter object using longdate and longtime template
    DateTimeFormatter^ dateFormatter = ref new DateTimeFormatter("longdate longtime");
    
    // Create date/time to format and display.
    Windows::Globalization::Calendar^ calendar = ref new Windows::Globalization::Calendar();
    Windows::Foundation::DateTime dateToFormat = calendar->GetDateTime();    
    
    // Show current time in timezones desired to be displayed including local timezone
    results += "Current date and time -" + "\n" +
        "In Local timezone:   " + dateFormatter->Format(dateToFormat) + "\n";
    for each (String^ timezone in timeZones)
    {
        results += "In " + timezone + " timezone:   " + dateFormatter->Format(dateToFormat, timezone) + "\n";
    }
    results += "\n";
    
    // Show a time on 14th day of second month of next year in local, and other desired timezones
    // This will show if there were day light savings in time
    calendar->AddYears(1); calendar->Month = 2; calendar->Day = 14;
    dateToFormat = calendar->GetDateTime();
    results += "Same time on 14th day of second month of next year -"+ "\n" +
        "In Local timezone:   " + dateFormatter->Format(dateToFormat) + "\n";
    for each (String^ timezone in timeZones)
    {
        results += "In " + timezone + " timezone:   " + dateFormatter->Format(dateToFormat, timezone) + "\n";
    }
    results += "\n";
    
    // Show a time on 14th day of 10th month of next year in local, and other desired timezones
    // This will show if there were day light savings in time
    calendar->AddMonths(8);
    dateToFormat = calendar->GetDateTime();
    results += "Same time on 14th day of tenth month of next year -"+ "\n" +
        "In Local timezone:   " + dateFormatter->Format(dateToFormat) + "\n";
    for each (String^ timezone in timeZones)
    {
        results += "In " + timezone + " timezone:   " + dateFormatter->Format(dateToFormat, timezone) + "\n";
    }
    results += "\n";

    // Display the results.
    OutputTextBlock->Text = results;
}
