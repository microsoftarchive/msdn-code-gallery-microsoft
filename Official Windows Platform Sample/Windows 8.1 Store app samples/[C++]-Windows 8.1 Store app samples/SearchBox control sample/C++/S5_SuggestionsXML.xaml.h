//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// S5_SuggestionsXML.xaml.h
// Declaration of the S5_SuggestionsXML class
//

#pragma once
#include "S5_SuggestionsXML.g.h"
#include "HttpRequest.h" 
#include "MainPage.xaml.h" 


#pragma warning(push)
#pragma warning(disable:4451) // Warns about possible invalid marshaling of objects across threads. In this case, the warning doesn't apply because the SearchPane object is only accessed from the UI thread.
namespace SDKSample
{
    namespace SearchControl
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class S5_SuggestionsXML sealed
        {
        public:
            S5_SuggestionsXML();

        private:
            void S5_SuggestionsXML::AddSuggestionFromNode(Windows::Data::Xml::Dom::IXmlNode^ node, Windows::ApplicationModel::Search::SearchSuggestionCollection^ suggestions);
            void S5_SuggestionsXML::GetSuggestions(Windows::Data::Xml::Dom::XmlDocument^ doc, Windows::UI::Xaml::Controls::SearchBoxSuggestionsRequestedEventArgs^ e);
            void S5_SuggestionsXML::OnSearchBoxEventsSuggestionsRequested(Object^ sender, Windows::UI::Xaml::Controls::SearchBoxSuggestionsRequestedEventArgs^ e) ;
            void S5_SuggestionsXML::OnSearchBoxEventsQuerySubmitted(Object^ sender, Windows::UI::Xaml::Controls::SearchBoxQuerySubmittedEventArgs^ e);
            void S5_SuggestionsXML::OnSearchBoxEventsResultSuggestionChosen(Object^ sender, Windows::UI::Xaml::Controls::SearchBoxResultSuggestionChosenEventArgs^ e);

            concurrency::cancellation_token_source cts;
        };
    }
}
#pragma warning(pop)