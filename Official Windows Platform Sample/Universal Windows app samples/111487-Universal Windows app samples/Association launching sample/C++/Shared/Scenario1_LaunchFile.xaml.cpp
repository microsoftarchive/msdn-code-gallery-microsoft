//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// LaunchFile.xaml.cpp
// Implementation of the LaunchFile class
//

#include "pch.h"
#include "Scenario1_LaunchFile.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::AssociationLaunching;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::Foundation;
using namespace Windows::Storage;
using namespace Windows::UI::Xaml::Navigation;
using namespace Platform;
using namespace concurrency;

Platform::String^ LaunchFile::fileToLaunch = "Assets\\microsoft-sdk.png";

LaunchFile::LaunchFile()
{
    InitializeComponent();
}

void LaunchFile::OnNavigatedTo(NavigationEventArgs^ e)
{
#if WINAPI_FAMILY == WINAPI_FAMILY_PHONE_APP
    // Disable scenarios that are not supported in Phone.
    LaunchFileOpenWithPanel->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    LaunchFileWithWarningPanel->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    LaunchFileSplitScreenPanel->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
#endif
}

void LaunchFile::LaunchFileButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    auto installFolder = Windows::ApplicationModel::Package::Current->InstalledLocation;

    // Get the file to launch.
    create_task(installFolder->GetFileAsync(fileToLaunch)).then([](StorageFile^ file)
    {
        // Now launch the file.
        return Windows::System::Launcher::LaunchFileAsync(file);
    }).then([](bool launchResult)
    {
        if (launchResult)
        {
            MainPage::Current->NotifyUser("File launch succeeded.", NotifyType::StatusMessage);
        }
        else
        {
            MainPage::Current->NotifyUser("File launch failed.", NotifyType::ErrorMessage);
        }
    });
}

void LaunchFile::LaunchFileWithWarningButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    auto installFolder = Windows::ApplicationModel::Package::Current->InstalledLocation;

    // Get the file to launch.
    create_task(installFolder->GetFileAsync(fileToLaunch)).then([](StorageFile^ file)
    {
        // Configure the warning prompt.
        auto launchOptions = ref new Windows::System::LauncherOptions();
        launchOptions->TreatAsUntrusted = true;

        // Now launch the file.
        return Windows::System::Launcher::LaunchFileAsync(file, launchOptions);
    }).then([](bool launchResult)
    {
        if (launchResult)
        {
            MainPage::Current->NotifyUser("File launch succeeded.", NotifyType::StatusMessage);
        }
        else
        {
            MainPage::Current->NotifyUser("File launch failed.", NotifyType::ErrorMessage);
        }
    });
}

void LaunchFile::LaunchFileOpenWithButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    auto installFolder = Windows::ApplicationModel::Package::Current->InstalledLocation;
    auto button = dynamic_cast<Button^>(sender);

    // Get the file to launch.
    create_task(installFolder->GetFileAsync(fileToLaunch)).then([button](StorageFile^ file)
    {
        // Calculate the position for the Open With dialog.
        Point openWithPosition = GetOpenWithPosition(button);
        auto launchOptions = ref new Windows::System::LauncherOptions();
        launchOptions->DisplayApplicationPicker = true;
        launchOptions->UI->InvocationPoint = dynamic_cast<IBox<Point>^>(PropertyValue::CreatePoint(openWithPosition));
        launchOptions->UI->PreferredPlacement = Windows::UI::Popups::Placement::Below;

        // Now launch the file.
        return Windows::System::Launcher::LaunchFileAsync(file, launchOptions);
    }).then([](bool launchResult)
    {
        if (launchResult)
        {
            MainPage::Current->NotifyUser("File launch succeeded.", NotifyType::StatusMessage);
        }
        else
        {
            MainPage::Current->NotifyUser("File launch failed.", NotifyType::ErrorMessage);
        }
    });
}

void LaunchFile::LaunchFileSplitScreenButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
#if WINAPI_FAMILY == WINAPI_FAMILY_PC_APP
    auto openPicker = ref new Windows::Storage::Pickers::FileOpenPicker();
    openPicker->FileTypeFilter->Append("*");

    create_task(openPicker->PickSingleFileAsync()).then([this](StorageFile^ file)
    {
        if (file)
        {
            // Configure the request for split screen launch.
            auto launchOptions = ref new Windows::System::LauncherOptions();

            if (this->AssociationLaunching::LaunchFile::Default->IsSelected == true)
            {
                launchOptions->DesiredRemainingView = Windows::UI::ViewManagement::ViewSizePreference::Default;
            }
            else if (this->AssociationLaunching::LaunchFile::UseLess->IsSelected == true)
            {
                launchOptions->DesiredRemainingView = Windows::UI::ViewManagement::ViewSizePreference::UseLess;
            }
            else if (this->AssociationLaunching::LaunchFile::UseHalf->IsSelected == true)
            {
                launchOptions->DesiredRemainingView = Windows::UI::ViewManagement::ViewSizePreference::UseHalf;
            }
            else if (this->AssociationLaunching::LaunchFile::UseMore->IsSelected== true)
            {
                launchOptions->DesiredRemainingView = Windows::UI::ViewManagement::ViewSizePreference::UseMore;
            }
            else if (this->AssociationLaunching::LaunchFile::UseMinimum->IsSelected== true)
            {
                launchOptions->DesiredRemainingView = Windows::UI::ViewManagement::ViewSizePreference::UseMinimum;
            }
            else if (this->AssociationLaunching::LaunchFile::UseNone->IsSelected == true)
            {
                launchOptions->DesiredRemainingView = Windows::UI::ViewManagement::ViewSizePreference::UseNone;
            }

            // Now launch the file.
            task<bool>(Windows::System::Launcher::LaunchFileAsync(file, launchOptions)).then([](bool launchResult)
            {
                if (launchResult)
                {
                    MainPage::Current->NotifyUser("File launch succeeded.", NotifyType::StatusMessage);
                }
                else
                {
                    MainPage::Current->NotifyUser("File launch failed.", NotifyType::ErrorMessage);
                }
            });
        }
        else
        {
            MainPage::Current->NotifyUser("No file was picked.", NotifyType::ErrorMessage);
        }
    });
#endif
}

#if WINAPI_FAMILY == WINAPI_FAMILY_PC_APP
void LaunchFile::PickAndLaunchFileButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // Get the file to launch via the picker.
    auto openPicker = ref new Windows::Storage::Pickers::FileOpenPicker();
    openPicker->FileTypeFilter->Append("*");

    create_task(openPicker->PickSingleFileAsync()).then([](StorageFile^ file)
    {
        if (file)
        {
            // Now launch the file.
            task<bool>(Windows::System::Launcher::LaunchFileAsync(file)).then([](bool launchResult)
            {
                if (launchResult)
                {
                    MainPage::Current->NotifyUser("File launch succeeded.", NotifyType::StatusMessage);
                }
                else
                {
                    MainPage::Current->NotifyUser("File launch failed.", NotifyType::ErrorMessage);
                }
            });
        }
        else
        {
            MainPage::Current->NotifyUser("No file was picked.", NotifyType::ErrorMessage);
        }
    });
}
#endif

#if WINAPI_FAMILY == WINAPI_FAMILY_PHONE_APP
void LaunchFile::PickAndLaunchFileButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // Get the file to launch via the picker.
    auto openPicker = ref new Windows::Storage::Pickers::FileOpenPicker();
    openPicker->FileTypeFilter->Append("*");
    openPicker->PickSingleFileAndContinue();
}

void LaunchFile::ContinueFileOpenPicker(FileOpenPickerContinuationEventArgs^ args)
{
    if (args->Files->Size > 0)
    {
        StorageFile^ file = args->Files->GetAt(0);
        // Now launch the file.
        task<bool>(Windows::System::Launcher::LaunchFileAsync(file)).then([](bool launchResult)
        {
            if (launchResult)
            {
                MainPage::Current->NotifyUser("File launch succeeded.", NotifyType::StatusMessage);
            }
            else
            {
                MainPage::Current->NotifyUser("File launch failed.", NotifyType::ErrorMessage);
            }
        });
    }
    else
    {
        MainPage::Current->NotifyUser("No file was picked.", NotifyType::ErrorMessage);
    }
}
#endif

// The Open With dialog will be displayed just under the element that triggered them.
// An alternative to using the point is to set the rect of the bounding element.
Point LaunchFile::GetOpenWithPosition(FrameworkElement^ element)
{
    Windows::UI::Xaml::Media::GeneralTransform^ buttonTransform = element->TransformToVisual(nullptr);
    Point origin(0, 0);

    Point desiredLocation = buttonTransform->TransformPoint(origin);
    desiredLocation.Y = desiredLocation.Y + safe_cast<float>(element->ActualHeight);

    return desiredLocation;
}

void LaunchFile::OnPage_SizeChanged(Platform::Object^ sender, Windows::UI::Xaml::SizeChangedEventArgs^ e)
{
#if WINAPI_FAMILY == WINAPI_FAMILY_PHONE_APP
    // Adjust UI elements layout for small screen on Phone
    LaunchFilePanel->Orientation = Orientation::Vertical;
    LaunchFileDescription->Width = this->ActualWidth;
    LaunchFileDescription->TextWrapping = TextWrapping::Wrap;
    LaunchFileDescription->HorizontalAlignment = Windows::UI::Xaml::HorizontalAlignment::Left;

    PickAndLaunchFilePanel->Orientation = Orientation::Vertical;
    PickAndLaunchFileDescription->Width = this->ActualWidth;
    PickAndLaunchFileDescription->TextWrapping = TextWrapping::Wrap;
    PickAndLaunchFileDescription->HorizontalAlignment = Windows::UI::Xaml::HorizontalAlignment::Left;
#endif
}
