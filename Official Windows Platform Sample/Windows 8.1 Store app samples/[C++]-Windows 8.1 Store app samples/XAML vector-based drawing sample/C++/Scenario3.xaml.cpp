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
// Scenario3.xaml.cpp
// Implementation of the Scenario3 class
//

#include "pch.h"
#include "Scenario3.xaml.h"

using namespace SDKSample::Drawing;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Navigation;

Scenario3::Scenario3()
{
    InitializeComponent();
}

// Invoked when this page is about to be displayed in a Frame.
void Scenario3::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;

	// Set color selections
	Color1Selection->SelectedIndex = 0;
	Color2Selection->SelectedIndex = 1;
	Color3Selection->SelectedIndex = 2;
	
	Color1OffsetSelection->Value = Color1OffsetSelection->Minimum;
	Color2OffsetSelection->Value = (Color2OffsetSelection->Maximum - Color2OffsetSelection->Minimum) / 2;
	Color3OffsetSelection->Value = Color3OffsetSelection->Maximum;	
}

// Any time a slider or combobox is changed in the description of scenario 3 
// an associated event handler calls the LoadGradient function. 

// Handler for color selection changed
void SDKSample::Drawing::Scenario3::ColorSelectionChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs e)
{
	LoadGradient();
}

// Hanlder for color offset slider
void SDKSample::Drawing::Scenario3::ColorOffsetValueChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::Primitives::RangeBaseValueChangedEventArgs e)
{
	LoadGradient();
}

// Load the gradient
void SDKSample::Drawing::Scenario3::LoadGradient()
{
	auto Gradient = ref new LinearGradientBrush();
	Point p;
	// Set start point 
	p.X = 0;
	p.Y = 0;
	Gradient->StartPoint = p;

	// Set end point
	p.X = 1;
	p.Y = 1;
	Gradient->EndPoint = p;
	
	// Set up Color1 
	auto Color1 = ref new GradientStop();
	Color1->Color = rootPage->ConvertIndexToColor(Color1Selection->SelectedIndex);
	Color1->Offset = Color1OffsetSelection->Value / 100.0;
	
	// Set up Color2
	auto Color2 = ref new GradientStop();
	Color2->Color = rootPage->ConvertIndexToColor(Color2Selection->SelectedIndex);
	Color2->Offset = Color2OffsetSelection->Value / 100.0;
	
	// Set up Color3
	auto Color3 = ref new GradientStop();
	Color3->Color = rootPage->ConvertIndexToColor(Color3Selection->SelectedIndex);
	Color3->Offset = Color3OffsetSelection->Value / 100.0;
	
	// Apply gradient
	Gradient->GradientStops->Append(Color1);
	Gradient->GradientStops->Append(Color2);
	Gradient->GradientStops->Append(Color3);
	Scenario3Rectangle->Fill = Gradient;
}