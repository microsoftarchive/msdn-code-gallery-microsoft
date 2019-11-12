// Copyright (c) Microsoft. All rights reserved.

#pragma once

#include "MainPage.g.h"
#include "SampleConfiguration.h"

namespace SDKSample
{
	public enum class NotifyType
	{
		StatusMessage,
		ErrorMessage
	};

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public ref class MainPage sealed
    {
	public:
		MainPage();

        property Windows::ApplicationModel::Activation::FileActivatedEventArgs^ FileEvent
        {
            Windows::ApplicationModel::Activation::FileActivatedEventArgs^ get()
            {
                return fileEventArgs;
            }

            void set(Windows::ApplicationModel::Activation::FileActivatedEventArgs^ value)
            {
                fileEventArgs = value;
            }
        }

        property Windows::ApplicationModel::Activation::ProtocolActivatedEventArgs^ ProtocolEvent
        {
            Windows::ApplicationModel::Activation::ProtocolActivatedEventArgs^ get()
            {
                return protocolEventArgs;
            }

            void set(Windows::ApplicationModel::Activation::ProtocolActivatedEventArgs^ value)
            {
                protocolEventArgs = value;
            }
        }

        void NavigateToFilePage();
        void NavigateToProtocolPage();

	protected:
		virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
		
	private:
		void ScenarioControl_SelectionChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e);
		void Footer_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);		

        Windows::ApplicationModel::Activation::FileActivatedEventArgs^ fileEventArgs;
        Windows::ApplicationModel::Activation::ProtocolActivatedEventArgs^ protocolEventArgs;

	internal:
		static MainPage^ Current;
		void NotifyUser(Platform::String^ strMessage, NotifyType type);

    };
}
