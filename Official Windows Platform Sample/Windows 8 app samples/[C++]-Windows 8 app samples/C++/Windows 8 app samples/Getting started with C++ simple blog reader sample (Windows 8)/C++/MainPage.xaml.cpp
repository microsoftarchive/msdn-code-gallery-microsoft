// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//
//
// MainPage.xaml.cpp
// Implementation of the MainPage class.
//

#include "pch.h"
#include "MainPage.xaml.h"

using namespace SimpleBlogReader;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Web::Syndication;
using namespace concurrency;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

MainPage::MainPage()
{
    InitializeComponent();
    feedData = ref new FeedData();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void MainPage::OnNavigatedTo(NavigationEventArgs^ e)
{
    (void) e;	// Unused parameter
}




void MainPage::GetFeedData(Platform::String^ feedUriString)
{
    // Create the SyndicationClient and the target uri
    SyndicationClient^ client = ref new SyndicationClient();
    Uri^ feedUri = ref new Uri(feedUriString);

    // Create the async operation. feedOp is an 
    // IAsyncOperationWithProgress<SyndicationFeed^, RetrievalProgress>^
    auto feedOp = client->RetrieveFeedAsync(feedUri);
    feedOp = client->RetrieveFeedAsync(feedUri);

    // Create the task object and pass it the async operation.
    // SyndicationFeed^ is the type of the return value
    // that the feedOp operation will eventually produce.    

    // Create a "continuation" that will run when the first task completes.
    // The continuation takes the return value of the first operation,
    // and then defines its own asynchronous operation by using a lambda.
    create_task(feedOp).then([this] (SyndicationFeed^ feed) -> SyndicationFeed^
    {
        // Get the title of the feed (not the individual posts).
        feedData->Title = feed ->Title->Text;

        // Retrieve the individual posts from the feed.
        auto feedItems = feed->Items;

        // Iterate over the posts. 
        for(auto item : feedItems)
        {  
            FeedItem^ feedItem = ref new FeedItem();
            feedItem->Title = item->Title->Text; 
            feedItem->PubDate = item->PublishedDate;

            feedItem->Author = item->Authors->GetAt(0)->Name; 

            if (feed->SourceFormat == SyndicationFormat::Atom10)
            {
                feedItem->Content = item->Content->Text;
            }
            else if (feed->SourceFormat == SyndicationFormat::Rss20)
            {
                feedItem->Content = item->Summary->Text;
            }
            feedData->Items->Append(feedItem);
        }

        this->DataContext = feedData;
        return feed;         
    })
        // The last continuation serves as an error handler. The
        // call to get() will surface any exceptions that were raised
        // at any point in the task chain.
        .then( [this] (concurrency::task<SyndicationFeed^> t)
    {
        try
        {
            t.get();
        }
        // SyndicationClient throws Platform::InvalidArgumentException 
        // if a URL contains illegal characters.
        // We catch this exception for demonstration purposes only.
        // In the current design of this app, an illegal
        // character can only be introduced by a coding error
        // and should not be caught. If we modify the app to allow
        // the user to manually add a new url, then we need to catch
        // the exception.
        catch(Platform::InvalidArgumentException^ e)
        {
            // For example purposes we just output error to console.
            // In a real world app you could alert the user
            // and prompt them to try again.
            OutputDebugString(e->Message->Data());
        }
    }); 
}

void MainPage::PageLoadedHandler( Platform::Object^ sender,
                                 Windows::UI::Xaml::RoutedEventArgs^ e)
{
    GetFeedData("http://windowsteamblog.com/windows/b/developers/atom.aspx");
}

// Implementation in MainPage.xaml.cpp
void MainPage::ItemListView_SelectionChanged ( 
    Platform::Object^ sender,
    Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e)
{
    FeedItem^ feedItem = safe_cast<FeedItem^>(ItemListView->SelectedItem);
    // Navigate the WebView to the blog post content HTML string.
    ContentView->NavigateToString(feedItem->Content);
}