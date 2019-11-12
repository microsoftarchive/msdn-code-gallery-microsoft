//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// ReceiveUri.xaml.cpp
// Implementation of the ReceiveUri class
//

#include "pch.h"
#include "Scenario4_ReceiveUri.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::AssociationLaunching;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

ReceiveUri::ReceiveUri()
{
    InitializeComponent();
}

void ReceiveUri::OnNavigatedTo(NavigationEventArgs^ e)
{
    // Show scenario description based on platform.
#if WINAPI_FAMILY == WINAPI_FAMILY_PHONE_APP
    this->WindowsScenarioDescription->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
#else
    this->PhoneScenarioDescription->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
#endif

    // Get a pointer to our main page.
    MainPage^ rootPage = MainPage::Current;

    // Display the result of the protocol activation if we got here as a result of being activated for a protocol.
    if (rootPage->ProtocolEvent != nullptr)
    {
        rootPage->NotifyUser("Protocol activation received. The received URI is " + rootPage->ProtocolEvent->Uri->AbsoluteUri + ".", NotifyType::StatusMessage);
    }
}
