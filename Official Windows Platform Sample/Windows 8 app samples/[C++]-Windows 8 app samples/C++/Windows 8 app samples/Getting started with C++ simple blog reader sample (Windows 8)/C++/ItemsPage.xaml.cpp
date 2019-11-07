// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//
//
// ItemsPage.xaml.cpp
// Implementation of the ItemsPage class
//

#include "pch.h"
#include "App.xaml.h"
#include "ItemsPage.xaml.h"
#include "SplitPage.xaml.h"

using namespace Windows::UI::Xaml::Interop;
using namespace SimpleBlogReader;

using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Graphics::Display;
using namespace Windows::UI::ViewManagement;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;

// The Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234233

ItemsPage::ItemsPage()
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
void ItemsPage::LoadState(Object^ navigationParameter, IMap<String^, Object^>^ pageState)
{
    // This is the first page to load on startup. The feedDataSource was constructed when the app loaded
    // in response to this declaration in app.xaml: <local:FeedDataSource x:Key="feedDataSource" />
    // and it was initialized aynchronously in the OnLaunched event handler in app.xaml.cpp. 
    // Initialization might still be happening, but that's ok. 
    FeedDataSource^ feedDataSource = safe_cast<FeedDataSource^>(App::Current->Resources->Lookup("feedDataSource"));

    // In ItemsPage.xaml (and every other page), the DefaultViewModel is set as DataContext:
    // DataContext="{Binding DefaultViewModel, RelativeSource={RelativeSource Self}}"
    // Because ItemsPage displays a collection of feeds, we set the Items element
    // to the FeedDataSource::Feeds collection. By comparison, the SplitPage sets this element to 
    // the Items collection of a single FeedData object.
    this->DefaultViewModel->Insert("Items", feedDataSource->Feeds);

}

void ItemsPage::ItemView_ItemClick(Object^ sender, ItemClickEventArgs^ e)
{
    // We must manually cast from Object^ to FeedData^
    auto feedData = safe_cast<FeedData^>(e->ClickedItem);

    // Store the current URI so that we can lookup the
    // correct feedData object on resume.
    App::Current->Resources->Insert("CurrentFeed", feedData);
      
    // Navigate to SplitPage and pass the title of the selected feed.
    // SplitPage will receive this in its LoadState method in the navigationParamter.
    this->Frame->Navigate(TypeName(SplitPage::typeid), feedData->Title);
}




