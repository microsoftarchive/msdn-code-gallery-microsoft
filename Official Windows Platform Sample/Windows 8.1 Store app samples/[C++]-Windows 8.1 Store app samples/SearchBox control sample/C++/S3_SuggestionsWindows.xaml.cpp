//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// S3_SuggestionsWindows.xaml.cpp
// Implementation of the S3_SuggestionsWindows class
//

#include "pch.h"
#include "S3_SuggestionsWindows.xaml.h"
#include "MainPage.xaml.h"

using namespace concurrency; 
using namespace Platform; 
using namespace SDKSample;
using namespace SDKSample::SearchControl;
using namespace Windows::ApplicationModel::Search; 
using namespace Windows::Foundation::Collections; 
using namespace Windows::Storage; 
using namespace Windows::Storage::Search; 
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

S3_SuggestionsWindows::S3_SuggestionsWindows()
{
    InitializeComponent();
}

void S3_SuggestionsWindows::SetLocalContentSuggestions(bool isLocal)
{
    // Have Windows provide suggestions from local files.
    // This code should be placed in your app's global scope and run as soon as your app is launched.
    auto settings = ref new LocalContentSuggestionSettings();
    settings->Enabled = isLocal;
    if (isLocal)
    {
        settings->Locations->Append(KnownFolders::MusicLibrary);
        settings->AqsFilter = "kind:Music";
    }
    SearchBoxSuggestions->SetLocalContentSuggestionSettings(settings);
}

void S3_SuggestionsWindows::OnNavigatedTo(NavigationEventArgs^ e)
{
    SetLocalContentSuggestions(true);
}

void S3_SuggestionsWindows::OnNavigatedFrom(NavigationEventArgs^ e)
{
    SetLocalContentSuggestions(false);
}

void S3_SuggestionsWindows::OnSearchBoxEventsQuerySubmitted(Object^ sender, SearchBoxQuerySubmittedEventArgs^ e)
{
    MainPage::Current->NotifyUser(e->QueryText, NotifyType::StatusMessage);
}
 



