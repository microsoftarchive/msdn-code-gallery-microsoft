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
// ProximityDeviceEvents.xaml.h
// Declaration of the ProximityDeviceEvents class
//

#pragma once

#include "pch.h"
#include "ProximityDeviceEvents.g.h"
#include "MainPage.xaml.h"

namespace ProximityCPP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
	[Windows::Foundation::Metadata::WebHostHidden]
    public ref class ProximityDeviceEvents sealed
    {
    public:
        ProximityDeviceEvents();
        void DeviceArrived(Windows::Networking::Proximity::ProximityDevice^ device);
        void DeviceDeparted(Windows::Networking::Proximity::ProximityDevice^ device);

    protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        virtual void OnNavigatingFrom(Windows::UI::Xaml::Navigation::NavigatingCancelEventArgs ^ e) override;
    private:
        MainPage^ m_rootPage;
        Windows::Foundation::EventRegistrationToken m_arrivedToken;
        Windows::Foundation::EventRegistrationToken m_departedToken;
        Windows::Networking::Proximity::ProximityDevice^ m_proximityDevice;
    };
}
