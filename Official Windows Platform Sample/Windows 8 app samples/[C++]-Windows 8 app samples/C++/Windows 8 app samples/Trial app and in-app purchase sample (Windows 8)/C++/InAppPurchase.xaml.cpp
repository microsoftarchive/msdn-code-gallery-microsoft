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
// InAppPurchase.xaml.cpp
// Implementation of the InAppPurchase class
//

#include "pch.h"
#include "InAppPurchase.xaml.h"

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

InAppPurchase::InAppPurchase()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void InAppPurchase::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
    LoadInAppPurchaseProxyFile();
}

void InAppPurchase::OnNavigatingFrom(Windows::UI::Xaml::Navigation::NavigatingCancelEventArgs^ e)
{
    CurrentAppSimulator::LicenseInformation->LicenseChanged -= eventRegistrationToken;
}

void InAppPurchase::LoadInAppPurchaseProxyFile()
{
    create_task(Package::Current->InstalledLocation->GetFolderAsync("data")).then([this](StorageFolder^ proxyDataFolder)
    {
        create_task(proxyDataFolder->GetFileAsync("in-app-purchase.xml")).then([this](StorageFile^ proxyFile)
        {
            eventRegistrationToken = (CurrentAppSimulator::LicenseInformation->LicenseChanged += ref new LicenseChangedEventHandler(this, &InAppPurchase::InAppPurchaseRefreshScenario));
            create_task(CurrentAppSimulator::ReloadSimulatorAsync(proxyFile)).then([this]()
            {
                create_task(CurrentAppSimulator::LoadListingInformationAsync()).then([this](ListingInformation^ listing)
                {
                    auto product1 = listing->ProductListings->Lookup("product1");
                    auto product2 = listing->ProductListings->Lookup("product2");
                    Product1SellMessage->Text = "You can buy " + product1->Name + " for: " + product1->FormattedPrice + ".";
                    Product2SellMessage->Text = "You can buy " + product2->Name + " for: " + product2->FormattedPrice + ".";
                });
            });
        });
    });
}

void InAppPurchase::InAppPurchaseRefreshScenario()
{
}

void InAppPurchase::TryProduct1_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    auto licenseInformation = CurrentAppSimulator::LicenseInformation;
    auto productLicense = licenseInformation->ProductLicenses->Lookup("product1");
    if (productLicense->IsActive)
    {
        rootPage->NotifyUser("You can use Product 1.", NotifyType::StatusMessage);
    }
    else
    {
        rootPage->NotifyUser("You don't own Product 1. You must buy Product 1 before you can use it.", NotifyType::ErrorMessage);
    }
}

void InAppPurchase::BuyProduct1Button_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    auto licenseInformation = CurrentAppSimulator::LicenseInformation;
    if (!licenseInformation->ProductLicenses->Lookup("product1")->IsActive)
    {
        rootPage->NotifyUser("Buying Product 1...", NotifyType::StatusMessage);
        create_task(CurrentAppSimulator::RequestProductPurchaseAsync("product1", false)).then([this](task<Platform::String^> currentTask)
        {
            try
            {
                currentTask.get();
                auto licenseInformation = CurrentAppSimulator::LicenseInformation;
                if (licenseInformation->ProductLicenses->Lookup("product1")->IsActive)
                {
                    rootPage->NotifyUser("You bought Product 1.", NotifyType::StatusMessage);
                }
                else
                {
                    rootPage->NotifyUser("", NotifyType::StatusMessage);
                }
            }
            catch(Platform::Exception^ exception)
            {
                rootPage->NotifyUser("Unable to buy Product 1.", NotifyType::ErrorMessage);
            }
        });
    }
    else
    {
        rootPage->NotifyUser("You already own Product 1.", NotifyType::ErrorMessage);
    }
}

void InAppPurchase::TryProduct2_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    auto licenseInformation = CurrentAppSimulator::LicenseInformation;
    auto productLicense = licenseInformation->ProductLicenses->Lookup("product2");
    if (productLicense->IsActive)
    {
        rootPage->NotifyUser("You can use Product 2.", NotifyType::StatusMessage);
    }
    else
    {
        rootPage->NotifyUser("You don't own Product 2. You must buy Product 1 before you can use it.", NotifyType::ErrorMessage);
    }
}

void InAppPurchase::BuyProduct2_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    auto licenseInformation = CurrentAppSimulator::LicenseInformation;
    if (!licenseInformation->ProductLicenses->Lookup("product2")->IsActive)
    {
        rootPage->NotifyUser("Buying Product 2...", NotifyType::StatusMessage);
        create_task(CurrentAppSimulator::RequestProductPurchaseAsync("product2", false)).then([this](task<Platform::String^> currentTask)
        {
            try
            {
                currentTask.get();
                auto licenseInformation = CurrentAppSimulator::LicenseInformation;
                if (licenseInformation->ProductLicenses->Lookup("product2")->IsActive)
                {
                    rootPage->NotifyUser("You bought Product 2.", NotifyType::StatusMessage);
                }
                else
                {
                    rootPage->NotifyUser("", NotifyType::StatusMessage);
                }
            }
            catch(Platform::Exception^ exception)
            {
                rootPage->NotifyUser("Unable to buy Product 2.", NotifyType::ErrorMessage);
            }
        });
    }
    else
    {
        rootPage->NotifyUser("You already own Product 2.", NotifyType::ErrorMessage);
    }
}
