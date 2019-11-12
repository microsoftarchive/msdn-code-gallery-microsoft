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
#include "StringTemplate.xaml.h"

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

    // Formatters for dates and times
    Array<DateTimeFormatter^> ^dateFormatters = {
		ref new DateTimeFormatter("month day"),
		ref new DateTimeFormatter("month year"),
		ref new DateTimeFormatter("month day year"),
		ref new DateTimeFormatter("month day dayofweek year"),
		ref new DateTimeFormatter("dayofweek.abbreviated"),
		ref new DateTimeFormatter("month.abbreviated"),
		ref new DateTimeFormatter("year.abbreviated"),
		ref new DateTimeFormatter("hour minute"),
		ref new DateTimeFormatter("hour minute second"),
		ref new DateTimeFormatter("hour")
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
