//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario4.xaml.cpp
// Implementation of the Scenario4 class
//

#include "pch.h"
#include "Scenario4.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::RequestedThemeCPP;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;

Scenario4::Scenario4()
{
	InitializeComponent();
}

void RequestedThemeCPP::Scenario4::Button_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	RevertRequestedTheme(panel);
}


void RequestedThemeCPP::Scenario4::RevertRequestedTheme(FrameworkElement^ fe)
{
	if (fe->RequestedTheme == ElementTheme::Default)
	{
		//The FrameworkElement doesn't have a RequestedTheme set, 
		//so we will need to ask to the Application what theme is using.
		if (Application::Current->RequestedTheme == ApplicationTheme::Dark)
		{
			fe->RequestedTheme = ElementTheme::Light;
		}
		else
		{
			fe->RequestedTheme = ElementTheme::Dark;
		}
	}
	else if (fe->RequestedTheme == ElementTheme::Dark)
		fe->RequestedTheme = ElementTheme::Light;
	else
		fe->RequestedTheme = ElementTheme::Dark;
	CurrentThemeTxtBlock->Text = "Current theme is " + fe->RequestedTheme.ToString() + ".";
}


