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
// TrialMode.xaml.cpp
// Implementation of the TrialMode class
//

#include "pch.h"
#include "TrialMode.xaml.h"

using namespace SDKSample::StoreSample;

using namespace concurrency;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Store;
using namespace Windows::Foundation;
using namespace Windows::Globalization;
using namespace Windows::Storage;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

TrialMode::TrialMode()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void TrialMode::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
    LoadTrialModeProxyFile();
}

void TrialMode::OnNavigatingFrom(Windows::UI::Xaml::Navigation::NavigatingCancelEventArgs^ e)
{
    CurrentAppSimulator::LicenseInformation->LicenseChanged -= eventRegistrationToken;
}

void TrialMode::LoadTrialModeProxyFile()
{
    create_task(Package::Current->InstalledLocation->GetFolderAsync("data")).then([this](StorageFolder^ proxyDataFolder)
    {
        create_task(proxyDataFolder->GetFileAsync("trial-mode.xml")).then([this](StorageFile^ proxyFile)
        {
            eventRegistrationToken = (CurrentAppSimulator::LicenseInformation->LicenseChanged += ref new LicenseChangedEventHandler(this, &TrialMode::TrialModeRefreshScenario));
            create_task(CurrentAppSimulator::ReloadSimulatorAsync(proxyFile)).then([this]()
            {
                create_task(CurrentAppSimulator::LoadListingInformationAsync()).then([this](ListingInformation^ listing)
                {
                    PurchasePrice->Text = "You can buy the full app for: " + listing->FormattedPrice + ".";
                });
            });
        });
    });
}

void TrialMode::TrialModeRefreshScenario()
{
    auto licenseInformation = CurrentAppSimulator::LicenseInformation;
    if (licenseInformation->IsActive)
    {
        if (licenseInformation->IsTrial)
        {
            LicenseMode->Text = "Current license mode: Trial license";
        }
        else
        {
            LicenseMode->Text = "Current license mode: Full license";
        }
    }
    else
    {
        LicenseMode->Text = "Current license mode: Inactive license";
    }
}

void TrialMode::TrialTime_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    auto licenseInformation = CurrentAppSimulator::LicenseInformation;
    if (licenseInformation->IsActive)
    {
        if (licenseInformation->IsTrial)
        {
            auto calendarObj = ref new Calendar();
            calendarObj->SetToNow();
            auto currentDateTime = calendarObj->GetDateTime();
            auto numberOfRemainingDays = (licenseInformation->ExpirationDate.UniversalTime - currentDateTime.UniversalTime) / 864000000000;
            rootPage->NotifyUser("You can use this app for " + numberOfRemainingDays + " more days before the trial period ends.", NotifyType::StatusMessage);
        }
        else
        {
            rootPage->NotifyUser("You have a full license. The trial time is not meaningful.", NotifyType::ErrorMessage);
        }
    }
    else
    {
        rootPage->NotifyUser("You don't have a license. The trial time can't be determined.", NotifyType::ErrorMessage);
    }
}

void TrialMode::TrialProduct_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    auto licenseInformation = CurrentAppSimulator::LicenseInformation;
    if (licenseInformation->IsActive)
    {
        if (licenseInformation->IsTrial)
        {
            rootPage->NotifyUser("You are using a trial version of this app.", NotifyType::StatusMessage);
        }
        else
        {
            rootPage->NotifyUser("You no longer have a trial version of this app.", NotifyType::ErrorMessage);
        }
    }
    else
    {
        rootPage->NotifyUser("You don't have a license for this app.", NotifyType::ErrorMessage);
    }
}

void TrialMode::FullProduct_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    auto licenseInformation = CurrentAppSimulator::LicenseInformation;
    if (licenseInformation->IsActive)
    {
        if (licenseInformation->IsTrial)
        {
            rootPage->NotifyUser("You are using a trial version of this app.", NotifyType::ErrorMessage);
        }
        else
        {
            rootPage->NotifyUser("You are using a fully-licensed version of this app.", NotifyType::StatusMessage);
        }
    }
    else
    {
        rootPage->NotifyUser("You don't have a license for this app.", NotifyType::ErrorMessage);
    }
}

void TrialMode::ConvertTrial_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    auto licenseInformation = CurrentAppSimulator::LicenseInformation;
    rootPage->NotifyUser("Buying the full license...", NotifyType::StatusMessage);
    if (licenseInformation->IsTrial)
    {
        create_task(CurrentAppSimulator::RequestAppPurchaseAsync(false)).then([this](task<Platform::String^> currentTask)
        {
            try
            {
                currentTask.get();
                auto licenseInformation = CurrentAppSimulator::LicenseInformation;
                if (licenseInformation->IsActive && !licenseInformation->IsTrial)
                {
                    rootPage->NotifyUser("You successfully upgraded your app to the fully-licensed version.", NotifyType::StatusMessage);
                }
                else
                {
                    rootPage->NotifyUser("You still have a trial license for this app.", NotifyType::ErrorMessage);
                }
            }
            catch(Platform::Exception^ exception)
            {
                rootPage->NotifyUser("The upgrade transaction failed. You still have a trial license for this app.", NotifyType::ErrorMessage);
            }
        });
    }
    else
    {
        rootPage->NotifyUser("You already bought this app and have a fully-licensed version.", NotifyType::ErrorMessage);
    }
}
