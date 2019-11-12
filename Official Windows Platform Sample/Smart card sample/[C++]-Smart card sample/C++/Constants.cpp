//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#include "pch.h"
#include "MainPage.xaml.h"
#include "Constants.h"

using namespace SDKSample;
using namespace SDKSample::SmartCardSample;

using namespace Concurrency;

using namespace Windows::Devices::SmartCards;
using namespace Windows::Foundation::Collections;

Platform::Array<Scenario>^ MainPage::scenariosInner = 
    ref new Platform::Array<Scenario>  
    {
        { "Create and provision a TPM virtual smart card",
          "SDKSample.SmartCardSample.Scenario1" }, 
        { "Change smart card PIN", "SDKSample.SmartCardSample.Scenario2" },
        { "Reset smart card PIN", "SDKSample.SmartCardSample.Scenario3" },
        { "Change smart card admin key", "SDKSample.SmartCardSample.Scenario4" },
        { "Verify response", "SDKSample.SmartCardSample.Scenario5" },
        { "Delete TPM virtual smart card",
          "SDKSample.SmartCardSample.Scenario6"},
        { "List all smart cards", "SDKSample.SmartCardSample.Scenario7" }
    }; 

task<SmartCard^> MainPage::GetSmartCard()
{
    return create_task(SmartCardReader::FromIdAsync(deviceId)).then(
    [&](task<SmartCardReader^> getReaderTask)
    {
        SmartCardReader^ reader = getReaderTask.get();
        return reader->FindAllCardsAsync();
    }).then(
    [&](task<IVectorView<SmartCard^>^>
        getCardsTask)
    {
        IVectorView<SmartCard^>^ cards = getCardsTask.get();
        return cards->GetAt(0);
    });
}
