//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario1.xaml.cpp
// Implementation of the Scenario1 class
//

#include "pch.h"
#include "Scenario1.xaml.h"
#include "MainPage.xaml.h"
#include "ProjectionViewPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::Projection;

using namespace Concurrency;
using namespace Platform;
using namespace SecondaryViewsHelpers;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::UI::Core;
using namespace Windows::UI::ViewManagement;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Interop;

Scenario1::Scenario1()
{
    InitializeComponent();
    thisViewId = ApplicationView::GetForCurrentView()->Id;
}

void Scenario1::StartProjecting_Click(Object^ sender, RoutedEventArgs^ e)
{
    auto prerequisite = task<void>([](){});
    // If projection is already in progress, then it could be shown on the monitor again
    // Otherwise, we need to create a new view to show the presentation
    if (MainPage::Current->ProjectionViewPageControl == nullptr)
    {
        // First, create a new, blank view
        auto mainDispatcher = Window::Current->Dispatcher;
        int mainViewId = thisViewId;
        prerequisite = create_task(CoreApplication::CreateNewView()->Dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler([mainViewId, mainDispatcher] ()
        {
            // ViewLifetimeControl is a wrapper to make sure the view is closed only
            // when the app is done with it
            MainPage::Current->ProjectionViewPageControl = ViewLifetimeControl::CreateForCurrentView();
                    
            // Assemble some data necessary for the new page
            auto initData = ref new ProjectionViewPageInitializationData();
            initData->MainDispatcher = mainDispatcher;
            initData->ProjectionViewPageControl = MainPage::Current->ProjectionViewPageControl;
            initData->MainViewId = mainViewId;
                    
            // Display the page in the view. Note that the view will not become visible
            // until "StartProjectingAsync" is called
            auto rootFrame = ref new Windows::UI::Xaml::Controls::Frame();
            rootFrame->Navigate(TypeName(ProjectionViewPage::typeid), initData);
            Window::Current->Content = rootFrame;
        })));
    }
    
    prerequisite.then([this] ()
    {
        try
        {
            // Start/StopViewInUse are used to signal that the app is interacting with the
            // view, so it shouldn't be closed yet, even if the user loses access to it
            MainPage::Current->ProjectionViewPageControl->StartViewInUse();
            // Show the view on a second display (if available) or on the primary display
            create_task(ProjectionManager::StartProjectingAsync(
                MainPage::Current->ProjectionViewPageControl->Id, 
                thisViewId
            )).then([] () {
                MainPage::Current->ProjectionViewPageControl->StopViewInUse();
            });
        }
        catch (ObjectDisposedException^)
        {
            // The projection view is being disposed
        }
    }, task_continuation_context::use_arbitrary());
}

void Scenario1::StopProjecting_Click(Object^ sender, RoutedEventArgs^ e)
{
    // Only stop a presentation if one is already in progress
    if (MainPage::Current->ProjectionViewPageControl != nullptr)
    {  
        try
        {
            MainPage::Current->ProjectionViewPageControl->StartViewInUse();
            // Note that as a result of making this call, the projection view will be "Consolidated"
            // ViewLifetimeControl, in turn, will close the view
            create_task(ProjectionManager::StopProjectingAsync(
                MainPage::Current->ProjectionViewPageControl->Id,
                thisViewId
            )).then([] ()
            {
                MainPage::Current->ProjectionViewPageControl->StopViewInUse();
            });
        }
        catch (ObjectDisposedException^)
        {
            MainPage::Current->NotifyUser("The projection view is being disposed", NotifyType::ErrorMessage);
        }
    }
    else
    {
        MainPage::Current->NotifyUser("A presentation has not been started", NotifyType::ErrorMessage);
    }
}
