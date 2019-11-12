//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// S3-Map-Address.xaml.cpp
// Implementation of the mapScenario class
//

#include "pch.h"
#include "S3-Map-Address.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::ContactActions;

using namespace Windows::ApplicationModel::Activation;
using namespace Windows::ApplicationModel::Contacts;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

MapAddressScenario::MapAddressScenario()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void MapAddressScenario::OnNavigatedTo(NavigationEventArgs^ e)
{
    auto rootPage = dynamic_cast<MainPage^>(e->Parameter);
    if (rootPage->ContactEvent != nullptr)
    {
        auto mapArgs = dynamic_cast<ContactMapActivatedEventArgs^>(rootPage->ContactEvent);
        if (mapArgs != nullptr)
        {
            ContactAddress^ address = mapArgs->Address;
            rootPage->NotifyUser(
                "Map address activation was received. The street address to map is " + 
                (address->StreetAddress->IsEmpty() ? "unspecified" : address->StreetAddress) + ".",
                NotifyType::StatusMessage);
        }
    }
}
