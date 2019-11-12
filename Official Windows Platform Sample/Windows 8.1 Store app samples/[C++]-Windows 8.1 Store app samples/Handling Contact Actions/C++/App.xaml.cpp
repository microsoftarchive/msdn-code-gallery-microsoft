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

using namespace SDKSample;
using namespace SDKSample::Common;

using namespace Concurrency;
using namespace Platform;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Activation;
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

/// <summary>
/// Initializes the singleton application object.  This is the first line of authored code
/// executed, and as such is the logical equivalent of main() or WinMain().
/// </summary>
App::App()
{
    InitializeComponent();
    this->Suspending += ref new SuspendingEventHandler(this, &SDKSample::App::OnSuspending);
}

/// <summary>
/// Initializes the window content of the app and navigates to the appropriate page. 
/// </summary>
/// <param name="rootFrame">The Frame to set as the content of the app window.</param>
/// <param name="args">Details about the activation request and process.</param>
void App::InitializeWindowContent(Frame^ rootFrame, IActivatedEventArgs^ args)
{
    auto launchArgs = dynamic_cast<LaunchActivatedEventArgs^>(args);
    String^ launchArguments = (launchArgs != nullptr) ? launchArgs->Arguments : nullptr;
    if (rootFrame->Content == nullptr || launchArguments != nullptr)
    {
        // When the navigation stack isn't restored or there are launch arguments
        // indicating an alternate launch (e.g.: via toast or secondary tile), 
        // navigate to the appropriate page, configuring the new page by passing required 
        // information as a navigation parameter
        if (!rootFrame->Navigate(TypeName(MainPage::typeid), launchArguments))
        {
            throw ref new FailureException("Failed to create initial page");
        }
    }
    // Place the frame in the current Window
    Window::Current->Content = rootFrame;

    // Navigate to appropriate page based on activation arguments if present.
    auto mainPage = dynamic_cast<MainPage^>(rootFrame->Content);
    mainPage->ContactEvent = nullptr;
    mainPage->ProtocolEvent = nullptr;
    
    auto contactArgs = dynamic_cast<IContactActivatedEventArgs^>(args);
    if (contactArgs != nullptr)
    {
        mainPage->ContactEvent = contactArgs;
        mainPage->NavigateToContactEventPage();
    }
    else
    {
        auto protocolArgs = dynamic_cast<ProtocolActivatedEventArgs^>(args);
        if (protocolArgs != nullptr)
        {
            mainPage->ProtocolEvent = protocolArgs;
            mainPage->NavigateToProtocolEventPage();
        }
    }

    // Ensure the current window is active
    Window::Current->Activate();
}

/// <summary>
/// Creates the root Frame if necessary and initializes the window content of the app.
/// This is called regardless of how the app is being activated. 
/// </summary>
/// <param name="args">Details about the activation request and process.</param>
void App::InitializeApp(IActivatedEventArgs^ args)
{
    auto rootFrame = dynamic_cast<Frame^>(Window::Current->Content);

    // Do not repeat app initialization when the Window already has content,
    // just ensure that the window is active
    if (rootFrame == nullptr)
    {
        // Create a Frame to act as the navigation context and associate it with
        // a SuspensionManager key
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

            InitializeWindowContent(rootFrame, args);

        }, task_continuation_context::use_current());
    }
    else
    {
        InitializeWindowContent(rootFrame, args);
    }
}

/// <summary>
/// Invoked when the application is launched normally by the end user.  Other entry points will
/// be used when the application is launched to open a specific file, to display search results,
/// and so forth.
/// </summary>
/// <param name="args">Details about the launch request and process.</param>
void App::OnLaunched(LaunchActivatedEventArgs^ args)
{
    InitializeApp(args);
}

/// <summary>
/// Invoked when the application is being activated to handle a contract such as the tel:
/// protocol, or actions on contact data, such as calling a phone number, and so forth.
/// </summary>
/// <param name="args">Details about the activation request and process.</param>
void App::OnActivated(IActivatedEventArgs^ args)
{
    if ((args->Kind == ActivationKind::Contact) || (args->Kind == ActivationKind::Protocol))
    {
        InitializeApp(args);
    }
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
