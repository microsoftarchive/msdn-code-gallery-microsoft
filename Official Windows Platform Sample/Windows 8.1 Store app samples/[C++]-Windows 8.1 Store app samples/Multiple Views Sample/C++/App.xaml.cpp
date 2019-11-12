//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// App.xaml.cpp
// Implementation of the App.xaml class.
//

#include "pch.h"
#include "MainPage.xaml.h"
#include "Common\SuspensionManager.h"
#include "SecondaryViewPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::Common;
using namespace SDKSample::MultipleViews;

using namespace Concurrency;
using namespace Platform;
using namespace Platform::Collections;
using namespace SecondaryViewsHelpers;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Interop;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::ViewManagement;

App::App()
{
    InitializeComponent();
    this->Suspending += ref new SuspendingEventHandler(this, &SDKSample::App::OnSuspending);
    secondaryViews = ref new Vector<ViewLifetimeControl^>();
}

IVector<ViewLifetimeControl^>^ App::SecondaryViews::get()
{
    return secondaryViews;
}

void App::OnLaunched(LaunchActivatedEventArgs^ args)
{
    // Check if a secondary view is supposed to be shown
    ViewLifetimeControl^ viewData;
    if (TryFindViewLifetimeControlForViewId(args->CurrentlyShownApplicationViewId, &viewData))
    {
        create_task(viewData->Dispatcher->RunAsync(CoreDispatcherPriority::Normal, 
            ref new DispatchedHandler([] ()
        {
            Window::Current->Activate();
        })));
    }
    else
    {
        // We don't have the specified view in the collection, likely because it's the main view
        // that got shown. Set up the main view to display.
        InitializeMainPage(args->PreviousExecutionState, "").then([] ()
        {
            // This is the usual path at application startup
            Window::Current->Activate();
        }, task_continuation_context::use_current());
    }
}

void App::OnActivated(IActivatedEventArgs^ pArgs)
{
    if (pArgs->Kind == ActivationKind::Protocol)
    {
        auto protocolArgs = dynamic_cast<ProtocolActivatedEventArgs^>(pArgs);
        // Find out which window the system chose to display
        // Unless you've set DisableShowingMainViewOnActivation,
        // it will always be your main view. See Scenario 2 for details
        ViewLifetimeControl^ viewControl;
        if (TryFindViewLifetimeControlForViewId(protocolArgs->CurrentlyShownApplicationViewId, &viewControl))
        {
            create_task(viewControl->Dispatcher->RunAsync(CoreDispatcherPriority::Normal,
                ref new DispatchedHandler([protocolArgs] () 
            {
                auto currentPage = dynamic_cast<SecondaryViewPage^>(dynamic_cast<Frame^>(Window::Current->Content)->Content);
                currentPage->HandleProtocolLaunch(protocolArgs->Uri);
                Window::Current->Activate();
            })));
        }
        else
        {
            // We don't have the specified view in the collection, likely because it's the main view
            // that got shown. Set up the main view to display.
            InitializeMainPage(pArgs->PreviousExecutionState, "").then([protocolArgs] ()
            {
                auto rootPage = dynamic_cast<MainPage^>(dynamic_cast<Frame^>(Window::Current->Content)->Content);
                dynamic_cast<ListBox^>(rootPage->FindName("Scenarios"))->SelectedIndex = 1;
                rootPage->NotifyUser("Main window was launched with protocol: " + protocolArgs->Uri->AbsoluteUri,
                                    NotifyType::StatusMessage);
                Window::Current->Activate();
            }, task_continuation_context::use_current());
        }
    }
}

bool App::TryFindViewLifetimeControlForViewId(int viewId, ViewLifetimeControl^* foundData)
{
    for (auto viewData : secondaryViews)
    {
        if (viewData->Id == viewId)
        {
            *foundData = viewData;
            return true;
        }
    }
    *foundData = nullptr;
    return false;
}


task<void> App::InitializeMainPage(ApplicationExecutionState previousExecutionState, String^ arguments)
{
    auto rootFrame = dynamic_cast<Frame^>(Window::Current->Content);
    auto prerequisite = task<void>([](){});
    // Do not repeat app initialization when the Window already has content,
    // just ensure that the window is active
    if (rootFrame == nullptr)
    {
        mainDispatcher = Window::Current->Dispatcher;
        mainViewId = ApplicationView::GetForCurrentView()->Id;
        // Create a Frame to act as the navigation context and associate it with
        // a SuspensionManager key
        rootFrame = ref new Frame();
        SuspensionManager::RegisterFrame(rootFrame, "AppFrame");

        
        if (previousExecutionState == ApplicationExecutionState::Terminated)
        {
            // Restore the saved session state only when appropriate, scheduling the
            // final launch steps after the restore is complete
            prerequisite = SuspensionManager::RestoreAsync();
        }
        return prerequisite.then([=](task<void> prerequisite)
        {
            try
            {
                prerequisite.get();
            }
            catch (Platform::Exception^)
            {
                //Something went wrong restoring state.
                //Assume there is no state and continue
            }

            if (rootFrame->Content == nullptr || arguments != nullptr)
            {
                // This is encountered on the first launch of the app. Make sure to call
                // DisableShowingMainViewOnActivation before the first call to Window::Activate
                auto shouldDisable = Windows::Storage::ApplicationData::Current->LocalSettings->Values->Lookup(DISABLE_MAIN_VIEW_KEY);
                if (shouldDisable != nullptr && safe_cast<bool>(shouldDisable) )
                {
                    ApplicationViewSwitcher::DisableShowingMainViewOnActivation();
                }

                // When the navigation stack isn't restored or there are launch arguments
                // indicating an alternate launch (e.g.: via toast or secondary tile), 
                // navigate to the appropriate page, configuring the new page by passing required 
                // information as a navigation parameter
                if (!rootFrame->Navigate(TypeName(MainPage::typeid), arguments))
                {
                    throw ref new FailureException("Failed to create initial page");
                }
            }
            // Place the frame in the current Window
            Window::Current->Content = rootFrame;
        }, task_continuation_context::use_current());
    }
    else
    {
        if (rootFrame->Content == nullptr || arguments != nullptr)
        {
            // When the navigation stack isn't restored or there are launch arguments
            // indicating an alternate launch (e.g.: via toast or secondary tile), 
            // navigate to the appropriate page, configuring the new page by passing required 
            // information as a navigation parameter
            if (!rootFrame->Navigate(TypeName(MainPage::typeid), arguments))
            {
                throw ref new FailureException("Failed to create initial page");
            }
        }
    }

    return prerequisite;
}


/// <summary>
/// Invoked when application execution is being suspended.  Application state is saved
/// without knowing whether the application will be terminated or resumed with the contents
/// of memory still intact.
/// </summary>
/// <param name="sender">The source of the suspend request.</param>
/// <param name="e">Details about the suspend request.</param>
void App::OnSuspending(Object^ sender, SuspendingEventArgs^ e)
{
    (void) sender;    // Unused parameter

    auto deferral = e->SuspendingOperation->GetDeferral();
    SuspensionManager::SaveAsync().then([=]()
    {
        deferral->Complete();
    });
}

void App::UpdateTitle(String^ newTitle, int viewId)
{
    ViewLifetimeControl^ viewControl;
    bool found = TryFindViewLifetimeControlForViewId(viewId, &viewControl);
    if (found)
    {
        viewControl->Title = newTitle;
    }
    else
    {
        throw ref new Exception(HRESULT_FROM_WIN32(ERROR_NO_MATCH), "Couldn't find the view ID in the collection");
    }
}

CoreDispatcher^ App::MainDispatcher::get()
{
    return mainDispatcher;
}

int App::MainViewId::get()
{
    return mainViewId;
}