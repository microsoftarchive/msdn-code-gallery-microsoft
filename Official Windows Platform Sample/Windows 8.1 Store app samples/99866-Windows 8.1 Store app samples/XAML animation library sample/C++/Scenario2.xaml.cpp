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

using namespace SDKSample::Animation;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario2::Scenario2()
{
    InitializeComponent();
	isScenario2StoryboardRunning = false;

	// Add click event handler to toggle button
	Scenario2ToggleStoryboard->Click += ref new RoutedEventHandler(this, &Scenario2::Scenario2ToggleStoryboard_Click);
	
}


// Invoked when this page is about to be displayed in a Frame.
void Scenario2::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}


// Handler for button that toggles the scenario
void SDKSample::Animation::Scenario2::Scenario2ToggleStoryboard_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	// Toggle the storyboards to stop or begin them depending on the current state
	if (isScenario2StoryboardRunning)
	{
	    ToggleScenario2(false);
	}
	else
	{
	    ToggleScenario2(true);
	}
}


// Toggle the scenario
void SDKSample::Animation::Scenario2::ToggleScenario2(bool startScenario)
{
	if (!startScenario)
	{
	    // Stop the storyboards
	    Scenario2ContinuousStoryboard->Stop();
	    Scenario2KeyFrameStoryboard->Stop();
	    Scenario2ToggleStoryboard->Content = "Begin storyboards";
	}
	else
	{
	    // Start the storyboards
	    Scenario2ContinuousStoryboard->Begin();
	    Scenario2KeyFrameStoryboard->Begin();
	    Scenario2ToggleStoryboard->Content = "Stop storyboards";
	}
	isScenario2StoryboardRunning = startScenario;
}