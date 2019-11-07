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
// Scenario4.xaml.cpp
// Implementation of the Scenario4 class
//

#include "pch.h"
#include "Scenario4.xaml.h"

using namespace SDKSample::SearchContract;

using namespace Windows::ApplicationModel::Search;
using namespace Windows::Foundation;
using namespace Windows::Storage;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario4::Scenario4()
{
    InitializeComponent();
}

void Scenario4::SetLocalContentSuggestions(bool isLocal)
{
    // Have Windows provide suggestions from local files.
    // This code should be placed in your apps global scope and run as soon as your app is launched.
    auto settings = ref new LocalContentSuggestionSettings();
    settings->Enabled = isLocal;
    if (isLocal)
    {
        settings->Locations->Append(KnownFolders::MusicLibrary);
        settings->AqsFilter = "kind:Music";
    }
    SearchPane::GetForCurrentView()->SetLocalContentSuggestionSettings(settings);
}

void Scenario4::OnNavigatedTo(NavigationEventArgs^ e)
{
    MainPage::Current->NotifyUser("Use the search pane to submit a query", NotifyType::StatusMessage);
    SetLocalContentSuggestions(true);
}

void Scenario4::OnNavigatedFrom(NavigationEventArgs^ e)
{
    SetLocalContentSuggestions(false);
}
