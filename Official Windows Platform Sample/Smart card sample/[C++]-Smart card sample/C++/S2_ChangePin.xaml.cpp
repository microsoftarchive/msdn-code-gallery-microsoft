//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario2.xaml.cpp
// Implementation of the Scenario2 class
//

#include "pch.h"
#include "S2_ChangePin.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::Common;
using namespace SDKSample::SmartCardSample;

using namespace concurrency; 
using namespace Platform; 

using namespace Windows::Devices::SmartCards;
using namespace Windows::Storage::Streams;
using namespace Windows::Security::Cryptography;
using namespace Windows::Security::Cryptography::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;

Scenario2::Scenario2()
{
    InitializeComponent();
}

void Scenario2::ChangePin_Click(Platform::Object^ sender,
                                Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if (MainPage::Current->SmartCardReaderDeviceId == nullptr)
    {
        MainPage::Current->NotifyUser(
            "Use Scenario One to create a TPM virtual smart card.",
            NotifyType::ErrorMessage);
        return;
    }

    Button^ b = dynamic_cast<Button^>(sender);
    b->IsEnabled = false;
    MainPage::Current->NotifyUser("Changing smart card PIN...",
                                  NotifyType::StatusMessage);

    create_task(MainPage::Current->GetSmartCard()).then(
    [=](task<SmartCard^> getCardTask)
    {
        SmartCard^ card = getCardTask.get();
        return SmartCardProvisioning::FromSmartCardAsync(card);
    }).then(
    [=](task<SmartCardProvisioning^> getProvisioningTask)
    {
        SmartCardProvisioning^ provisioning = getProvisioningTask.get();
        return provisioning->RequestPinChangeAsync();
    }).then(
    [=](task<bool> changePinTask)
    {
        try
        {
            bool result = changePinTask.get();
            if(result)
            {
                MainPage::Current->NotifyUser(
                    "Smart card change PIN operation completed.",
                    NotifyType::StatusMessage);
            }
            else
            {
                MainPage::Current->NotifyUser(
                    "Smart card change PIN operation was canceled "
                    "by the user.",
                    NotifyType::StatusMessage);
            }
        }
        catch(COMException ^ex)
        {
            MainPage::Current->NotifyUser(
                "Changing smart card PIN failed with exception: " +
                ex->ToString(),
                NotifyType::ErrorMessage);
        }

        b->IsEnabled = true;
    });
}
