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
// InAppPurchaseLargeCatalog.xaml.cpp
// Implementation of the InAppPurchaseLargeCatalog class
//

#include "pch.h"
#include "InAppPurchaseLargeCatalog.xaml.h"

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

InAppPurchaseLargeCatalog::InAppPurchaseLargeCatalog()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void InAppPurchaseLargeCatalog::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
    numberOfConsumablesPurchased = 0;
    consumedTransactionIds = ref new Platform::Collections::Vector<Platform::Guid>();
    LoadInAppPurchaseLargeCatalogProxyFile();
}

void InAppPurchaseLargeCatalog::LoadInAppPurchaseLargeCatalogProxyFile()
{
    create_task(Package::Current->InstalledLocation->GetFolderAsync("data")).then([this](StorageFolder^ proxyDataFolder)
    {
        create_task(proxyDataFolder->GetFileAsync("in-app-purchase-large-catalog.xml")).then([this](StorageFile^ proxyFile)
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
                        product1ListingName = product1->Name;
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

void InAppPurchaseLargeCatalog::BuyAndFulfillProduct1Button_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Platform::String ^offerId = OfferIdTextBox->Text;
    Platform::String ^displayPropertiesName = DisplayPropertiesNameTextBox->Text;
    ProductPurchaseDisplayProperties ^displayProperties = ref new ProductPurchaseDisplayProperties(displayPropertiesName);

    Log("Buying Product 1...", NotifyType::StatusMessage);
    create_task(CurrentAppSimulator::RequestProductPurchaseAsync("product1", offerId, displayProperties)).then([this](task<PurchaseResults^> currentTask)
    {
        try
        {
            PurchaseResults^ results = currentTask.get();
            switch (results->Status)
            {
            case ProductPurchaseStatus::Succeeded:
                GrantFeatureLocally(results->TransactionId);
                FulfillProduct1("product1", results);
                break;
            case ProductPurchaseStatus::NotFulfilled:
                if (!IsLocallyFulfilled(results->TransactionId))
                {
                    GrantFeatureLocally(results->TransactionId);
                }
                FulfillProduct1("product1", results);
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

void InAppPurchaseLargeCatalog::FulfillProduct1(Platform::String ^productId, PurchaseResults ^purchaseResults)
{
    create_task(CurrentAppSimulator::ReportConsumableFulfillmentAsync(productId, purchaseResults->TransactionId)).then([this, purchaseResults](task<FulfillmentResult> currentTask)
    {
        Platform::String ^displayPropertiesName = DisplayPropertiesNameTextBox->Text;

        if (displayPropertiesName->IsEmpty())
        {
            displayPropertiesName = product1ListingName;
        }
        Platform::String ^offerIdMsg = " with no offer id";
        if (!purchaseResults->OfferId->IsEmpty())
        {
            offerIdMsg = " with offer id " + purchaseResults->OfferId;
        }
        Platform::String ^purchaseStringSimple = "You bought product 1.";
        if (purchaseResults->Status == ProductPurchaseStatus::NotFulfilled) {
            purchaseStringSimple = "You already purchased product 1.";
        }


        try
        {
            FulfillmentResult result = currentTask.get();
            switch (result)
            {
            case FulfillmentResult::Succeeded:
                if (purchaseResults->Status == ProductPurchaseStatus::NotFulfilled)
                {
                    Log("You had already purchased " + product1ListingName + offerIdMsg + " and it was just fulfilled.", NotifyType::StatusMessage);
                }
                else
                {
                    Log("You bought and fulfilled " + displayPropertiesName + offerIdMsg, NotifyType::StatusMessage);
                }
                break;
            case FulfillmentResult::NothingToFulfill:
                Log("There is no purchased product 1 to fulfill with that transaction id.", NotifyType::StatusMessage);
                break;
            case FulfillmentResult::PurchasePending:
                Log(purchaseStringSimple + " The purchase is pending so we cannot fulfill the product.", NotifyType::StatusMessage);
                break;
            case FulfillmentResult::PurchaseReverted:
                Log(purchaseStringSimple + " But your purchase has been reverted.", NotifyType::StatusMessage);
                // Since the user's purchase was revoked, they got their money back.
                // You may want to revoke the user's access to the consumable content that was granted.
                break;
            case FulfillmentResult::ServerError:
                Log(purchaseStringSimple + " There was an error when fulfilling.", NotifyType::StatusMessage);
                break;
            }
        }
        catch(Platform::Exception^ exception)
        {
            Log(purchaseStringSimple + " There was an error when fulfilling.", NotifyType::ErrorMessage);
        }
    });
}

void InAppPurchaseLargeCatalog::Log(Platform::String ^message, SDKSample::NotifyType type)
{
    Platform::String ^consumablesTable = "\n\nProduct 1 has been fulfilled " + numberOfConsumablesPurchased + " time" + (numberOfConsumablesPurchased == 1 ? "" : "s") + ".";
    Platform::String ^newMessage = message + consumablesTable;
    rootPage->NotifyUser(newMessage, type);
}

void InAppPurchaseLargeCatalog::GrantFeatureLocally(Platform::Guid transactionId)
{
    consumedTransactionIds->Append(transactionId);

    // Grant the user their content. You will likely increase some kind of gold/coins/some other asset count.
    numberOfConsumablesPurchased++;
}

bool InAppPurchaseLargeCatalog::IsLocallyFulfilled(Platform::Guid transactionId)
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

