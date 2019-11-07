//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// S2_SuggestionsEastAsian.xaml.h
// Declaration of the S2_SuggestionsEastAsian class
//

#pragma once
#include "S2_SuggestionsEastAsian.g.h"

namespace SDKSample
{
    namespace SearchControl
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class S2_SuggestionsEastAsian sealed
        {
        public:
            S2_SuggestionsEastAsian();

        private:
            void OnSearchBoxEventsSuggestionsRequested(Object^ sender, Windows::UI::Xaml::Controls::SearchBoxSuggestionsRequestedEventArgs^ e);
            void OnSearchBoxEventsQuerySubmitted(Object^ sender,  Windows::UI::Xaml::Controls::SearchBoxQuerySubmittedEventArgs^ e);
            void AppendSuggestion(std::wstring textToMatch, Windows::UI::Xaml::Controls::SearchBoxSuggestionsRequestedEventArgs^ e);
        };
    }
}
