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
// Scenario1_TrackPosition.xaml.cpp
// Implementation of the Scenario1 class
//

#include "pch.h"
#include "Scenario1_TrackPosition.xaml.h"

using namespace SDKSample::GeolocationCPP;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Devices::Geolocation;
using namespace Windows::Foundation;
using namespace Windows::UI::Core;
using namespace Platform;

Scenario1::Scenario1() : rootPage(MainPage::Current)
{
    InitializeComponent();

    geolocator = ref new Geolocator();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario1::OnNavigatedTo(NavigationEventArgs^ e)
{
    StartTrackingButton->IsEnabled = true;
    StopTrackingButton->IsEnabled = false;
}

/// <summary>
/// Invoked when this page is no longer displayed.
/// </summary>
/// <param name="e"></param>
void Scenario1::OnNavigatedFrom(NavigationEventArgs^ e)
{
    if (StopTrackingButton->IsEnabled)
    {
        geolocator->PositionChanged::remove(positionToken);
        geolocator->StatusChanged::remove(statusToken);
    }
}

void Scenario1::OnPositionChanged(Geolocator^ sender, PositionChangedEventArgs^ e)
{
    // We need to dispatch to the UI thread to display the output
    Dispatcher->RunAsync(
        CoreDispatcherPriority::Normal,
        ref new DispatchedHandler(
            [this, e]()
            {
                auto coordinate = e->Position->Coordinate;

                rootPage->NotifyUser("Updated", NotifyType::StatusMessage);

                ScenarioOutput_Latitude->Text = coordinate->Latitude.ToString();
                ScenarioOutput_Longitude->Text = coordinate->Longitude.ToString();
                ScenarioOutput_Accuracy->Text = coordinate->Accuracy.ToString();
            },
            CallbackContext::Any
            )
        );
}

void Scenario1::OnStatusChanged(Geolocator^ sender, StatusChangedEventArgs^ e)
{
    // We need to dispatch to the UI thread to display the output
    Dispatcher->RunAsync(
        CoreDispatcherPriority::Normal,
        ref new DispatchedHandler(
            [this, e]()
            {
                switch (e->Status)
                {
                case Windows::Devices::Geolocation::PositionStatus::Ready :
                    // Location platform is providing valid data.
                    ScenarioOutput_Status->Text = "Ready";
                    break;

                case Windows::Devices::Geolocation::PositionStatus::Initializing :
                    // Location platform is acquiring a fix. It may or may not have data. Or the data may be less accurate.
                    ScenarioOutput_Status->Text = "Initializing";
                    break;

                case Windows::Devices::Geolocation::PositionStatus::NoData :
                    // Location platform could not obtain location data.
                    ScenarioOutput_Status->Text = "No data";
                    break;

                case Windows::Devices::Geolocation::PositionStatus::Disabled :
                    // The permission to access location data is denied by the user or other policies.
                    ScenarioOutput_Status->Text = "Disabled";

                    // Clear cached location data if any
                    ScenarioOutput_Latitude->Text = "No data";
                    ScenarioOutput_Longitude->Text = "No data";
                    ScenarioOutput_Accuracy->Text = "No data";
                    break;

                case Windows::Devices::Geolocation::PositionStatus::NotInitialized :
                    // The location platform is not initialized. This indicates that the application has not made a request for location data.
                    ScenarioOutput_Status->Text = "Not initialized";
                    break;

                case Windows::Devices::Geolocation::PositionStatus::NotAvailable :
                    // The location platform is not available on this version of the OS.
                    ScenarioOutput_Status->Text = "Not available";
                    break;

                default:
                    ScenarioOutput_Status->Text = "Unknown";
                    break;
                }
            },
            CallbackContext::Any
            )
        );
}

void Scenario1::StartTracking(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    rootPage->NotifyUser("Waiting for update...", NotifyType::StatusMessage);

    positionToken = geolocator->PositionChanged::add(ref new TypedEventHandler<Geolocator^, PositionChangedEventArgs^>(this, &Scenario1::OnPositionChanged));
    statusToken = geolocator->StatusChanged::add(ref new TypedEventHandler<Geolocator^, StatusChangedEventArgs^>(this, &Scenario1::OnStatusChanged));

    StartTrackingButton->IsEnabled = false;
    StopTrackingButton->IsEnabled = true;
}

void Scenario1::StopTracking(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    geolocator->PositionChanged::remove(positionToken);
    geolocator->StatusChanged::remove(statusToken);

    StartTrackingButton->IsEnabled = true;
    StopTrackingButton->IsEnabled = false;
}
