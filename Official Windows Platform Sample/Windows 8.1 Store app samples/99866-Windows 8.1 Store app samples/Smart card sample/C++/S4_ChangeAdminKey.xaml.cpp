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
#include "S4_ChangeAdminKey.xaml.h"
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

Scenario4::Scenario4()
{
    InitializeComponent();
}

void Scenario4::ChangeAdminKey_Click(Platform::Object^ sender,
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

    IBuffer^ newadminkey =
        CryptographicBuffer::GenerateRandom(ADMIN_KEY_LENGTH_IN_BYTES);

    MainPage::Current->NotifyUser("Changing smart card admin key...",
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

        return context->ChangeAdministrativeKeyAsync(response, newadminkey);
    }).then(
    [=](task<void> changeAdminKeyTask)
    {
        try
        {
            changeAdminKeyTask.get();
            MainPage::Current->AdminKey = newadminkey;
            MainPage::Current->NotifyUser(
                "Smart card change admin key operation completed.",
                NotifyType::StatusMessage);
        }
        catch(COMException ^ex)
        {
            MainPage::Current->NotifyUser(
                "Changing smart card admin key failed with exception." +
                ex->ToString(),
                NotifyType::ErrorMessage);
        }

        b->IsEnabled = true;
    });

}
