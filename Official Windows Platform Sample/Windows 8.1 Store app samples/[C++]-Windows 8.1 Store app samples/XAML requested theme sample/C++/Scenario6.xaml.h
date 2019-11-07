//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario6.xaml.h
// Declaration of the Scenario6 class
//

#pragma once

#include "Scenario6.g.h"
namespace SDKSample
{
	namespace RequestedThemeCPP
	{
		/// <summary>
		/// An empty page that can be used on its own or navigated to within a Frame.
		/// </summary>
		public ref class Scenario6 sealed
		{
		public:
			Scenario6();
		private:
			void lightButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void darkButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void SDKSample::RequestedThemeCPP::Scenario6::SaveTheme(bool isLightTheme);
		}

	}
}
