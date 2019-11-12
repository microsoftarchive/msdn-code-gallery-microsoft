// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// UserSettings.cpp
// Implementation of the UserSettings class
// This class is used in Scenarios 3 and 4.
//

#include "pch.h"
#include "UserSettings.h"

using namespace SDKSample::RequestedThemeCPP;
using namespace Platform;
using namespace Windows::UI;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Media;

UserSettings::UserSettings()
{
	_selectedTheme = Windows::UI::Xaml::ElementTheme::Dark;
}

void UserSettings::SelectedTheme::set(Windows::UI::Xaml::ElementTheme value)
{
    if (_selectedTheme != value)
    {
       _selectedTheme = value;
        OnPropertyChanged("SelectedTheme");
    }
}

Windows::UI::Xaml::ElementTheme UserSettings::SelectedTheme::get()
{
    return _selectedTheme;
}

void UserSettings::OnPropertyChanged(String^ propertyName)
{
    PropertyChanged(this, ref new PropertyChangedEventArgs(propertyName));
}
