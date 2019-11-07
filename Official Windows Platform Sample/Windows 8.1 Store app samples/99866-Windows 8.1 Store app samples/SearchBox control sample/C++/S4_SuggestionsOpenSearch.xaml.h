//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// S4_SuggestionsOpenSearch.xaml.h
// Declaration of the S4_SuggestionsOpenSearch class
//

#pragma once
#include "S4_SuggestionsOpenSearch.g.h"
#include "HttpRequest.h" 

#pragma warning(push)
#pragma warning(disable:4451) // Warns about possible invalid marshaling of objects across threads. In this case, the warning doesn't apply because the SearchPane object is only accessed from the UI thread.
namespace SDKSample
{
    namespace SearchControl
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class S4_SuggestionsOpenSearch sealed
        {
        public:
            S4_SuggestionsOpenSearch();

        private:
			void S4_SuggestionsOpenSearch::OnSearchBoxEventsSuggestionsRequested(Object^ sender, Windows::UI::Xaml::Controls::SearchBoxSuggestionsRequestedEventArgs^ e);
			void S4_SuggestionsOpenSearch::OnSearchBoxEventsQuerySubmitted(Object^ sender, Windows::UI::Xaml::Controls::SearchBoxQuerySubmittedEventArgs^ e);
			concurrency::cancellation_token_source cts;
			Web::HttpRequest httpRequest; 
        };
    }
}
#pragma warning(pop)