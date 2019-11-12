//
// App.xaml.cpp
// Implementation of the App class.
//

#include "pch.h"
#include "MainPage.xaml.h"
using namespace CppUniversalAppImageToVideo;
using namespace CppUniversalAppImageToVideo::Common;

using namespace Platform;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml::Media::Animation;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Interop;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;
using namespace Concurrency;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

/// <summary>
/// Initializes the singleton application object. This is the first line of authored code
/// executed, and as such is the logical equivalent of main() or WinMain().
/// </summary>
App::App()
{
	InitializeComponent();
	Suspending += ref new SuspendingEventHandler(this, &App::OnSuspending);
}

/// <summary>
/// Invoked when the application is launched normally by the end user. Other entry points
/// will be used when the application is launched to open a specific file, to display
/// search results, and so forth.
/// </summary>
/// <param name="e">Details about the launch request and process.</param>
void App::OnLaunched(LaunchActivatedEventArgs^ e)
{
#if _DEBUG
	if (IsDebuggerPresent())
	{
		DebugSettings->EnableFrameRateCounter = true;
	}
#endif

	auto rootFrame = dynamic_cast<Frame^>(Window::Current->Content);

	// Do not repeat app initialization when the Window already has content,
	// just ensure that the window is active.
	if (rootFrame == nullptr)
	{
		// Create a Frame to act as the navigation context and associate it with
		// a SuspensionManager key
		rootFrame = ref new Frame();

		// Associate the frame with a SuspensionManager key.
		SuspensionManager::RegisterFrame(rootFrame, "AppFrame");

		// TODO: Change this value to a cache size that is appropriate for your application.
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
				// Something went wrong restoring state.
				// Assume there is no state and continue.
				__debugbreak();
			}

			if (rootFrame->Content == nullptr)
			{

#if WINAPI_FAMILY==WINAPI_FAMILY_PHONE_APP
				// Removes the turnstile navigation for startup.
				if (rootFrame->ContentTransitions != nullptr)
				{
					_transitions = ref new TransitionCollection();
					for (auto transition : rootFrame->ContentTransitions)
					{
						_transitions->Append(transition);
					}
				}

				rootFrame->ContentTransitions = nullptr;
				_firstNavigatedToken = rootFrame->Navigated += ref new NavigatedEventHandler(this, &App::RootFrame_FirstNavigated);
#endif

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
			// parameter.
			if (!rootFrame->Navigate(MainPage::typeid, e->Arguments))
			{
				throw ref new FailureException("Failed to create initial page");
			}
		}
		
		// Ensure the current window is active
		Window::Current->Activate();
	}

	
}

#if WINAPI_FAMILY==WINAPI_FAMILY_PHONE_APP
/// <summary>
/// Handle OnActivated event to deal with File Open/Save continuation activation kinds
/// </summary>
/// <param name="e">Application activated event arguments, it can be casted to proper sub-type based on ActivationKind</param>
void App::OnActivated(IActivatedEventArgs^ e)
{
	Application::OnActivated(e);

	continuationManager = ref new ContinuationManager();
	auto continuationEventArgs = dynamic_cast<IContinuationActivatedEventArgs^>(e);
	if (continuationEventArgs != nullptr)
	{
		// Call ContinuationManager to handle continuation activation
		continuationManager->Continue(continuationEventArgs);

	}
}

/// <summary>
/// Restores the content transitions after the app has launched.
/// </summary>
void App::RootFrame_FirstNavigated(Object^ sender, NavigationEventArgs^ e)
{
	auto rootFrame = safe_cast<Frame^>(sender);

	TransitionCollection^ newTransitions;
	if (_transitions == nullptr)
	{
		newTransitions = ref new TransitionCollection();
		newTransitions->Append(ref new NavigationThemeTransition());
	}
	else
	{
		newTransitions = _transitions;
	}

	rootFrame->ContentTransitions = newTransitions;

	rootFrame->Navigated -= _firstNavigatedToken;
}

#endif


/// <summary>
/// Invoked when application execution is being suspended. Application state is saved
/// without knowing whether the application will be terminated or resumed with the contents
/// of memory still intact.
/// </summary>
void App::OnSuspending(Object^ sender, SuspendingEventArgs^ e)
{
	auto deferral = e->SuspendingOperation->GetDeferral();
	SuspensionManager::SaveAsync().then([=](){
		deferral->Complete();
	});
}
