// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// Scenario1.xaml.h
// Declaration of the Scenario1 class
//

#pragma once
#include "Scenario1.g.h"

namespace SDKSample
{
	namespace Flyouts
	{
		/// <summary>
		/// An empty page that can be used on its own or navigated to within a Frame.
		/// </summary>
		public ref class Scenario1 sealed
		{
		public:
			Scenario1();

		private:
			void flyout_Opened(Platform::Object^ sender);
			void menuFlyout_Opened(Platform::Object^ sender);
			void confirmPurchase_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void option_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void enable_Click(Platform::Object^ sender);
		};
	}
}
