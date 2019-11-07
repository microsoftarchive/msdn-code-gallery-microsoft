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
#include "S5_VerifyResponse.xaml.h"
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

Scenario5::Scenario5()
{
    InitializeComponent();
}

void Scenario5::VerifyResponse_Click(Platform::Object^ sender,
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
    MainPage::Current->NotifyUser("Verifying smart card response...",
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
        return provisioning->GetChallengeContextAsync();
    }).then(
    [=](task<SmartCardChallengeContext^> getChallengeContextTask)
    {
        SmartCardChallengeContext^ context = getChallengeContextTask.get();
        IBuffer^ response = ChallengeResponseAlgorithm::CalculateResponse(
            context->Challenge,
            MainPage::Current->AdminKey);
        return context->VerifyResponseAsync(response);
    }).then(
    [=](task<bool> verifyResponseTask)
    {
        try
        {
            bool verifyResult = verifyResponseTask.get();
            MainPage::Current->NotifyUser(
                "Smart card response verification completed. Result: " +
                verifyResult.ToString(),
                NotifyType::StatusMessage);
        }
        catch(COMException ^ex)
        {
            MainPage::Current->NotifyUser(
                "Smart card response verification failed with exception: " +
                ex->ToString(),
                NotifyType::ErrorMessage);
        }

        b->IsEnabled = true;
    });

}
