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
// StringTemplate.xaml.cpp
// Implementation of the StringTemplate class
//

#include "pch.h"
#include "Scenario2_StringTemplate.xaml.h"

using namespace SDKSample::DateTimeFormatting;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Globalization;
using namespace Windows::Globalization::DateTimeFormatting;

StringTemplate::StringTemplate()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void StringTemplate::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void SDKSample::DateTimeFormatting::StringTemplate::Display_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // This scenario uses the Windows.Globalization.DateTimeFormatting.DateTimeFormatter class
    // to format a date/time via a string template.  Note that the order specifed in the string pattern does 
    // not determine the order of the parts of the formatted string.  The current application language and region value will
    // determine the pattern of the date returned based on the specified parts.

    //Get the current application context language
    String^ currentLanguage = ApplicationLanguages::Languages->GetAt(0);

    // Formatters for dates.
    Array<DateTimeFormatter^> ^dateFormatters = {
        ref new DateTimeFormatter("month day"),
        ref new DateTimeFormatter("month year"),
        ref new DateTimeFormatter("month day year"),
        ref new DateTimeFormatter("month day dayofweek year"),
        ref new DateTimeFormatter("dayofweek.abbreviated"),
        ref new DateTimeFormatter("month.abbreviated"),
        ref new DateTimeFormatter("year.abbreviated")
    };

    // Formatters for time.
    Array<DateTimeFormatter^> ^timeFormatters = {
        ref new DateTimeFormatter("hour minute"),
        ref new DateTimeFormatter("hour minute second"),
        ref new DateTimeFormatter("hour")
    };

    // Formatters for timezone.
    Array<DateTimeFormatter^> ^timezoneFormatters = {
        ref new DateTimeFormatter("timezone"),
        ref new DateTimeFormatter("timezone.full"),
        ref new DateTimeFormatter("timezone.abbreviated")
    };
                
    // Formatters for combinations.
    Array<DateTimeFormatter^> ^combinationFormatters = {
        ref new DateTimeFormatter("hour minute second timezone.full"),
        ref new DateTimeFormatter("day month year hour minute timezone"),
        ref new DateTimeFormatter("dayofweek day month year hour minute second"),
        ref new DateTimeFormatter("dayofweek.abbreviated day month hour minute"),
        ref new DateTimeFormatter("dayofweek day month year hour minute second timezone.abbreviated")
    };

    // Obtain the date that will be formatted.
    Windows::Globalization::Calendar^ cal = ref new Windows::Globalization::Calendar();
    Windows::Foundation::DateTime dateToFormat = cal->GetDateTime();
    
    // Keep the results here
    String^ results = "";
    results = results + "Current application context language: " + currentLanguage + "\n\n";
    results = results + "Formatted Dates:\n";

    // Generate the results.
    for (unsigned int i = 0; i < dateFormatters->Length; i++)
    {
        // Perform the actual formatting. 
        results = results + dateFormatters[i]->Template + ": " + dateFormatters[i]->Format(dateToFormat) + "\n";
    }

    results = results + "\n";
    results = results + "Formatted Times:\n";

    // Generate the results.
    for (unsigned int i = 0; i < timeFormatters->Length; i++)
    {
        // Perform the actual formatting. 
        results = results + timeFormatters[i]->Template + ": " + timeFormatters[i]->Format(dateToFormat) + "\n";
    }

    results = results + "\n";
    results = results + "Formatted timezones:\n";
        
    // Generate the results.
    for (unsigned int i = 0; i < timezoneFormatters->Length; i++)
    {
        // Perform the actual formatting. 
        results = results + timezoneFormatters[i]->Template + ": " + timezoneFormatters[i]->Format(dateToFormat) + "\n";
    }

    results = results + "\n";
    results = results + "Formatted Date and Time Combinations:\n";
        
    // Generate the results.
    for (unsigned int i = 0; i < combinationFormatters->Length; i++)
    {
        // Perform the actual formatting. 
        results = results + combinationFormatters[i]->Template + ": " + combinationFormatters[i]->Format(dateToFormat) + "\n";
    }

    // Display the results.
    OutputTextBlock->Text = results;
}
