//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario3.xaml.cpp
// Implementation of the Scenario3 class
//

#include "pch.h"
#include "Scenario3.xaml.h"
#include "MainPage.xaml.h"
#include "SecondaryViewPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::MultipleViews;

using namespace Concurrency;
using namespace Platform;
using namespace SecondaryViewsHelpers;
using namespace Windows::Foundation;
using namespace Windows::UI::Core;
using namespace Windows::UI::ViewManagement;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Media::Animation;
using namespace Windows::UI::Xaml::Navigation;

Scenario3::Scenario3()
{
    InitializeComponent();
    rootPage = MainPage::Current;
    ViewChooser->ItemsSource = dynamic_cast<App^>(App::Current)->SecondaryViews;

    fadeOutStoryboard = ref new Storyboard();
    auto fadeOut = ref new FadeOutThemeAnimation();
    fadeOutStoryboard->Children->Append(fadeOut);

    // Normally you can point directly to an object named in your XAML. Since
    // the sample hosts multiple pages, it's convenient for this scenario to
    // get the root element
    Storyboard::SetTarget(fadeOut, dynamic_cast<DependencyObject^>(dynamic_cast<FrameworkElement^>(rootPage->Content)->FindName("ContentRoot")));
}

void Scenario3::Fade_Completed(Object^ sender, Object^ e)
{
    // This event always fires on the UI thread, along with Current_VisibilityChanged,
    // so there is no race condition with the two methods both changing this
    // value
    auto animationTask = animationTaskPtr.get();
    if (animationTask != nullptr)
    {
        animationTask->set();
        animationTaskPtr = nullptr;
    }
}

task<void> Scenario3::FadeOutContents()
{
    auto animationTask = animationTaskPtr.get();
    if (animationTask != nullptr)
    {
        animationTask->set();
    }

    animationTaskPtr = std::unique_ptr<task_completion_event<void>>(new task_completion_event<void>());

    fadeOutStoryboard->Begin();
    return create_task(*animationTaskPtr);
}

void Scenario3::OnNavigatedTo(NavigationEventArgs^ e)
{
    visibilityToken = Window::Current->VisibilityChanged += ref new WindowVisibilityChangedEventHandler(this, &Scenario3::Current_VisibilityChanged);
    fadeOutToken = fadeOutStoryboard->Completed += ref new EventHandler<Object^>(this, &Scenario3::Fade_Completed);
}

void Scenario3::OnNavigatedFrom(NavigationEventArgs^ e)
{
    Window::Current->VisibilityChanged -= visibilityToken;
    fadeOutStoryboard->Completed -= fadeOutToken;
}

void Scenario3::Current_VisibilityChanged(Object^ sender, VisibilityChangedEventArgs^ e)
{
    auto now = GetCurrentTime();
    // Timeout the animation if the secondary window fails to respond in 500
    // ms. Since this animation clears out the main view of the app, it's not desirable
    // to leave it unusable
    auto animationTask = animationTaskPtr.get();
    if (e->Visible || (now - lastFadeOutTime) > 500)
    {
        // This event always fires on the UI thread, along with Fade_Completed,
        // so there is no race condition with the two methods both changing this
        // value
        if (animationTask != nullptr)
        {
            animationTask->set();
            animationTaskPtr = nullptr;
        }
        fadeOutStoryboard->Stop();
    }
}

void Scenario3::AnimatedSwitch_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // The sample demonstrates a general strategy for doing custom animations when switching view
    // It's technically only possible to animate the contents of a particular view. But, it is possible
    // to animate the outgoing view to some common visual (like a blank background), have the incoming
    // view draw that same visual, switch in the incoming window (which will be imperceptible to the
    // user since both windows will be showing the same thing), then animate the contents of the incoming
    // view in from the common visual.
    auto selectedItem = dynamic_cast<ViewLifetimeControl^>(ViewChooser->SelectedItem);
    if (selectedItem != nullptr)
    {
        try
        {
            // Prevent the view from being closed while switching to it
            selectedItem->StartViewInUse();

            // Signal that the window is about to be switched to
            // If the view is already shown to the user, then the app
            // shouldn't run any extra animations
            auto currentId = ApplicationView::GetForCurrentView()->Id;
            auto isViewVisiblePtr = std::make_shared<bool>();
            create_task(ApplicationViewSwitcher::PrepareForCustomAnimatedSwitchAsync(
                selectedItem->Id,
                currentId,
                ApplicationViewSwitchingOptions::SkipAnimation)
            ).then([this, isViewVisiblePtr] (bool isViewVisible)
            {
                // The view may already be on screen, in which case there's no need to animate its
                // contents (or animate out the contents of the current window). This affects later
                // "then" statements, so keep the value around
                *isViewVisiblePtr = isViewVisible;
                if (!isViewVisible)
                {
                    // The view isn't visible, so animate it on screen
                    lastFadeOutTime = GetCurrentTime();

                    // Make the current window totally blank. The incoming window is
                    // going to be totally blank as well when the two windows switch,
                    return FadeOutContents();
                }

                // Null task to make sure the following "then" statements work
                return task<void>([] () {});
            }, task_continuation_context::use_current()).then([isViewVisiblePtr, selectedItem, currentId] ()
            {
                if (!(*isViewVisiblePtr))
                {
                    // Once the current view is blank, switch in the other window
                    return create_task(selectedItem->Dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler([currentId] ()
                    {
                        // More details are in SecondaryViewPage.xaml.cpp
                        // This function makes the view totally blank, swaps the two view (which
                        // is not apparent to the user since both windows are blank), then animates
                        // in the content of newly visible view
                        auto currentPage = dynamic_cast<SecondaryViewPage^>(dynamic_cast<Windows::UI::Xaml::Controls::Frame^>(Window::Current->Content)->Content);
                        currentPage->SwitchAndAnimate(currentId);
                    })));
                }
                return task<void>([] () {});

                // It's not allowed to call directly from one UI thread to another
                // Since the previous continued from the main view, but
                // the subsequent one does not, continue on a threadpool thread
            }, task_continuation_context::use_arbitrary()).then([selectedItem] ()
            {
                selectedItem->StopViewInUse();
            }, task_continuation_context::use_arbitrary());
        }
        catch (ObjectDisposedException^)
        {
            // The view could be in the process of closing, and
            // this thread just hasn't updated. As part of being closed,
            // this thread will be informed to clean up its list of
            // views (see SecondaryViewPage.xaml.cs)
        }
    }
    else
    {
        rootPage->NotifyUser("Select a window to see a switch animation. You can create a window in scenario 1", NotifyType::ErrorMessage);
    }
}

unsigned long long Scenario3::GetCurrentTime()
{
    FILETIME ft = {0};
    GetSystemTimeAsFileTime(&ft);
    ULARGE_INTEGER timeInteger = {0};
    timeInteger.HighPart = ft.dwHighDateTime;
    timeInteger.LowPart = ft.dwLowDateTime;

    return timeInteger.QuadPart;
}