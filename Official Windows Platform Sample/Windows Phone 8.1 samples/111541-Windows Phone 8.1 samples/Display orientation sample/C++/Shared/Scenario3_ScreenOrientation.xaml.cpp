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
// Scenario3_ScreenOrientation.xaml.cpp
// Implementation of the Scenario3 class
//

#include "pch.h"
#include "Scenario3_ScreenOrientation.xaml.h"

using namespace SDKSample::DisplayOrientation;

using namespace Windows::Foundation;
using namespace Windows::Graphics::Display;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario3::Scenario3()
{
    InitializeComponent();
}

void Scenario3::OnNavigatedTo(NavigationEventArgs^ e)
{
    m_orientationChangedEventToken = DisplayInformation::GetForCurrentView()->OrientationChanged::add(
        ref new TypedEventHandler<DisplayInformation^, Platform::Object^>(this, &Scenario3::OnOrientationChanged)
        );

    TransitionStoryboardState();
}

void Scenario3::OnNavigatedFrom(NavigationEventArgs^ e)
{
    // If the navigation is external to the app don't deregister the accelerometer.
    // This can occur on Phone when suspending the app.
    if (e->NavigationMode == NavigationMode::Forward && e->Uri == nullptr)
    {
        return;
    }

    DisplayInformation::GetForCurrentView()->OrientationChanged::remove(m_orientationChangedEventToken);
}


void Scenario3::OnOrientationChanged(_In_ DisplayInformation^ sender, _In_ Platform::Object^ args)
{
    TransitionStoryboardState();
}

/// <summary>
/// Invoked to determine the name of the visual state that corresponds to an application
/// display orientation.
/// </summary>
void Scenario3::TransitionStoryboardState()
{
    Platform::String^ displayOrientation;

    switch (DisplayInformation::GetForCurrentView()->CurrentOrientation)
    {
    case DisplayOrientations::Portrait:
    case DisplayOrientations::PortraitFlipped:
        displayOrientation = "Portrait";
        break;

    case DisplayOrientations::Landscape:
    case DisplayOrientations::LandscapeFlipped:
    default:
        displayOrientation = "Landscape";
        break;
    }

    VisualStateManager::GoToState(this, displayOrientation, false);
}
