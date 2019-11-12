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
#include "Scenario4_CommandBar.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::AppBarControl;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Xaml::Controls;

Scenario4::Scenario4()
{
    InitializeComponent();
	rootPage = MainPage::Current;
}

void Scenario4::OnNavigatedTo(NavigationEventArgs^ e)
{
	// Save original AppBar so we can restore it afterward
	originalAppBar = rootPage->BottomAppBar;

	// Use a CommandBar rather than an AppBar so that we get default layout
	CommandBar^ commandBar = ref new CommandBar();

	// Create the 'Add' button
	AppBarButton^ add = ref new AppBarButton();
	add->Label = "Add";
	add->Icon = ref new SymbolIcon(Symbol::Add);

	commandBar->PrimaryCommands->Append(add);

	// Create the 'Remove' button
	AppBarButton^ remove = ref new AppBarButton();
	remove->Label = "Remove";
	remove->Icon = ref new SymbolIcon(Symbol::Remove);

	commandBar->PrimaryCommands->Append(remove);

	commandBar->PrimaryCommands->Append(ref new AppBarSeparator());

	// Create the 'Delete' button
	AppBarButton^ del = ref new AppBarButton();
	del->Label = "Delete";
	del->Icon = ref new SymbolIcon(Symbol::Delete);

	commandBar->PrimaryCommands->Append(del);

	// Create the 'Camera' button
	AppBarButton^ camera = ref new AppBarButton();
	camera->Label = "Camera";
	camera->Icon = ref new SymbolIcon(Symbol::Camera);
	commandBar->SecondaryCommands->Append(camera);

	// Create the 'Bold' button
	AppBarButton^ bold = ref new AppBarButton();
	bold->Label = "Bold";
	bold->Icon = ref new SymbolIcon(Symbol::Bold);
	commandBar->SecondaryCommands->Append(bold);

	// Create the 'Italic' button
	AppBarButton^ italic = ref new AppBarButton();
	italic->Label = "Italic";
	italic->Icon = ref new SymbolIcon(Symbol::Italic);
	commandBar->SecondaryCommands->Append(italic);

	// Create the 'Underline' button
	AppBarButton^ underline = ref new AppBarButton();
	underline->Label = "Underline";
	underline->Icon = ref new SymbolIcon(Symbol::Underline);
	commandBar->SecondaryCommands->Append(underline);

	// Create the 'Align Left' button
	AppBarButton^ left = ref new AppBarButton();
	left->Label = "Align Left";
	left->Icon = ref new SymbolIcon(Symbol::AlignLeft);
	commandBar->SecondaryCommands->Append(left);

	// Create the 'Align Center' button
	AppBarButton^ center = ref new AppBarButton();
	center->Label = "Align Center";
	center->Icon = ref new SymbolIcon(Symbol::AlignCenter);
	commandBar->SecondaryCommands->Append(center);

	// Create the 'Align Right' button
	AppBarButton^ right = ref new AppBarButton();
	right->Label = "Align Right";
	right->Icon = ref new SymbolIcon(Symbol::AlignRight);
	commandBar->SecondaryCommands->Append(right);

	rootPage->BottomAppBar = commandBar;
}

void Scenario4::OnNavigatedFrom(NavigationEventArgs^ e)
{
	rootPage->BottomAppBar = originalAppBar;
}
