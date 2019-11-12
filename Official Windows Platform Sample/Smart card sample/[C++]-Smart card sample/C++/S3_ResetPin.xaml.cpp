//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario3.xaml.cpp
// Implementation of the Scenario3 class
//

#include "pch.h"
#include "S3_ResetPin.xaml.h"
#include "MainPage.xaml.h"
#include "ChallengeResponseAlgorithm.h"

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

Scenario3::Scenario3()
{
    InitializeComponent();
}

void Scenario3::ResetPin_Click(Platform::Object^ sender,
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
    MainPage::Current->NotifyUser("Resetting smart card PIN...",
                                  NotifyType::StatusMessage);

    create_task(MainPage::Current->GetSmartCard()).then(
    [=](task<SmartCard^> getSmartCardTask)
    { 
        SmartCard^ card = getSmartCardTask.get();
        return SmartCardProvisioning::FromSmartCardAsync(card);
    }).then(
    [=](task<SmartCardProvisioning^> getProvisioningTask)
    {
        SmartCardProvisioning^ provisioning = getProvisioningTask.get();

        // When requesting a PIN reset, a SmartCardPinResetHandler must be
        // provided as an argument.  This handler must use the challenge
        // it receives and the card's admin key to calculate and set the
        // response.
        return provisioning->RequestPinResetAsync(
            ref new SmartCardPinResetHandler(
                [=](SmartCardProvisioning^ sender,
                    SmartCardPinResetRequest^ request)
            {
                IBuffer^ response =
                    ChallengeResponseAlgorithm::CalculateResponse(
                       request->Challenge,
                       MainPage::Current->AdminKey);
                request->SetResponse(response);

                SmartCardPinResetDeferral^ deferral =
                    request->GetDeferral();
                deferral->Complete();

            }));
    }).then(
    [=](task<bool> pinResetTask)
    {
        try
        {
            bool result = pinResetTask.get();
            if(result)
            {
                MainPage::Current->NotifyUser(
                    "Smart card PIN reset operation completed.",
                    NotifyType::StatusMessage);
            }
            else
            {
                MainPage::Current->NotifyUser(
                    "Smart card PIN reset operation was canceled by the user.",
                    NotifyType::StatusMessage);
            }
        }
        catch(COMException ^ex)
        {
            MainPage::Current->NotifyUser(
                "Resetting smart card PIN failed with exception: " +
                ex->ToString(),
                NotifyType::ErrorMessage);
        }

        b->IsEnabled = true;
    });
}
