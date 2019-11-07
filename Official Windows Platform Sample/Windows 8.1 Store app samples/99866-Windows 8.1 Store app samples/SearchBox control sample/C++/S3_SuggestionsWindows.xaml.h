//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// S3_SuggestionsWindows.xaml.h
// Declaration of the S3_SuggestionsWindows class
//

#pragma once
#include "S3_SuggestionsWindows.g.h"

namespace SDKSample
{
    namespace SearchControl
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class S3_SuggestionsWindows sealed
        {
        public:
            S3_SuggestionsWindows();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
            virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

        private:
            void SetLocalContentSuggestions(bool isLocal);
            void OnSearchBoxEventsQuerySubmitted(Object^ sender, Windows::UI::Xaml::Controls::SearchBoxQuerySubmittedEventArgs^ e);
        };
    }
}
