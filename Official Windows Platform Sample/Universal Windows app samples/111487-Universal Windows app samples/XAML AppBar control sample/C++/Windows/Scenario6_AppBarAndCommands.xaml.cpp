//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario6.xaml.cpp
// Implementation of the Scenario6 class
//

#include "pch.h"
#include "Scenario6_AppBarAndCommands.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace Platform::Collections;
using namespace SDKSample::AppBarControl;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;


Scenario6::Scenario6()
{
    InitializeComponent();
	commands = ref new Vector<UIElement^>();
}

void Scenario6::OnNavigatedTo(NavigationEventArgs^ e)
{
	rootPage = MainPage::Current;

	openedToken = rootPage->BottomAppBar->Opened += ref new Windows::Foundation::EventHandler<Object^>(this, &Scenario6::BottomAppBar_Opened);
	closedToken = rootPage->BottomAppBar->Closed += ref new Windows::Foundation::EventHandler<Object^>(this, &Scenario6::BottomAppBar_Closed);

	leftPanel = safe_cast<StackPanel^>(rootPage->FindName("LeftPanel"));
	rightPanel = safe_cast<StackPanel^>(rootPage->FindName("RightPanel"));

	ShowAppBar->IsEnabled = true;
}

void Scenario6::OnNavigatedFrom(NavigationEventArgs^ e)
{
	rootPage->BottomAppBar->Opened -= openedToken;
	rootPage->BottomAppBar->Closed -= closedToken;

	rootPage->BottomAppBar->IsOpen = false;
	rootPage->BottomAppBar->IsSticky = false;
	ShowAppBarButtons();
}

void Scenario6::BottomAppBar_Opened(Platform::Object^ sender, Object^ e)
{
	ShowAppBar->IsEnabled = false;

	AppBar^ ab = safe_cast<AppBar^>(sender);
	if (ab != nullptr)
	{
		ab->IsSticky = true;
		if (leftPanel->Children->Size > 0 && rightPanel->Children->Size > 0)
		{
			HideCommands->IsEnabled = true;
		}
	}
}

void Scenario6::BottomAppBar_Closed(Platform::Object^ sender, Object^ e)
{
	ShowAppBar->IsEnabled = true;
	HideCommands->IsEnabled = false;
}

void Scenario6::ShowCommands_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	HideCommands->IsEnabled = true;
	ShowCommands->IsEnabled = false;
	ShowAppBarButtons();
}


void Scenario6::HideCommands_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	Button^ b = safe_cast<Button^>(sender);
	if (b != nullptr)
	{
		b->IsEnabled = false;
		ShowCommands->IsEnabled = true;
		commands->Clear();
		HideAppBarButtons(rightPanel);
	}
}


void Scenario6::ShowAppBar_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	rootPage->BottomAppBar->IsOpen = true;
	HideCommands->IsEnabled = true;
}

void Scenario6::HideAppBarButtons(StackPanel^ panel)
{
	for each (UIElement^ item in panel->Children)
	{
		commands->Append(item);
	}
	panel->Children->Clear();
}

void Scenario6::ShowAppBarButtons()
{
	for each(UIElement^ item in commands)
	{
		rightPanel->Children->Append(item);
	}
	commands->Clear();
}