//
// App.xaml.h
// Declaration of the App class.
//

#pragma once

#include "App.g.h"

#if WINAPI_FAMILY == WINAPI_FAMILY_PHONE_APP
#include "ContinuationManager.h"
#endif
namespace CppUniversalAppImageToVideo
{
	/// <summary>
	/// Provides application-specific behavior to supplement the default Application class.
	/// </summary>
	ref class App sealed
	{
	public:
		App();

		virtual void OnLaunched(Windows::ApplicationModel::Activation::LaunchActivatedEventArgs^ e) override;
#if WINAPI_FAMILY==WINAPI_FAMILY_PHONE_APP
		virtual void OnActivated(Windows::ApplicationModel::Activation::IActivatedEventArgs^ e) override;
#endif
	private:
#if WINAPI_FAMILY==WINAPI_FAMILY_PHONE_APP
		ContinuationManager^ continuationManager;
		
		Windows::UI::Xaml::Media::Animation::TransitionCollection^ _transitions;
		Windows::Foundation::EventRegistrationToken _firstNavigatedToken;

		void RootFrame_FirstNavigated(Platform::Object^ sender, Windows::UI::Xaml::Navigation::NavigationEventArgs^ e);
#endif

		void OnSuspending(Platform::Object^ sender, Windows::ApplicationModel::SuspendingEventArgs^ e);
	};
}
