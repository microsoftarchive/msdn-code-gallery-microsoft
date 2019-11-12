// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// UserSettings.h
// Declaration of the UserSettings class
// This class is used in Scenarios 3 and 4.
//
#pragma once

#include "pch.h"
namespace SDKSample
{
	namespace RequestedThemeCPP
	{
		[Windows::UI::Xaml::Data::Bindable]
		[Windows::Foundation::Metadata::WebHostHidden]
		public ref class UserSettings sealed: Windows::UI::Xaml::Data::INotifyPropertyChanged
		{
		private:
			Windows::UI::Xaml::ElementTheme _selectedTheme;

		public:
			UserSettings();
			virtual event Windows::UI::Xaml::Data::PropertyChangedEventHandler^ PropertyChanged;

			property Windows::UI::Xaml::ElementTheme SelectedTheme
			{
				Windows::UI::Xaml::ElementTheme get();
				void set(Windows::UI::Xaml::ElementTheme value);
			}

		protected:
			void OnPropertyChanged(Platform::String^ propertyName);
		};

	}
}

