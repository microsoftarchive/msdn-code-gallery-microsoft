// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// LaunchFileInput.xaml.cpp
// Implementation of the LaunchFileInput class
//

#include "pch.h"
#include "LaunchFileInput.xaml.h"

using namespace AssociationLaunching;

using namespace concurrency;
using namespace CppSamplesUtils;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Graphics::Display;
using namespace Windows::Storage;
using namespace Windows::UI::ViewManagement;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;

Platform::String^ LaunchFileInput::fileToLaunch = "Icon.Targetsize-256.png";

LaunchFileInput::LaunchFileInput()
{
    InitializeComponent();
}

LaunchFileInput::~LaunchFileInput()
{
}

#pragma region Template-Related Code - Do not remove
void LaunchFileInput::OnNavigatedTo(NavigationEventArgs^ e)
{
    // Get a pointer to our main page.
    rootPage = dynamic_cast<MainPage^>(e->Parameter);
}

void LaunchFileInput::OnNavigatedFrom(NavigationEventArgs^ e)
{
}

#pragma endregion

void LaunchFileInput::LaunchFileButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    auto installFolder = Windows::ApplicationModel::Package::Current->InstalledLocation;
    auto page = this->rootPage;

    // Get the file to launch.
    create_task(installFolder->GetFileAsync(fileToLaunch)).then([page](StorageFile^ file)
    {
        // Now launch the file.
        return Windows::System::Launcher::LaunchFileAsync(file);
    }).then([page](bool launchResult)
    {
        if (launchResult)
        {
            page->NotifyUser("File launch succeeded.", NotifyType::StatusMessage);
        }
        else
        {
            page->NotifyUser("File launch failed.", NotifyType::ErrorMessage);
        }
    });
}

void LaunchFileInput::LaunchFileWithWarningButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    auto installFolder = Windows::ApplicationModel::Package::Current->InstalledLocation;
    auto page = this->rootPage;

    // Get the file to launch.
    create_task(installFolder->GetFileAsync(fileToLaunch)).then([page](StorageFile^ file)
    {
        // Configure the warning prompt.
        auto launchOptions = ref new Windows::System::LauncherOptions();
        launchOptions->TreatAsUntrusted = true;

        // Now launch the file.
        return Windows::System::Launcher::LaunchFileAsync(file, launchOptions);
    }).then([page](bool launchResult)
    {
        if (launchResult)
        {
            page->NotifyUser("File launch succeeded.", NotifyType::StatusMessage);
        }
        else
        {
            page->NotifyUser("File launch failed.", NotifyType::ErrorMessage);
        }
    });
}

void LaunchFileInput::LaunchFileOpenWithButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    auto installFolder = Windows::ApplicationModel::Package::Current->InstalledLocation;
    auto page = this->rootPage;
    auto button = LaunchFileOpenWithButton;

    // Get the file to launch.
    create_task(installFolder->GetFileAsync(fileToLaunch)).then([page, button](StorageFile^ file)
    {
        // Calculate the position for the Open With dialog.
        Point openWithPosition = GetOpenWithPosition(button);
        auto launchOptions = ref new Windows::System::LauncherOptions();
        launchOptions->DisplayApplicationPicker = true;
        launchOptions->UI->InvocationPoint = dynamic_cast<IBox<Point>^>(PropertyValue::CreatePoint(openWithPosition));
        launchOptions->UI->PreferredPlacement = Windows::UI::Popups::Placement::Below;

        // Now launch the file.
        return Windows::System::Launcher::LaunchFileAsync(file, launchOptions);
    }).then([page](bool launchResult)
    {
        if (launchResult)
        {
            page->NotifyUser("File launch succeeded.", NotifyType::StatusMessage);
        }
        else
        {
            page->NotifyUser("File launch failed.", NotifyType::ErrorMessage);
        }
    });
}

void LaunchFileInput::PickAndLaunchFileButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    auto page = this->rootPage;

    // Get the file to launch via the picker.
    // To use the picker, the sample must not be snapped.
    if (Windows::UI::ViewManagement::ApplicationView::Value == Windows::UI::ViewManagement::ApplicationViewState::Snapped)
    {
        if (!Windows::UI::ViewManagement::ApplicationView::TryUnsnap())
        {
            page->NotifyUser("Unable to unsnap the sample.", NotifyType::ErrorMessage);
            return;
        }
    }

    auto openPicker = ref new Windows::Storage::Pickers::FileOpenPicker();
    openPicker->FileTypeFilter->Append("*");

    create_task(openPicker->PickSingleFileAsync()).then([page](StorageFile^ file)
    {
        if (file)
        {
            // Now launch the file.
            task<bool>(Windows::System::Launcher::LaunchFileAsync(file)).then([page](bool launchResult)
            {
                if (launchResult)
                {
                    page->NotifyUser("File launch succeeded.", NotifyType::StatusMessage);
                }
                else
                {
                    page->NotifyUser("File launch failed.", NotifyType::ErrorMessage);
                }
            });
        }
        else
        {
            page->NotifyUser("No file was picked.", NotifyType::ErrorMessage);
        }
    });
}

// The Open With dialog will be displayed just under the element that triggered them.
// An alternative to using the point is to set the rect of the bounding element.
Point LaunchFileInput::GetOpenWithPosition(FrameworkElement^ element)
{
    Windows::UI::Xaml::Media::GeneralTransform^ buttonTransform = element->TransformToVisual(nullptr);
    Point origin(0, 0);

    Point desiredLocation = buttonTransform->TransformPoint(origin);
    desiredLocation.Y = desiredLocation.Y + safe_cast<float>(element->ActualHeight);

    return desiredLocation;
}