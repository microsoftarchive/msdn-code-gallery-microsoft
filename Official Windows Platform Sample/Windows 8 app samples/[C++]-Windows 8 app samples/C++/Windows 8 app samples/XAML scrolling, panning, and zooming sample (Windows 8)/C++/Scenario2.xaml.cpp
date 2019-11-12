//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

//
// Scenario2.xaml.cpp
// Implementation of the Scenario2 class
//

#include "pch.h"
#include "Scenario2.xaml.h"

using namespace SDKSample::ScrollViewerCPP;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario2::Scenario2()
{
    InitializeComponent();
}

// Invoked when this page is about to be displayed in a Frame.
void Scenario2::OnNavigatedTo(NavigationEventArgs^ e)
{
    rootPage = MainPage::Current;
}

// Handler for combobox
void SDKSample::ScrollViewerCPP::Scenario2::ComboBox_SelectionChanged_1(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e)
{
	if (scrollViewer == nullptr)
	{
	    return;
	}
	
	auto cb = (ComboBox^) sender;
	
	if (cb != nullptr)
	{
	    switch (cb->SelectedIndex)
	    {
	        case 0: // None
	            scrollViewer->HorizontalSnapPointsType = SnapPointsType::None;
	            break;
	        case 1: //Optional
	            scrollViewer->HorizontalSnapPointsType = SnapPointsType::Optional;
	            break;
	        case 2: // Optional Single
	            scrollViewer->HorizontalSnapPointsType = SnapPointsType::OptionalSingle;
	            break;
	        case 3: // Mandatory
	            scrollViewer->HorizontalSnapPointsType = SnapPointsType::Mandatory;
	            break;
	        case 4: // Mandatory Single
	            scrollViewer->HorizontalSnapPointsType = SnapPointsType::MandatorySingle;
	            break;
	        default:
	            scrollViewer->HorizontalSnapPointsType = SnapPointsType::None;
	            break;
	    }
	}
}

// Call Scenario2Reset()
void SDKSample::ScrollViewerCPP::Scenario2::Scenario2Reset(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	Scenario2Reset();
}

// Reset the scenario
void SDKSample::ScrollViewerCPP::Scenario2::Scenario2Reset()
{
	//Restore to defaults
	scrollViewer->HorizontalSnapPointsType = SnapPointsType::None;
	snapPointsCombo->SelectedIndex = 0;
}