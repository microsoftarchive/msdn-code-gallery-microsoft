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

using namespace SDKSample::Drawing;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Media;



Scenario2::Scenario2()
{
    InitializeComponent();
	FillSelection->SelectedIndex = 0;
	FillSelection->SelectionChanged += ref new SelectionChangedEventHandler(this, &Scenario2::FillSelection_SelectionChanged);
	StrokeThicknessSelection->ValueChanged += ref new RangeBaseValueChangedEventHandler(this, &Scenario2::StrokeWidthSelection_ValueChanged);
}

// Invoked when this page is about to be displayed in a Frame.
void Scenario2::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}


// Handler for fill selection changed
void SDKSample::Drawing::Scenario2::FillSelection_SelectionChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e)
{
	Scenario2Rectangle->Fill = ref new SolidColorBrush(rootPage->ConvertIndexToColor(FillSelection->SelectedIndex));
}


// Handler for stroke width slider
void SDKSample::Drawing::Scenario2::StrokeWidthSelection_ValueChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::Primitives::RangeBaseValueChangedEventArgs^ e)
{
	Scenario2Rectangle->StrokeThickness = StrokeThicknessSelection->Value;
}
