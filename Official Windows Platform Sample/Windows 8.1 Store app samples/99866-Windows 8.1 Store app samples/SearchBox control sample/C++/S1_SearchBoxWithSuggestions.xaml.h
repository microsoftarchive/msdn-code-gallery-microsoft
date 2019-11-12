//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// S1_SearchBoxWithSuggestions.xaml.h
// Declaration of the S1_SearchBoxWithSuggestions class
//

#pragma once
#include "S1_SearchBoxWithSuggestions.g.h"

namespace SDKSample
{
    namespace SearchControl
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class S1_SearchBoxWithSuggestions sealed
        {
        public:
            S1_SearchBoxWithSuggestions();

        private:
			void OnSearchBoxEventsSuggestionsRequested(Object^ sender, Windows::UI::Xaml::Controls::SearchBoxSuggestionsRequestedEventArgs^ e);
			void OnSearchBoxEventsQuerySubmitted(Object^ sender, Windows::UI::Xaml::Controls::SearchBoxQuerySubmittedEventArgs^ e);
        };
    }
}
