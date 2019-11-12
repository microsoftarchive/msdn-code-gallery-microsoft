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

using namespace SDKSample;
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

#if (WINAPI_FAMILY == WINAPI_FAMILY_PHONE_APP)
    // Desired Accuracy needs to be set
    // before polling for desired accuracy.
    geolocator->DesiredAccuracyInMeters = static_cast<unsigned int>(10);
#endif
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
    DesiredAccuracyInMeters->IsEnabled = false;
    SetDesiredAccuracyInMetersButton->IsEnabled = false;
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

            DesiredAccuracyInMeters->IsEnabled = true;

            ScenarioOutput_Latitude->Text = pos->Coordinate->Point->Position.Latitude.ToString();
            ScenarioOutput_Longitude->Text = pos->Coordinate->Point->Position.Longitude.ToString();
            ScenarioOutput_Accuracy->Text = pos->Coordinate->Accuracy.ToString();
            ScenarioOutput_Source->Text = pos->Coordinate->PositionSource.ToString();

            if (pos->Coordinate->PositionSource == PositionSource::Satellite)
            {
                // show labels and satellite data
                Label_PosPrecision->Opacity = 1;
                ScenarioOutput_PosPrecision->Opacity = 1;
                ScenarioOutput_PosPrecision->Text = pos->Coordinate->SatelliteData->PositionDilutionOfPrecision->ToString();
                Label_HorzPrecision->Opacity = 1;
                ScenarioOutput_HorzPrecision->Opacity = 1;
                ScenarioOutput_HorzPrecision->Text = pos->Coordinate->SatelliteData->HorizontalDilutionOfPrecision->ToString();
                Label_VertPrecision->Opacity = 1;
                ScenarioOutput_VertPrecision->Opacity = 1;
                ScenarioOutput_VertPrecision->Text = pos->Coordinate->SatelliteData->VerticalDilutionOfPrecision->ToString();
            }
            else
            {
                // hide labels and satellite data
                Label_PosPrecision->Opacity = 0;
                ScenarioOutput_PosPrecision->Opacity = 0;
                Label_HorzPrecision->Opacity = 0;
                ScenarioOutput_HorzPrecision->Opacity = 0;
                Label_VertPrecision->Opacity = 0;
                ScenarioOutput_VertPrecision->Opacity = 0;
            }

            ScenarioOutput_DesiredAccuracyInMeters->Text = geolocator->DesiredAccuracyInMeters->ToString();
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
        catch (Exception^ ex)
        {
#if (WINAPI_FAMILY == WINAPI_FAMILY_PC_APP)
            // If there are no location sensors GetGeopositionAsync()
            // will timeout -- that is acceptable.

            if (ex->HResult == HRESULT_FROM_WIN32(WAIT_TIMEOUT))
            {
                rootPage->NotifyUser("Operation accessing location sensors timed out. Possibly there are no location sensors.", NotifyType::StatusMessage);
            }
            else
#endif
            {
                rootPage->NotifyUser(ex->ToString(), NotifyType::ErrorMessage);
            }
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

void Scenario2::DesiredAccuracyInMeters_TextChanged(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if (DesiredAccuracyInMeters->Text->IsEmpty())
    {
        SetDesiredAccuracyInMetersButton->IsEnabled = false;
    }
    else
    {
        bool goodValue = true;

        unsigned int value = FromStringTo<unsigned int>(DesiredAccuracyInMeters->Text);

        if (0 == value)
        {
            // make sure string was '0'
            if ("0" != DesiredAccuracyInMeters->Text)
            {
                rootPage->NotifyUser("Desired Accuracy must be a number", NotifyType::StatusMessage);
                SetDesiredAccuracyInMetersButton->IsEnabled = false;
                goodValue = false;
            }
        }

        if (true == goodValue)
        {
            SetDesiredAccuracyInMetersButton->IsEnabled = true;

            // clear out status message
            rootPage->NotifyUser("", NotifyType::StatusMessage);
        }
    }
}

void Scenario2::SetDesiredAccuracyInMeters(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // convert desired accuracy string to an unsigned int and assign to geolocator
    geolocator->DesiredAccuracyInMeters = FromStringTo<unsigned int>(DesiredAccuracyInMeters->Text);

    // update get field
    ScenarioOutput_DesiredAccuracyInMeters->Text = geolocator->DesiredAccuracyInMeters->ToString();
}

