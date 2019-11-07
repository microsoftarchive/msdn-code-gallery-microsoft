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
    /// MainPage holds the status block and frame in which scenario files are loaded.
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
		void HardwareButtons_BackPressed(Object^ sender, Windows::Phone::UI::Input::BackPressedEventArgs^ e);		

        Windows::ApplicationModel::Activation::FileActivatedEventArgs^ fileEventArgs;
        Windows::ApplicationModel::Activation::ProtocolActivatedEventArgs^ protocolEventArgs;

	internal:
		static MainPage^ Current;
		void NotifyUser(Platform::String^ strMessage, NotifyType type);
    };
}
