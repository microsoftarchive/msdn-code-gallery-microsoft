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

using namespace SDKSample::SimpleOrientationSensorCPP;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Devices::Sensors;
using namespace Windows::Foundation;
using namespace Windows::UI::Core;
using namespace Platform;

Scenario1::Scenario1() : rootPage(MainPage::Current)
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
void Scenario1::OnNavigatedTo(NavigationEventArgs^ e)
{
    ScenarioEnableButton->IsEnabled = true;
    ScenarioDisableButton->IsEnabled = false;
}

/// <summary>
/// Invoked when this page is no longer displayed.
/// </summary>
/// <param name="e"></param>
void Scenario1::OnNavigatedFrom(NavigationEventArgs^ e)
{
    if (ScenarioDisableButton->IsEnabled)
    {
        Window::Current->VisibilityChanged::remove(visibilityToken);
        sensor->OrientationChanged::remove(orientationToken);
    }
}

/// <summary>
/// This is the event handler for VisibilityChanged events. You would register for these notifications
/// if handling sensor data when the app is not visible could cause unintended actions in the app.
/// </summary>
/// <param name="sender"></param>
/// <param name="e">
/// Event data that can be examined for the current visibility state.
/// </param>
void Scenario1::VisibilityChanged(Object^ sender, VisibilityChangedEventArgs^ e)
{
    // The app should watch for VisibilityChanged events to disable and re-enable sensor input as appropriate
    if (ScenarioDisableButton->IsEnabled)
    {
        if (e->Visible)
        {
            // Re-enable sensor input (no need to restore the desired reportInterval... it is restored for us upon app resume)
            orientationToken = sensor->OrientationChanged::add(ref new TypedEventHandler<SimpleOrientationSensor^, SimpleOrientationSensorOrientationChangedEventArgs^>(this, &Scenario1::OrientationChanged));
        }
        else
        {
            // Disable sensor input (no need to restore the default reportInterval... resources will be released upon app suspension)
            sensor->OrientationChanged::remove(orientationToken);
        }
    }
}

void Scenario1::DisplayOrientation(SimpleOrientation orientation)
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

void Scenario1::OrientationChanged(SimpleOrientationSensor^ sender, SimpleOrientationSensorOrientationChangedEventArgs^ e)
{
    // We need to dispatch to the UI thread to display the output
    Dispatcher->RunAsync(
        CoreDispatcherPriority::Normal,
        ref new DispatchedHandler(
            [this, e]()
            {
                DisplayOrientation(e->Orientation);
            },
            CallbackContext::Any
            )
        );
}

void Scenario1::ScenarioEnable(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if (sensor != nullptr)
    {
        visibilityToken = Window::Current->VisibilityChanged::add(ref new WindowVisibilityChangedEventHandler(this, &Scenario1::VisibilityChanged));
        orientationToken = sensor->OrientationChanged::add(ref new TypedEventHandler<SimpleOrientationSensor^, SimpleOrientationSensorOrientationChangedEventArgs^>(this, &Scenario1::OrientationChanged));

        // Display the current orientation once while waiting for the next orientation change
        DisplayOrientation(sensor->GetCurrentOrientation());

        ScenarioEnableButton->IsEnabled = false;
        ScenarioDisableButton->IsEnabled = true;
    }
    else
    {
        rootPage->NotifyUser("No simple orientation sensor found", NotifyType::ErrorMessage);
    }
}

void Scenario1::ScenarioDisable(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Window::Current->VisibilityChanged::remove(visibilityToken);
    sensor->OrientationChanged::remove(orientationToken);

    ScenarioEnableButton->IsEnabled = true;
    ScenarioDisableButton->IsEnabled = false;
}
