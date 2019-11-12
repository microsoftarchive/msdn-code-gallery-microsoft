//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

#include "pch.h"
#include "MainPage.xaml.h"
#include "Constants.h"

using namespace SDKSample;

using namespace std;
using namespace Platform;
using namespace Windows::ApplicationModel::Search;

Array<Scenario>^ MainPage::scenariosInner = ref new Array<Scenario>
{
    { "Using the Search contract",                "SDKSample.SearchContract.Scenario1" },
    { "Suggestions from an app-defined list",     "SDKSample.SearchContract.Scenario2" },
    { "Suggestions in East Asian languages",      "SDKSample.SearchContract.Scenario3" },
    { "Suggestions provided by Windows",          "SDKSample.SearchContract.Scenario4" },
    { "Suggestions from Open Search",             "SDKSample.SearchContract.Scenario5" },
    { "Suggestions from a service returning XML", "SDKSample.SearchContract.Scenario6" },
    { "Open Search charm by typing",              "SDKSample.SearchContract.Scenario7" },
};

void MainPage::ProcessQueryText(Platform::String^ queryText)
{
    NotifyUser("Query submitted: " + queryText, NotifyType::StatusMessage);
}

// Replace "{searchTerms}" part of the Open Search URL with the query text user typed into the search box.
String^ MainPage::ReplaceUrlSearchTerms(String^ strUrl, String^ queryText)
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

