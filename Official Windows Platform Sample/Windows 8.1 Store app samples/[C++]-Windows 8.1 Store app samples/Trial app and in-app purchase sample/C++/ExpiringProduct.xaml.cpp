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
// ExpiringProduct.xaml.cpp
// Implementation of the ExpiringProduct class
//

#include "pch.h"
#include "ExpiringProduct.xaml.h"

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

ExpiringProduct::ExpiringProduct()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void ExpiringProduct::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
    LoadExpiringProductProxyFile();
}

void ExpiringProduct::OnNavigatingFrom(Windows::UI::Xaml::Navigation::NavigatingCancelEventArgs^ e)
{
    CurrentAppSimulator::LicenseInformation->LicenseChanged -= eventRegistrationToken;
}

void ExpiringProduct::LoadExpiringProductProxyFile()
{
    create_task(Package::Current->InstalledLocation->GetFolderAsync("data")).then([this](StorageFolder^ proxyDataFolder)
    {
        create_task(proxyDataFolder->GetFileAsync("expiring-product.xml")).then([this](StorageFile^ proxyFile)
        {
            eventRegistrationToken = (CurrentAppSimulator::LicenseInformation->LicenseChanged += ref new LicenseChangedEventHandler(this, &ExpiringProduct::ExpiringProductRefreshScenario));
            create_task(CurrentAppSimulator::ReloadSimulatorAsync(proxyFile)).then([this]()
            {
            });
        });
    });
}

void ExpiringProduct::ExpiringProductRefreshScenario()
{
    auto licenseInformation = CurrentAppSimulator::LicenseInformation;
    if (licenseInformation->IsActive)
    {
        if (licenseInformation->IsTrial)
        {
            rootPage->NotifyUser("You don't have full license", NotifyType::ErrorMessage);
        }
        else
        {
            auto productLicense1 = licenseInformation->ProductLicenses->Lookup("product1");
            if (productLicense1->IsActive)
            {
                auto longdateTemplate = ref new DateTimeFormatting::DateTimeFormatter("longdate");
                Product1Message->Text = "Product 1 expires on: " + longdateTemplate->Format(productLicense1->ExpirationDate);

                auto calendarObj = ref new Calendar();
                calendarObj->SetToNow();
                auto currentDateTime = calendarObj->GetDateTime();
                auto numberOfRemainingDays = (productLicense1->ExpirationDate.UniversalTime - currentDateTime.UniversalTime) / 864000000000;
                rootPage->NotifyUser("Product 1 expires in: " + numberOfRemainingDays + " days.", NotifyType::StatusMessage);
            }
            else
            {
                rootPage->NotifyUser("Product 1 is not available.", NotifyType::ErrorMessage);
            }
        }
    }
    else
    {
        rootPage->NotifyUser("You don't have active license.", NotifyType::ErrorMessage);
    }
}
