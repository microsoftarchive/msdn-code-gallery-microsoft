//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario3.xaml.cpp
// Implementation of the Scenario3 class
//

#include "pch.h"
#include "Scenario3.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::RequestedThemeCPP;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;

Scenario3::Scenario3()
{
	InitializeComponent();
	_userSettings =  ref new  UserSettings();
	_userSettings->SelectedTheme = ElementTheme::Light;
	panel->DataContext = _userSettings;
}

void RequestedThemeCPP::Scenario3::Button_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	RevertRequestedTheme();
}

void  RequestedThemeCPP::Scenario3::RevertRequestedTheme()
{
	if (_userSettings->SelectedTheme == ElementTheme::Dark)
		_userSettings->SelectedTheme = ElementTheme::Light;
	else
		_userSettings->SelectedTheme = ElementTheme::Dark;

}

