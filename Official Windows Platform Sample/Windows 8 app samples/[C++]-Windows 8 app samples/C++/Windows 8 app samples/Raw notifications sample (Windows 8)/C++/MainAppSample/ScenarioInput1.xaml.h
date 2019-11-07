// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//
// ScenarioInput1.xaml.h
// Declaration of the ScenarioInput1 class.
//

#pragma once

#include "pch.h"
#include "ScenarioInput1.g.h"
#include "MainPage.xaml.h"

using namespace Windows::UI::Xaml;

namespace RawNotificationsSampleCPP
{
	[Windows::Foundation::Metadata::WebHostHidden]
	public ref class ScenarioInput1 sealed
	{
	public:
		ScenarioInput1();

	protected:
		virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
		virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

	private:
		~ScenarioInput1();
		MainPage^ rootPage;
		void OutputFrameLoaded(Object^ sender, Object^ e);
		void Scenario1Open_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void Scenario1Unregister_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void OutputToTextBox(Platform::String^ text);

        Windows::UI::Core::CoreDispatcher^ _dispatcher;
		EventRegistrationToken _frameLoadedToken;

        void OpenChannelAndRegisterTask();
        void RegisterBackgroundTask();
		bool UnregisterBackgroundTask();
        void BackgroundTaskCompleted(Windows::ApplicationModel::Background::BackgroundTaskRegistration^ sender, Windows::ApplicationModel::Background::BackgroundTaskCompletedEventArgs^ args);
	};
}
