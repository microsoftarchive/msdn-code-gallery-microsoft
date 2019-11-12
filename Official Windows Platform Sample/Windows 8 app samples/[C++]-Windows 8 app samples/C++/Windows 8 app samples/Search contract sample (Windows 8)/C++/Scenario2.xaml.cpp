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
// Scenario2.xaml.cpp
// Implementation of the Scenario2 class
//

#include "pch.h"
#include "Scenario2.xaml.h"

using namespace SDKSample::SearchContract;

using namespace std;
using namespace Platform;
using namespace Windows::ApplicationModel::Search;
using namespace Windows::Foundation;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario2::Scenario2()
{
    InitializeComponent();
    searchPane = SearchPane::GetForCurrentView();
}

void Scenario2::OnNavigatedTo(NavigationEventArgs^ e)
{
    MainPage::Current->NotifyUser("Use the search pane to submit a query", NotifyType::StatusMessage);
    // This event should be registered when your app first creates its main window in App::OnWindowCreated().
    token = searchPane->SuggestionsRequested += ref new TypedEventHandler<SearchPane^, SearchPaneSuggestionsRequestedEventArgs^>(this, &Scenario2::OnSearchPaneSuggestionsRequested);
}

void Scenario2::OnNavigatedFrom(NavigationEventArgs^ e)
{
    searchPane->SuggestionsRequested -= token;
}

void Scenario2::OnSearchPaneSuggestionsRequested(SearchPane^ sender, SearchPaneSuggestionsRequestedEventArgs^ e)
{
    static wstring suggestionList [] =
        {
            L"shanghai", L"istanbul", L"karachi", L"delhi", L"mumbai", L"moscow", L"são paulo", L"seoul", L"beijing", L"jakarta",
            L"tokyo", L"mexico city", L"kinshasa", L"new york city", L"lagos", L"london", L"lima", L"bogota", L"tehran", L"ho chi minh city",
            L"hong kong", L"bangkok", L"dhaka", L"cairo", L"hanoi", L"rio de janeiro", L"lahore", L"chonquing", L"bengaluru", L"tianjin",
            L"baghdad", L"riyadh", L"singapore", L"santiago", L"saint petersburg", L"surat", L"chennai", L"kolkata", L"yangon", L"guangzhou",
            L"alexandria", L"shenyang", L"hyderabad", L"ahmedabad", L"ankara", L"johannesburg", L"wuhan", L"los angeles", L"yokohama",
            L"abidjan", L"busan", L"cape town", L"durban", L"pune", L"jeddah", L"berlin", L"pyongyang", L"kanpur", L"madrid", L"jaipur",
            L"nairobi", L"chicago", L"houston", L"philadelphia", L"phoenix", L"san antonio", L"san diego", L"dallas", L"san jose",
            L"jacksonville", L"indianapolis", L"san francisco", L"austin", L"columbus", L"fort worth", L"charlotte", L"detroit",
            L"el paso", L"memphis", L"baltimore", L"boston", L"seattle washington", L"nashville", L"denver", L"louisville", L"milwaukee",
            L"portland", L"las vegas", L"oklahoma city", L"albuquerque", L"tucson", L"fresno", L"sacramento", L"long beach", L"kansas city",
            L"mesa", L"virginia beach", L"atlanta", L"colorado springs", L"omaha", L"raleigh", L"miami", L"cleveland", L"tulsa", L"oakland",
            L"minneapolis", L"wichita", L"arlington", L"bakersfield", L"new orleans", L"honolulu", L"anaheim", L"tampa", L"aurora",
            L"santa ana", L"st. louis", L"pittsburgh", L"corpus christi", L"riverside", L"cincinnati", L"lexington", L"anchorage",
            L"stockton", L"toledo", L"st. paul", L"newark", L"greensboro", L"buffalo", L"plano", L"lincoln", L"henderson", L"fort wayne",
            L"jersey city", L"st. petersburg", L"chula vista", L"norfolk", L"orlando", L"chandler", L"laredo", L"madison", L"winston-salem",
            L"lubbock", L"baton rouge", L"durham", L"garland", L"glendale", L"reno", L"hialeah", L"chesapeake", L"scottsdale",
            L"north las vegas", L"irving", L"fremont", L"irvine", L"birmingham", L"rochester", L"san bernardino", L"spokane",
            L"toronto", L"montreal", L"vancouver", L"ottawa-gatineau", L"calgary", L"edmonton", L"quebec city", L"winnipeg", L"hamilton"
        };

    auto queryText = e->QueryText;
    wstring query = wstring(queryText->Data());
    // convert query string to lower case.
    transform(query.begin(), query.end(), query.begin(), tolower);
    if (queryText->Length() == 0)
    {
        MainPage::Current->NotifyUser("Use the search pane to submit a query", NotifyType::StatusMessage);
    }
    else
    {
        auto request = e->Request;
        for each(wstring suggestion in suggestionList)
        {
            if (suggestion.find(query) == 0)
            {
                // if the string starts with the queryText (ignoring case), add suggestion to search pane.
                request->SearchSuggestionCollection->AppendQuerySuggestion(ref new String(const_cast<wchar_t*>(suggestion.c_str())));

                // break since the search pane can show at most 5 suggestions.
                if (request->SearchSuggestionCollection->Size >= MainPage::SearchPaneMaxSuggestions)
                {
                    break;
                }
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
