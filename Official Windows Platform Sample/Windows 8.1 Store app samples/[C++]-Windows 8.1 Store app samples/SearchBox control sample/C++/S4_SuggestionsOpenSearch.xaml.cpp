//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// S4_SuggestionsOpenSearch.xaml.cpp
// Implementation of the S4_SuggestionsOpenSearch class
//

#include "pch.h"
#include "S4_SuggestionsOpenSearch.xaml.h"
#include "MainPage.xaml.h"

using namespace std;
using namespace concurrency; 
using namespace Platform; 
using namespace SDKSample;
using namespace SDKSample::SearchControl;
using namespace Windows::ApplicationModel::Search; 
using namespace Windows::Data::Json;
using namespace Windows::Foundation; 
using namespace Windows::Storage; 
using namespace Windows::Storage::Search; 
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation; 

S4_SuggestionsOpenSearch::S4_SuggestionsOpenSearch()
{
    InitializeComponent();
}

void S4_SuggestionsOpenSearch::OnSearchBoxEventsSuggestionsRequested(Object^ sender, SearchBoxSuggestionsRequestedEventArgs^ e) 
{ 
	auto queryText = e->QueryText;
	unsigned int queryTextLength = queryText->Length();
	if (queryTextLength == 0)
	{
		MainPage::Current->NotifyUser("Use the search control to submit a query", NotifyType::StatusMessage);
	}
	else if (UrlTextBox->Text == "")
	{
		MainPage::Current->NotifyUser("Use the search control to submit a query", NotifyType::StatusMessage);
	}
	else
	{
		cts.cancel();
		cts = cancellation_token_source();
		auto request = e->Request;
		// The deferral object is used to supply suggestions asynchronously for example when fetching suggestions from a web service.
		auto defer = request->GetDeferral();
		try
		{
			String^ strUrl = MainPage::ReplaceUrlSearchTerms(UrlTextBox->Text, Uri::EscapeComponent(queryText));
			auto uri = ref new Uri(strUrl);
			auto token = cts.get_token();
			httpRequest.GetAsync(uri,token).then([=](task<wstring> response)
			{
				 Finally f ([&] { 
					 // Indicate we're done supplying suggestions.
                    defer->Complete();
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

                        MainPage::Current->NotifyUser("Suggestions could not be displayed -- please verify that the service provides valid OpenSearch suggestions", NotifyType::ErrorMessage); 
                    } 
                } 
            }, task_continuation_context::use_current()); // Ensure that the task continuation comes back to the UI thread. 
        } 
        catch (Exception^) 
        { 
            MainPage::Current->NotifyUser("Suggestions could not be displayed -- please verify that the service provides valid OpenSearch suggestions", NotifyType::ErrorMessage); 
        } 
	}
}	

void S4_SuggestionsOpenSearch::OnSearchBoxEventsQuerySubmitted(Object^ sender, SearchBoxQuerySubmittedEventArgs^ e)
{
	MainPage::Current->NotifyUser(e->QueryText, NotifyType::StatusMessage);
}


 



