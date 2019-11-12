//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// S1-Call.xaml.cpp
// Implementation of the CallScenario class
//

#include "pch.h"
#include "S1-Call.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::ContactActions;

using namespace Windows::ApplicationModel::Activation;
using namespace Windows::ApplicationModel::Contacts;
using namespace Windows::Foundation;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

CallScenario::CallScenario()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void CallScenario::OnNavigatedTo(NavigationEventArgs^ e)
{
    auto rootPage = dynamic_cast<MainPage^>(e->Parameter);
    if (rootPage->ContactEvent != nullptr)
    {
        auto callArgs = dynamic_cast<ContactCallActivatedEventArgs^>(rootPage->ContactEvent);
        if (callArgs != nullptr)
        {
            if (callArgs->ServiceId == "telephone")
            {
                rootPage->NotifyUser(
                    "Call activation was received. The phone number to call is " + callArgs->ServiceUserId + ".",
                    NotifyType::StatusMessage);
            }
            else
            {
                rootPage->NotifyUser(
                    "This app doesn't support calling by using the " + callArgs->ServiceId + " service.",
                    NotifyType::ErrorMessage);
            }
        }
    }
    else if (rootPage->ProtocolEvent != nullptr)
    {
        Uri^ protocolUri = rootPage->ProtocolEvent->Uri;
        if (protocolUri->SchemeName == "tel")
        {
            rootPage->NotifyUser(
                "Tel: activation was received. The phone number to call is " + protocolUri->Path + ".",
                NotifyType::StatusMessage);
        }
        else
        {
            rootPage->NotifyUser(
                "This app doesn't support the " + protocolUri->SchemeName + " protocol.",
                NotifyType::ErrorMessage);
        }
    }
}
