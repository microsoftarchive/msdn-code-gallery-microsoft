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
// Scenario6.xaml.cpp
// Implementation of the Scenario6 class
//

#include "pch.h"
#include "Scenario6.xaml.h"

using namespace SDKSample::SearchContract;

using namespace concurrency;
using namespace std;
using namespace Platform;
using namespace Windows::ApplicationModel::Search;
using namespace Windows::Data::Xml::Dom;
using namespace Windows::Foundation;
using namespace Windows::Storage;
using namespace Windows::Storage::Streams;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario6::Scenario6()
{
    InitializeComponent();
    searchPane = SearchPane::GetForCurrentView();
}

void Scenario6::AddSuggestionFromNode(IXmlNode^ node, SearchSuggestionCollection^ suggestions)
{
    String^ text = "";
    String^ description = "";
    String^ url = "";
    String^ imageUrl = "";
    String^ imageAlt = "";

    for (unsigned int i = 0; i < node->ChildNodes->Size; i++)
    {
        IXmlNode^ subNode = node->ChildNodes->GetAt(i);
        if (subNode->NodeType != NodeType::ElementNode)
        {
            continue;
        }

        if (subNode->NodeName->Equals("Text"))
        {
            text = subNode->InnerText;
        }
        else if (subNode->NodeName->Equals("Description"))
        {
            description = subNode->InnerText;
        }
        else if (subNode->NodeName->Equals("Url"))
        {
            url = subNode->InnerText;
        }
        else if (subNode->NodeName->Equals("Image"))
        {
            if (subNode->Attributes->GetNamedItem("source"))
            {
                imageUrl = subNode->Attributes->GetNamedItem("source")->InnerText;
            }

            if (subNode->Attributes->GetNamedItem("alt"))
            {
                imageAlt = subNode->Attributes->GetNamedItem("alt")->InnerText;
            }
        }
    }

    if (text->IsEmpty())
    {
        // No proper suggestion item exists
    }
    else if (url->IsEmpty())
    {
        suggestions->AppendQuerySuggestion(text);
    }
    else
    {
        // The following image should not be used in your application for Result Suggestions.  Replace the image with one that is tailored to your content
        Uri^ uri = imageUrl->IsEmpty() ? ref new Uri("ms-appx:///Assets/SDK_ResultSuggestionImage.png") : ref new Uri(imageUrl);
        RandomAccessStreamReference^ imageSource = RandomAccessStreamReference::CreateFromUri(uri);
        suggestions->AppendResultSuggestion(text, description, url, imageSource, imageAlt);
    }
}

void Scenario6::GetSuggestions(XmlDocument^ doc, SearchPaneSuggestionsRequest^ request)
{
    XmlNodeList^ nodes = doc->GetElementsByTagName("Section");
    if (nodes->Size > 0)
    {
        IXmlNode^ section = nodes->GetAt(0);
        for (unsigned int i = 0; i < section->ChildNodes->Size; i++)
        {
            IXmlNode^ node = section->ChildNodes->GetAt(i);
            if (node->NodeType != NodeType::ElementNode)
            {
                continue;
            }

            if (node->NodeName->Equals("Separator"))
            {
                String^ title = nullptr;
                IXmlNode^ titleAttr = node->Attributes->GetNamedItem("title");
                if (titleAttr != nullptr)
                {
                    title = titleAttr->NodeValue->ToString();
                }
                if (title == nullptr || title->IsEmpty())
                {
                    title = "Suggestions";
                }
                request->SearchSuggestionCollection->AppendSearchSeparator(title);
            }
            else
            {
                AddSuggestionFromNode(node, request->SearchSuggestionCollection);
            }
        }
    }
}

void Scenario6::OnSearchPaneSuggestionsRequested(SearchPane^ sender, SearchPaneSuggestionsRequestedEventArgs^ e)
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
            // Use the web service Url entered in the UrlTextBox that supports XML Search Suggestions in order to see suggestions come from the web service.
            // See http://msdn.microsoft.com/en-us/library/cc848863(v=vs.85).aspx for details on XML Search Suggestions format.
            // Replace "{searchTerms}" of the Url with the query string.
            String^ strUrl = MainPage::ReplaceUrlSearchTerms(UrlTextBox->Text, Uri::EscapeComponent(queryText));
            auto token = cts.get_token();
            create_task(XmlDocument::LoadFromUriAsync(ref new Uri(strUrl)), token).then([=](XmlDocument^ doc)
            {
                Finally f ([&] {
                    deferral->Complete(); // Indicate we're done supplying suggestions.
                });
                if (token.is_canceled())
                {
                    cancel_current_task();
                }
                GetSuggestions(doc, request);
                if (request->SearchSuggestionCollection->Size > 0)
                {
                    MainPage::Current->NotifyUser("Suggestions provided for query: " + queryText, NotifyType::StatusMessage);
                }
                else
                {
                    MainPage::Current->NotifyUser("No suggestions provided for query: " + queryText, NotifyType::StatusMessage);
                }
            });
        }
        catch (Exception^)
        {
            MainPage::Current->NotifyUser("Suggestions could not be displayed, please verify that the service provides valid OpenSearch suggestions", NotifyType::ErrorMessage);
        }
    }
}

void Scenario6::OnResultSuggestionChosen(SearchPane^ sender, SearchPaneResultSuggestionChosenEventArgs^ e)
{
    // Handle the selection of a result suggestion since the XML Suggestion Format can return these.
    MainPage::Current->NotifyUser("Result suggestion selected with tag: " + e->Tag, NotifyType::StatusMessage);
}

void Scenario6::OnNavigatedTo(NavigationEventArgs^ e)
{
    MainPage::Current->NotifyUser("Use the search pane to submit a query", NotifyType::StatusMessage);
    // These events should be registered when your app first creates its main window after receiving an activated event, like OnLaunched, OnSearchActivated.
    // Typically this occurs in App.xaml.cpp.
    suggestionsRequestedToken = searchPane->SuggestionsRequested += ref new TypedEventHandler<SearchPane^, SearchPaneSuggestionsRequestedEventArgs^>(this, &Scenario6::OnSearchPaneSuggestionsRequested);
    resultSuggestionChosenToken = searchPane->ResultSuggestionChosen += ref new TypedEventHandler<SearchPane^, SearchPaneResultSuggestionChosenEventArgs^>(this, &Scenario6::OnResultSuggestionChosen);
}

void Scenario6::OnNavigatedFrom(NavigationEventArgs^ e)
{
    searchPane->SuggestionsRequested -= suggestionsRequestedToken;
    searchPane->ResultSuggestionChosen -= resultSuggestionChosenToken;
}
