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
// ProximityDeviceScenario.xaml.h
// Declaration of the ProximityDeviceScenario class
//

#pragma once

#include "pch.h"
#include "ProximityDeviceScenario.g.h"
#include "MainPage.xaml.h"

namespace ProximityCPP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
	[Windows::Foundation::Metadata::WebHostHidden]
    public ref class ProximityDeviceScenario sealed
    {
    public:
        ProximityDeviceScenario();

    protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        virtual void OnNavigatingFrom(Windows::UI::Xaml::Navigation::NavigatingCancelEventArgs ^ e) override;
        void ProximityDevice_PublishMessage(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void MessageReceived(Windows::Networking::Proximity::ProximityDevice^ device, Windows::Networking::Proximity::ProximityMessage^ message);
        void ProximityDevice_SubscribeForMessage(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    private:
        MainPage^ m_rootPage;


        long long m_publishedMessageId;
        Windows::Networking::Proximity::ProximityDevice^ m_proximityDevice;
        long long m_subscribedMessageId;
    };
}
