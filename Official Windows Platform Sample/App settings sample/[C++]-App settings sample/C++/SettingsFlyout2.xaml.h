// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// SettingsFlyout2.xaml.h
// Declaration of the SettingsFlyout2 class
//

#pragma once

#include "SettingsFlyout2.g.h"

namespace SDKSample
{
	namespace ApplicationSettings
	{
		[Windows::Foundation::Metadata::WebHostHidden]
		public ref class SettingsFlyout2 sealed
		{
		public:
			SettingsFlyout2();
			void button_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

		private:
			Windows::Foundation::EventRegistrationToken backClickEventRegistrationToken;
			void backClick(Platform::Object^ sender, Windows::UI::Xaml::Controls::BackClickEventArgs^ e);
			bool _navigationShortcutsRegistered;
			Windows::Foundation::EventRegistrationToken _acceleratorKeyEventToken;
			void SettingsFlyout2_AcceleratorKeyActivated(Windows::UI::Core::CoreDispatcher^ sender,
				Windows::UI::Core::AcceleratorKeyEventArgs^ args);
			void OnLoaded(Platform::Object ^sender, Windows::UI::Xaml::RoutedEventArgs ^e);
			void OnUnloaded(Platform::Object ^sender, Windows::UI::Xaml::RoutedEventArgs ^e);
			bool _isSecondContentLayer;
		};
	}
}