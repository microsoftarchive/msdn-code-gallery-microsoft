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
// InAppPurchaseConsumables.xaml.cpp
// Implementation of the InAppPurchaseConsumables class
//

#include "pch.h"
#include "InAppPurchaseConsumables.xaml.h"

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

InAppPurchaseConsumables::InAppPurchaseConsumables()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void InAppPurchaseConsumables::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
    numberOfConsumablesPurchased = 0;
    consumedTransactionIds = ref new Platform::Collections::Vector<Platform::Guid>();
    LoadInAppPurchaseConsumablesProxyFile();
}

void InAppPurchaseConsumables::LoadInAppPurchaseConsumablesProxyFile()
{
    create_task(Package::Current->InstalledLocation->GetFolderAsync("data")).then([this](StorageFolder^ proxyDataFolder)
    {
        create_task(proxyDataFolder->GetFileAsync("in-app-purchase-consumables.xml")).then([this](StorageFile^ proxyFile)
        {
            create_task(CurrentAppSimulator::ReloadSimulatorAsync(proxyFile)).then([this]()
            {
                create_task(CurrentAppSimulator::LoadListingInformationAsync()).then([this](task<ListingInformation^> currentTask)
                {
                    try
                    {
                        ListingInformation^ listing = currentTask.get();
                        auto product1 = listing->ProductListings->Lookup("product1");
                        Product1SellMessage->Text = "You can buy " + product1->Name + " for: " + product1->FormattedPrice + ".";
                        Log("", NotifyType::StatusMessage);
                    }
                    catch(Platform::Exception^ exception)
                    {
                        rootPage->NotifyUser("LoadListingInformationAsync API call failed", NotifyType::ErrorMessage);
                    }
                });
            });
        });
    });
}

void InAppPurchaseConsumables::BuyAndFulfillProduct1Button_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Log("Buying Product 1...", NotifyType::StatusMessage);
    create_task(CurrentAppSimulator::RequestProductPurchaseAsync("product1")).then([this](task<PurchaseResults^> currentTask)
    {
        try
        {
            PurchaseResults^ results = currentTask.get();
            switch (results->Status)
            {
            case ProductPurchaseStatus::Succeeded:
                GrantFeatureLocally(results->TransactionId);
                FulfillProduct1("product1", results->TransactionId);
                break;
            case ProductPurchaseStatus::NotFulfilled:
                if (!IsLocallyFulfilled(results->TransactionId))
                {
                    GrantFeatureLocally(results->TransactionId);
                }
                FulfillProduct1("product1", results->TransactionId);
                break;
            case ProductPurchaseStatus::NotPurchased:
                Log("Product 1 was not purchased.", NotifyType::StatusMessage);
                break;
            }
        }
        catch(Platform::Exception^ exception)
        {
            Log("Unable to buy Product 1.", NotifyType::ErrorMessage);
        }
    });
}

void InAppPurchaseConsumables::FulfillProduct1(Platform::String ^productId, Platform::Guid transactionId)
{
    create_task(CurrentAppSimulator::ReportConsumableFulfillmentAsync(productId, transactionId)).then([this](task<FulfillmentResult> currentTask)
    {
        try
        {
            FulfillmentResult result = currentTask.get();
            switch (result)
            {
            case FulfillmentResult::Succeeded:
                Log("You bought and fulfilled product 1.", NotifyType::StatusMessage);
                break;
            case FulfillmentResult::NothingToFulfill:
                Log("There is no purchased product 1 to fulfill.", NotifyType::StatusMessage);
                break;
            case FulfillmentResult::PurchasePending:
                Log("You bought product 1. The purchase is pending so we cannot fulfill the product.", NotifyType::StatusMessage);
                break;
            case FulfillmentResult::PurchaseReverted:
                Log("You bought product 1. But your purchase has been reverted.", NotifyType::StatusMessage);
                // Since the user's purchase was revoked, they got their money back.
                // You may want to revoke the user's access to the consumable content that was granted.
                break;
            case FulfillmentResult::ServerError:
                Log("You bought product 1. There was an error when fulfilling.", NotifyType::StatusMessage);
                break;
            }
        }
        catch(Platform::Exception^ exception)
        {
            Log("You bought Product 1. There was an error when fulfilling.", NotifyType::ErrorMessage);
        }
    });
}

void InAppPurchaseConsumables::Log(Platform::String ^message, SDKSample::NotifyType type)
{
    Platform::String ^consumablesTable = "\n\nProduct 1 has been fulfilled " + numberOfConsumablesPurchased + " time" + (numberOfConsumablesPurchased == 1 ? "" : "s") + ".";
    Platform::String ^newMessage = message + consumablesTable;
    rootPage->NotifyUser(newMessage, type);
}

void InAppPurchaseConsumables::GrantFeatureLocally(Platform::Guid transactionId)
{
    consumedTransactionIds->Append(transactionId);

    // Grant the user their content. You will likely increase some kind of gold/coins/some other asset count.
    numberOfConsumablesPurchased++;
}

bool InAppPurchaseConsumables::IsLocallyFulfilled(Platform::Guid transactionId)
{
    bool inList = false;
    for (unsigned int i = 0; i < consumedTransactionIds->Size; i++)
    {
        if (consumedTransactionIds->GetAt(i) == transactionId)
        {
            inList = true;
            break;
        }
    }

    return inList;
}

