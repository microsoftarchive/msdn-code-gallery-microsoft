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

using namespace SDKSample::SnapSample;

using namespace Platform;
using namespace Windows::UI::ViewManagement;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario1::Scenario1()
{
    InitializeComponent();

    // Register for the window resize handler
    _layoutHandlerToken = Window::Current->SizeChanged += ref new Windows::UI::Xaml::WindowSizeChangedEventHandler(this, &Scenario1::Page_SizeChanged);

    // Bind a click event to the unsnap button
    UnsnapButton->Click += ref new RoutedEventHandler(this, &Scenario1::UnsnapButton_Click);

    // Update view on load (because you can be launched into any view state)
    UpdateForViewState();
}

String^ Scenario1::ConvertViewState(ApplicationViewState state)
{
    // Assign text according to view state
    switch (state)
    {
    case ApplicationViewState::Filled:
        UnsnapButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
        return "This app is in filled state.";
    case ApplicationViewState::FullScreenLandscape:
        UnsnapButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
        return "This app is full screen landscape.";
    case ApplicationViewState::FullScreenPortrait:
        UnsnapButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
        return "This app is full screen portrait.";
    case ApplicationViewState::Snapped:
        UnsnapButton->Visibility = Windows::UI::Xaml::Visibility::Visible;
        return "This app is snapped.";
    }
    return "";
}

void Scenario1::UpdateForViewState()
{
    // Query for the current view state
    ApplicationViewState state = ApplicationView::Value;
    // Display text
    OutputText->Text = ConvertViewState(state);
}

#pragma region Event handlers

void Scenario1::Page_SizeChanged(Object^ sender, Windows::UI::Core::WindowSizeChangedEventArgs^ e)
{
    // Update view for the new window size
    UpdateForViewState();
}

void Scenario1::UnsnapButton_Click(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    bool fUnsnapped = ApplicationView::TryUnsnap();

    if (!fUnsnapped)
    {
        Scenario1::OutputText->Text = "Programmatic unsnap did not work.";
    }
}
