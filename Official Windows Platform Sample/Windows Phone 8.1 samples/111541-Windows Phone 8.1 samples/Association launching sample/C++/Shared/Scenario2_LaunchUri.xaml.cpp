//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// LaunchUri.xaml.cpp
// Implementation of the LaunchUri class
//

#include "pch.h"
#include "Scenario2_LaunchUri.xaml.h"
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

LaunchUri::LaunchUri()
{
    InitializeComponent();
}

void LaunchUri::OnNavigatedTo(NavigationEventArgs^ e)
{
#if WINAPI_FAMILY == WINAPI_FAMILY_PHONE_APP
    // Disable scenarios that are not supported in Phone.
    LaunchUriOpenWithPanel->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    LaunchUriWithWarningPanel->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    LaunchUriSplitScreenPanel->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
#endif
}

void LaunchUri::LaunchUriButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // Create the URI from a string.
    auto uri = ref new Uri(SDKSample::AssociationLaunching::LaunchUri::UriToLaunch->Text);

    // Launch the URI.
    create_task(Windows::System::Launcher::LaunchUriAsync(uri)).then([](bool launchResult)
    {
        if (launchResult)
        {
            MainPage::Current->NotifyUser("Uri launch succeeded.", NotifyType::StatusMessage);
        }
        else
        {
            MainPage::Current->NotifyUser("Uri launch failed.", NotifyType::ErrorMessage);
        }
    });
}

void LaunchUri::LaunchUriWithWarningButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // Create the URI from a string.
    auto uri = ref new Uri(SDKSample::AssociationLaunching::LaunchUri::UriToLaunch->Text);

    // Configure the warning prompt.
    auto launchOptions = ref new Windows::System::LauncherOptions();
    launchOptions->TreatAsUntrusted = true;

    // Launch the URI.
    create_task(Windows::System::Launcher::LaunchUriAsync(uri, launchOptions)).then([](bool launchResult)
    {
        if (launchResult)
        {
            MainPage::Current->NotifyUser("Uri launch succeeded.", NotifyType::StatusMessage);
        }
        else
        {
            MainPage::Current->NotifyUser("Uri launch failed.", NotifyType::ErrorMessage);
        }
    });
}

void LaunchUri::LaunchUriOpenWithButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    auto button = dynamic_cast<Button^>(sender);

    // Create the URI from a string.
    auto uri = ref new Uri(SDKSample::AssociationLaunching::LaunchUri::UriToLaunch->Text);

    // Calulcate the position for the Open With dialog.
    Point openWithPosition = GetOpenWithPosition(button);
    auto launchOptions = ref new Windows::System::LauncherOptions();
    launchOptions->DisplayApplicationPicker = true;
    launchOptions->UI->InvocationPoint = dynamic_cast<IBox<Point>^>(PropertyValue::CreatePoint(openWithPosition));
    launchOptions->UI->PreferredPlacement = Windows::UI::Popups::Placement::Below;

    // Launch the URI.
    create_task(Windows::System::Launcher::LaunchUriAsync(uri, launchOptions)).then([](bool launchResult)
    {
        if (launchResult)
        {
            MainPage::Current->NotifyUser("Uri launch succeeded.", NotifyType::StatusMessage);
        }
        else
        {
            MainPage::Current->NotifyUser("Uri launch failed.", NotifyType::ErrorMessage);
        }
    });
}

void LaunchUri::LaunchUriSplitScreenButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // Create the URI from a string.
    auto uri = ref new Uri(SDKSample::AssociationLaunching::LaunchUri::UriToLaunch->Text);

    // Configure the request for split screen launch.
    auto launchOptions = ref new Windows::System::LauncherOptions();
#if WINAPI_FAMILY == WINAPI_FAMILY_PC_APP
    if (SDKSample::AssociationLaunching::LaunchUri::Default->IsSelected == true)
    {
        launchOptions->DesiredRemainingView = Windows::UI::ViewManagement::ViewSizePreference::Default;
    }
    else if (SDKSample::AssociationLaunching::LaunchUri::UseLess->IsSelected == true)
    {
        launchOptions->DesiredRemainingView = Windows::UI::ViewManagement::ViewSizePreference::UseLess;
    }
    else if (SDKSample::AssociationLaunching::LaunchUri::UseHalf->IsSelected == true)
    {
        launchOptions->DesiredRemainingView = Windows::UI::ViewManagement::ViewSizePreference::UseHalf;
    }
    else if (SDKSample::AssociationLaunching::LaunchUri::UseMore->IsSelected == true)
    {
        launchOptions->DesiredRemainingView = Windows::UI::ViewManagement::ViewSizePreference::UseMore;
    }
    else if (SDKSample::AssociationLaunching::LaunchUri::UseMinimum->IsSelected == true)
    {
        launchOptions->DesiredRemainingView = Windows::UI::ViewManagement::ViewSizePreference::UseMinimum;
    }
    else if (SDKSample::AssociationLaunching::LaunchUri::UseNone->IsSelected == true)
    {
        launchOptions->DesiredRemainingView = Windows::UI::ViewManagement::ViewSizePreference::UseNone;
    }
#endif

    // Launch the URI.
    create_task(Windows::System::Launcher::LaunchUriAsync(uri, launchOptions)).then([](bool launchResult)
    {
        if (launchResult)
        {
            MainPage::Current->NotifyUser("Uri launch succeeded.", NotifyType::StatusMessage);
        }
        else
        {
            MainPage::Current->NotifyUser("Uri launch failed.", NotifyType::ErrorMessage);
        }
    });
}

// The Open With dialog will be displayed just under the element that triggered them.
// An alternative to using the point is to set the rect of the bounding element.
Point LaunchUri::GetOpenWithPosition(FrameworkElement^ element)
{
    Windows::UI::Xaml::Media::GeneralTransform^ buttonTransform = element->TransformToVisual(nullptr);
    Point origin(0, 0);

    Point desiredLocation = buttonTransform->TransformPoint(origin);
    desiredLocation.Y = desiredLocation.Y + safe_cast<float>(element->ActualHeight);

    return desiredLocation;
}

void LaunchUri::OnPage_SizeChanged(Platform::Object^ sender, Windows::UI::Xaml::SizeChangedEventArgs^ e)
{
#if WINAPI_FAMILY == WINAPI_FAMILY_PHONE_APP
    // Adjust UI elements layout for small screen on Phone
    LaunchUriPanel->Orientation = Orientation::Vertical;
    LaunchUriDescription->Width = this->ActualWidth;
    LaunchUriDescription->TextWrapping = TextWrapping::Wrap;
    LaunchUriDescription->HorizontalAlignment = Windows::UI::Xaml::HorizontalAlignment::Left;

    UriToLaunchPanel->Orientation = Orientation::Vertical;
    UriToLaunchDescription->Width = this->ActualWidth;
    UriToLaunchDescription->TextWrapping = TextWrapping::Wrap;
    UriToLaunchDescription->HorizontalAlignment = Windows::UI::Xaml::HorizontalAlignment::Left;
#endif
}
