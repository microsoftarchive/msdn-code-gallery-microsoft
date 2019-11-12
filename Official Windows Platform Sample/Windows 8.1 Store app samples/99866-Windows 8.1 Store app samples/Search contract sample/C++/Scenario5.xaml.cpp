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
// Scenario5.xaml.cpp
// Implementation of the Scenario5 class
//

#include "pch.h"
#include "Scenario5.xaml.h"

using namespace SDKSample::SearchContract;

using namespace concurrency;
using namespace std;
using namespace Platform;
using namespace Windows::ApplicationModel::Search;
using namespace Windows::Data::Json;
using namespace Windows::Foundation;
using namespace Windows::Storage;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario5::Scenario5()
{
    InitializeComponent();
    searchPane = SearchPane::GetForCurrentView();
}

void Scenario5::OnSearchPaneSuggestionsRequested(SearchPane^ sender, SearchPaneSuggestionsRequestedEventArgs^ e)
{
    auto queryText = e->QueryText;
    unsigned int queryTextLength = queryText->Length();
    if (queryTextLength == 0)
    {
        MainPage::Current->NotifyUser("Use the search pane to submit a query", NotifyType::StatusMessage);
    }
    else if (UrlTextBox->Text == "")
    {
        MainPage::Current->NotifyUser("Please enter the web service URL", NotifyType::StatusMessage);
    }
    else
    {
        // Cancel the previous suggestion request if it is not finished.
        cts.cancel();
        cts = cancellation_token_source();

        // The deferral object is used to supply suggestions asynchronously for example when fetching suggestions from a web service.
        // Indicate that we'll do this asynchronously:
        auto request = e->Request;
        auto deferral = request->GetDeferral();

        try
        {
            // Use the web service Url entered in the UrlTextBox that supports OpenSearch Suggestions in order to see suggestions come from the web service.
            // See http://www.opensearch.org/Specifications/OpenSearch/Extensions/Suggestions/1.0 for details on OpenSearch Suggestions format.
            // Replace "{searchTerms}" of the Url with the query string.
            String^ strUrl = MainPage::ReplaceUrlSearchTerms(UrlTextBox->Text, Uri::EscapeComponent(queryText));
            auto uri = ref new Uri(strUrl);
            auto token = cts.get_token();
            httpRequest.GetAsync(uri, token).then([=](task<wstring> response)
            {
                Finally f ([&] {
                    deferral->Complete(); // Indicate we're done supplying suggestions.
                });
                if (token.is_canceled())
                {
                    cancel_current_task();
                }
                else
                {
                    try
                    {
                        String^ jsonText = ref new String(response.get().c_str());
                        JsonArray^ parsedResponse = JsonArray::Parse(jsonText);
                        if (parsedResponse->Size > 1)
                        {
                            parsedResponse = JsonArray::Parse(parsedResponse->GetAt(1)->Stringify());
                            for (unsigned int i = 0; i < parsedResponse->Size; i++)
                            {
                                request->SearchSuggestionCollection->AppendQuerySuggestion(parsedResponse->GetAt(i)->GetString());
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
                    catch (task_canceled &)
                    {
                        // We have canceled the task
                    }
                    catch (Exception^)
                    {
                        MainPage::Current->NotifyUser("Suggestions could not be displayed, please verify that the service provides valid OpenSearch suggestions", NotifyType::ErrorMessage);
                    }
                }
            }, task_continuation_context::use_current()); // Ensure that the task continuation comes back to the UI thread.
        }
        catch (Exception^)
        {
            MainPage::Current->NotifyUser("Suggestions could not be displayed, please verify that the service provides valid OpenSearch suggestions", NotifyType::ErrorMessage);
        }
    }
}

void Scenario5::OnNavigatedTo(NavigationEventArgs^ e)
{
    MainPage::Current->NotifyUser("Use the search pane to submit a query", NotifyType::StatusMessage);
    // This event should be registered when your app first creates its main window after receiving an activated event, like OnLaunched, OnSearchActivated.
    // Typically this occurs in App.xaml.cpp.
    token = searchPane->SuggestionsRequested += ref new TypedEventHandler<SearchPane^, SearchPaneSuggestionsRequestedEventArgs^>(this, &Scenario5::OnSearchPaneSuggestionsRequested);
}

void Scenario5::OnNavigatedFrom(NavigationEventArgs^ e)
{
    searchPane->SuggestionsRequested -= token;
}
