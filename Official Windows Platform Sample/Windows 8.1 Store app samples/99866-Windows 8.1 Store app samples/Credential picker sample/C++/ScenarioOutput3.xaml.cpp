// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// Scenario3Output.xaml.cpp
// Implementation of the Scenario3Output class
//

#include "pch.h"
#include "ScenarioOutput3.xaml.h"
#include "MainPage.xaml.h"

using namespace CredentialPickerCPP;

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
using namespace Windows::UI::ViewManagement;

ScenarioOutput3::ScenarioOutput3()
{
    InitializeComponent();

    Loaded += ref new RoutedEventHandler(this, &ScenarioOutput3::Page_Loaded);

    Window::Current->SizeChanged += ref new Windows::UI::Xaml::WindowSizeChangedEventHandler(this, &ScenarioOutput3::Page_SizeChanged);
}

ScenarioOutput3::~ScenarioOutput3()
{
}

void ScenarioOutput3::Page_Loaded(Object^ sender, RoutedEventArgs^ e)
{
}

void ScenarioOutput3::Page_SizeChanged(Object^ sender, Windows::UI::Core::WindowSizeChangedEventArgs^ e)
{
    CheckLayout();
}



void ScenarioOutput3::CheckLayout()
{
    String^ visualState = this->ActualWidth < 768 ? "Below768Layout" : "DefaultLayout";
    VisualStateManager::GoToState(this, visualState, false);
}


void ScenarioOutput3::OnNavigatedTo(NavigationEventArgs^ e)
{
    // Get a pointer to our main page.
    rootPage = dynamic_cast<MainPage^>(e->Parameter);

    // We want to be notified with the OutputFrame is loaded so we can get to the content.
    _frameLoadedToken = rootPage->InputFrameLoaded += ref new Windows::Foundation::EventHandler<Platform::Object^>(this, &ScenarioOutput3::rootPage_InputFrameLoaded);
}

void ScenarioOutput3::OnNavigatedFrom(NavigationEventArgs^ e)
{
    rootPage->InputFrameLoaded -= _frameLoadedToken;
}


void ScenarioOutput3::rootPage_InputFrameLoaded(Object^ sender, Object^ e)
{
    // Get a pointer to the content within the OutputFrame.
    Page^ inputFrame = dynamic_cast<Page^>(rootPage->InputFrame->Content);

    // Go find the elements that we need for this scenario
    // ex: flipView1 = dynamic_cast<FlipView^>(inputFrame->FindName("FlipView1"));
}

#pragma endregion
