// Copyright (c) Microsoft. All rights reserved.

#pragma once

#include "pch.h"
#include "Scenario1_Registering.g.h"
#include "MainPage.xaml.h"

using namespace Windows::UI::Xaml;

namespace SDKSample
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	[Windows::Foundation::Metadata::WebHostHidden]
    public ref class Scenario1 sealed
	{
	public:
		Scenario1();
	private:
		MainPage^ rootPage;
		void Scenario1Open_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void Scenario1Unregister_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void OutputToTextBox(Platform::String^ text);

        Windows::UI::Core::CoreDispatcher^ _dispatcher;

        void OpenChannelAndRegisterTask();
        void RegisterBackgroundTask();
		bool UnregisterBackgroundTask();
        void BackgroundTaskCompleted(Windows::ApplicationModel::Background::BackgroundTaskRegistration^ sender, Windows::ApplicationModel::Background::BackgroundTaskCompletedEventArgs^ args);
	};
}
