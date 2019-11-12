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
// InAppPurchaseConsumablesAdvanced.xaml.cpp
// Implementation of the InAppPurchaseConsumablesAdvanced class
//

#include "pch.h"
#include "InAppPurchaseConsumablesAdvanced.xaml.h"

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

InAppPurchaseConsumablesAdvanced::InAppPurchaseConsumablesAdvanced()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void InAppPurchaseConsumablesAdvanced::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;

    product1TempTransactionId = Platform::Guid();
    product1NumPurchases = 0;
    product1NumFulfillments = 0;
    
    product2TempTransactionId = Platform::Guid();
    product2NumPurchases = 0;
    product2NumFulfillments = 0;

    consumedTransactionIds = ref new Platform::Collections::Vector<Platform::Guid>();

    LoadInAppPurchaseConsumablesAdvancedProxyFile();
}

void InAppPurchaseConsumablesAdvanced::LoadInAppPurchaseConsumablesAdvancedProxyFile()
{
    create_task(Package::Current->InstalledLocation->GetFolderAsync("data")).then([this](StorageFolder^ proxyDataFolder)
    {
        create_task(proxyDataFolder->GetFileAsync("in-app-purchase-consumables-advanced.xml")).then([this](StorageFile^ proxyFile)
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
                        auto product2 = listing->ProductListings->Lookup("product2");
                        Product2SellMessage->Text = "You can buy " + product2->Name + " for: " + product2->FormattedPrice + ".";
                        Log("", NotifyType::StatusMessage);
                    }
                    catch(Platform::Exception^ exception)
                    {
                        rootPage->NotifyUser("LoadListingInformationAsync API call failed", NotifyType::ErrorMessage);
                    }
                });

                // recover already purchased consumables
                create_task(CurrentAppSimulator::GetUnfulfilledConsumablesAsync()).then([this](task<Windows::Foundation::Collections::IVectorView<UnfulfilledConsumable^>^> currentTask)
                {
                    try
                    {
                        Windows::Foundation::Collections::IVectorView<UnfulfilledConsumable^> ^products = currentTask.get();
                        for (unsigned int i = 0; i < products->Size; i++)
                        {
                            // This is where you would normally grant the user their consumable content and call currentApp.reportConsumableFulfillment
                            if (products->GetAt(i)->ProductId->Equals("product1"))
                            {
                                product1TempTransactionId = products->GetAt(i)->TransactionId;
                            }
                            else if (products->GetAt(i)->ProductId->Equals("product2"))
                            {
                                product2TempTransactionId = products->GetAt(i)->TransactionId;
                            }
                        }
                    }
                    catch (Platform::Exception ^exception)
                    {
                        rootPage->NotifyUser("GetUnfulfilledConsumables API call failed", NotifyType::ErrorMessage);
                    }
                });
            });
        });
    });
}

void InAppPurchaseConsumablesAdvanced::GetUnfulfilledConsumables()
{
    create_task(CurrentAppSimulator::GetUnfulfilledConsumablesAsync()).then([this](task<Windows::Foundation::Collections::IVectorView<UnfulfilledConsumable^>^> currentTask)
    {
        try
        {
            Platform::String ^logMessage = "List of unfulfilled consumables:";
            Windows::Foundation::Collections::IVectorView<UnfulfilledConsumable^> ^products = currentTask.get();

            for (unsigned int i = 0; i < products->Size; i++)
            {
                logMessage += "\nProduct Id: " + products->GetAt(i)->ProductId + " Transaction Id: " + products->GetAt(i)->TransactionId;
                // This is where you would grant the user their consumable content and call currentApp.reportConsumableFulfillment
            }

            if (products->Size == 0)
            {
                logMessage = "There are no consumable purchases awaiting fulfillment.";
            }
            Log(logMessage, NotifyType::StatusMessage, products->Size);
        }
        catch (Platform::Exception ^exception)
        {
            Log("GetUnfulfilledConsumables API call failed", NotifyType::ErrorMessage);
        }
    });
}

void InAppPurchaseConsumablesAdvanced::GetUnfulfilledButton1_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    GetUnfulfilledConsumables();
}

void InAppPurchaseConsumablesAdvanced::GetUnfulfilledButton2_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    GetUnfulfilledConsumables();
}


void InAppPurchaseConsumablesAdvanced::BuyProduct1Button_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
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
                product1NumPurchases++;
                product1TempTransactionId = results->TransactionId;
                Log("You bought product 1. Transaction Id: " + results->TransactionId, NotifyType::StatusMessage);

                // Normally you would grant the user their content here, and then call currentApp.reportConsumableFulfillment
                break;
            case ProductPurchaseStatus::NotFulfilled:
                product1TempTransactionId = results->TransactionId;
                Log("You have an unfulfilled copy of product 1. Hit \"Fulfill product 1\" before you can purchase a second copy.", NotifyType::StatusMessage);

                // Normally you would grant the user their content here, and then call currentApp.reportConsumableFulfillment
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

void InAppPurchaseConsumablesAdvanced::BuyProduct2Button_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Log("Buying Product 2...", NotifyType::StatusMessage);
    create_task(CurrentAppSimulator::RequestProductPurchaseAsync("product2")).then([this](task<PurchaseResults^> currentTask)
    {
        try
        {
            PurchaseResults^ results = currentTask.get();
            switch (results->Status)
            {
            case ProductPurchaseStatus::Succeeded:
                product2NumPurchases++;
                product2TempTransactionId = results->TransactionId;
                Log("You bought product 2. Transaction Id: " + results->TransactionId, NotifyType::StatusMessage);

                // Normally you would grant the user their content here, and then call currentApp.reportConsumableFulfillment
                break;
            case ProductPurchaseStatus::NotFulfilled:
                product2TempTransactionId = results->TransactionId;
                Log("You have an unfulfilled copy of product 2. Hit \"Fulfill product 2\" before you can purchase a second copy.", NotifyType::StatusMessage);

                // Normally you would grant the user their content here, and then call currentApp.reportConsumableFulfillment
                break;
            case ProductPurchaseStatus::NotPurchased:
                Log("Product 2 was not purchased.", NotifyType::StatusMessage);
                break;
            }
        }
        catch(Platform::Exception^ exception)
        {
            Log("Unable to buy Product 2.", NotifyType::ErrorMessage);
        }
    });
}

void InAppPurchaseConsumablesAdvanced::FulfillProduct1Button_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if (!IsLocallyFulfilled(product1TempTransactionId))
    {
        GrantFeatureLocally("product1", product1TempTransactionId);
    }
    create_task(CurrentAppSimulator::ReportConsumableFulfillmentAsync("product1", product1TempTransactionId)).then([this](task<FulfillmentResult> currentTask)
    {
        try
        {
            FulfillmentResult result = currentTask.get();
            switch (result)
            {
            case FulfillmentResult::Succeeded:
                product1NumFulfillments++;
                Log("Product 1 was fulfilled. You are now able to buy product 1 again.", NotifyType::StatusMessage);
                break;
            case FulfillmentResult::NothingToFulfill:
                Log("There is nothing to fulfill. You must purchase product 1 before it can be fulfilled.", NotifyType::StatusMessage);
                break;
            case FulfillmentResult::PurchasePending:
                Log("Purchase hasn't completed yet. Wait and try again.", NotifyType::StatusMessage);
                break;
            case FulfillmentResult::PurchaseReverted:
                Log("Purchase was reverted before fulfillment.", NotifyType::StatusMessage);
                // Since the user's purchase was revoked, they got their money back.
                // You may want to revoke the user's access to the consumable content that was granted.
                break;
            case FulfillmentResult::ServerError:
                Log("There was an error when fulfilling.", NotifyType::StatusMessage);
                break;
            }
        }
        catch(Platform::Exception^ exception)
        {
            Log("There was an error when fulfilling.", NotifyType::ErrorMessage);
        }
    });
}

void InAppPurchaseConsumablesAdvanced::FulfillProduct2Button_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if (!IsLocallyFulfilled(product2TempTransactionId))
    {
        GrantFeatureLocally("product2", product2TempTransactionId);
    }
    create_task(CurrentAppSimulator::ReportConsumableFulfillmentAsync("product2", product2TempTransactionId)).then([this](task<FulfillmentResult> currentTask)
    {
        try
        {
            FulfillmentResult result = currentTask.get();
            switch (result)
            {
            case FulfillmentResult::Succeeded:
                product2NumFulfillments++;
                Log("Product 2 was fulfilled. You are now able to buy product 2 again.", NotifyType::StatusMessage);
                break;
            case FulfillmentResult::NothingToFulfill:
                Log("There is nothing to fulfill. You must purchase product 2 before it can be fulfilled.", NotifyType::StatusMessage);
                break;
            case FulfillmentResult::PurchasePending:
                Log("Purchase hasn't completed yet. Wait and try again.", NotifyType::StatusMessage);
                break;
            case FulfillmentResult::PurchaseReverted:
                Log("Purchase was reverted before fulfillment.", NotifyType::StatusMessage);
                // Since the user's purchase was revoked, they got their money back.
                // You may want to revoke the user's access to the consumable content that was granted.
                break;
            case FulfillmentResult::ServerError:
                Log("There was an error when fulfilling.", NotifyType::StatusMessage);
                break;
            }
        }
        catch(Platform::Exception^ exception)
        {
            Log("There was an error when fulfilling.", NotifyType::ErrorMessage);
        }
    });
}

void InAppPurchaseConsumablesAdvanced::Log(Platform::String ^message, SDKSample::NotifyType type)
{
    Log(message, type, 0);
}

void InAppPurchaseConsumablesAdvanced::Log(Platform::String ^message, SDKSample::NotifyType type, int blankLines)
{
    Platform::String ^newMessage = message + "\n\n";

    if (blankLines == 1)
    {
        newMessage += "\n";
    }
    else if (blankLines != 2)
    {
        newMessage += "\n\n";
    }

    Platform::String ^resultsTable = "Product 1 has been purchased " + product1NumPurchases + " time" + (product1NumPurchases == 1 ? "" : "s") + " and fulfilled " + product1NumFulfillments + " time" + (product1NumFulfillments == 1 ? "" : "s") + ".";
    resultsTable += "\n" + "Product 2 has been purchased " + product2NumPurchases + " time" + (product2NumPurchases == 1 ? "" : "s") + " and fulfilled " + product2NumFulfillments + " time" + (product2NumFulfillments == 1 ? "" : "s") + ".";

    newMessage += resultsTable;
    rootPage->NotifyUser(newMessage, type);
}

void InAppPurchaseConsumablesAdvanced::GrantFeatureLocally(Platform::String ^productId, Platform::Guid transactionId)
{
    consumedTransactionIds->Append(transactionId);

    // Grant the user their content. You will likely increase some kind of gold/coins/some other asset count.
}

bool InAppPurchaseConsumablesAdvanced::IsLocallyFulfilled(Platform::Guid transactionId)
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

