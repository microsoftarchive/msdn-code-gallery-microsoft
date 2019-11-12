//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// S6_KeyboardFocus.xaml.h
// Declaration of the S1_SearchBoxWithSuggestions class
//

#pragma once
#include "S6_KeyboardFocus.g.h"

namespace SDKSample
{
    namespace SearchControl
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class S6_KeyboardFocus sealed
        {
        public:
            S6_KeyboardFocus();

        private:
			void OnSearchBoxEventsSuggestionsRequested(Object^ sender, Windows::UI::Xaml::Controls::SearchBoxSuggestionsRequestedEventArgs^ e);
			void OnSearchBoxEventsQuerySubmitted(Object^ sender, Windows::UI::Xaml::Controls::SearchBoxQuerySubmittedEventArgs^ e);
        };
    }
}
