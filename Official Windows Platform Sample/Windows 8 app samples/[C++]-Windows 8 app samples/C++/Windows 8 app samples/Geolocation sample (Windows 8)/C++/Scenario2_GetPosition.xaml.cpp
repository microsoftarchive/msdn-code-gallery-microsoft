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
// Scenario2_GetPosition.xaml.cpp
// Implementation of the Scenario2 class
//

#include "pch.h"
#include "Scenario2_GetPosition.xaml.h"

using namespace SDKSample::GeolocationCPP;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Devices::Geolocation;
using namespace Windows::Foundation;
using namespace Windows::UI::Core;
using namespace Platform;
using namespace concurrency;

Scenario2::Scenario2() : rootPage(MainPage::Current)
{
    InitializeComponent();

    geolocator = ref new Geolocator();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario2::OnNavigatedTo(NavigationEventArgs^ e)
{
    GetGeolocationButton->IsEnabled = true;
    CancelGetGeolocationButton->IsEnabled = false;
}

/// <summary>
/// Invoked when this page is no longer displayed.
/// </summary>
/// <param name="e"></param>
void Scenario2::OnNavigatedFrom(NavigationEventArgs^ e)
{
    if (CancelGetGeolocationButton->IsEnabled)
    {
        geopositionTaskTokenSource.cancel();
    }
}

void Scenario2::GetGeolocation(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    rootPage->NotifyUser("Waiting for update...", NotifyType::StatusMessage);

    GetGeolocationButton->IsEnabled = false;
    CancelGetGeolocationButton->IsEnabled = true;

    task<Geoposition^> geopositionTask(geolocator->GetGeopositionAsync(), geopositionTaskTokenSource.get_token());
    geopositionTask.then([this](task<Geoposition^> getPosTask)
    {
        try
        {
            // Get will throw an exception if the task was canceled or failed with an error
            Geoposition^ pos = getPosTask.get();

            rootPage->NotifyUser("Updated", NotifyType::StatusMessage);

            ScenarioOutput_Latitude->Text = pos->Coordinate->Latitude.ToString();
            ScenarioOutput_Longitude->Text = pos->Coordinate->Longitude.ToString();
            ScenarioOutput_Accuracy->Text = pos->Coordinate->Accuracy.ToString();
        }
        catch (AccessDeniedException^)
        {
            rootPage->NotifyUser("Disabled", NotifyType::ErrorMessage);

            // Clear cached location data if any
            ScenarioOutput_Latitude->Text = "No data";
            ScenarioOutput_Longitude->Text = "No data";
            ScenarioOutput_Accuracy->Text = "No data";
        }
        catch (task_canceled&)
        {
            rootPage->NotifyUser("Operation canceled", NotifyType::StatusMessage);
        }

        GetGeolocationButton->IsEnabled = true;
        CancelGetGeolocationButton->IsEnabled = false;
    });
}

void Scenario2::CancelGetGeolocation(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    geopositionTaskTokenSource.cancel();

    GetGeolocationButton->IsEnabled = true;
    CancelGetGeolocationButton->IsEnabled = false;
}
