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
// Scenario6.xaml.h
// Declaration of the Scenario6 class
//

#pragma once

#include "pch.h"
#include "Scenario6.g.h"
#include "MainPage.xaml.h"

#pragma warning(push)
#pragma warning(disable:4451) // Warns about possible invalid marshaling of objects across threads. In this case, the warning doesn't apply because the SearchPane object is only accessed from the UI thread.
namespace SDKSample
{
    namespace SearchContract
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class Scenario6 sealed
        {
        public:
            Scenario6();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
            virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
            void AddSuggestionFromNode(Windows::Data::Xml::Dom::IXmlNode^ node, Windows::ApplicationModel::Search::SearchSuggestionCollection^ suggestions);
            void GetSuggestions(Windows::Data::Xml::Dom::XmlDocument^ doc, Windows::ApplicationModel::Search::SearchPaneSuggestionsRequest^ request);
            void OnSearchPaneSuggestionsRequested(Windows::ApplicationModel::Search::SearchPane^ sender, Windows::ApplicationModel::Search::SearchPaneSuggestionsRequestedEventArgs^ e);
            void OnResultSuggestionChosen(Windows::ApplicationModel::Search::SearchPane^ sender, Windows::ApplicationModel::Search::SearchPaneResultSuggestionChosenEventArgs^ e);

        private:
            Windows::ApplicationModel::Search::SearchPane^ searchPane;
            Windows::Foundation::EventRegistrationToken suggestionsRequestedToken, resultSuggestionChosenToken;
            concurrency::cancellation_token_source cts;
        };
    }
}
#pragma warning(pop) // Reenable warning 4451
