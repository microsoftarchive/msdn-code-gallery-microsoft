// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// SettingsFlyout1.xaml.h
// Declaration of the SettingsFlyout1 class
//

#pragma once

#include "SettingsFlyout1.g.h"

namespace SDKSample
{
	namespace ApplicationSettings
	{
		[Windows::Foundation::Metadata::WebHostHidden]
		public ref class SettingsFlyout1 sealed
		{
		public:
			SettingsFlyout1();

		private:
			bool _navigationShortcutsRegistered;
			Windows::Foundation::EventRegistrationToken _acceleratorKeyEventToken;
			void SettingsFlyout1_AcceleratorKeyActivated(Windows::UI::Core::CoreDispatcher^ sender,
				Windows::UI::Core::AcceleratorKeyEventArgs^ args);
			void OnLoaded(Platform::Object ^sender, Windows::UI::Xaml::RoutedEventArgs ^e);
			void OnUnloaded(Platform::Object ^sender, Windows::UI::Xaml::RoutedEventArgs ^e);
		};
	}
}