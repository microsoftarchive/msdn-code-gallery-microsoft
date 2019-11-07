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

using namespace SDKSample::DisplayOrientation;

using namespace Platform;
using namespace Windows::Devices::Sensors;
using namespace Windows::Foundation;
using namespace Windows::Graphics::Display;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

const double toDegrees = 180.0 / M_PI;

Scenario2::Scenario2()
{
    InitializeComponent();

    rotationAngle = 0;
    accelerometer = Accelerometer::GetDefault();
}

void Scenario2::OnNavigatedTo(NavigationEventArgs^ e)
{
    deviceRotation->Text = rotationAngle.ToString();

    if (accelerometer != nullptr)
    {
        readingChangedEventToken = accelerometer->ReadingChanged::add(
            ref new TypedEventHandler<Accelerometer^, AccelerometerReadingChangedEventArgs^>(this, &Scenario2::CalculateDeviceRotation)
            );
    }

    if (DisplayProperties::AutoRotationPreferences == DisplayOrientations::None)
    {
        LockButton->Content = "Lock";
    }
    else
    {
        LockButton->Content = "Unlock";
    }
}

void Scenario2::OnNavigatedFrom(NavigationEventArgs^ e)
{
    if (accelerometer != nullptr)
    {
        accelerometer->ReadingChanged::remove(readingChangedEventToken);
    }
}

void Scenario2::Scenario2Button_Click(Object^ sender, RoutedEventArgs^ e)
{
    if (DisplayProperties::AutoRotationPreferences == DisplayOrientations::None)
    {
        // since there is no preference currently set, get the current screen orientation and set it as the preference
        DisplayProperties::AutoRotationPreferences = DisplayProperties::CurrentOrientation;
        LockButton->Content = "Unlock";
    }
    else
    {
        // something is already set, so reset to no preference
        DisplayProperties::AutoRotationPreferences = DisplayOrientations::None;
        LockButton->Content = "Lock";
    }
}

void Scenario2::CalculateDeviceRotation(Accelerometer^ sender, AccelerometerReadingChangedEventArgs^ args)
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

void Scenario2::UpdateArrowForRotation()
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
                transform->CenterX = scenario2Image->ActualWidth / 2;
                transform->CenterY = scenario2Image->ActualHeight / 2;

                scenario2Image->RenderTransform = transform;
            })
        );
}
