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
// Scenario1.xaml.cpp
// Implementation of the Scenario1 class
//

#include "pch.h"
#include "Scenario1.xaml.h"

using namespace SDKSample::ScrollViewerCPP;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario1::Scenario1()
{
    InitializeComponent();
}

// Invoked when this page is about to be displayed in a Frame.
void Scenario1::OnNavigatedTo(NavigationEventArgs^ e)
{
    rootPage = MainPage::Current;
}

// Handler for the horizontal mode visibility combo box
void SDKSample::ScrollViewerCPP::Scenario1::hsmCombo_SelectionChanged_1(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e)
{
	if (scrollViewer == nullptr)
	{
	    return;
	}
	
	auto cb = (ComboBox^)sender;
	
	if (cb != nullptr)
	{
	    switch (cb->SelectedIndex)
	    {
	        case 0: // Auto
	            scrollViewer->HorizontalScrollMode = ScrollMode::Auto;
	            break;
	        case 1: //Enabled
	            scrollViewer->HorizontalScrollMode = ScrollMode::Enabled;
	            break;
	        case 2: // Disabled
	            scrollViewer->HorizontalScrollMode = ScrollMode::Disabled;
	            break;
	        default:
	            scrollViewer->HorizontalScrollMode = ScrollMode::Enabled;
	            break;
	    }
	}
}

// Handler for the horizontal scroll visibility combo box
void SDKSample::ScrollViewerCPP::Scenario1::hsbvCombo_SelectionChanged_1(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e)
{
	if (scrollViewer == nullptr)
	{
	    return;
	}
	
	auto cb = (ComboBox^)sender;
	
	if (cb != nullptr)
	{
	    switch (cb->SelectedIndex)
	    {
	        case 0: // Auto
	            scrollViewer->HorizontalScrollBarVisibility = ScrollBarVisibility::Auto;
	            break;
	        case 1: //Enabled
	            scrollViewer->HorizontalScrollBarVisibility = ScrollBarVisibility::Visible;
	            break;
	        case 2: // Disabled
	            scrollViewer->HorizontalScrollBarVisibility = ScrollBarVisibility::Hidden;
	            break;
	        default:
	            scrollViewer->HorizontalScrollBarVisibility = ScrollBarVisibility::Disabled;
	            break;
	    }
	}
}

// Handler for the vertical scroll mode combo box
void SDKSample::ScrollViewerCPP::Scenario1::vsmCombo_SelectionChanged_1(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e)
{
	if (scrollViewer == nullptr)
	{
	    return;
	}
	
	auto cb = (ComboBox^)sender;
	
	if (cb != nullptr)
	{
	    switch (cb->SelectedIndex)
	    {
	        case 0: // Auto
	            scrollViewer->VerticalScrollMode = ScrollMode::Auto;
	            break;
	        case 1: //Enabled
	            scrollViewer->VerticalScrollMode = ScrollMode::Enabled;
	            break;
	        case 2: // Disabled
	            scrollViewer->VerticalScrollMode = ScrollMode::Disabled;
	            break;
	        default:
	            scrollViewer->VerticalScrollMode = ScrollMode::Enabled;
	            break;
	    }
	}
}

// Handler for the vertical scroll visibility combo box
void SDKSample::ScrollViewerCPP::Scenario1::vsbvCombo_SelectionChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e)
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
	        case 0: // Auto
	            scrollViewer->VerticalScrollBarVisibility = ScrollBarVisibility::Auto;
	            break;
	        case 1: //Visible
	            scrollViewer->VerticalScrollBarVisibility = ScrollBarVisibility::Visible;
	            break;
	        case 2: // Hidden
	            scrollViewer->VerticalScrollBarVisibility = ScrollBarVisibility::Hidden;
	            break;
	        case 3: // Disabled
	            scrollViewer->VerticalScrollBarVisibility = ScrollBarVisibility::Disabled;
	            break;
	        default:
	            scrollViewer->VerticalScrollBarVisibility = ScrollBarVisibility::Disabled;
	            break;
	    }
	}
}

// Checked handler for horizontal railed check box
void SDKSample::ScrollViewerCPP::Scenario1::CheckBox_Checked_HorizontalRailed(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	scrollViewer->IsHorizontalRailEnabled = true;
}

// Unchecked handler for horizontal railed check box
void SDKSample::ScrollViewerCPP::Scenario1::CheckBox_Unchecked_HorizontalRailed(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	scrollViewer->IsHorizontalRailEnabled = false;
}

// Checked handler for vertical railed check box
void SDKSample::ScrollViewerCPP::Scenario1::CheckBox_Checked_VerticalRailed(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	scrollViewer->IsVerticalRailEnabled = true;
}

// Unchecked handler for vertical railed check box
void SDKSample::ScrollViewerCPP::Scenario1::CheckBox_Unchecked_VerticalRailed(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	scrollViewer->IsVerticalRailEnabled = false;
}

// Call Scenario1Reset
void SDKSample::ScrollViewerCPP::Scenario1::Scenario1Reset(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	Scenario1Reset();
}

// Reset the scenario
void SDKSample::ScrollViewerCPP::Scenario1::Scenario1Reset()
{
	//Restore to defaults
	hsbvCombo->SelectedIndex = 3; 
	hsmCombo->SelectedIndex = 1;  
	vsbvCombo->SelectedIndex = 1; 
	vsmCombo->SelectedIndex = 1;  
	hrCheckbox->IsChecked = true; 
	vrCheckbox->IsChecked = true;
}