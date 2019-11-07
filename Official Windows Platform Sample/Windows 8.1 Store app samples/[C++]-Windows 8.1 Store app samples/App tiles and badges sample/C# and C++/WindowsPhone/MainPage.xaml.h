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
		
	protected:		
		virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;		

	private:		
		void HardwareButtons_BackPressed(Object^ sender, Windows::Phone::UI::Input::BackPressedEventArgs^ e);		

	internal:
		static MainPage^ Current;
		void NotifyUser(Platform::String^ strMessage, NotifyType type);
    };
}
