//
// SplitPage.xaml.cpp
// Implementation of the SplitPage class
//

#include "pch.h"
#include "App.xaml.h"
#include "SplitPage.xaml.h"
#include "DetailPage.xaml.h"
#include "FeedData.h"

using namespace concurrency;
using namespace Platform::Collections;

using namespace SimpleBlogReader;
using namespace SimpleBlogReader::Common;

using namespace Platform;

using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::ViewManagement;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Interop;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Media::Animation;
using namespace Windows::UI::Xaml::Navigation;


// The Split Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234234

SplitPage::SplitPage()
{
    InitializeComponent();
}

#pragma region Page state management

/// <summary>
/// Populates the page with content passed during navigation.  Any saved state is also
/// provided when recreating a page from a prior session.
/// </summary>
/// <param name="navigationParameter">The parameter value passed to
/// <see cref="Frame::Navigate(Type, Object)"/> when this page was initially requested.
/// </param>
/// <param name="pageState">A map of state preserved by this page during an earlier
/// session.  This will be null the first time a page is visited.</param>
void SplitPage::LoadState(Object^ navigationParameter, IMap<String^, Object^>^ pageState)
{
    // Run the PopInThemeAnimation 
    auto sb = dynamic_cast<Storyboard^>(this->FindName("PopInStoryboard"));
    if (sb != nullptr) 
    {
        sb->Begin();
    }

    // We are navigating forward from ItemsPage, so there is no existing page state.
    if (pageState == nullptr)
    {
        // Current feed was set in the click event in ItemsPage. We don't pass it in
        // via navigationParameter because on suspension, the default serialization 
        // mechanism will try to store that value but fail, because it can only handle 
        // primitives, strings and Guids.
        auto fd = safe_cast<FeedData^>(App::Current->Resources->Lookup("CurrentFeed"));

        // Insert into the ViewModel for this page to initialize itemsViewSource->View
        this->DefaultViewModel->Insert("Feed", fd);
        this->DefaultViewModel->Insert("Items", fd->Items);

        // When this is a new page, select the first item automatically unless logical page
        // navigation is being used (see the logical page navigation #region below).
        if (!UsingLogicalPageNavigation() && itemsViewSource->View != nullptr)
        {
            this->itemsViewSource->View->MoveCurrentToFirst();
        }
        else
        {
            this->itemsViewSource->View->MoveCurrentToPosition(-1);
        }
    }

    // pageState != null means either (1) we are returning from DetailPage
    // or (2) we are resuming from termination. If (1) we still have our
    // state and don't have to do anything. If (2) we need to restore the page.
    else if (!this->DefaultViewModel->HasKey("Feed"))
    {   
        // All we stored is the Uri string for the feed, not the object. 
        String^ uri = safe_cast<String^>(pageState->Lookup("Uri"));

        // FeedDataSource::InitDataSource has already been called. 
        // It's an asynchronous operation, so our FeedData might not
        // be available yet. GetFeedAsync uses a task_completion_event to  
        // wait (on its own thread) until the specified FeedData is available.
        // The next three methods follow the basic async pattern in C++:
        // 1. Call the async method.
        auto feedDataOp = FeedDataSource::GetFeedAsync(uri);

        // 2. Create a task from it.
        auto feedDataTask = create_task(feedDataOp);

        // 3. Define the work to be performed after the task completes.
        feedDataTask.then([this, pageState](FeedData^ feedData)
        {
            // Now we have the feedData, so it is now safe to get the FeedItem
            // synchronously. Inserting into DefaultViewModel
            // initializes the itemsViewSource-View object.
            this->DefaultViewModel->Insert("Feed", feedData);
            this->DefaultViewModel->Insert("Items", feedData->Items);

            // DetailsPage will need to get the new Uri from this value.
            App::Current->Resources->Insert("CurrentFeed", feedData);

            // Now that we have a FeedData^, we can call GetFeedItem safely,
            // passing in the title that we stored before we were terminated.
            String^ itemTitle = safe_cast<String^>(pageState->Lookup("SelectedItem"));
            auto selectedItem = FeedDataSource::GetFeedItem(feedData, itemTitle);

            if (selectedItem != nullptr)
            {
                this->itemsViewSource->View->MoveCurrentTo(selectedItem);
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
void SplitPage::SaveState(IMap<String^, Object^>^ pageState)
{
    if (itemsViewSource->View != nullptr)
    {
        auto selectedItem = itemsViewSource->View->CurrentItem;
        // Derive a serializable navigation parameter and pass it to
        // pageState->Insert("SelectedItem", <value>).
        if (selectedItem != nullptr)
        {
            auto feedItem = safe_cast<FeedItem^>(selectedItem);
            String^ itemTitle = feedItem->Title;
            pageState->Insert("SelectedItem", itemTitle);
        }

        // Save the feed title also.
        auto feedData = safe_cast<FeedData^>(this->DefaultViewModel->Lookup("Feed"));
        pageState->Insert("Uri", feedData->Uri);
    }
}

#pragma endregion

#pragma region Logical page navigation

// Visual state management typically reflects the four application view states directly (full
// screen landscape and portrait plus snapped and filled views.)  The split page is designed so
// that the snapped and portrait view states each have two distinct sub-states: either the item
// list or the details are displayed, but not both at the same time.
//
// This is all implemented with a single physical page that can represent two logical pages.
// The code below achieves this goal without making the user aware of the distinction.

/// <summary>
/// Invoked to determine whether the page should act as one logical page or two.
/// </summary>
/// <returns>True when the current view state is portrait or snapped, false
/// otherwise.</returns>
bool SplitPage::UsingLogicalPageNavigation()
{
    return UsingLogicalPageNavigation(ApplicationView::Value);
}

/// <summary>
/// Invoked to determine whether the page should act as one logical page or two.
/// </summary>
/// <param name="viewState">The view state for which the question is being posed.</param>
/// <returns>True when the view state in question is portrait or snapped, false
/// otherwise.</returns>
bool SplitPage::UsingLogicalPageNavigation(ApplicationViewState viewState)
{
    return viewState == ApplicationViewState::FullScreenPortrait ||
        viewState == ApplicationViewState::Snapped;
}

/// <summary>
/// Invoked when an item within the list is selected.
/// </summary>
/// <param name="sender">The GridView (or ListView when the application is Snapped)
/// displaying the selected item.</param>
/// <param name="e">Event data that describes how the selection was changed.</param>
void SplitPage::ItemListView_SelectionChanged(Object^ sender, SelectionChangedEventArgs^ e)
{
    (void) sender;	// Unused parameter
    (void) e;	// Unused parameter

    // Invalidate the view state when logical page navigation is in effect, as a change in
    // selection may cause a corresponding change in the current logical page.  When an item is
    // selected this has the effect of changing from displaying the item list to showing the
    // selected item's details.  When the selection is cleared this has the opposite effect.
    if (UsingLogicalPageNavigation()) 
    {
        InvalidateVisualState();
    }

    // Sometimes there is no selected item, e.g. when navigating back
    // from detail in logical page navigation.
    auto fi = dynamic_cast<FeedItem^>(itemListView->SelectedItem);
    if(fi != nullptr)
    {
        contentView->NavigateToString(fi->Content);
    }    
}

/// <summary>
/// Invoked when the page's back button is pressed.
/// </summary>
/// <param name="sender">The back button instance.</param>
/// <param name="e">Event data that describes how the back button was clicked.</param>
void SplitPage::GoBack(Object^ sender, RoutedEventArgs^ e)
{
    if (UsingLogicalPageNavigation() && itemListView->SelectedItem != nullptr)
    {
        // When logical page navigation is in effect and there's a selected item that item's
        // details are currently displayed.  Clearing the selection will return to the item list.
        // From the user's point of view this is a logical backward navigation.
        itemListView->SelectedItem = nullptr;
    }
    else
    {
        // When logical page navigation is not in effect, or when there is no selected item, use
        // the default back button behavior.
        LayoutAwarePage::GoBack(sender, e);
    }
}

/// <summary>
/// Invoked to determine the name of the visual state that corresponds to an application view
/// state.
/// </summary>
/// <param name="viewState">The view state for which the question is being posed.</param>
/// <returns>The name of the desired visual state.  This is the same as the name of the view state
/// except when there is a selected item in portrait and snapped views where this additional
/// logical page is represented by adding a suffix of _Detail.</returns>
String^ SplitPage::DetermineVisualState(ApplicationViewState viewState)
{
    // Update the back button's enabled state when the view state changes
    auto logicalPageBack = UsingLogicalPageNavigation(viewState) && itemListView->SelectedItem != nullptr;
    auto physicalPageBack = Frame != nullptr && Frame->CanGoBack;
    DefaultViewModel->Insert("CanGoBack", logicalPageBack || physicalPageBack);

    // Determine visual states for landscape layouts based not on the view state, but
    // on the width of the window.  This page has one layout that is appropriate for
    // 1366 virtual pixels or wider, and another for narrower displays or when a snapped
    // application reduces the horizontal space available to less than 1366.
    if (viewState == ApplicationViewState::Filled ||
        viewState == ApplicationViewState::FullScreenLandscape)
    {
        auto windowWidth = Window::Current->Bounds.Width;
        if (windowWidth >= 1366) return "FullScreenLandscapeOrWide";
        return "FilledOrNarrow";
    }

    // When in portrait or snapped start with the default visual state name, then add a
    // suffix when viewing details instead of the list
    String^ defaultStateName = LayoutAwarePage::DetermineVisualState(viewState);
    return logicalPageBack ? defaultStateName + "_Detail" : defaultStateName;
}
#pragma endregion

void SplitPage::ViewDetail_Click(Object^ sender, RoutedEventArgs^ e)
{
    // Navigate to the appropriate destination page, and configure the new page
    // by passing required information as a navigation parameter.

    auto selectedItem = dynamic_cast<FeedItem^>(this->itemListView->SelectedItem);

    // selectedItem will be nullptr if the user invokes the app bar
    // and clicks on "view web page" without selecting an item.
    if (this->Frame != nullptr && selectedItem != nullptr)
    {
        auto itemTitle = safe_cast<String^>(selectedItem->Title);
        this->Frame->Navigate(TypeName(DetailPage::typeid), itemTitle);
    }
}
