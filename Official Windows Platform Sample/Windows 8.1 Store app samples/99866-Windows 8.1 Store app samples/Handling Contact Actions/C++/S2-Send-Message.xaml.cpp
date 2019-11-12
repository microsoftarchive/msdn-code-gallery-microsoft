//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// S2-Send-Message.xaml.cpp
// Implementation of the SendMessageScenario class
//

#include "pch.h"
#include "S2-Send-Message.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::ContactActions;

using namespace Windows::ApplicationModel::Activation;
using namespace Windows::ApplicationModel::Contacts;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

SendMessageScenario::SendMessageScenario()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void SendMessageScenario::OnNavigatedTo(NavigationEventArgs^ e)
{
    auto rootPage = dynamic_cast<MainPage^>(e->Parameter);
    if (rootPage->ContactEvent != nullptr)
    {
        auto messageArgs = dynamic_cast<ContactMessageActivatedEventArgs^>(rootPage->ContactEvent);
        if (messageArgs != nullptr)
        {
            rootPage->NotifyUser(
                "Send message activation was received. The service to use is " + messageArgs->ServiceId + ". The user ID to message is " + messageArgs->ServiceUserId + ".",
                NotifyType::StatusMessage);
        }
    }
}
