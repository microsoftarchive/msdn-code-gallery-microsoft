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

using namespace SDKSample::SimpleOrientationSensorCPP;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Devices::Sensors;
using namespace Windows::Foundation;
using namespace Windows::UI::Core;
using namespace Platform;

Scenario2::Scenario2() : rootPage(MainPage::Current)
{
    InitializeComponent();

    sensor = SimpleOrientationSensor::GetDefault();
    if (sensor == nullptr)
    {
        rootPage->NotifyUser("No simple orientation sensor found", NotifyType::ErrorMessage);
    }
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario2::OnNavigatedTo(NavigationEventArgs^ e)
{
}

void Scenario2::DisplayOrientation(SimpleOrientation orientation)
{
    switch (orientation)
    {
        case SimpleOrientation::NotRotated:
            ScenarioOutput_Orientation->Text = "Not Rotated";
            break;
        case SimpleOrientation::Rotated90DegreesCounterclockwise:
            ScenarioOutput_Orientation->Text = "Rotated 90 Degrees Counterclockwise";
            break;
        case SimpleOrientation::Rotated180DegreesCounterclockwise:
            ScenarioOutput_Orientation->Text = "Rotated 180 Degrees Counterclockwise";
            break;
        case SimpleOrientation::Rotated270DegreesCounterclockwise:
            ScenarioOutput_Orientation->Text = "Rotated 270 Degrees Counterclockwise";
            break;
        case SimpleOrientation::Faceup:
            ScenarioOutput_Orientation->Text = "Faceup";
            break;
        case SimpleOrientation::Facedown:
            ScenarioOutput_Orientation->Text = "Facedown";
            break;
        default:
            ScenarioOutput_Orientation->Text = "Unknown orientation";
            break;
    }
}

void Scenario2::ScenarioGet(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if (sensor != nullptr)
    {
        DisplayOrientation(sensor->GetCurrentOrientation());
    }
    else
    {
        rootPage->NotifyUser("No simple orientation sensor found", NotifyType::ErrorMessage);
    }
}
