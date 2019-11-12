// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// LaunchUriInput.xaml.cpp
// Implementation of the LaunchUriInput class
//

#include "pch.h"
#include "LaunchUriInput.xaml.h"

using namespace AssociationLaunching;

using namespace concurrency;
using namespace CppSamplesUtils;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Graphics::Display;
using namespace Windows::UI::ViewManagement;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;

Platform::String^ LaunchUriInput::uriToLaunch = "http://www.bing.com";

LaunchUriInput::LaunchUriInput()
{
    InitializeComponent();
}

LaunchUriInput::~LaunchUriInput()
{
}

#pragma region Template-Related Code - Do not remove
void LaunchUriInput::OnNavigatedTo(NavigationEventArgs^ e)
{
    // Get a pointer to our main page.
    rootPage = dynamic_cast<MainPage^>(e->Parameter);
}

void LaunchUriInput::OnNavigatedFrom(NavigationEventArgs^ e)
{
}

#pragma endregion

void LaunchUriInput::LaunchUriButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    auto page = this->rootPage;

    // Create the URI from a string.
    auto uri = ref new Uri(uriToLaunch);

    // Launch the URI.
    create_task(Windows::System::Launcher::LaunchUriAsync(uri)).then([page](bool launchResult)
    {
        if (launchResult)
        {
            page->NotifyUser("Uri launch succeeded.", NotifyType::StatusMessage);
        }
        else
        {
            page->NotifyUser("Uri launch failed.", NotifyType::ErrorMessage);
        }
    });
}

void LaunchUriInput::LaunchUriWithWarningButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    auto page = this->rootPage;

    // Create the URI from a string.
    auto uri = ref new Uri(uriToLaunch);

    // Configure the warning prompt.
    auto launchOptions = ref new Windows::System::LauncherOptions();
    launchOptions->TreatAsUntrusted = true;

    // Launch the URI.
    create_task(Windows::System::Launcher::LaunchUriAsync(uri, launchOptions)).then([page](bool launchResult)
    {
        if (launchResult)
        {
            page->NotifyUser("Uri launch succeeded.", NotifyType::StatusMessage);
        }
        else
        {
            page->NotifyUser("Uri launch failed.", NotifyType::ErrorMessage);
        }
    });
}

void LaunchUriInput::LaunchUriOpenWithButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    auto page = this->rootPage;
    auto button = LaunchUriOpenWithButton;

    // Create the URI from a string.
    auto uri = ref new Uri(uriToLaunch);

    // Calulcate the position for the Open With dialog.
    Point openWithPosition = GetOpenWithPosition(button);
    auto launchOptions = ref new Windows::System::LauncherOptions();
    launchOptions->DisplayApplicationPicker = true;
    launchOptions->UI->InvocationPoint = dynamic_cast<IBox<Point>^>(PropertyValue::CreatePoint(openWithPosition));
    launchOptions->UI->PreferredPlacement = Windows::UI::Popups::Placement::Below;

    // Launch the URI.
    create_task(Windows::System::Launcher::LaunchUriAsync(uri, launchOptions)).then([page](bool launchResult)
    {
        if (launchResult)
        {
            page->NotifyUser("Uri launch succeeded.", NotifyType::StatusMessage);
        }
        else
        {
            page->NotifyUser("Uri launch failed.", NotifyType::ErrorMessage);
        }
    });
}

// The Open With dialog will be displayed just under the element that triggered them.
// An alternative to using the point is to set the rect of the bounding element.
Point LaunchUriInput::GetOpenWithPosition(FrameworkElement^ element)
{
    Windows::UI::Xaml::Media::GeneralTransform^ buttonTransform = element->TransformToVisual(nullptr);
    Point origin(0, 0);

    Point desiredLocation = buttonTransform->TransformPoint(origin);
    desiredLocation.Y = desiredLocation.Y + safe_cast<float>(element->ActualHeight);

    return desiredLocation;
}