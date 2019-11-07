//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#include "pch.h"
#include "MainPage.xaml.h"
#include "Constants.h"

using namespace std; 
using namespace Platform; 
using namespace Windows::ApplicationModel::Search; 
using namespace SDKSample;
using namespace SDKSample::SearchControl;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>  
{
    { "App provided suggestions",                    "SDKSample.SearchControl.S1_SearchBoxWithSuggestions" }, 
    { "Suggestions in East Asian Languages",         "SDKSample.SearchControl.S2_SuggestionsEastAsian" },
    { "Windows provided suggestions",                "SDKSample.SearchControl.S3_SuggestionsWindows" },
    { "Suggestions from Open Search",                "SDKSample.SearchControl.S4_SuggestionsOpenSearch" },
    { "Search box and suggestions from Open Search", "SDKSample.SearchControl.S5_SuggestionsXML" },
    { "Give the search box focus by typing",         "SDKSample.SearchControl.S6_KeyboardFocus" }
}; 

// Replace "{searchTerms}" part of the Open Search URL with the query text user typed into the search box. 
Platform::String^ MainPage::ReplaceUrlSearchTerms(Platform::String^ strUrl, Platform::String^ queryText) 
{ 
    wstring url = wstring(strUrl->Data()); 
    const wstring fixed = L"{searchTerms}"; 
    size_t start = url.find(fixed); 
    if (start != std::string::npos) 
    { 
        url.replace(start, fixed.length(), wstring(queryText->Data())); 
    } 
 
    return ref new String(url.c_str()); 
}

