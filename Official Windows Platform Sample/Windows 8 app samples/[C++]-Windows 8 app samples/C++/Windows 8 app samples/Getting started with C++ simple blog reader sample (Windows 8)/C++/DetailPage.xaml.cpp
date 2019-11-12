// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//
//
// DetailPage.xaml.cpp
// Implementation of the DetailPage class
//

#include "pch.h"
#include "DetailPage.xaml.h"
#include "FeedData.h"

using namespace SimpleBlogReader;
using namespace concurrency;
using namespace Platform::Collections;
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

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

DetailPage::DetailPage()
{
    InitializeComponent();
}

/// <summary>
/// Populates the page with content passed during navigation.  Any saved state is also
/// provided when recreating a page from a prior session.
/// </summary>
/// <param name="navigationParameter">The parameter value passed to
/// <see cref="Frame::Navigate(Type, Object)"/> when this page was initially requested.
/// </param>
/// <param name="pageState">A map of state preserved by this page during an earlier
/// session.  This will be null the first time a page is visited.</param>
void DetailPage::LoadState(Object^ navigationParameter, IMap<String^, Object^>^ pageState)
{
    // Demonstrates one way to output debug info to output window in VS.
    OutputDebugString(L"Enter DetailPage::LoadState\r\n");
    
    // Run the PopInThemeAnimation 
    Windows::UI::Xaml::Media::Animation::Storyboard^ sb =
        safe_cast<Windows::UI::Xaml::Media::Animation::Storyboard^>(this->FindName("PopInStoryboard"));
    sb->Begin();

   
    // Lookup the URL for the blog title that was either
    // (a) passed to us in this session or
    // (b) saved in the SaveState method when our app was suspended.
    m_itemTitle = safe_cast<String^>(navigationParameter);

    // We are navigating forward from SplitPage
    if (pageState == nullptr)
    {
        FeedData^ feedData = safe_cast<FeedData^>(App::Current->Resources->Lookup("CurrentFeed"));
        m_feedUri = feedData->Uri;

        auto feedItem = FeedDataSource::GetFeedItem(feedData, m_itemTitle);
        if (feedItem != nullptr)
        {               
            DefaultViewModel->Insert("Title", m_itemTitle);
            // Display the web page.
            contentView->Navigate(feedItem->Link);
        }
    }
   
    // We are resuming from suspension:
    else
    {
        // We are resuming, and might not have our FeedData object yet
        // so must get it asynchronously and wait on the result.
        String^ uri = safe_cast<String^>(pageState->Lookup("FeedUri"));
        auto feedDataOp = FeedDataSource::GetFeedAsync(uri); //URL
        auto feedDataTask = create_task(feedDataOp);

        feedDataTask.then([this, pageState](FeedData^ feedData)
        {            
            App::Current->Resources->Insert("CurrentFeed", feedData);

            m_feedUri = feedData->Uri;
            m_itemTitle = safe_cast<String^>(pageState->Lookup("Item"));
            auto feedItem = FeedDataSource::GetFeedItem(feedData, m_itemTitle);

            if (feedItem != nullptr)
            {               
                DefaultViewModel->Insert("Title", m_itemTitle);
                // Display the web page.
                contentView->Navigate(feedItem->Link);
            }
        });
    }
}

/// <summary>
/// Preserves state associated with this page in case the application is suspended or the
/// page is discarded from the navigation cache.  Values must conform to the serialization
/// requirements of <see cref="SuspensionManager::SessionState"/>.
/// </summary>
/// <param name="pageState">An empty map to be populated with serializable state.</param>
void DetailPage::SaveState(IMap<String^, Object^>^ pageState)
{
    // Store the itemTitle in case we are suspended or terminated.
    pageState->Insert("Item", m_itemTitle);
    pageState->Insert("FeedUri", m_feedUri);
}



