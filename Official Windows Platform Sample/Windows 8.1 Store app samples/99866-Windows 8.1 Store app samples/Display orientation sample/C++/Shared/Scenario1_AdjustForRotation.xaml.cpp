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
// Scenario1_AdjustForRotation.xaml.cpp
// Implementation of the Scenario1 class
//

#include "pch.h"
#include "AppUIConstants.h"
#include "Scenario1_AdjustForRotation.xaml.h"

#define _USE_MATH_DEFINES
#include <math.h>

using namespace SDKSample::DisplayOrientation;

using namespace Windows::Devices::Sensors;
using namespace Windows::Foundation;
using namespace Windows::Graphics::Display;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario1::Scenario1()
{
    InitializeComponent();

    m_rotationAngle = 0;
    m_accelerometer = Accelerometer::GetDefault();
}

void Scenario1::OnNavigatedTo(NavigationEventArgs^ e)
{
    deviceRotation->Text = m_rotationAngle.ToString();

    if (m_accelerometer != nullptr)
    {
        m_readingChangedEventToken = m_accelerometer->ReadingChanged::add(
            ref new TypedEventHandler<Accelerometer^, AccelerometerReadingChangedEventArgs^>
            (this, &Scenario1::CalculateDeviceRotation)
            );
    }
}

void Scenario1::OnNavigatedFrom(NavigationEventArgs^ e)
{
    // If the navigation is external to the app don't deregister the accelerometer.
    // This can occur on Phone when suspending the app.
    if (e->NavigationMode == NavigationMode::Forward && e->Uri == nullptr)
    {
        return;
    }

    if (m_accelerometer != nullptr)
    {
        m_accelerometer->ReadingChanged::remove(m_readingChangedEventToken);
    }
}

void Scenario1::CalculateDeviceRotation(Accelerometer^ sender, AccelerometerReadingChangedEventArgs^ args)
{
    // Compute the difference, in degrees, between the device's orientation and the up direction.
    // We only take into account the X and Y dimensions, i.e. device screen is perpendicular to the ground.
    m_rotationAngle = Constants::UIAngleOffset -
        atan2(args->Reading->AccelerationY, args->Reading->AccelerationX) * 180.0 / M_PI;

    // Ensure that the range of the value is within [0, 360).
    if (m_rotationAngle >= 360)
    {
        m_rotationAngle -= 360;
    }

    // Update the UI with the new value.
    Dispatcher->RunAsync(
        CoreDispatcherPriority::Normal,
        ref new DispatchedHandler(
            [this]()
            {
                deviceRotation->Text = floor(m_rotationAngle).ToString();
                UpdateArrowForRotation();
            })
        );
}

/// <summary>
/// Rotate the UI arrow image to point up, adjusting for the accelerometer and screen rotation.
/// </summary>
void Scenario1::UpdateArrowForRotation()
{
    auto screenRotation = 0;

    // Adjust the UI steering angle to account for screen rotation.
    switch (DisplayInformation::GetForCurrentView()->CurrentOrientation)
    {
        case DisplayOrientations::Landscape:
            screenRotation = 0;
            break;

        case DisplayOrientations::Portrait:
            screenRotation = 270;
            break;

        case DisplayOrientations::LandscapeFlipped:
            screenRotation = 180;
            break;

        case DisplayOrientations::PortraitFlipped:
            screenRotation = 90;
            break;

        default:
            screenRotation = 0;
            break;
    }

    double steeringAngle = m_rotationAngle - screenRotation;

    // Ensure the steering angle is positive.
    if (steeringAngle < 0)
    {
        steeringAngle += 360;
    }

    // Update the UI based on steering action.
    RotateTransform^ transform = ref new RotateTransform();
    transform->Angle = steeringAngle;
    transform->CenterX = scenario1Image->ActualWidth / 2;
    transform->CenterY = scenario1Image->ActualHeight / 2;
    scenario1Image->RenderTransform = transform;
}
