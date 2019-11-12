//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// S6_KeyboardFocus.xaml.cpp
// Implementation of the S6_KeyBoardFocus class
//

#include "pch.h"
#include "S6_KeyboardFocus.xaml.h"
#include "MainPage.xaml.h"

using namespace std; 
using namespace Platform;
using namespace SDKSample;
using namespace SDKSample::SearchControl;
using namespace Windows::ApplicationModel::Search;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;

S6_KeyboardFocus::S6_KeyboardFocus()
{
    InitializeComponent();
}

void S6_KeyboardFocus::OnSearchBoxEventsSuggestionsRequested(Object^ sender, SearchBoxSuggestionsRequestedEventArgs^ e) 
{ 
    //App provided suggestions list
    static wstring suggestionList [] = 
        { 
            L"Shanghai", L"Istanbul", L"Karachi", L"Delhi", L"Mumbai", L"Moscow", L"São Paulo", L"Seoul", L"Beijing", L"Jakarta",
            L"Tokyo", L"Mexico City", L"Kinshasa", L"New York City", L"Lagos", L"London", L"Lima", L"Bogota", L"Tehran", L"Ho Chi Minh City",
            L"Hong Kong", L"Bangkok", L"Dhaka", L"Cairo", L"Hanoi", L"Rio de Janeiro", L"Lahore", L"Chonquing", L"Bengaluru", L"Tianjin",
            L"Baghdad", L"Riyadh", L"Singapore", L"Santiago", L"Saint Petersburg", L"Surat", L"Chennai", L"Kolkata", L"Yangon", L"Guangzhou",
            L"Alexandria", L"Shenyang", L"Hyderabad", L"Ahmedabad", L"Ankara", L"Johannesburg", L"Wuhan", L"Los Angeles", L"Yokohama",
            L"Abidjan", L"Busan", L"Cape Town", L"Durban", L"Pune", L"Jeddah", L"Berlin", L"Pyongyang", L"Kanpur", L"Madrid", L"Jaipur",
            L"Nairobi", L"Chicago", L"Houston", L"Philadelphia", L"Phoenix", L"San Antonio", L"San Diego", L"Dallas", L"San Jose",
            L"Jacksonville", L"Indianapolis", L"San Francisco", L"Austin", L"Columbus", L"Fort Worth", L"Charlotte", L"Detroit",
            L"El Paso", L"Memphis", L"Baltimore", L"Boston", L"Seattle Washington", L"Nashville", L"Denver", L"Louisville", L"Milwaukee",
            L"Portland", L"Las Vegas", L"Oklahoma City", L"Albuquerque", L"Tucson", L"Fresno", L"Sacramento", L"Long Beach", L"Kansas City",
            L"Mesa", L"Virginia Beach", L"Atlanta", L"Colorado Springs", L"Omaha", L"Raleigh", L"Miami", L"Cleveland", L"Tulsa", L"Oakland",
            L"Minneapolis", L"Wichita", L"Arlington", L"Bakersfield", L"New Orleans", L"Honolulu", L"Anaheim", L"Tampa", L"Aurora",
            L"Santa Ana", L"St. Louis", L"Pittsburgh", L"Corpus Christi", L"Riverside", L"Cincinnati", L"Lexington", L"Anchorage",
            L"Stockton", L"Toledo", L"St. Paul", L"Newark", L"Greensboro", L"Buffalo", L"Plano", L"Lincoln", L"Henderson", L"Fort Wayne",
            L"Jersey City", L"St. Petersburg", L"Chula Vista", L"Norfolk", L"Orlando", L"Chandler", L"Laredo", L"Madison", L"Winston-Salem",
            L"Lubbock", L"Baton Rouge", L"Durham", L"Garland", L"Glendale", L"Reno", L"Hialeah", L"Chesapeake", L"Scottsdale",
            L"North Las Vegas", L"Irving", L"Fremont", L"Irvine", L"Birmingham", L"Rochester", L"San Bernardino", L"Spokane",
            L"Toronto", L"Montreal", L"Vancouver", L"Ottawa-Gatineau", L"Calgary", L"Edmonton", L"Quebec City", L"Winnipeg", L"Hamilton"
        }; 
 
    auto queryText = e->QueryText; 
    wstring query = wstring(queryText->Data()); 
    // convert query string to lower case. 
    transform(query.begin(), query.end(), query.begin(), tolower); 
    if (queryText->Length() == 0) 
    {
        MainPage::Current->NotifyUser("Use the search control to submit a query", NotifyType::StatusMessage); 
    } 
    else 
    {
        // Capital first letter
        transform(query.begin(), query.begin() + 1, query.begin(), toupper);
        auto request = e->Request; 
        for (const auto& suggestion : suggestionList)
        { 
            if (suggestion.find(query) == 0) 
            { 
                // if the string starts with the queryText (ignoring case), add suggestion to search pane. 
                request->SearchSuggestionCollection->AppendQuerySuggestion(ref new String(suggestion.c_str())); 
            } 
 
        } 
 
        if (request->SearchSuggestionCollection->Size > 0) 
        { 
            MainPage::Current->NotifyUser("Suggestions provided for query: " + queryText, NotifyType::StatusMessage); 
        } 
        else 
        { 
            MainPage::Current->NotifyUser("No suggestions provided for query: " + queryText, NotifyType::StatusMessage); 
        } 
    } 
}

void S6_KeyboardFocus::OnSearchBoxEventsQuerySubmitted(Object^ sender, SearchBoxQuerySubmittedEventArgs^ e) 
{
    auto queryText = e->QueryText; 
    unsigned int queryTextLength = queryText->Length(); 
    if (queryTextLength != 0) 
    {
        MainPage::Current->NotifyUser(queryText, NotifyType::StatusMessage);
    }
}