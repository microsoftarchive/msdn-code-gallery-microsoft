//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario2.xaml.cpp
// Implementation of the Scenario2 class
//

#include "pch.h"
#include "Scenario2_CustomizeColor.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::AppBarControl;

using namespace Windows::UI;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Interop;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario2::Scenario2()
{
    InitializeComponent();
	rootPage = MainPage::Current;
}

void Scenario2::OnNavigatedTo(NavigationEventArgs^ e)
{
	// Save off original background
	originalBackgroundBrush = rootPage->BottomAppBar->Background;

	// Set AppBar Background
	rootPage->BottomAppBar->Background = ref new SolidColorBrush(ColorHelper::FromArgb(255, 20, 20, 90));

	// Find our stack panels that contain our AppBarButtons

	leftPanel = safe_cast<StackPanel^>(rootPage->FindName("LeftPanel"));
	rightPanel = safe_cast<StackPanel^>(rootPage->FindName("RightPanel"));

	// Change the color of all AppBarButtons in each panel
	ColorButtons(leftPanel);
	ColorButtons(rightPanel);
}

void Scenario2::OnNavigatedFrom(NavigationEventArgs^ e)
{
	rootPage->BottomAppBar->Background = originalBackgroundBrush;
	RestoreButtons(leftPanel);
	RestoreButtons(rightPanel);
}

/// <summary>
/// This method will change the style of each AppBarButton to use a green foreground color
/// </summary>
/// <param name="panel"></param>
void Scenario2::ColorButtons(StackPanel^ panel)
{
	for each(Platform::Object^ item in panel->Children)
	{
		AppBarButton^ abb = dynamic_cast<AppBarButton^>(item) ;
		AppBarSeparator^ abs = dynamic_cast<AppBarSeparator^>(item) ;

		// For AppBarButton, change the style
		if (nullptr != abb)
		{
			AppBarButton^ b = dynamic_cast<AppBarButton^>(item) ;
			originalButtonStyle = b->Style;
			b->Style = safe_cast<Windows::UI::Xaml::Style^>(rootPage->Resources->Lookup("GreenAppBarButtonStyle"));
		}
		else
		{
			// For AppBarSeparator(s), just change the foreground color
			if (abs != nullptr)
			{
				originalSeparatorBrush = abs->Foreground;
				abs->Foreground = ref new SolidColorBrush(ColorHelper::FromArgb(255, 90, 200, 90));
			}
		}
	}
}

void Scenario2::RestoreButtons(StackPanel^ panel)
{
	for each(Platform::Object^ item in panel->Children)
	{
		AppBarButton^ abb = dynamic_cast<AppBarButton^>(item) ;
		AppBarSeparator^ abs = dynamic_cast<AppBarSeparator^>(item) ;

		// For AppBarButton, change the style
		if (nullptr != abb)
		{
			AppBarButton^ b = dynamic_cast<AppBarButton^>(item) ;
			b->Style = originalButtonStyle;
		}
		else
		{
			// For AppBarSeparator(s), just change the foreground color
			if (abs != nullptr)
			{
				abs->Foreground = originalSeparatorBrush;
			}
		}
	}
}
