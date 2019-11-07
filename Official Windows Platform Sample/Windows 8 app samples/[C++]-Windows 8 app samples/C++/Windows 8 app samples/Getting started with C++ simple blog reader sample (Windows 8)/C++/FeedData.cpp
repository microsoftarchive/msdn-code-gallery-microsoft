// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//

#include "pch.h"
#include "FeedData.h"


using namespace std;
using namespace concurrency;
using namespace SimpleBlogReader;
using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Foundation;
using namespace Windows::Web::Syndication;



FeedDataSource::FeedDataSource()
{
    m_feeds = ref new Vector<FeedData^>();
}

// Retrieve the data for each atom or rss feed and put it
// into our custom data structures.
void FeedDataSource::InitDataSource()
{
    // Left as an exercise: store the urls separately and let the user configure them.
    // It might be more convenient to use Platform::Strings here, but using wstring 
    // serves to demonstrate how standard C++ types can be used here.
    std::vector<std::wstring> urls; 
    urls.push_back(L"http://windowsteamblog.com/windows/b/developers/atom.aspx");
    urls.push_back(L"http://windowsteamblog.com/windows/b/windowsexperience/atom.aspx");
    urls.push_back(L"http://windowsteamblog.com/windows/b/extremewindows/atom.aspx");

    urls.push_back(L"http://windowsteamblog.com/windows/b/business/atom.aspx");
    urls.push_back(L"http://windowsteamblog.com/windows/b/bloggingwindows/atom.aspx");
    urls.push_back(L"http://windowsteamblog.com/windows/b/windowssecurity/atom.aspx");
    urls.push_back(L"http://windowsteamblog.com/windows/b/springboard/atom.aspx");
    urls.push_back(L"http://windowsteamblog.com/windows/b/windowshomeserver/atom.aspx");
    // There is no Atom feed for this blog, so we use the RSS feed.
    urls.push_back(L"http://windowsteamblog.com/windows_live/b/windowslive/rss.aspx");
    urls.push_back(L"http://windowsteamblog.com/windows_live/b/developer/atom.aspx");
    urls.push_back(L"http://windowsteamblog.com/ie/b/ie/atom.aspx");
    urls.push_back(L"http://windowsteamblog.com/windows_phone/b/wpdev/atom.aspx");
    urls.push_back(L"http://windowsteamblog.com/windows_phone/b/wmdev/atom.aspx");

    // If we are resuming, we need to create a map of completion events so that
    // we don't attempt to restore page's state before it's backing data has been loaded.
    // First we create all the  events in an "unset" state, mapped to the urls as keys.
    // we'll set the event after we asynchronously load the feed.
    for( wstring url : urls)
    {
        String^ uri = ref new String(url.c_str());
        task_completion_event<FeedData^> e;
        m_feedCompletionEvents.insert(make_pair(uri, e));
    }

    SyndicationClient^ client = ref new SyndicationClient();   

    // Range-based for loop. Never write a regular for loop again!
    for(wstring url : urls)
    {
        // Create the async operation. feedOp is an 
        // IAsyncOperationWithProgress<SyndicationFeed^, RetrievalProgress>^

        String^ uri = ref new String(url.c_str());
        auto feedUri = ref new Uri(uri);
        auto feedOp = client->RetrieveFeedAsync(feedUri);

        // Create the task object and pass it the async operation.
        // SyndicationFeed^ is the type of the return value
        // that the feedOp operation will eventually produce.       

        // Then, initialize a FeedData object with the feed info. Each
        // operation is independent and does not have to happen on the
        // UI thread. Therefore, we specify use_arbitrary.
        create_task(feedOp)

            .then([this, uri]  (SyndicationFeed^ feed) -> FeedData^
        {
            return GetFeedData(uri, feed);
        }, concurrency::task_continuation_context::use_arbitrary())


            // Append the initialized FeedData object to the list
            // that is the data source for the items collection.
            // This has to happen on the UI thread. By default, a .then
            // continuation runs in the same apartment thread that it was called on.
            // Because the actions will be synchronized for us, we can append 
            // safely to the Vector without taking an explicit lock.
            .then([this] (FeedData^ fd)
        {
            m_feeds->Append(fd);
            m_feedCompletionEvents[fd->Uri].set(fd);

            // Write to VS output window in debug mode only. Requires <windows.h>.
            OutputDebugString(fd->Title->Data());
            OutputDebugString(L"\r\n");
        })

            // The last continuation serves as an error handler. The
            // call to get() will surface any exceptions that were raised
            // at any point in the task chain.
            .then( [] (task<void> t)
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
                // In a real world app that allowed the user to enter
                // a url manually, you could prompt them to try again.
                OutputDebugString(e->Message->Data());
            }
        }); //end task chain
    };
}


FeedData^ FeedDataSource::GetFeedData(String^ feedUri, SyndicationFeed^ feed)
{

    FeedData^ feedData = ref new FeedData();

    // Knowing this makes it easier to map completion_events 
    // when we resume from termination.
    feedData->Uri = feedUri;

    // Get the title of the feed (not the individual posts).
    feedData->Title = feed->Title->Text; 

    if (feed->Subtitle->Text != nullptr)
    {
        feedData->Description = feed->Subtitle->Text;
    }	 
    // Use the date of the latest post as the last updated date.
    feedData->PubDate = feed->Items->GetAt(0)->PublishedDate;	

    // Construct a FeedItem object for each post in the feed
    // using a range-based for loop. Preferable to a 
    // C-style for loop, or std::for_each.
    for (auto  item : feed->Items)
    {
        FeedItem^ feedItem = ref new FeedItem();
        feedItem->Title = item->Title->Text; 
        feedItem->PubDate = item->PublishedDate;		

        //We only get first author in case of multiple entries.
        feedItem->Author = item->Authors->GetAt(0)->Name; 

        if (feed->SourceFormat == SyndicationFormat::Atom10)
        {
            feedItem->Content = item->Content->Text;
            String^ s(L"http://windowsteamblog.com");
            feedItem->Link = ref new Uri(s + item->Id);
        }

        else if (feed->SourceFormat == SyndicationFormat::Rss20)
        {
            feedItem->Content = item->Summary->Text;
            feedItem->Link = item->Links->GetAt(0)->Uri;
        }

        feedData->Items->Append(feedItem);
    };

    return feedData;

} //end GetFeedData


// We use this method to get the proper FeedData object when resuming
// from shutdown. We need to wait for this data to be populated before
// we attempt to restore page state. Note the use of task_completion_event
// which doesn't block the UI thread.
IAsyncOperation<FeedData^>^ FeedDataSource::GetFeedAsync(String^ uri)
{
    return create_async([uri]()
    {
        auto feedDataSource = safe_cast<FeedDataSource^>( 
            App::Current->Resources->Lookup("feedDataSource"));

        // Does not block the UI thread.
        auto f = feedDataSource->m_feedCompletionEvents[uri];

        // In the callers we continue from this task after the event is 
        // set in InitDataSource and we know we have a FeedData^.
        task<FeedData^> t = create_task(f);
        return t;
    });
}

// We stored the stringID when the app was suspended
// because storing the FeedItem itself would have required
// more custom serialization code. Here is where we retrieve
// the FeedItem based on its string ID.
FeedItem^ FeedDataSource::GetFeedItem(FeedData^ feed, String^ uniqueId)
{
    auto itEnd = end(feed->Items);
    auto it = std::find_if(begin(feed->Items), itEnd, 
        [uniqueId] (FeedItem^ fi)
    {
        return fi->Title == uniqueId;
    });

    if (it != itEnd)
        return safe_cast<FeedItem^>(*it);

    return nullptr;
}



