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
	commands = ref new Vector<ICommandBarElement^>();
}

void Scenario6::OnNavigatedTo(NavigationEventArgs^ e)
{
	rootPage = MainPage::Current;

	openedToken = this->BottomAppBar->Opened += ref new Windows::Foundation::EventHandler<Object^>(this, &Scenario6::BottomAppBar_Opened);
	closedToken = this->BottomAppBar->Closed += ref new Windows::Foundation::EventHandler<Object^>(this, &Scenario6::BottomAppBar_Closed);
	HideCommands->IsEnabled = true;
}

void Scenario6::OnNavigatedFrom(NavigationEventArgs^ e)
{
	this->BottomAppBar->Opened -= openedToken;
	this->BottomAppBar->Closed -= closedToken;

	ShowAppBarButtons();
}

void Scenario6::BottomAppBar_Opened(Platform::Object^ sender, Object^ e)
{

	CommandBar^ cb = safe_cast<CommandBar^>(sender);
	if (cb != nullptr)
	{
		cb->IsSticky = true;
		if (cb->PrimaryCommands->Size > 0)
		{
			HideCommands->IsEnabled = true;
		}
	}
}

void Scenario6::BottomAppBar_Closed(Platform::Object^ sender, Object^ e)
{
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
		HideAppBarButtons();
	}
}

void Scenario6::HideAppBarButtons()
{
	CommandBar^ commandBar = safe_cast<CommandBar^>(this->BottomAppBar);
	for each (ICommandBarElement^ item in commandBar->PrimaryCommands)
	{
		commands->Append(item);
	}
	commandBar->PrimaryCommands->Clear();
}

void Scenario6::ShowAppBarButtons()
{
	CommandBar^ commandBar = safe_cast<CommandBar^>(this->BottomAppBar);
	for each(ICommandBarElement^ item in commands)
	{
		commandBar->PrimaryCommands->Append(item);
	}
	commands->Clear();
}