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

using namespace SDKSample::DisplayOrientation;

using namespace Windows::Devices::Sensors;
using namespace Windows::Foundation;
using namespace Windows::Graphics::Display;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

const double toDegrees = 180.0 / M_PI;

Scenario1::Scenario1()
{
    InitializeComponent();

    rotationAngle = 0;
    accelerometer = Accelerometer::GetDefault();
}

void Scenario1::OnNavigatedTo(NavigationEventArgs^ e)
{
    deviceRotation->Text = rotationAngle.ToString();

    if (accelerometer != nullptr)
    {
        readingChangedEventToken = accelerometer->ReadingChanged::add(
            ref new TypedEventHandler<Accelerometer^, AccelerometerReadingChangedEventArgs^>(this, &Scenario1::CalculateDeviceRotation)
            );
    }
}

void Scenario1::OnNavigatedFrom(NavigationEventArgs^ e)
{
    if (accelerometer != nullptr)
    {
        accelerometer->ReadingChanged::remove(readingChangedEventToken);
    }
}

void Scenario1::CalculateDeviceRotation(Accelerometer^ sender, AccelerometerReadingChangedEventArgs^ args)
{
    // Compute the rotation angle based on the accelerometer's position
    auto angle = atan2(args->Reading->AccelerationY, args->Reading->AccelerationX) * toDegrees;

    // Since our arrow points upwards insted of the right, we rotate the coordinate system by 90 degrees
    angle += 90;

    // Ensure that the range of the value is between [0, 360)
    if (angle < 0)
    {
        angle += 360;
    }

    rotationAngle = angle;

    // Update the UI with the new value
    Dispatcher->RunAsync(
        CoreDispatcherPriority::Normal,
        ref new DispatchedHandler(
            [this]()
            {
                deviceRotation->Text = rotationAngle.ToString();
            })
        );

    UpdateArrowForRotation();
}

void Scenario1::UpdateArrowForRotation()
{
    // Obtain current rotation taking into account a Landscape first or a Portrait first device
    auto screenRotation = 0;

    // Native orientation can only be Landscape or Portrait
    if (DisplayProperties::NativeOrientation == DisplayOrientations::Landscape)
    {
        switch (DisplayProperties::CurrentOrientation)
        {
            case DisplayOrientations::Landscape:
                screenRotation = 0;
                break;

            case DisplayOrientations::Portrait:
                screenRotation = 90;
                break;

            case DisplayOrientations::LandscapeFlipped:
                screenRotation = 180;
                break;

            case DisplayOrientations::PortraitFlipped:
                screenRotation = 270;
                break;

            default:
                screenRotation = 0;
                break;
        }
    }
    else
    {
        switch (DisplayProperties::CurrentOrientation)
        {
            case DisplayOrientations::Landscape:
                screenRotation = 270;
                break;

            case DisplayOrientations::Portrait:
                screenRotation = 0;
                break;

            case DisplayOrientations::LandscapeFlipped:
                screenRotation = 90;
                break;

            case DisplayOrientations::PortraitFlipped:
                screenRotation = 180;
                break;

            default:
                screenRotation = 270;
                break;
        }
    }

    double steeringAngle = rotationAngle - screenRotation;

    // Keep the steering angle positive
    if (steeringAngle < 0)
    {
        steeringAngle += 360;
    }

    // Update the UI based on steering action
    Dispatcher->RunAsync(
        CoreDispatcherPriority::Normal,
        ref new DispatchedHandler(
            [this, steeringAngle]()
            {
                RotateTransform^ transform = ref new RotateTransform();
                transform->Angle = -steeringAngle;
                transform->CenterX = scenario1Image->ActualWidth / 2;
                transform->CenterY = scenario1Image->ActualHeight / 2;

                scenario1Image->RenderTransform = transform;
            })
        );
}
