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
// ParametrizedTemplate.xaml.cpp
// Implementation of the ParametrizedTemplate class
//

#include "pch.h"
#include "ParametrizedTemplate.xaml.h"

using namespace SDKSample::DateTimeFormatting;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Globalization;
using namespace Windows::Globalization::DateTimeFormatting;


ParametrizedTemplate::ParametrizedTemplate()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void ParametrizedTemplate::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void SDKSample::DateTimeFormatting::ParametrizedTemplate::Display_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	// This scenario uses the Windows.Globalization.DateTimeFormatting.DateTimeFormatter class
	// to format a date/time by specifying a template via parameters.  Note that the current application language 
	// language and region value will determine the pattern of the date returned based on the 
	// specified parts.

	//Get the current application context language
	String^ currentLanguage = ApplicationLanguages::Languages->GetAt(0);

    // Formatters for dates and times
    Array<DateTimeFormatter^> ^dateFormatters = {
		ref new DateTimeFormatter(
			YearFormat::Full, 
			MonthFormat::Abbreviated, 
			DayFormat::Default, 
			DayOfWeekFormat::Abbreviated),

		ref new DateTimeFormatter(
			YearFormat::Abbreviated, 
			MonthFormat::Abbreviated, 
			DayFormat::Default, 
			DayOfWeekFormat::None),

		ref new DateTimeFormatter(
			YearFormat::Full, 
			MonthFormat::Full, 
			DayFormat::None, 
			DayOfWeekFormat::None),

        ref new DateTimeFormatter(
			YearFormat::None, 
			MonthFormat::Full, 
			DayFormat::Default, 
			DayOfWeekFormat::None),

		ref new DateTimeFormatter(
			HourFormat::Default, 
			MinuteFormat::Default, 
			SecondFormat::Default),

		ref new DateTimeFormatter(
			HourFormat::Default, 
			MinuteFormat::Default, 
			SecondFormat::None),

		ref new DateTimeFormatter(
			HourFormat::Default, 
			MinuteFormat::None, 
			SecondFormat::None)
    };

	// Obtain the date that will be formatted.
	Windows::Globalization::Calendar^ cal = ref new Windows::Globalization::Calendar();
    Windows::Foundation::DateTime dateToFormat = cal->GetDateTime();
    
    // Keep the results here
    String^ results = "";
	results = results + "Current application context language: " + currentLanguage + "\n\n";

	// Generate the results.
    for (unsigned int i = 0; i < dateFormatters->Length; i++)
    {
        // Perform the actual formatting. 
        try 
        { 
            results = results + dateFormatters[i]->Template + ": " + dateFormatters[i]->Format(dateToFormat) + "\n";
        }
        catch(Platform::Exception^)
        {
            // Retrieve and display formatter properties. 
            results = results + "Unable to format using formatter with template: " +
                dateFormatters[i]->Template + ", language: " + 
                dateFormatters[i]->Languages->GetAt(0) + ", region: " + 
                dateFormatters[i]->GeographicRegion + ", calendar: " +
                dateFormatters[i]->Calendar + ", clock:" + 
                dateFormatters[i]->Clock + "\n";
        }
    }

	// Display the results.
    rootPage->NotifyUser(results, NotifyType::StatusMessage);
}
