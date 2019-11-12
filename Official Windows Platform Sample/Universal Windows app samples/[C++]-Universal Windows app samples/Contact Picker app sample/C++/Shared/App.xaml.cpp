// Copyright (c) Microsoft. All rights reserved.

#include "pch.h"

#if WINAPI_FAMILY == WINAPI_FAMILY_PC_APP
#include "..\Windows\ContactPickerPage.xaml.h"
#endif

#if WINAPI_FAMILY == WINAPI_FAMILY_PHONE_APP
#include "PhoneConfiguration.h"
#endif

using namespace SDKSample;

using namespace Concurrency;
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

	#if WINAPI_FAMILY == WINAPI_FAMILY_PHONE_APP
	PhoneConfiguration::CreateContactStore();
	#endif
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

        rootFrame->NavigationFailed += ref new Windows::UI::Xaml::Navigation::NavigationFailedEventHandler(this, &App::OnNavigationFailed);

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

			//MainPage is always in rootFrame so we don't have to worry about restoring the navigation state on resume
			rootFrame->Navigate(TypeName(MainPage::typeid), e->Arguments);

			// Place the frame in the current Window
			Window::Current->Content = rootFrame;

			// Ensure the current window is active
			Window::Current->Activate();

		}, task_continuation_context::use_current());
    }
}

/// <summary>
/// Invoked when application execution is being suspended.	Application state is saved
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

/// <summary>
/// Invoked when Navigation to a certain page fails
/// </summary>
/// <param name="sender">The Frame which failed navigation</param>
/// <param name="e">Details about the navigation failure</param>
void App::OnNavigationFailed(Platform::Object ^sender, Windows::UI::Xaml::Navigation::NavigationFailedEventArgs ^e)
{
    throw ref new FailureException("Failed to load Page " + e->SourcePageType.Name);
}

/// <summary>
/// Activates the ContactPickerPage.
/// </summary>
/// <param name="args">Activation args</param>
void App::OnActivated(IActivatedEventArgs^ args)
{
	#if WINAPI_FAMILY == WINAPI_FAMILY_PC_APP
	if (args->Kind == ActivationKind::ContactPicker)
	{
		auto page = ref new ContactPickerPage();
		page->Activate(safe_cast<ContactPickerActivatedEventArgs^>(args));
	}
	else
	{
		__super::OnActivated(args);
	}
	#endif
}