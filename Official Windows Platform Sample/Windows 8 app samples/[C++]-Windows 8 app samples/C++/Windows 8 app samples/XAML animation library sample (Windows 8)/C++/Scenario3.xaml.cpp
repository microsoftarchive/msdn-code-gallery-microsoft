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

using namespace SDKSample::Animation;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::UI;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Media::Animation;
using namespace Windows::UI::Xaml::Shapes;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::ViewManagement;

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

	// Set the initaial combo box values
	Scenario3EasingModeSelector->SelectedIndex = 0;
	Scenario3FunctionSelector->SelectedIndex = 0;
}

void Scenario3::Scenario3EasingFunctionChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e)
{
	// Stop the storyboard
	Scenario3Storyboard->Stop();
	
	EasingFunctionBase^ easingFunction = nullptr;
	
	// Select an easing function based on the user's selection
	auto selectedFunctionItem = (ComboBoxItem^) Scenario3FunctionSelector->SelectedItem;

	if (selectedFunctionItem != nullptr)
	{
	
		auto selectedFunction = selectedFunctionItem->Content->ToString();
	    
	        if(selectedFunction == "BounceEase")
			{
	            easingFunction = ref new BounceEase();
			}
			else if(selectedFunction == "CircleEase")
			{
	            easingFunction = ref new CircleEase();
			}
			else if(selectedFunction == "ExponentialEase")
			{
	            easingFunction = ref new ExponentialEase();
			}
			else if(selectedFunction == "PowerEase")
			{
	            easingFunction = ref new PowerEase();
			} 
			else {}
	}
	
	// If no valid easing function was specified, let the storyboard stay stopped and do not continue
	if (easingFunction == nullptr)
		return;
	
	auto selectedEasingModeItem = (ComboBoxItem^) Scenario3EasingModeSelector->SelectedItem;

	// Select an easing mode based on the user's selection, defaulting to EaseIn if no selection was given
	if (selectedEasingModeItem != nullptr)
	{
	   auto easingMode = selectedEasingModeItem->Content->ToString();
	    
	        if(easingMode == "EaseOut")
			{
	            easingFunction->EasingMode = EasingMode::EaseOut;
			}
			if(easingMode == "EaseIn")
			{
	            easingFunction->EasingMode = EasingMode::EaseIn;
			}
			if(easingMode == "EaseInOut")
			{
	            easingFunction->EasingMode = EasingMode::EaseInOut;
			}
	}
	
	// Plot a graph of the easing function
	PlotEasingFunctionGraph(easingFunction, 0.005);
	
	// Set the easing functions
	RectanglePositionAnimation->EasingFunction = easingFunction;
	GraphPositionMarkerXAnimation->EasingFunction = easingFunction;
	GraphPositionMarkerYAnimation->EasingFunction = easingFunction;
	
	// Start the storyboard
	Scenario3Storyboard->Begin();
}


// Plot an easing function graph
void Scenario3::PlotEasingFunctionGraph(EasingFunctionBase^ easingFunction, double samplingInterval)
{
    UISettings^ UserSettings = ref new UISettings();
	// Clear the element
	Graph->Children->Clear();
	
	// Initialize path
	auto path = ref new Path();
	auto pathGeometry = ref new PathGeometry();
	auto pathFigure = ref new PathFigure();

	// Set starting point
	Point  startPoint;
	startPoint.X = 0;
	startPoint.Y = 0;
	pathFigure->StartPoint = startPoint;

	auto pathSegmentCollection = ref new PathSegmentCollection();
	
	// Note that an easing function is just like a regular function that operates on doubles.
	// Here we plot the range of the easing function's output on the y-axis of a graph.
	for (double i = 0; i < 1; i += samplingInterval)
	{
	    auto x = i * GraphContainer->Width;
	    auto y = easingFunction->Ease(i) * GraphContainer->Height;
	
	    auto segment = ref new LineSegment();
		Point p;
		p.X = (float)x;
		p.Y = (float)y;
	    segment->Point = p;
	    pathSegmentCollection->Append(segment);
	}

	// Define the path parameters
	pathFigure->Segments = pathSegmentCollection;
	pathGeometry->Figures->Append(pathFigure);
	path->Data = pathGeometry;
	path->Stroke = ref new SolidColorBrush(UserSettings->UIElementColor(UIElementType::ButtonText));
	path->StrokeThickness = 1;
	
	// Append the path to the Canvas
	Graph->Children->Append(path);
}