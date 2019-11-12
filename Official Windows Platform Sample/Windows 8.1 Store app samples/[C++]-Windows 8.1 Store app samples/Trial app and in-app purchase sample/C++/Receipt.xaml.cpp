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
// Receipt.xaml.cpp
// Implementation of the ExpiringProduct class
//

#include "pch.h"
#include "Receipt.xaml.h"

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

Receipt::Receipt()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Receipt::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
    LoadReceiptProxyFile();
}
void Receipt::OnNavigatingFrom(Windows::UI::Xaml::Navigation::NavigatingCancelEventArgs^ e)
{
    CurrentAppSimulator::LicenseInformation->LicenseChanged -= eventRegistrationToken;
}

void Receipt::LoadReceiptProxyFile()
{
    create_task(Package::Current->InstalledLocation->GetFolderAsync("data")).then([this](StorageFolder^ proxyDataFolder)
    {
        create_task(proxyDataFolder->GetFileAsync("receipt.xml")).then([this](StorageFile^ proxyFile)
        {
            eventRegistrationToken = (CurrentAppSimulator::LicenseInformation->LicenseChanged += ref new LicenseChangedEventHandler(this, &Receipt::ReceiptRefreshScenario));
            create_task(CurrentAppSimulator::ReloadSimulatorAsync(proxyFile)).then([this]()
            {
            });
        });
    });
}

void Receipt::ReceiptRefreshScenario()
{
}

void Receipt::ShowReceipt_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    create_task(CurrentAppSimulator::GetAppReceiptAsync()).then([this](task<Platform::String^> currentTask)
    {
        try
        {
            Platform::String^ receipt = currentTask.get();
            rootPage->NotifyUser(receipt, NotifyType::StatusMessage);
        }
        catch(Platform::Exception^ exception)
        {
            rootPage->NotifyUser("Get Receipt failed.", NotifyType::ErrorMessage);
        }
    });
}
