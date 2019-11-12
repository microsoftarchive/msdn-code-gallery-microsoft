// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// Scenario1Input.xaml.cpp
// Implementation of the Scenario1Input class
//

#include "pch.h"
#include "ScenarioInput1.xaml.h"

using namespace KeyboardEventsSampleCPP;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Graphics::Display;
using namespace Windows::UI::ViewManagement;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;

ScenarioInput1::ScenarioInput1()
{
    InitializeComponent();
}

ScenarioInput1::~ScenarioInput1()
{
}

#pragma region Template-Related Code - Do not remove
void ScenarioInput1::OnNavigatedTo(NavigationEventArgs^ e)
{
	// Get a pointer to our main page.
	rootPage = dynamic_cast<MainPage^>(e->Parameter);

	// We want to be notified with the OutputFrame is loaded so we can get to the content.
	_frameLoadedToken = rootPage->OutputFrameLoaded += ref new Windows::Foundation::EventHandler<Platform::Object^>(this, &ScenarioInput1::rootPage_OutputFrameLoaded);
}

void ScenarioInput1::OnNavigatedFrom(NavigationEventArgs^ e)
{
    rootPage->OutputFrameLoaded -= _frameLoadedToken;
}

#pragma endregion

void ScenarioInput1::rootPage_OutputFrameLoaded(Object^ sender, Object^ e)
{
    // Get a pointer to the content within the OutputFrame.
    Page^ inputFrame = dynamic_cast<Page^>(rootPage->OutputFrame->Content);
}

void ScenarioInput1::Scenario1Open_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    TypeName pageType = { "KeyboardEventsSampleCPP.KeyboardPage", TypeKind::Custom };
    rootPage->Frame->Navigate(pageType);
}


