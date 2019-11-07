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
#include "SecondaryViewPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::MultipleViews;

using namespace Concurrency;
using namespace Platform;
using namespace Platform::Collections;
using namespace SecondaryViewsHelpers;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::UI::Core;
using namespace Windows::UI::ViewManagement;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Interop;

const wchar_t* DEFAULT_TITLE = L"New window";

Scenario1::Scenario1()
{
    InitializeComponent();
    rootPage = MainPage::Current;

    SizePreferenceChooser->ItemsSource = GenerateSizePreferenceBinding();
    SizePreferenceChooser->SelectedIndex = 0;

    // "UseNone" is not a valid choice for the incoming view, so only include
    // it in the anchor size preference chooser
    auto anchorSizeChoices = GenerateSizePreferenceBinding();
    anchorSizeChoices->Append(ref new SizePreferenceString(ViewSizePreference::UseNone, "UseNone"));
    AnchorSizePreferenceChooser->ItemsSource = anchorSizeChoices;
    AnchorSizePreferenceChooser->SelectedIndex = 0;

    // This collection is being bound to the current thread. 
    // So, make sure you only update the collection and items
    // contained in it from this thread.
    ViewChooser->ItemsSource = dynamic_cast<App^>(App::Current)->SecondaryViews;
}

void Scenario1::CreateView_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // Set up the secondary view, but don't show it yet
    auto viewControlPtr = std::make_shared<ViewLifetimeControl^>();
    auto thisDispatcher = Window::Current->Dispatcher;
    
    CoreApplication::CreateNewView()->Dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler([viewControlPtr, thisDispatcher] ()
    {
        // This object is used to keep track of the views and important
        // details about the contents of those views across threads
        // In your app, you would probably want to track information
        // like the open document or page inside that window
        auto newViewControl = ViewLifetimeControl::CreateForCurrentView();
        *viewControlPtr = newViewControl;

        newViewControl->Title = ref new String(DEFAULT_TITLE);

        auto frame = ref new Windows::UI::Xaml::Controls::Frame();
        frame->Navigate(TypeName(SecondaryViewPage::typeid), newViewControl);
        Window::Current->Content = frame;

        ApplicationView::GetForCurrentView()->Title = newViewControl->Title;

        // Be careful! This collection is bound to the main thread,
        // so make sure to update it only from this thread
        thisDispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler([viewControlPtr]()
        {
            dynamic_cast<App^>(App::Current)->SecondaryViews->Append(*viewControlPtr);
        }));
    }));
}

void Scenario1::ShowAsStandalone_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    auto selectedView = dynamic_cast<ViewLifetimeControl^>(ViewChooser->SelectedItem);
    auto sizePreference = dynamic_cast<SizePreferenceString^>(SizePreferenceChooser->SelectedItem);
    auto anchorSizePreference = dynamic_cast<SizePreferenceString^>(AnchorSizePreferenceChooser->SelectedItem);
    if (ViewChooser->SelectedItem != nullptr && sizePreference != nullptr && anchorSizePreference != nullptr)
    {

        try
        {
            // Prevent the view from closing while
            // switching to it
            selectedView->StartViewInUse();

            // Show the previously created secondary view, using the size
            // preferences the user specified. In your app, you should
            // choose a size that's best for your scenario and code it,
            // instead of requiring the user to decide.
            create_task(ApplicationViewSwitcher::TryShowAsStandaloneAsync(
                selectedView->Id,
                sizePreference->Preference,
                ApplicationView::GetForCurrentView()->Id,
                anchorSizePreference->Preference))
            .then([selectedView, this] (bool viewShown)
            {
                if (!viewShown)
                {
                    // The window wasn't actually shown, so StopViewInUse the reference to it
                    // This may trigger the window to be destroyed
                    rootPage->NotifyUser("The view was not shown. Make sure it has focus", NotifyType::ErrorMessage);
                }
                // Signal that switching has completed and let the view close
                selectedView->StopViewInUse();
            }, task_continuation_context::use_current());
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
        rootPage->NotifyUser("Please choose a view to show, a size preference for each view", NotifyType::ErrorMessage);
    }
}

Vector<SizePreferenceString^>^ Scenario1::GenerateSizePreferenceBinding()
{
    auto sizeChoices = ref new Vector<SizePreferenceString^>();
    
    sizeChoices->Append(ref new SizePreferenceString(ViewSizePreference::Default,    "Default"));
    sizeChoices->Append(ref new SizePreferenceString(ViewSizePreference::UseHalf,    "UseHalf"));
    sizeChoices->Append(ref new SizePreferenceString(ViewSizePreference::UseLess,    "UseLess"));
    sizeChoices->Append(ref new SizePreferenceString(ViewSizePreference::UseMinimum, "UseMinimum"));
    sizeChoices->Append(ref new SizePreferenceString(ViewSizePreference::UseMore,    "UseMore"));

    return sizeChoices;
}


SizePreferenceString::SizePreferenceString(ViewSizePreference preference, String^ title)
{
    this->title = title;
    this->preference = preference;
}

String^ SizePreferenceString::Title::get()
{
    return title;
}

ViewSizePreference SizePreferenceString::Preference::get()
{
    return preference;
}