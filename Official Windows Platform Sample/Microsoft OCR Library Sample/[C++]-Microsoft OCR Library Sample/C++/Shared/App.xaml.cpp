// Copyright (c) Microsoft. All rights reserved.

#include "pch.h"
#include "MainPage.xaml.h"

using namespace OCR;

using namespace concurrency;
using namespace Platform;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
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
    Suspending += ref new SuspendingEventHandler(this, &App::OnSuspending);
}

/// <summary>
/// Invoked when the application is launched normally by the end user.	Other entry points
/// will be used such as when the application is launched to open a specific file.
/// </summary>
/// <param name="e">Details about the launch request and process.</param>
void App::OnLaunched(Windows::ApplicationModel::Activation::LaunchActivatedEventArgs^ e)
{	
    auto rootFrame = dynamic_cast<Frame^>(Window::Current->Content);	

    // Do not repeat app initialization when the Window already has content,
    // just ensure that the window is active
    if (rootFrame == nullptr)
    {
        // Create a Frame to act as the navigation context 
        rootFrame = ref new Frame();

        // Set the default language
        rootFrame->Language = Windows::Globalization::ApplicationLanguages::Languages->GetAt(0);

		// Change this value to a cache size that is appropriate for your application.
		rootFrame->CacheSize = 1;

		auto prerequisite = task<void>([](){});
		if (e->PreviousExecutionState == ApplicationExecutionState::Terminated)
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
				// parameter
				rootFrame->Navigate(MainPage::typeid, e->Arguments);
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
			// parameter
			if (!rootFrame->Navigate(MainPage::typeid, e->Arguments))
			{
				throw ref new FailureException("Failed to create initial page");
			}
		}

		// Ensure the current window is active
		Window::Current->Activate();
	}
}

/// <summary>
/// Invoked when application execution is being suspended. Application state is saved
/// without knowing whether the application will be terminated or resumed with the contents
/// of memory still intact.
/// </summary>
void App::OnSuspending(Object^ /* sender */, SuspendingEventArgs^ e)
{
	auto deferral = e->SuspendingOperation->GetDeferral();
	SuspensionManager::SaveAsync().then([=]()
	{
		deferral->Complete();
	});
}

#if WINAPI_FAMILY == WINAPI_FAMILY_PHONE_APP

Frame^ App::CreateRootFrame()
{
    Frame^ rootFrame = dynamic_cast<Frame^>(Window::Current->Content);

    if (rootFrame == nullptr)
    {
        // Create a Frame to act as the navigation context 
        rootFrame = ref new Frame();

        // Set the default language
        rootFrame->Language = Windows::Globalization::ApplicationLanguages::Languages->GetAt(0);
        //rootFrame->NavigationFailed += ref new Windows::UI::Xaml::Navigation::NavigationFailedEventHandler(this, &App::OnNavigationFailed);

        // Place the frame in the current Window
        Window::Current->Content = rootFrame;
    }

    return rootFrame;
}

/// <summary>
/// Handle OnActivated event to deal with File Open/Save continuation activation kinds
/// </summary>
/// <param name="e">Application activated event arguments, it can be casted to proper sub-type based on ActivationKind</param>
void App::OnActivated(IActivatedEventArgs^ e)
{
    Application::OnActivated(e);

    continuationManager = ref new ContinuationManager();
    auto rootFrame = CreateRootFrame();

    RestoreStatus(e->PreviousExecutionState).then([=](task<void> prerequisite)
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
            rootFrame->Navigate(TypeName(MainPage::typeid));
        }

        auto continuationEventArgs = dynamic_cast<IContinuationActivatedEventArgs^>(e);
        if (continuationEventArgs != nullptr)
        {
            MainPage^ mainPage = MainPage::Current;
            Frame^ scenarioFrame = dynamic_cast<Frame^>(mainPage->FindName("ScenarioFrame"));
            if (scenarioFrame != nullptr)
            {
                // Call ContinuationManager to handle continuation activation
                continuationManager->Continue(continuationEventArgs, scenarioFrame);
            }
        }

        Window::Current->Activate();
    }, task_continuation_context::use_current());
}

concurrency::task<void> App::RestoreStatus(ApplicationExecutionState previousExecutionState)
{
    auto prerequisite = task<void>([](){});
    if (previousExecutionState == ApplicationExecutionState::Terminated)
    {
        prerequisite = SuspensionManager::RestoreAsync();
    }

    return prerequisite;
}

#endif //WINAPI_FAMILY == WINAPI_FAMILY_PHONE_APP