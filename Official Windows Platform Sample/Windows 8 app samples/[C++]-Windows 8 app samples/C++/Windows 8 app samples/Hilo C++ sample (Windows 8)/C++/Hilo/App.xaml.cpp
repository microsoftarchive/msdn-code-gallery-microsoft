// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#include "pch.h"
#include "Common\SuspensionManager.h"
#include "MainHubView.g.h"
#include "TileUpdateScheduler.h"
#include "ExceptionPolicyFactory.h"
#include "HiloPage.h"
#include "LocalResourceLoader.h"
#include "FileSystemRepository.h"

using namespace Concurrency;
using namespace Hilo;
using namespace Hilo::Common;
using namespace Platform;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Interop;
using namespace Windows::Foundation;

// See http://go.microsoft.com/fwlink/?LinkId=267274 for info about this app.

/// <summary>
/// Initializes the singleton application object.  This is the first line of authored code
/// executed, and as such is the logical equivalent of main() or WinMain().
/// </summary>
App::App()
{
#ifndef NDEBUG
    // remember thread ID of main thread for assertion checking of runtime context
    RecordMainThread();
#endif
    InitializeComponent();
    // See http://go.microsoft.com/fwlink/?LinkId=267280 for more info on Hilo's implementation of suspend/resume.
    Suspending += ref new SuspendingEventHandler(this, &App::OnSuspending);
    Resuming += ref new EventHandler<Platform::Object^>(this, &App::OnResume);
    m_exceptionPolicy = ExceptionPolicyFactory::GetCurrentPolicy();
    m_repository = std::make_shared<FileSystemRepository>(m_exceptionPolicy);
}

/// <summary>
/// Invoked when the application is launched normally by the end user.  Other entry points will
/// be used when the application is launched to open a specific file, to display search results,
/// and so forth.
/// </summary>
/// <param name="args">Details about the launch request and process.</param>
void App::OnLaunched(LaunchActivatedEventArgs^ args)
{
    assert(IsMainThread());
    auto rootFrame = dynamic_cast<Frame^>(Window::Current->Content);

    // Do not repeat app initialization when the Window already has content,
    // just ensure that the window is active
    if (rootFrame == nullptr)
    {
        // Create a Frame to act as the navigation context and associate it with
        // a SuspensionManager key. See http://go.microsoft.com/fwlink/?LinkId=267280 for more info 
        // on Hilo's implementation of suspend/resume.
        rootFrame = ref new Frame();
        SuspensionManager::RegisterFrame(rootFrame, "AppFrame");

        auto prerequisite = task<void>([](){});
        if (args->PreviousExecutionState == ApplicationExecutionState::Terminated)
        {
            // Restore the saved session state only when appropriate, scheduling the
            // final launch steps after the restore is complete
            prerequisite = SuspensionManager::RestoreAsync();
        }
        prerequisite.then([=](task<void> prerequisite)
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

            if (rootFrame->Content == nullptr)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter.  See http://go.microsoft.com/fwlink/?LinkId=267278 for a walkthrough of how 
                // Hilo creates pages and navigates to pages.
                if (!rootFrame->Navigate(TypeName(MainHubView::typeid)))
                {
                    throw ref new FailureException((ref new LocalResourceLoader())->GetString("ErrorFailedToCreateInitialPage"));
                }
            }

            // Place the frame in the current Window
            Window::Current->Content = rootFrame;
            // Ensure the current window is active
            Window::Current->Activate();

        }, task_continuation_context::use_current());
    }
    else
    {
        if (rootFrame->Content == nullptr)
        {
            // When the navigation stack isn't restored navigate to the first page,
            // configuring the new page by passing required information as a navigation
            // parameter. See http://go.microsoft.com/fwlink/?LinkId=267278 for a walkthrough of how 
            // Hilo creates pages and navigates to pages.
            if (!rootFrame->Navigate(TypeName(MainHubView::typeid)))
            {
                throw ref new FailureException((ref new LocalResourceLoader())->GetString("ErrorFailedToCreateInitialPage"));
            }
        }
        // Ensure the current window is active
        Window::Current->Activate();
    }

    // Schedule updates to the tile. See http://go.microsoft.com/fwlink/?LinkId=267275 for
    // info about how Hilo manages tiles.
    m_tileUpdateScheduler = std::make_shared<TileUpdateScheduler>();
    m_tileUpdateScheduler->ScheduleUpdateAsync(m_repository, m_exceptionPolicy);
}

// See http://go.microsoft.com/fwlink/?LinkId=267280 for more info on Hilo's implementation of suspend/resume.

/// <summary>
/// Invoked when application execution is being suspended.  Application state is saved
/// without knowing whether the application will be terminated or resumed with the contents
/// of memory still intact.
/// </summary>
/// <param name="sender">The source of the suspend request.</param>
/// <param name="e">Details about the suspend request.</param>
void App::OnSuspending(Object^ sender, SuspendingEventArgs^ e)
{
    (void) sender; // Unused parameter
    assert(IsMainThread());

    auto deferral = e->SuspendingOperation->GetDeferral();
    HiloPage::IsSuspending = true;
    SuspensionManager::SaveAsync().then([=](task<void> antecedent)
    {
        HiloPage::IsSuspending = false;
        antecedent.get();
        deferral->Complete();
    });
}

// See http://go.microsoft.com/fwlink/?LinkId=267280 for more info on Hilo's implementation of suspend/resume.
void App::OnResume(Object^ sender, Platform::Object^ e)
{
    (void) sender; // Unused parameter
    (void) e;      // Unused parameter
    assert(IsMainThread());

    if (m_repository != nullptr)
    {
        // Hilo does not receive data change events when suspended. Create these events on resume.
        m_repository->NotifyAllObservers();
    }
}
